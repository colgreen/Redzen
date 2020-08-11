using System;
using MathNet.Numerics.Statistics;
using Xunit;

namespace Redzen.Random.Tests
{
    internal static class RandomTestUtils
    {
        #region Private Static Methods

        public static double[] CreateSampleArray(int length, Func<double> sampleFn)
        {
            double[] arr = new double[length];
            for(int i=0; i < length; i++) {
                arr[i] = sampleFn();
            }
            return arr;
        }

        public static void UniformDistributionTest(
            double[] sampleArr, double minValue, double maxValue)
        {
            Array.Sort(sampleArr);
            RunningStatistics runningStats = new RunningStatistics(sampleArr);

            // Skewness should be pretty close to zero (evenly distributed samples)
            Assert.True(Math.Abs(runningStats.Skewness) <= 0.01);
            
            // Mean test.
            double range = maxValue - minValue;
            double expectedMean = minValue + (range / 2.0);
            double meanErr = expectedMean - runningStats.Mean;
            double maxExpectedErr = range / 1000.0;
            Assert.True(Math.Abs(meanErr) <= maxExpectedErr);

            // Test a range of centile/quantile values.
            for(double tau=0; tau <= 1.0; tau += 0.1)
            {
                double quantile = SortedArrayStatistics.Quantile(sampleArr, tau);
                double expectedQuantile = minValue + (tau * range);
                double quantileError = expectedQuantile - quantile;
                Assert.True(Math.Abs(quantileError) <= maxExpectedErr);
            }

            // Test that no samples are outside the defined range.
            for(int i=0; i < sampleArr.Length; i++) {
                Assert.True(sampleArr[i] >= minValue && sampleArr[i] < maxValue);
            }
        }

        public static void UniformDistributionTest(byte[] sampleArr)
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
                Assert.True(countErr <= maxExpectedCountErr);
            }
        }

        #endregion
    }
}
