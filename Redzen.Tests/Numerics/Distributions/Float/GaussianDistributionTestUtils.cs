using System;
using FluentAssertions;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;

namespace Redzen.Numerics.Distributions.Float.Tests;

public static class GaussianDistributionTestUtils
{
    #region Test Methods

    public static void TestSimpleStats(ISampler<float> sampler)
    {
        const int sampleCount = 20_000_000;

        RunningStatistics runningStats = new();
        for(int i=0; i < sampleCount; i++)
            runningStats.Push(sampler.Sample());

        Math.Abs(runningStats.Mean).Should().BeLessThan(0.001);
        Math.Abs(runningStats.StandardDeviation-1.0).Should().BeLessThan(0.0005);
        Math.Abs(runningStats.Skewness).Should().BeLessThan(0.01);
        Math.Abs(runningStats.Kurtosis).Should().BeLessThan(0.01);
    }

    public static void TestDistribution(ISampler<float> sampler, float mean, float stdDev)
    {
        // Take a set of samples.
        const int sampleCount = 10_000_000;
        float[] sampleArr = new float[sampleCount];

        sampler.Sample(sampleArr);

        // Sort the samples so that we can use SortedArrayStatistics.
        Array.Sort(sampleArr);

        for(float tau=0f; tau <= 1f; tau += 0.1f)
        {
            // Notes.
            // Here we calc the tau'th quartile over a range of values in he interval [0,1],
            // the resulting quantile is the sample value (and CDF x-axis value) at which the
            // CDF crosses tau on the y-axis.
            //
            // We then take that sample x-axis value, pass it through the CDF function for the
            // Gaussian to obtain the expected y value at that x, and compare with tau.

            // Determine the x value at which tau (as a proportion) of samples are <= x.
            float sample_x = SortedArrayStatistics.Quantile(sampleArr, tau);

            // Put sample_x into the Gaussian CDF function, to obtain a CDF y coord..
            double cdf_y = (0.5 * SpecialFunctions.Erfc((mean - sample_x) / (stdDev*Constants.Sqrt2)));

            // Compare the expected and actual CDF y values.
            double y_error = Math.Abs(tau - cdf_y);
            y_error.Should().BeLessThan(0.001);
        }
    }

    #endregion
}
