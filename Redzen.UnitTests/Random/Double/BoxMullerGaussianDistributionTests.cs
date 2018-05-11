using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random.Double;

namespace Redzen.UnitTests.Random.Double
{
    [TestClass]
    public class BoxMullerGaussianDistributionTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("BoxMullerGaussianDistribution-Double")]
        public void TestSimpleStats()
        {
            var dist = new BoxMullerGaussianDistribution();
            GaussianDistributionTestUtils.TestSimpleStats(dist);
        }

        [TestMethod]
        [TestCategory("BoxMullerGaussianDistribution-Double")]
        public void TestCumulativeDistribution()
        {
            // Standard normal.
            var dist = new BoxMullerGaussianDistribution(0.0, 1.0);
            GaussianDistributionTestUtils.TestDistribution(dist, 0.0, 1.0);

            // Non-zero mean tests.
            dist = new BoxMullerGaussianDistribution(10.0, 1.0);
            GaussianDistributionTestUtils.TestDistribution(dist, 10.0, 1.0);

            dist = new BoxMullerGaussianDistribution(-100.0, 1.0);
            GaussianDistributionTestUtils.TestDistribution(dist, -100.0, 1.0);

            // Non-1.0 standard deviations
            dist = new BoxMullerGaussianDistribution(0.0, 0.2);
            GaussianDistributionTestUtils.TestDistribution(dist, 0.0, 0.2);

            dist = new BoxMullerGaussianDistribution(0.0, 5.0);
            GaussianDistributionTestUtils.TestDistribution(dist, 0.0, 5.0);

            // Non-zero mean and non-1.0 standard deviation.
            dist = new BoxMullerGaussianDistribution(10.0, 2.0);
            GaussianDistributionTestUtils.TestDistribution(dist, 10.0, 2.0);

            dist = new BoxMullerGaussianDistribution(-10.0, 3.0);
            GaussianDistributionTestUtils.TestDistribution(dist, -10.0, 3.0);
        }

        #endregion
    }
}
