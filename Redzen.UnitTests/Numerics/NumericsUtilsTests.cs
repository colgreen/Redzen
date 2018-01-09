using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Numerics;
using Redzen.Random;

namespace Redzen.UnitTests.Numerics
{
    [TestClass]
    public class NumericsUtilsTests
    {
        [TestMethod]
        [TestCategory("NumericsUtils")]
        public void TestProbabilisticRound()
        {
            var rng = new XorShiftRandom(0);

            for(int i=0; i < 1000000; i++)
            {
                double valReal = 100 * rng.NextDouble();
                double valRound = NumericsUtils.ProbabilisticRound(valReal, rng);
                Assert.IsTrue(valRound == Math.Floor(valReal) || valRound == Math.Ceiling(valReal));
            }
        }
    }
}
