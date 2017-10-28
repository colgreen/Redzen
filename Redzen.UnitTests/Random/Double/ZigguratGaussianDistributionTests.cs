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
        public void TestMean()
        {
            var dist = new ZigguratGaussianDistribution();
            GaussianDistributionTestUtils.TestMean(dist);
        }

        [TestMethod]
        [TestCategory("ZigguratGaussianDistribution-Double")]
        public void TestStandardDeviation()
        {
            var dist = new ZigguratGaussianDistribution();
            GaussianDistributionTestUtils.TestStandardDeviation(dist);
        }

        #endregion
    }
}
