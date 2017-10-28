using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;

namespace Redzen.UnitTests.Random.Double
{
    public static class GaussianDistributionTestUtils
    {
        #region Test Methods
        
        public static void TestMean(IGaussianDistribution<double> dist)
        {
            int sampleCount = 10000000;

            double sum = 0.0;
            for (int i = 0; i < sampleCount; i++) {
                sum += dist.Sample();
            }

            double mean = sum / sampleCount;
            Assert.IsTrue(Math.Abs(mean) < 0.001);
        }

        public static void TestStandardDeviation(IGaussianDistribution<double> dist)
        {
            int sampleCount = 10000000;

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
