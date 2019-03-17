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
        public void SumSquaredDelta()
        {
            var sampler = new UniformDistributionSampler(100.0, true, 0);

            // Test multiple times, with a range of array lengths;
            // The vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 20; len++) {
                SumSquaredDelta(sampler, len);
            }
        }

        [TestMethod]
        [TestCategory("MathArrayUtils")]
        public void MeanSquaredDelta()
        {
            var sampler = new UniformDistributionSampler(100.0, true, 0);

            // Test multiple times, with a range of array lengths;
            // The vectorised code has edge cases related to array length, so this is a sensible test to do.
            for(int len = 1; len < 20; len++) {
                MeanSquaredDelta(sampler, len);
            }
        }

        #endregion

        #region Private Static Methods

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

        #endregion
    }
}
