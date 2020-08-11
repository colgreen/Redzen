using Redzen.Numerics.Distributions.Float;
using Xunit;

namespace Redzen.UnitTests
{
    public class MathFSpanUtilsTests
    {
        #region Test Methods

        [Fact]
        public void Clip()
        {
            var sampler = new UniformDistributionSampler(20f, true, 0);

            // Test with a range of array lengths;
            // the vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 20; len++) {
                Clip_Inner(sampler, len);
            }
        }

        [Fact]
        public void MeanSquaredDelta()
        {
            var sampler = new UniformDistributionSampler(10f, true, 0);

            // Test with a range of array lengths;
            // the vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 20; len++) {
                MeanSquaredDelta_Inner(sampler, len);
            }
        }

        [Fact]
        public void SumSquaredDelta()
        {
            var sampler = new UniformDistributionSampler(10f, true, 0);

            // Test with a range of array lengths;
            // the vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 20; len++) {
                SumSquaredDelta_Inner(sampler, len);
            }
        }

        [Fact]
        public void MinMax()
        {
            var sampler = new UniformDistributionSampler(100f, true, 0);

            // Test with a range of array lengths;
            // the vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 20; len++) {
                MinMax_Inner(sampler, len);
            }
        }

        #endregion

        #region Private Static Methods [Test Subroutines]

        private static void Clip_Inner(UniformDistributionSampler sampler, int len)
        {
            // Alloc array and fill with uniform random noise.
            float[] x = new float[len];
            sampler.Sample(x);

            // Clip the elements of the array with the safe routine.
            float[] expected = (float[])x.Clone();
            PointwiseClip(expected, -1.1f, 18.8f);

            // Clip the elements of the array.
            float[] actual = (float[])x.Clone();
            MathFSpanUtils.Clip(actual, -1.1f, 18.8f);

            // Compare expected with actual array.
            Assert.True(SpanUtils.Equals<float>(expected, actual));
        }

        private static void SumSquaredDelta_Inner(UniformDistributionSampler sampler, int len)
        {
            // Alloc arrays and fill with uniform random noise.
            float[] a = new float[len];
            float[] b = new float[len];
            sampler.Sample(a);
            sampler.Sample(b);

            // Calc results and compare.
            float expected = PointwiseSumSquaredDelta(a, b);
            float actual = MathFSpanUtils.SumSquaredDelta(a, b);
            Assert.Equal(expected, actual, 10);
        }

        private static void MeanSquaredDelta_Inner(UniformDistributionSampler sampler, int len)
        {
            // Alloc arrays and fill with uniform random noise.
            float[] a = new float[len];
            float[] b = new float[len];
            sampler.Sample(a);
            sampler.Sample(b);

            // Calc results and compare.
            float expected = PointwiseSumSquaredDelta(a, b) / a.Length;
            float actual = MathFSpanUtils.MeanSquaredDelta(a, b);
            Assert.Equal(expected, actual, 10);
        }

        private static void MinMax_Inner(UniformDistributionSampler sampler, int len)
        {
            // Alloc arrays and fill with uniform random noise.
            float[] a = new float[len];
            sampler.Sample(a);

            // Calc results and compare.
            PointwiseMinMax(a, out float expectedMin, out float expectedMax);
            MathFSpanUtils.MinMax(a, out float actualMin, out float actualMax);

            Assert.Equal(expectedMin, actualMin, 10);
            Assert.Equal(expectedMax, actualMax, 10);
        }

        #endregion

        #region Private Static Methods [Scalar Math Routines]

        private static void PointwiseClip(float[] x, float min, float max)
        {
            for(int i=0; i < x.Length; i++)
            {
                if(x[i] < min)
                    x[i] = min;
                else if(x[i] > max)
                    x[i] = max;
            }
        }

        private static float PointwiseSumSquaredDelta(float[] a, float[] b)
        {
            float total = 0f;

            // Calc sum(squared error).
            for(int i=0; i < a.Length; i++)
            {
                float err = a[i] - b[i];
                total += err * err;
            }

            return total;
        }

        private static void PointwiseMinMax(float[] a, out float min, out float max)
        {
            min = max = a[0];
            for(int i=1; i < a.Length; i++)
            {
                float val = a[i];
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
