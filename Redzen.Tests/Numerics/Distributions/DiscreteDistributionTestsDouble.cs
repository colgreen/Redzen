using FluentAssertions;
using Xunit;

namespace Redzen.Numerics.Distributions;

public class DiscreteDistributionTestsDouble
{
    [Fact]
    public void Sample()
    {
        var dist = new DiscreteDistribution<double>(
            [
                0.688,
                0.05,
                0.05,
                0.05,
                0.05,
                0.002,
                0.01,
                0.1,
            ]);

        var sampler = new DiscreteDistributionSampler<double>(dist, 0);

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
}
