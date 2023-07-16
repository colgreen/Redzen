using System.Numerics;
using Redzen.Random;

namespace Redzen.Numerics.Distributions;

/// <summary>
/// Discrete distribution static utility methods.
/// </summary>
public static class DiscreteDistributionUtils
{
    /// <summary>
    /// Sample from a binary/Bernoulli distribution with the given probability of sampling 'true'.
    /// </summary>
    /// <typeparam name="T">Floating point data type.</typeparam>
    /// <param name="probability">Probability of sampling boolean true.</param>
    /// <param name="rng">Random number generator.</param>
    /// <returns>A boolean random sample.</returns>
    public static bool SampleBernoulli<T>(
        T probability,
        IRandomSource rng)
        where T : struct, IBinaryFloatingPointIeee754<T>
    {
        return rng.NextUnitInterval<T>() < probability;
    }

    /// <summary>
    /// Fill a span with samples from a binary/Bernoulli distribution with the given probability of sampling 'true'.
    /// </summary>
    /// <typeparam name="T">Floating point data type.</typeparam>
    /// <param name="probability">Probability of sampling 'true'.</param>
    /// <param name="span">The span to fill with samples.</param>
    /// <param name="rng">Random source.</param>
    public static void SampleBernoulli<T>(
        T probability,
        Span<bool> span,
        IRandomSource rng)
        where T : struct, IBinaryFloatingPointIeee754<T>
    {
        for(int i=0; i < span.Length; i++)
            span[i] = rng.NextUnitInterval<T>() < probability;
    }

    /// <summary>
    /// Take multiple samples from a set of possible outcomes with equal probability (i.e., a uniform discrete distribution)
    /// without replacement, (i.e., any given value will only occur once at most in the set of samples).
    /// </summary>
    /// <param name="numberOfOutcomes">The number of possible outcomes per sample.</param>
    /// <param name="numberOfSamples">The number of samples to take.</param>
    /// <param name="rng">Random source.</param>
    /// <returns>A new integer array containing the samples.</returns>
    public static int[] SampleUniformWithoutReplacement(
        int numberOfOutcomes,
        int numberOfSamples,
        IRandomSource rng)
    {
        int[] sampleArr = new int[numberOfSamples];
        SampleUniformWithoutReplacement(numberOfOutcomes, sampleArr, rng);
        return sampleArr;
    }

    /// <summary>
    /// Take multiple samples from a set of possible outcomes with equal probability, i.e. a uniform discrete distribution,
    /// without replacement, i.e. any given value will only occur once at most in the set of samples.
    /// </summary>
    /// <param name="numberOfOutcomes">The number of possible outcomes per sample.</param>
    /// <param name="sampleSpan">A span to fill with samples.</param>
    /// <param name="rng">Random source.</param>
    public static void SampleUniformWithoutReplacement(
        int numberOfOutcomes,
        Span<int> sampleSpan,
        IRandomSource rng)
    {
        if(sampleSpan.Length > numberOfOutcomes)
            throw new ArgumentException("sampleArr length must be less then or equal to numberOfOutcomes.");

        // Create a temp span containing all possible choices from 0 to numberOfOutcomes-1.
        Span<int> tmpSpan = stackalloc int[numberOfOutcomes];
        for(int i = 0; i < numberOfOutcomes; i++)
            tmpSpan[i] = i;

        // Perform a Fisher–Yates shuffle over tmpSpan, but only for the number of items required for
        // sampleSpan. I.e., we could complete the shuffle over the full length of tmpSpan, but we can
        // improve performance by terminating early, once we have all the samples we require.
        for(int i = 0; i < sampleSpan.Length; i++)
        {
            // Select an index at random.
            int idx = rng.Next(i, numberOfOutcomes);

            // Swap elements i and idx.
            int tmp = tmpSpan[i];
            tmpSpan[i] = tmpSpan[idx];
            tmpSpan[idx] = tmp;
        }

        // Copy the samples into sampleSpan.
        tmpSpan.Slice(0, sampleSpan.Length).CopyTo(sampleSpan);
    }
}
