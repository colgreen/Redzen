using System;
using MathNet.Numerics.Statistics;
using Redzen.Random;
using Xunit;

namespace Redzen.Numerics.Distributions.Double.Tests
{
    public class UniformDistributionTests
    {
        #region Public Test Methods

        [Fact]
        public void Sample()
        {
            int sampleCount = 10_000_000;
            UniformDistributionSampler sampler = new();
            var sampleArr = new double[sampleCount];

            for(int i=0; i < sampleCount; i++)
                sampleArr[i] = sampler.Sample();

            UniformDistributionTest(sampleArr, 0.0, 1.0);

            // Configure a scale and a signed flag.
            sampler = new UniformDistributionSampler(100.0, true);

            for(int i=0; i < sampleCount; i++)
                sampleArr[i] = sampler.Sample();

            UniformDistributionTest(sampleArr, -100.0, 100.0);
        }

        [Fact]
        public void SampleScale()
        {
            int sampleCount = 10_000_000;
            IRandomSource rng = RandomDefaults.CreateRandomSource();
            var sampleArr = new double[sampleCount];

            for(int i=0; i < sampleCount; i++)
                sampleArr[i] = UniformDistribution.Sample(rng, 20.0);

            UniformDistributionTest(sampleArr, 0.0, 20.0);
        }

        [Fact]
        public void SampleScaleSigned()
        {
            int sampleCount = 10_000_000;
            IRandomSource rng = RandomDefaults.CreateRandomSource();
            var sampleArr = new double[sampleCount];

            for(int i=0; i < sampleCount; i++)
                sampleArr[i] = UniformDistribution.SampleSigned(rng, 20.0);

            UniformDistributionTest(sampleArr, -20.0, 20.0);
        }

        [Fact]
        public void SampleUnit()
        {
            int sampleCount = 10_000_000;
            IRandomSource rng = RandomDefaults.CreateRandomSource();
            var sampleArr = new double[sampleCount];

            for(int i=0; i < sampleCount; i++)
                sampleArr[i] = UniformDistribution.Sample(rng);

            UniformDistributionTest(sampleArr, 0, 1.0);
        }

        [Fact]
        public void SampleUnitSigned()
        {
            int sampleCount = 10_000_000;
            IRandomSource rng = RandomDefaults.CreateRandomSource();
            var sampleArr = new double[sampleCount];

            for(int i=0; i < sampleCount; i++)
                sampleArr[i] = UniformDistribution.SampleSigned(rng);

            UniformDistributionTest(sampleArr, -1.0, 1.0);
        }

        #endregion

        #region Private Static Methods

        private static void UniformDistributionTest(double[] sampleArr, double lowerBound, double upperBound)
        {
            Array.Sort(sampleArr);
            RunningStatistics runningStats = new(sampleArr);

            // Skewness should be pretty close to zero (evenly distributed samples)
            Assert.True(Math.Abs(runningStats.Skewness) < 0.01);

            // Mean test.
            double range = upperBound - lowerBound;
            double expectedMean = lowerBound + (range / 2.0);
            double meanErr = expectedMean - runningStats.Mean;
            double maxExpectedErr = range / 1000.0;

            Assert.True(Math.Abs(meanErr) < maxExpectedErr);

            // Test a range of centile/quantile values.
            for(double tau=0.0; tau <= 1.0; tau += 0.1)
            {
                double quantile = SortedArrayStatistics.Quantile(sampleArr, tau);
                double expectedQuantile = lowerBound + (tau * range);
                double quantileError = expectedQuantile - quantile;

                Assert.True(Math.Abs(quantileError) < maxExpectedErr);
            }
        }

        #endregion
    }
}
