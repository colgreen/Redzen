using FluentAssertions;
using Redzen.Random;
using Xunit;

namespace Redzen.Numerics.Distributions.Float.Tests;

public class DiscreteDistributionTests
{
    [Fact]
    public void Sample()
    {
        var dist = new DiscreteDistribution(
            new float[]
            {
                0.688f,
                0.05f,
                0.05f,
                0.05f,
                0.05f,
                0.002f,
                0.01f,
                0.1f,
            });

        var sampler = new DiscreteDistributionSampler(dist, 0);

        const int sampleCount = 100_000_000;
        int[] histogram = new int[8];

        for(int i=0; i < sampleCount; i++)
            histogram[sampler.Sample()]++;

        for(int i=0; i < histogram.Length; i++)
        {
            double sampleP = histogram[i] / (double)sampleCount;
            double samplePErr = sampleP - (dist.Probabilities[i]);

            Math.Abs(samplePErr).Should().BeLessThan(0.0001);
        }
    }

    [Fact]
    public void SampleUniformWithoutReplacement_SampleAllChoices()
    {
        const int size = 5;
        IRandomSource rng = RandomDefaults.CreateRandomSource();

        // Sample all of the elements.
        int[] sampleArr = new int[size];
        DiscreteDistribution.SampleUniformWithoutReplacement(rng, size, sampleArr);

        // Sort the samples.
        Array.Sort(sampleArr);

        // Confirm that all of the choices were selected.
        for(int i=0; i < size; i++)
            sampleArr[i].Should().Be(i);
    }

    [Fact]
    public void SampleUniformWithoutReplacement_SampleSubsetOfChoices()
    {
        const int numberOfOutcomes = 1000;

        IRandomSource rng = RandomDefaults.CreateRandomSource();

        // Sample all of the elements.
        int[] sampleArr = new int[100];
        DiscreteDistribution.SampleUniformWithoutReplacement(rng, numberOfOutcomes, sampleArr);

        // Sort the samples.
        Array.Sort(sampleArr);

        // Confirm that the choices are in the defined range.
        sampleArr.Should().OnlyContain(x => x >=0 && x < numberOfOutcomes);
    }
}
