using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Numerics;
using Redzen.Random;

namespace Redzen.UnitTests.Numerics
{
    [TestClass]
    public class DiscreteDistributionTests
    {
        [TestMethod]
        [TestCategory("DiscreteDistribution")]
        public void Sample()
        {
            var dist = new DiscreteDistribution(
                new double[] 
                { 
                    0.688,
                    0.05,
                    0.05,
                    0.05,
                    0.05,
                    0.002,
                    0.01,
                    0.1,
                }, 0);

            const int sampleCount = 100_000_000;
            int[] histogram = new int[8];

            for(int i=0; i < sampleCount; i++)
            {
                histogram[dist.Sample()]++;
            }

            for(int i=0; i < histogram.Length; i++)
            {
                double sampleP = histogram[i] / (double)sampleCount;
                double samplePErr = sampleP - (dist.Probabilities[i]);

                Assert.IsTrue(Math.Abs(samplePErr) < 0.0001);
            }
        }

        [TestMethod]
        [TestCategory("DiscreteDistribution")]
        public void SampleUniformWithoutReplacement_SampleAllChoices()
        {
            const int size = 5;
            XorShiftRandom rng = new XorShiftRandom();

            // Sample all of the elements.
            int[] sampleArr = new int[size];
            DiscreteDistributionUtils.SampleUniformWithoutReplacement(size, sampleArr, rng);

            // Sort the samples.
            Array.Sort(sampleArr);

            // Confirm that all of the choices were selected.
            for(int i=0; i<size; i++) {
                Assert.AreEqual(i, sampleArr[i]);
            }
        }
    }
}
