using FluentAssertions;
using Redzen.Random;
using Xunit;

namespace Redzen.Numerics;

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

            valRound.Should().BeOneOf(Math.Floor(valReal), Math.Ceiling(valReal));
        }
    }
}
