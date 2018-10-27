using System;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Numerics.Distributions;

namespace Redzen.UnitTests.Numerics.Distributions.Double
{
    public static class GaussianDistributionTestUtils
    {
        #region Test Methods
        
        public static void TestSimpleStats(ISampler<double> sampler)
        {
            const int sampleCount = 20_000_000;

            RunningStatistics runningStats = new RunningStatistics();
            for (int i = 0; i < sampleCount; i++) {
                runningStats.Push(sampler.Sample());
            }

            Assert.IsTrue(Math.Abs(runningStats.Mean) < 0.001);
            Assert.IsTrue(Math.Abs(runningStats.StandardDeviation-1.0) < 0.0005);
            Assert.IsTrue(Math.Abs(runningStats.Skewness) < 0.01);
            Assert.IsTrue(Math.Abs(runningStats.Kurtosis) < 0.01);
        }

        public static void TestDistribution(ISampler<double> sampler, double mean, double stdDev)
        {
            // Take a set of samples.
            const int sampleCount = 10_000_000;
            double[] sampleArr = new double[sampleCount];

            for (int i = 0; i < sampleCount; i++) {
                sampleArr[i] = sampler.Sample();
            }

            // Sort the ample so that we can use SortedArrayStatistics.
            Array.Sort(sampleArr);

            //// Test a range of centile/quantile values.
            double lowerBound = -5;
            double upperBound = 5;

            double tauStep = (upperBound - lowerBound) / 30.0;

            for(double tau=0; tau <= 1.0; tau += 0.1)
            {
                // Notes. 
                // Here we calc the tau'th quartile over a range of values in he interval [0,1],
                // the resulting quantile is the sample value (and CDF x-axis value) at which the
                // CDF crosses tau on the y-axis.
                //
                // We then take that sample x-axis value, pass it through the CDF function for the
                // gaussian to obtain the expected y value at that x, and compare with tau.

                // Determine the x value at which tau (as a proportion) of samples are <= x.
                double sample_x = SortedArrayStatistics.Quantile(sampleArr, tau);

                // Put sample_x into the gaussian CDF function, to obtain a CDF y coord..
                double cdf_y = 0.5 * SpecialFunctions.Erfc((mean - sample_x) / (stdDev*Constants.Sqrt2));

                // Compare the expected and actual CDF y values.
                double y_error = Math.Abs(tau - cdf_y);
                Assert.IsTrue(y_error < 0.0005);    
            }
        }

        #endregion
    }
}
