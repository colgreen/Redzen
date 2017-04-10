using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Numerics;

namespace Redzen.UnitTests
{
    [TestClass]
    public class SamplingWithoutReplacementTests
    {
        [TestMethod]
        [TestCategory("SamplingWithoutReplacement")]
        public void SampleAllChoices()
        {
            const int size = 5;
            XorShiftRandom rng = new XorShiftRandom();

            // Sample all of the elements.
            int[] sampleArr = SamplingWithoutReplacement.TakeSamples(size, size, rng);

            // Sort the samples.
            Array.Sort(sampleArr);

            // Confirm that all of the choices were selected.
            for(int i=0; i<size; i++) {
                Assert.AreEqual(i, sampleArr[i]);
            }
        }
    }
}
