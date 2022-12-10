using FluentAssertions;
using Redzen.Random;
using Xunit;

namespace Redzen.Numerics;

public class HistogramDataTests
{
    [Fact]
    public void BuildHistogramData()
    {
        IRandomSource rng = RandomDefaults.CreateRandomSource(0);

        const int iters = 10_000;
        double[] vals = new double[iters];
        for(int i = 0; i < iters; i++)
            vals[i] = 1000.0 + (rng.NextDouble() * 2.0) - 1.0;

        // Construct a histogram on the array of values.
        HistogramData hd = HistogramData.BuildHistogramData(vals, 8);
        Span<HistogramBin> binSpan = hd.GetBinSpan();

        // We expect samples to be approximately evenly distributed over the histogram bins.
        for(int i = 0; i < binSpan.Length; i++)
            binSpan[i].Frequency.Should().BeGreaterThan((int)Math.Ceiling((iters / 8) * 0.8));

        // We expect min and max to be close to 999 and 1001 respectively.
        hd.Max.Should().BeInRange(1000.9, 1001.0);
        hd.Min.Should().BeInRange(999.0, 999.1);
    }
}
