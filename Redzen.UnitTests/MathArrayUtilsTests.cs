using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Numerics.Distributions.Double;

namespace Redzen.UnitTests
{
    [TestClass]
    public class MathArrayUtilsTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("MathArrayUtils")]
        public void Clip()
        {
            var sampler = new UniformDistributionSampler(20.0, true, 0);

            // Test with a range of array lengths;
            // the vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 20; len++) {
                Clip(sampler, len);
            }
        }

        [TestMethod]
        [TestCategory("MathArrayUtils")]
        public void MeanSquaredDelta()
        {
            var sampler = new UniformDistributionSampler(100.0, true, 0);

            // Test with a range of array lengths;
            // the vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 20; len++) {
                MeanSquaredDelta(sampler, len);
            }
        }

        [TestMethod]
        [TestCategory("MathArrayUtils")]
        public void SumSquaredDelta()
        {
            var sampler = new UniformDistributionSampler(100.0, true, 0);

            // Test with a range of array lengths;
            // the vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 20; len++) {
                SumSquaredDelta(sampler, len);
            }
        }

        [TestMethod]
        [TestCategory("MathArrayUtils")]
        public void MinMax()
        {
            var sampler = new UniformDistributionSampler(100.0, true, 0);

            // Test with a range of array lengths;
            // the vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 20; len++) {
                MinMax(sampler, len);
            }
        }

        #endregion

        #region Private Static Methods [Test Subroutines]

        private static void Clip(UniformDistributionSampler sampler, int len)
        {
            // Alloc array and fill with uniform random noise.
            double[] x = new double[len];
            sampler.Sample(x);

            // Clip the elements of the array with the safe routine.
            double[] expected = (double[])x.Clone();
            Clip(expected, -1.1, 18.8);

            // Clip the elements of the array.
            double[] actual = (double[])x.Clone();
            MathArrayUtils.Clip(actual, -1.1, 18.8);

            // Compare expected with actual array.
            Assert.IsTrue(ArrayUtils.Equals(expected, actual));
        }

        private static void SumSquaredDelta(UniformDistributionSampler sampler, int len)
        {
            // Alloc arrays and fill with uniform random noise.
            double[] a = new double[len];
            double[] b = new double[len];
            sampler.Sample(a);
            sampler.Sample(b);

            // Calc results and compare.
            double expected = SumSquaredDelta(a, b);
            double actual = MathArrayUtils.SumSquaredDelta(a, b);
            Assert.AreEqual(expected, actual, 1e-10);
        }

        private static void MeanSquaredDelta(UniformDistributionSampler sampler, int len)
        {
            // Alloc arrays and fill with uniform random noise.
            double[] a = new double[len];
            double[] b = new double[len];
            sampler.Sample(a);
            sampler.Sample(b);

            // Calc results and compare.
            double expected = SumSquaredDelta(a, b) / a.Length;
            double actual = MathArrayUtils.MeanSquaredDelta(a, b);
            Assert.AreEqual(expected, actual, 1e-10);
        }

        private static void MinMax(UniformDistributionSampler sampler, int len)
        {
            // Alloc arrays and fill with uniform random noise.
            double[] a = new double[len];
            sampler.Sample(a);

            // Calc results and compare.
            MinMax(a, out double expectedMin, out double expectedMax);
            MathArrayUtils.MinMax(a, out double actualMin, out double actualMax);

            Assert.AreEqual(expectedMin, actualMin, 1e-10);
            Assert.AreEqual(expectedMax, actualMax, 1e-10);
        }

        #endregion

        #region Private Static Methods [Scalar Math Routines]

        private static void Clip(double[] x, double min, double max)
        {
            for(int i=0; i < x.Length; i++)
            {
                if(x[i] < min)
                    x[i] = min;
                else if(x[i] > max)
                    x[i] = max;
            }
        }

        private static double SumSquaredDelta(double[] a, double[] b)
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

        private static void MinMax(double[] a, out double min, out double max)
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
