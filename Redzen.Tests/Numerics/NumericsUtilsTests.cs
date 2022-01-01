using System;
using FluentAssertions;
using Redzen.Random;
using Xunit;

namespace Redzen.Numerics.Tests
{
    public class NumericsUtilsTests
    {
        [Fact]
        public void StochasticRound()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(0);

            for(int i=0; i < 1_000_000; i++)
            {
                double valReal = 100 * rng.NextDouble();
                double valRound = NumericsUtils.StochasticRound(valReal, rng);

                valRound.Should().BeOneOf(Math.Floor(valReal), Math.Ceiling(valReal));
            }
        }

        [Fact]
        public void BuildHistogramData()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(0);

            int iters = 10_000;
            double[] vals = new double[iters];
            for(int i=0; i < iters; i++)
                vals[i] = 1000.0 + (rng.NextDouble() * 2.0) - 1.0;

            // Construct a histogram on the array of values.
            HistogramData hist = NumericsUtils.BuildHistogramData(vals, 8);

            // We expect samples to be approximately evenly distributed over the histogram buckets.
            for(int i = 0; i < hist.FrequencyArray.Length; i++)
                hist.FrequencyArray[i].Should().BeGreaterThan((int)Math.Ceiling((iters / 8) * 0.8));

            // We expect min and max to be close to 999 and 1001 respectively.
            hist.Max.Should().BeInRange(1000.9, 1001.0);
            hist.Min.Should().BeInRange(999.0, 999.1);
        }
    }
}
