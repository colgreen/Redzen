using System;
using Redzen.Random;
using Xunit;

namespace Redzen.Numerics.Tests
{
    public class NumericsUtilsTests
    {
        [Fact]
        public void ProbabilisticRound()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(0);

            for(int i=0; i < 1000000; i++)
            {
                double valReal = 100 * rng.NextDouble();
                double valRound = NumericsUtils.ProbabilisticRound(valReal, rng);
                Assert.True(valRound == Math.Floor(valReal) || valRound == Math.Ceiling(valReal));
            }
        }
    }
}
