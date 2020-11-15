using System;
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
                Assert.True(valRound == Math.Floor(valReal) || valRound == Math.Ceiling(valReal));
            }
        }
    }
}
