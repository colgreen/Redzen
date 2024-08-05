using FluentAssertions;
using Xunit;

namespace Redzen.Numerics.Distributions;

public class DiscreteDistributionTestsHalf
{
    [Fact]
    public void Sample()
    {
        var dist = new DiscreteDistribution<Half>(
            [
                (Half)0.688f,
                (Half)0.05f,
                (Half)0.05f,
                (Half)0.05f,
                (Half)0.05f,
                (Half)0.002f,
                (Half)0.01f,
                (Half)0.1f,
            ]);

        var sampler = new DiscreteDistributionSampler<Half>(dist, 0);

        const int sampleCount = 100_000_000;
        int[] histogram = new int[8];

        for (int i=0; i < sampleCount; i++)
            histogram[sampler.Sample()]++;

        for (int i=0; i < histogram.Length; i++)
        {
            double sampleP = histogram[i] / (double)sampleCount;
            double samplePErr = sampleP - (double)dist.Probabilities[i];

            Math.Abs(samplePErr).Should().BeLessThan(0.01);
        }
    }
}
