using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Numerics.Distributions.Double;

namespace Redzen.UnitTests.Numerics.Distributions.Double
{
    [TestClass]
    public class BoxMullerGaussianDistributionTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("BoxMullerGaussianDistribution-Double")]
        public void TestSimpleStats()
        {
            var dist = new BoxMullerGaussianSampler(0, 1.0);
            GaussianDistributionTestUtils.TestSimpleStats(dist);
        }

        [TestMethod]
        [TestCategory("BoxMullerGaussianDistribution-Double")]
        public void TestCumulativeDistribution()
        {
            // Standard normal.
            var sampler = new BoxMullerGaussianSampler(0.0, 1.0);
            GaussianDistributionTestUtils.TestDistribution(sampler, 0.0, 1.0);

            // Non-zero mean tests.
            sampler = new BoxMullerGaussianSampler(10.0, 1.0);
            GaussianDistributionTestUtils.TestDistribution(sampler, 10.0, 1.0);

            sampler = new BoxMullerGaussianSampler(-100.0, 1.0);
            GaussianDistributionTestUtils.TestDistribution(sampler, -100.0, 1.0);

            // Non-1.0 standard deviations
            sampler = new BoxMullerGaussianSampler(0.0, 0.2);
            GaussianDistributionTestUtils.TestDistribution(sampler, 0.0, 0.2);

            sampler = new BoxMullerGaussianSampler(0.0, 5.0);
            GaussianDistributionTestUtils.TestDistribution(sampler, 0.0, 5.0);

            // Non-zero mean and non-1.0 standard deviation.
            sampler = new BoxMullerGaussianSampler(10.0, 2.0);
            GaussianDistributionTestUtils.TestDistribution(sampler, 10.0, 2.0);

            sampler = new BoxMullerGaussianSampler(-10.0, 3.0);
            GaussianDistributionTestUtils.TestDistribution(sampler, -10.0, 3.0);
        }

        #endregion
    }
}
