using System.Text;
using Confluent.Kafka;

namespace FIXME.Kafka;

public static class KafkaHeadersExtension
{
    public static string GetTraceparentHeader(this Headers headers)
    {
        if (headers.TryGetLastBytes("traceparent", out byte[] traceparent))
        {
            return Encoding.UTF8.GetString(traceparent);
        }

        return string.Empty;
    }
}
