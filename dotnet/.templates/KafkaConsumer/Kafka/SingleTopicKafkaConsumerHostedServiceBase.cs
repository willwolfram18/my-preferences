using System.Diagnostics;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;

namespace FIXME.Kafka;

public abstract class SingleTopicKafkaConsumerHostedServiceBase<TKey, TValue> : BackgroundService
{
    private readonly ConsumerBuilder<TKey, TValue> _consumerBuilder;
    private readonly ActivitySource _trace;

    protected SingleTopicKafkaConsumerHostedServiceBase(ILogger log, ConsumerBuilder<TKey, TValue> consumerBuilder,
        ActivitySource trace)
    {
        Log = log;
        _consumerBuilder = consumerBuilder;
        _trace = trace;
    }

    protected ILogger Log { get; }

    protected abstract Task ProcessAsync(ConsumeResult<TKey, TValue?> message, CancellationToken cancellationToken);

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Use Task.Run to get the task into a separate task
        return Task.Run(() => ConsumeMessagesAsync(stoppingToken), stoppingToken);
    }

    protected virtual Action<IConsumer<TKey, TValue>, CommittedOffsets>? CommitOffsetsDelegate => null;

    protected abstract string TopicToConsume { get; }

    private async Task ConsumeMessagesAsync(CancellationToken stoppingToken)
    {
        if (CommitOffsetsDelegate is not null)
        {
            _consumerBuilder.SetOffsetsCommittedHandler(CommitOffsetsDelegate);
        }

        using var consumer = _consumerBuilder.Build();

        stoppingToken.Register(() =>
        {
            Log.LogInformation("Cancellation requested; issuing unsubscribe");
            consumer.Unsubscribe();
            Log.LogInformation("Unsubscribe finished successfully");
        });
        consumer.Subscribe(TopicToConsume);


        while (!stoppingToken.IsCancellationRequested)
        {
            Activity? consumeActivity = null;
            Activity? processActivity = null;
            try
            {
                ConsumeResult<TKey, TValue?> result = consumer.Consume(stoppingToken);
                string traceparent = result.Message.Headers.GetTraceparentHeader();

                consumeActivity = _trace.StartActivity($"receive {typeof(TValue).FullName!.ToLower()}", ActivityKind.Consumer, traceparent)
                    ?.SetTag("messaging.system", "kafka")
                    .SetTag("messaging.operation", "receive")
                    .SetTag("messaging.source.name", result.Topic)
                    .SetTag("messaging.kafka.message.key", result.Message.Key)
                    .SetTag("messaging.kafka.source.partition", result.Partition.Value)
                    .SetTag("messaging.kafka.source.offset", result.Offset.Value)
                    .SetTag("messaging.kafka.source.timestamp_ms", result.Message.Timestamp)
                    .SetTag("messaging.kafka.message.tombstone", result.Message.Value is null);

                processActivity = _trace.StartActivity($"process {typeof(TValue).FullName!.ToLower()}", ActivityKind.Internal);

                await ProcessAsync(result, stoppingToken);

                processActivity?.SetStatus(ActivityStatusCode.Ok);
                consumeActivity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception e) when (e is OperationCanceledException or TaskCanceledException)
            {
                Log.LogInformation(e, "Cancellation requested, shutting down");
            }
            catch (Exception e)
            {
                consumeActivity?.SetStatus(ActivityStatusCode.Error, e.Message)
                    .SetTag("exception.message", e.Message)
                    .SetTag("exception.stacktrace", e.StackTrace)
                    .SetTag("exception.type", e.GetType().FullName);

                Log.LogError(e, "An error occurred during consuming from Kafka");
            }
            finally
            {
                processActivity?.Dispose();
                consumeActivity?.Dispose();
            }
        }

    }
}
