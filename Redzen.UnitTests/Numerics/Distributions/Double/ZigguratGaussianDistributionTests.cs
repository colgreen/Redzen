using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Numerics.Distributions.Double;

namespace Redzen.UnitTests.Numerics.Distributions.Double
{
    [TestClass]
    public class ZigguratGaussianDistributionTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("ZigguratGaussianDistribution-Double")]
        public void TestSimpleStats()
        {
            var sampler = new ZigguratGaussianSampler(0.0, 1.0);
            GaussianDistributionTestUtils.TestSimpleStats(sampler);
        }

        [TestMethod]
        [TestCategory("ZigguratGaussianDistribution-Double")]
        public void TestCumulativeDistribution()
        {
            // Standard normal.
            var sampler = new ZigguratGaussianSampler(0.0, 1.0);
            GaussianDistributionTestUtils.TestDistribution(sampler, 0.0, 1.0);

            // Non-zero mean tests.
            sampler = new ZigguratGaussianSampler(10.0, 1.0);
            GaussianDistributionTestUtils.TestDistribution(sampler, 10.0, 1.0);

            sampler = new ZigguratGaussianSampler(-100.0, 1.0);
            GaussianDistributionTestUtils.TestDistribution(sampler, -100.0, 1.0);

            // Non-1.0 standard deviations
            sampler = new ZigguratGaussianSampler(0.0, 0.2);
            GaussianDistributionTestUtils.TestDistribution(sampler, 0.0, 0.2);

            sampler = new ZigguratGaussianSampler(0.0, 5.0);
            GaussianDistributionTestUtils.TestDistribution(sampler, 0.0, 5.0);

            // Non-zero mean and non-1.0 standard deviation.
            sampler = new ZigguratGaussianSampler(10.0, 2.0);
            GaussianDistributionTestUtils.TestDistribution(sampler, 10.0, 2.0);

            sampler = new ZigguratGaussianSampler(-10.0, 3.0);
            GaussianDistributionTestUtils.TestDistribution(sampler, -10.0, 3.0);
        }

        #endregion
    }
}
