using FluentAssertions;
using FluentAssertions.Equivalency;
using Moq;

namespace willwolfram18.FluentAssertions.Extensions;

/// <summary>
/// Combines Moq and FluentAssertions.
/// A better <see cref="It"/>, for cases when a failed setup match must fail the test
/// </summary>
public static class ItShould
{
    /// <summary>
    /// Fails if equivalence check fails
    /// </summary>
    public static T Be<T>(T expected)
    {
        return Be(expected, string.Empty);
    }

    /// <summary>
    /// Fails if equivalence check fails
    /// </summary>
    public static T Be<T>(T expected, string because)
    {
        return It.Is<T>(actual => AssertEquivalence(actual, expected, config => config, because));
    }

    /// <summary>
    /// Fails if equivalence check fails
    /// </summary>
    public static T Be<T>(T expected, Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> config)
    {
        return Be(expected, config, string.Empty);
    }

    /// <summary>
    /// Fails if equivalence check fails
    /// </summary>
    public static T Be<T>(T expected, Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> config, string because)
    {
        return It.Is<T>(actual => AssertEquivalence(actual, expected, config, because));
    }

    /// <summary>
    /// Assert the Equivalence of the Be
    /// </summary>
    private static bool AssertEquivalence<T>(T actual,
        T expected,
        Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> config,
        string because)
    {
        actual.Should().BeEquivalentTo(expected, config, because);

        return true;
    }
}
