using System;
using System.Linq;
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
        public void TestMean()
        {
            int sampleCount = 10_000_000;
            var dist = new BoxMullerGaussianDistribution();
            double[] sampleArr = new double[sampleCount];

            for (int i = 0; i < sampleCount; i++) {
                sampleArr[i] = dist.Sample();
            }

            double sum = Math.Abs(sampleArr.Sum());
            double mean = sum / sampleCount;
            Assert.IsTrue(Math.Abs(mean) < 0.001);
        }

        [TestMethod]
        [TestCategory("BoxMullerGaussianDistribution-Double")]
        public void TestStandardDeviation()
        {
            int sampleCount = 10_000_000;
            var dist = new BoxMullerGaussianDistribution();
            
            double sqrSum = 0.0;
            for(int i=0; i< sampleCount; i++)
            {
                double x = dist.Sample();
                sqrSum += x*x;
            }

            double var = sqrSum / sampleCount;
            double stdDev = Math.Sqrt(var);
            Assert.IsTrue(Math.Abs(stdDev-1.0) < 0.001);
        }

        #endregion
    }
}
