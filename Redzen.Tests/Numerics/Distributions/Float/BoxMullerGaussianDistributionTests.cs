using Xunit;

namespace Redzen.Numerics.Distributions.Float.Tests;

public class BoxMullerGaussianDistributionTests
{
    #region Test Methods

    [Fact]
    public void SimpleStats()
    {
        var dist = new BoxMullerGaussianSampler(0f, 1f);
        GaussianDistributionTestUtils.TestSimpleStats(dist);
    }

    [Theory]
    [InlineData(0f, 1f)]      // Standard normal.
    [InlineData(10f, 1f)]     // Non-zero mean tests.
    [InlineData(-100f, 1f)]   //
    [InlineData(0f, 0.2f)]    // Non-1.0 standard deviations
    [InlineData(0f, 5f)]      //
    [InlineData(10f, 2f)]     // Non-zero mean and non-1.0 standard deviation.
    [InlineData(-10f, 3f)]
    public void TestCumulativeDistribution(float mean, float stdDev)
    {
        var sampler = new BoxMullerGaussianSampler(mean, stdDev);
        GaussianDistributionTestUtils.TestDistribution(sampler, mean, stdDev);
    }

    #endregion
}
