using System;
using MathNet.Numerics.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Redzen.UnitTests.Random
{
    internal static class RandomTestUtils
    {
        #region Private Static Methods

        public static double[] CreateSampleArray(int length, Func<double> sampleFn)
        {
            double[] arr = new double[length];
            for(int i=0; i<length; i++) {
                arr[i] = sampleFn();
            }
            return arr;
        }

        public static void UniformDistributionTest(double[] sampleArr, double minValue, double maxValue)
        {
            Array.Sort(sampleArr);
            RunningStatistics runningStats = new RunningStatistics(sampleArr);

            // Skewness should be pretty close to zero (evenly distributed samples)
            if(Math.Abs(runningStats.Skewness) > 0.01) {
                Assert.Fail();
            }
            
            // Mean test.
            double range = maxValue - minValue;
            double expectedMean = minValue + (range / 2.0);
            double meanErr = expectedMean - runningStats.Mean;
            double maxExpectedErr = range / 1000.0;

            if(Math.Abs(meanErr) > maxExpectedErr) {
                Assert.Fail();
            }

            // Test a range of centile/quantile values.
            double tauStep = (maxValue - minValue) / 10.0;

            for(double tau=0; tau <= 1.0; tau += 0.1)
            {
                double quantile = SortedArrayStatistics.Quantile(sampleArr, tau);
                double expectedQuantile = minValue + (tau * range);
                double quantileError = expectedQuantile - quantile;
                if(Math.Abs(quantileError) > maxExpectedErr) Assert.Fail();
            }

            // Test that no samples are outside the defined range.
            for(int i=0; i < sampleArr.Length; i++) {
                Assert.IsTrue(sampleArr[i] >= minValue && sampleArr[i] < maxValue);
            }
        }

        public static void NextByteInner(byte[] sampleArr)
        {
            int[] countArr = new int[256];
            int sampleCount = sampleArr.Length;
            for(int i=0; i < sampleCount; i++) {
                countArr[sampleArr[i]]++;
            }

            double expectedCount = sampleCount / 256;
            double maxExpectedCountErr = sampleCount / 10_000;
            for(int i=0; i < 256; i++)
            {
                double countErr = Math.Abs(countArr[i] - expectedCount);

                Assert.IsTrue(countErr <= maxExpectedCountErr);
            }
        }

        #endregion
    }
}
