using System;
using Redzen.Numerics.Distributions.Double;
using Xunit;

namespace Redzen.Tests
{
    public class MathSpanDoubleTests
    {
        #region Test Methods

        [Fact]
        public void Sum()
        {
            var sampler = new UniformDistributionSampler(20.0, true, 0);

            // Test with a range of array lengths;
            // the vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 20; len++) {
                Sum_Inner(sampler, len);
            }
        }

        [Fact]
        public void MedianOfSorted()
        {
            // Empty array.
            var arr = new double[0];
            Assert.Throws<ArgumentException>(() => MathSpan.MedianOfSorted(arr));

            // Single element.
            arr = new double[] { 5 };
            double actual = MathSpan.MedianOfSorted(arr);
            Assert.Equal(5, actual);

            // Two elements.
            arr = new double[] { 2, 4 };
            actual = MathSpan.MedianOfSorted(arr);
            Assert.Equal(3.0, actual);

            // Three elements.
            arr = new double[] { 1, 2, 3 };
            actual = MathSpan.MedianOfSorted(arr);
            Assert.Equal(2, actual);

            // Five elements.
            arr = new double[] { 1, 2, 3, 4, 5 };
            actual = MathSpan.MedianOfSorted(arr);
            Assert.Equal(3, actual);

            // Six elements.
            arr = new double[] { 1, 2, 3, 4, 5, 6 };
            actual = MathSpan.MedianOfSorted(arr);
            Assert.Equal(3.5, actual);
        }

        [Fact]
        public void Clip()
        {
            var sampler = new UniformDistributionSampler(20.0, true, 0);

            // Test with a range of array lengths;
            // the vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 40; len++) {
                Clip_Inner(sampler, len);
            }
        }

        [Fact]
        public void MinMax()
        {
            var sampler = new UniformDistributionSampler(100.0, true, 0);

            // Test with a range of array lengths;
            // the vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 40; len++) {
                MinMax_Inner(sampler, len);
            }
        }

        [Fact]
        public void MeanSquaredDelta()
        {
            var sampler = new UniformDistributionSampler(100.0, true, 0);

            // Test with a range of array lengths;
            // the vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 40; len++) {
                MeanSquaredDelta_Inner(sampler, len);
            }
        }

        [Fact]
        public void SumSquaredDelta()
        {
            var sampler = new UniformDistributionSampler(100.0, true, 0);

            // Test with a range of array lengths;
            // the vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 40; len++) {
                SumSquaredDelta_Inner(sampler, len);
            }
        }

        #endregion

        #region Private Static Methods [Test Subroutines]

        private static void Sum_Inner(UniformDistributionSampler sampler, int len)
        {
            // Alloc array and fill with uniform random noise.
            double[] x = new double[len];
            sampler.Sample(x);

            // Sum the array elements.
            double expected = PointwiseSum(x);
            double actual = MathSpan.Sum(x);

            // Compare expected and actual sum.
            Assert.Equal(expected, actual, 12);
        }

        private static void Clip_Inner(UniformDistributionSampler sampler, int len)
        {
            // Alloc array and fill with uniform random noise.
            double[] x = new double[len];
            sampler.Sample(x);

            // Clip the elements of the array with the safe routine.
            double[] expected = (double[])x.Clone();
            PointwiseClip(expected, -1.1, 18.8);

            // Clip the elements of the array.
            double[] actual = (double[])x.Clone();
            MathSpan.Clip(actual, -1.1, 18.8);

            // Compare expected with actual array.
            Assert.True(SpanUtils.Equal<double>(expected, actual));
        }

        private static void MinMax_Inner(UniformDistributionSampler sampler, int len)
        {
            // Alloc arrays and fill with uniform random noise.
            double[] a = new double[len];
            sampler.Sample(a);

            // Calc results and compare.
            PointwiseMinMax(a, out double expectedMin, out double expectedMax);
            MathSpan.MinMax(a, out double actualMin, out double actualMax);

            Assert.Equal(expectedMin, actualMin, 10);
            Assert.Equal(expectedMax, actualMax, 10);
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
            double actual = MathSpan.SumSquaredDelta(a, b);
            Assert.Equal(expected, actual, 8);
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
            double actual = MathSpan.MeanSquaredDelta(a, b);
            Assert.Equal(expected, actual, 10);
        }

        #endregion

        #region Private Static Methods [Scalar Math Routines]

        private static double PointwiseSum(double[] x)
        {
            double sum = 0.0;
            for(int i=0; i < x.Length; i++) {
                sum += x[i];
            }
            return sum;
        }

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

        #endregion
    }
}
