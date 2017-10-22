using System;
using MathNet.Numerics.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random.Double;

namespace Redzen.UnitTests.Random.Double
{
    [TestClass]
    public class UniformDistributionTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("UniformDistribution-Double")]
        public void TestSample()
        {
            int sampleCount = 10000000;
            UniformDistribution dist = new UniformDistribution();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = dist.Sample();
            }

            UniformDistributionTest(sampleArr, 0.0, 1.0);

            // Configure a scale and a signed flag.
            dist = new UniformDistribution(100.0, true);

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = dist.Sample();
            }

            UniformDistributionTest(sampleArr, -100.0, 100.0);
        }

        [TestMethod]
        [TestCategory("UniformDistribution-Double")]
        public void TestSampleScale()
        {
            int sampleCount = 10000000;
            UniformDistribution dist = new UniformDistribution();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = dist.Sample(20.0);
            }

            UniformDistributionTest(sampleArr, 0.0, 20.0);
        }

        [TestMethod]
        [TestCategory("UniformDistribution-Double")]
        public void TestSampleScaleSigned()
        {
            int sampleCount = 10000000;
            UniformDistribution dist = new UniformDistribution();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = dist.Sample(20.0, true);
            }

            UniformDistributionTest(sampleArr, -20.0, 20.0);
        }

        [TestMethod]
        [TestCategory("UniformDistribution-Double")]
        public void TestSampleUnit()
        {
            int sampleCount = 10000000;
            UniformDistribution dist = new UniformDistribution();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = dist.SampleUnit();
            }

            UniformDistributionTest(sampleArr, 0, 1.0);
        }

        [TestMethod]
        [TestCategory("UniformDistribution-Double")]
        public void TestSampleUnitSigned()
        {
            int sampleCount = 10000000;
            UniformDistribution dist = new UniformDistribution();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = dist.SampleUnitSigned();
            }

            UniformDistributionTest(sampleArr, -1.0, 1.0);
        }

        #endregion

        #region Private Static Methods

        private static void UniformDistributionTest(double[] sampleArr, double lowerBound, double upperBound)
        {
            Array.Sort(sampleArr);
            RunningStatistics runningStats = new RunningStatistics(sampleArr);

            // Skewness should be pretty close to zero (evenly distributed samples)
            if(Math.Abs(runningStats.Skewness) > 0.01) Assert.Fail();
            
            // Mean test.
            double range = upperBound - lowerBound;
            double expectedMean = lowerBound + (range / 2.0);
            double meanErr = expectedMean - runningStats.Mean;
            double maxExpectedErr = range / 1000.0;

            if(Math.Abs(meanErr) > maxExpectedErr) Assert.Fail();

            // Test a range of centile/quantile values.
            double tauStep = (upperBound - lowerBound) / 10.0;

            for(double tau=0; tau <= 1.0; tau += 0.1)
            {
                double quantile = SortedArrayStatistics.Quantile(sampleArr, tau);
                double expectedQuantile = lowerBound + (tau * range);
                double quantileError = expectedQuantile - quantile;
                if(Math.Abs(quantileError) > maxExpectedErr) Assert.Fail();
            }
        }

        #endregion
    }
}
