using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random.Double;

namespace Redzen.UnitTests.Random.Double
{
    [TestClass]
    public class ZigguratGaussianDistributionTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("ZigguratGaussianDistribution-Double")]
        public void TestSimpleStats()
        {
            var dist = new ZigguratGaussianDistribution();
            GaussianDistributionTestUtils.TestSimpleStats(dist);
        }

        [TestMethod]
        [TestCategory("ZigguratGaussianDistribution-Double")]
        public void TestCumulativeDistribution()
        {
            // Standard normal.
            var dist = new ZigguratGaussianDistribution(0.0, 1.0);
            GaussianDistributionTestUtils.TestDistribution(dist, 0.0, 1.0);

            // Non-zero mean tests.
            dist = new ZigguratGaussianDistribution(10.0, 1.0);
            GaussianDistributionTestUtils.TestDistribution(dist, 10.0, 1.0);

            dist = new ZigguratGaussianDistribution(-100.0, 1.0);
            GaussianDistributionTestUtils.TestDistribution(dist, -100.0, 1.0);

            // Non-1.0 standard deviations
            dist = new ZigguratGaussianDistribution(0.0, 0.2);
            GaussianDistributionTestUtils.TestDistribution(dist, 0.0, 0.2);

            dist = new ZigguratGaussianDistribution(0.0, 5.0);
            GaussianDistributionTestUtils.TestDistribution(dist, 0.0, 5.0);

            // Non-zero mean and non-1.0 standard deviation.
            dist = new ZigguratGaussianDistribution(10.0, 2.0);
            GaussianDistributionTestUtils.TestDistribution(dist, 10.0, 2.0);

            dist = new ZigguratGaussianDistribution(-10.0, 3.0);
            GaussianDistributionTestUtils.TestDistribution(dist, -10.0, 3.0);
        }

        #endregion
    }
}
