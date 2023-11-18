using FluentAssertions;
using Xunit;

namespace Redzen.Numerics.Distributions;

#pragma warning disable CA1861 // Avoid constant arrays as arguments

public class DiscreteDistributionTestsSingle
{
    [Fact]
    public void Sample()
    {
        var dist = new DiscreteDistribution<float>(
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

        var sampler = new DiscreteDistributionSampler<float>(dist, 0);

        const int sampleCount = 100_000_000;
        int[] histogram = new int[8];

        for (int i = 0; i < sampleCount; i++)
            histogram[sampler.Sample()]++;

        for (int i = 0; i < histogram.Length; i++)
        {
            double sampleP = histogram[i] / (double)sampleCount;
            double samplePErr = sampleP - dist.Probabilities[i];

            Math.Abs(samplePErr).Should().BeLessThan(0.0001);
        }
    }
}
