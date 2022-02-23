using FluentAssertions;
using MathNet.Numerics.Statistics;
using Redzen.Random;
using Xunit;

namespace Redzen.Numerics.Distributions.Double.Tests;

public class UniformDistributionTests
{
    #region Public Test Methods

    [Fact]
    public void Sample()
    {
        int sampleCount = 10_000_000;
        UniformDistributionSampler sampler = new();
        var sampleArr = new double[sampleCount];

        for(int i=0; i < sampleCount; i++)
            sampleArr[i] = sampler.Sample();

        UniformDistributionTest(sampleArr, 0.0, 1.0);

        // Configure a scale and a signed flag.
        sampler = new UniformDistributionSampler(100.0, true);

        for(int i=0; i < sampleCount; i++)
            sampleArr[i] = sampler.Sample();

        UniformDistributionTest(sampleArr, -100.0, 100.0);
    }

    [Fact]
    public void SampleSigned()
    {
        int sampleCount = 10_000_000;
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        var sampleArr = new double[sampleCount];

        for(int i = 0; i < sampleCount; i++)
            sampleArr[i] = UniformDistribution.SampleSigned(rng, 20.0);

        UniformDistributionTest(sampleArr, -20.0, 20.0);
    }

    [Fact]
    public void SampleMax()
    {
        int sampleCount = 10_000_000;
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        var sampleArr = new double[sampleCount];

        for(int i=0; i < sampleCount; i++)
            sampleArr[i] = UniformDistribution.Sample(rng, 20.0);

        UniformDistributionTest(sampleArr, 0.0, 20.0);
    }

    [Fact]
    public void SampleSignedMax()
    {
        int sampleCount = 10_000_000;
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        var sampleArr = new double[sampleCount];

        for(int i = 0; i < sampleCount; i++)
            sampleArr[i] = UniformDistribution.SampleSigned(rng, 20.0);

        UniformDistributionTest(sampleArr, -20.0, 20.0);
    }

    [Fact]
    public void SampleMinMax()
    {
        int sampleCount = 10_000_000;
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        var sampleArr = new double[sampleCount];

        for(int i = 0; i < sampleCount; i++)
            sampleArr[i] = UniformDistribution.Sample(rng, -20.0, 20.0);

        UniformDistributionTest(sampleArr, -20.0, 20.0);
    }

    [Fact]
    public void SampleSpan()
    {
        int sampleCount = 10_000_000;
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        var sampleArr = new double[sampleCount];

        UniformDistribution.Sample(rng, sampleArr.AsSpan());

        UniformDistributionTest(sampleArr, 0.0, 1.0);
    }

    [Fact]
    public void SampleMaxSpan()
    {
        int sampleCount = 10_000_000;
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        var sampleArr = new double[sampleCount];

        UniformDistribution.Sample(rng, 20, sampleArr.AsSpan());

        UniformDistributionTest(sampleArr, 0.0, 20.0);
    }

    [Fact]
    public void SampleSignedMaxSpan()
    {
        int sampleCount = 10_000_000;
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        var sampleArr = new double[sampleCount];

        UniformDistribution.SampleSigned(rng, 20, sampleArr.AsSpan());

        UniformDistributionTest(sampleArr, -20.0, 20.0);
    }

    [Fact]
    public void SampleMinMaxSpan()
    {
        int sampleCount = 10_000_000;
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        var sampleArr = new double[sampleCount];

        UniformDistribution.Sample(rng, -20, 20, sampleArr.AsSpan());

        UniformDistributionTest(sampleArr, -20.0, 20.0);
    }

    #endregion

    #region Private Static Methods

    private static void UniformDistributionTest(double[] sampleArr, double lowerBound, double upperBound)
    {
        Array.Sort(sampleArr);
        RunningStatistics runningStats = new(sampleArr);

        // Skewness should be pretty close to zero (evenly distributed samples).
        Math.Abs(runningStats.Skewness).Should().BeLessThan(0.01);

        // Mean test.
        double range = upperBound - lowerBound;
        double expectedMean = lowerBound + (range / 2.0);
        double meanErr = expectedMean - runningStats.Mean;
        double maxExpectedErr = range / 1000.0;

        Math.Abs(meanErr).Should().BeLessThan(maxExpectedErr);

        // Test a range of centile/quantile values.
        for(double tau=0.0; tau <= 1.0; tau += 0.1)
        {
            double quantile = SortedArrayStatistics.Quantile(sampleArr, tau);
            double expectedQuantile = lowerBound + (tau * range);
            double quantileError = expectedQuantile - quantile;

            Math.Abs(quantileError).Should().BeLessThan(maxExpectedErr);
        }
    }

    #endregion
}
