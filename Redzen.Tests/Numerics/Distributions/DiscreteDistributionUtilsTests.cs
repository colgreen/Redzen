using FluentAssertions;
using Redzen.Random;
using Xunit;

namespace Redzen.Numerics.Distributions;

public class DiscreteDistributionUtilsTests
{
    [Fact]
    public void SampleUniformWithoutReplacement_SampleAllChoices()
    {
        const int size = 5;
        IRandomSource rng = RandomDefaults.CreateRandomSource();

        // Sample all of the elements.
        int[] sampleArr = new int[size];
        DiscreteDistributionUtils.SampleUniformWithoutReplacement(size, sampleArr, rng);

        // Sort the samples.
        Array.Sort(sampleArr);

        // Confirm that all of the choices were selected.
        for(int i = 0; i < size; i++)
            sampleArr[i].Should().Be(i);
    }

    [Fact]
    public void SampleUniformWithoutReplacement_SampleSubsetOfChoices()
    {
        const int numberOfOutcomes = 1000;

        IRandomSource rng = RandomDefaults.CreateRandomSource();

        // Sample all of the elements.
        int[] sampleArr = new int[100];
        DiscreteDistributionUtils.SampleUniformWithoutReplacement(numberOfOutcomes, sampleArr, rng);

        // Sort the samples.
        Array.Sort(sampleArr);

        // Confirm that the choices are in the defined range.
        sampleArr.Should().OnlyContain(x => x >= 0 && x < numberOfOutcomes);
    }
}
