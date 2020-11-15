using Redzen.Numerics.Distributions.Double;
using Xunit;

namespace Redzen.Tests
{
    public class MathSpanUtilsTests
    {
        #region Test Methods

        [Fact]
        public void Clamp()
        {
            var sampler = new UniformDistributionSampler(20.0, true, 0);

            // Test with a range of array lengths;
            // the vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 20; len++) {
                Clamp_Inner(sampler, len);
            }
        }

        [Fact]
        public void MeanSquaredDelta()
        {
            var sampler = new UniformDistributionSampler(100.0, true, 0);

            // Test with a range of array lengths;
            // the vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 20; len++) {
                MeanSquaredDelta_Inner(sampler, len);
            }
        }

        [Fact]
        public void SumSquaredDelta()
        {
            var sampler = new UniformDistributionSampler(100.0, true, 0);

            // Test with a range of array lengths;
            // the vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 20; len++) {
                SumSquaredDelta_Inner(sampler, len);
            }
        }

        [Fact]
        public void MinMax()
        {
            var sampler = new UniformDistributionSampler(100.0, true, 0);

            // Test with a range of array lengths;
            // the vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 20; len++) {
                MinMax_Inner(sampler, len);
            }
        }

        #endregion

        #region Private Static Methods [Test Subroutines]

        private static void Clamp_Inner(UniformDistributionSampler sampler, int len)
        {
            // Alloc array and fill with uniform random noise.
            double[] x = new double[len];
            sampler.Sample(x);

            // Clip the elements of the array with the safe routine.
            double[] expected = (double[])x.Clone();
            PointwiseClip(expected, -1.1, 18.8);

            // Clip the elements of the array.
            double[] actual = (double[])x.Clone();
            MathSpanUtils.Clamp(actual, -1.1, 18.8);

            // Compare expected with actual array.
            Assert.True(SpanUtils.Equal<double>(expected, actual));
        }

        private static void SumSquaredDelta_Inner(UniformDistributionSampler sampler, int len)
        {
            // Alloc arrays and fill with uniform random noise.
            double[] a = new double[len];
            double[] b = new double[len];
            sampler.Sample(a);
            sampler.Sample(b);

            // Calc results and compare.
            double expected = PointwiseSumSquaredDelta(a, b);
            double actual = MathSpanUtils.SumSquaredDelta(a, b);
            Assert.Equal(expected, actual, 10);
        }

        private static void MeanSquaredDelta_Inner(UniformDistributionSampler sampler, int len)
        {
            // Alloc arrays and fill with uniform random noise.
            double[] a = new double[len];
            double[] b = new double[len];
            sampler.Sample(a);
            sampler.Sample(b);

            // Calc results and compare.
            double expected = PointwiseSumSquaredDelta(a, b) / a.Length;
            double actual = MathSpanUtils.MeanSquaredDelta(a, b);
            Assert.Equal(expected, actual, 10);
        }

        private static void MinMax_Inner(UniformDistributionSampler sampler, int len)
        {
            // Alloc arrays and fill with uniform random noise.
            double[] a = new double[len];
            sampler.Sample(a);

            // Calc results and compare.
            PointwiseMinMax(a, out double expectedMin, out double expectedMax);
            MathSpanUtils.MinMax(a, out double actualMin, out double actualMax);

            Assert.Equal(expectedMin, actualMin, 10);
            Assert.Equal(expectedMax, actualMax, 10);
        }

        #endregion

        #region Private Static Methods [Scalar Math Routines]

        private static void PointwiseClip(double[] x, double min, double max)
        {
            for(int i=0; i < x.Length; i++)
            {
                if(x[i] < min)
                    x[i] = min;
                else if(x[i] > max)
                    x[i] = max;
            }
        }

        private static double PointwiseSumSquaredDelta(double[] a, double[] b)
        {
            double total = 0.0;

            // Calc sum(squared error).
            for(int i=0; i < a.Length; i++)
            {
                double err = a[i] - b[i];
                total += err * err;
            }

            return total;
        }

        private static void PointwiseMinMax(double[] a, out double min, out double max)
        {
            min = max = a[0];
            for(int i=1; i < a.Length; i++)
            {
                double val = a[i];
                if(val < min) {
                    min = val;
                }
                else if(val > max) {
                    max = val;
                }
            }
        }

        #endregion
    }
}
