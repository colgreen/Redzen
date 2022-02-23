using FluentAssertions;
using MathNet.Numerics.Statistics;
using Redzen.Random;
using Xunit;

namespace Redzen.Numerics.Distributions.Float.Tests;

public class UniformDistributionTests
{
    #region Public Test Methods

    [Fact]
    public void Sample()
    {
        int sampleCount = 10_000_000;
        UniformDistributionSampler sampler = new();
        var sampleArr = new float[sampleCount];

        for(int i = 0; i < sampleCount; i++)
            sampleArr[i] = sampler.Sample();

        UniformDistributionTest(sampleArr, 0.0f, 1.0f);

        // Configure a scale and a signed flag.
        sampler = new UniformDistributionSampler(100.0f, true);

        for(int i = 0; i < sampleCount; i++)
            sampleArr[i] = sampler.Sample();

        UniformDistributionTest(sampleArr, -100.0f, 100.0f);
    }

    [Fact]
    public void SampleSigned()
    {
        int sampleCount = 10_000_000;
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        var sampleArr = new float[sampleCount];

        for(int i = 0; i < sampleCount; i++)
            sampleArr[i] = UniformDistribution.SampleSigned(rng, 20.0f);

        UniformDistributionTest(sampleArr, -20.0f, 20.0f);
    }

    [Fact]
    public void SampleMax()
    {
        int sampleCount = 10_000_000;
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        var sampleArr = new float[sampleCount];

        for(int i = 0; i < sampleCount; i++)
            sampleArr[i] = UniformDistribution.Sample(rng, 20.0f);

        UniformDistributionTest(sampleArr, 0.0f, 20.0f);
    }

    [Fact]
    public void SampleSignedMax()
    {
        int sampleCount = 10_000_000;
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        var sampleArr = new float[sampleCount];

        for(int i = 0; i < sampleCount; i++)
            sampleArr[i] = UniformDistribution.SampleSigned(rng, 20.0f);

        UniformDistributionTest(sampleArr, -20.0f, 20.0f);
    }

    [Fact]
    public void SampleMinMax()
    {
        int sampleCount = 10_000_000;
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        var sampleArr = new float[sampleCount];

        for(int i = 0; i < sampleCount; i++)
            sampleArr[i] = UniformDistribution.Sample(rng, -20.0f, 20.0f);

        UniformDistributionTest(sampleArr, -20.0f, 20.0f);
    }

    [Fact]
    public void SampleSpan()
    {
        int sampleCount = 10_000_000;
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        var sampleArr = new float[sampleCount];

        UniformDistribution.Sample(rng, sampleArr.AsSpan());

        UniformDistributionTest(sampleArr, 0.0f, 1.0f);
    }

    [Fact]
    public void SampleMaxSpan()
    {
        int sampleCount = 10_000_000;
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        var sampleArr = new float[sampleCount];

        UniformDistribution.Sample(rng, 20, sampleArr.AsSpan());

        UniformDistributionTest(sampleArr, 0.0f, 20.0f);
    }

    [Fact]
    public void SampleSignedMaxSpan()
    {
        int sampleCount = 10_000_000;
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        var sampleArr = new float[sampleCount];

        UniformDistribution.SampleSigned(rng, 20, sampleArr.AsSpan());

        UniformDistributionTest(sampleArr, -20.0f, 20.0f);
    }

    [Fact]
    public void SampleMinMaxSpan()
    {
        int sampleCount = 10_000_000;
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        var sampleArr = new float[sampleCount];

        UniformDistribution.Sample(rng, -20, 20, sampleArr.AsSpan());

        UniformDistributionTest(sampleArr, -20.0f, 20.0f);
    }

    #endregion

    #region Private Static Methods

    private static void UniformDistributionTest(float[] sampleArr, float lowerBound, float upperBound)
    {
        Array.Sort(sampleArr);
        var samples = new double[sampleArr.Length];
        for(int i=0; i < sampleArr.Length; i++)
            samples[i] = sampleArr[i];

        RunningStatistics runningStats = new(samples);

        // Skewness should be pretty close to zero (evenly distributed samples).
        Math.Abs(runningStats.Skewness).Should().BeLessThan(0.01);

        // Mean test.
        double range = upperBound - lowerBound;
        double expectedMean = lowerBound + (range / 2.0);
        double meanErr = expectedMean - runningStats.Mean;
        double maxExpectedErr = range / 1000.0;

        Math.Abs(meanErr).Should().BeLessThan(maxExpectedErr);

        // Test a range of centile/quantile values.
        for(float tau=0f; tau <= 1f; tau += 0.1f)
        {
            float quantile = SortedArrayStatistics.Quantile(sampleArr, tau);
            float expectedQuantile = lowerBound + (tau * (float)range);
            float quantileError = expectedQuantile - quantile;

            Math.Abs(quantileError).Should().BeLessThan((float)maxExpectedErr);
        }
    }

    #endregion
}
