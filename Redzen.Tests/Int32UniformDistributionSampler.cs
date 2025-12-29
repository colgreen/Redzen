using Redzen.Numerics.Distributions;
using Redzen.Random;

namespace Redzen;

internal sealed class Int32UniformDistributionSampler : ISampler<int>
{
    readonly Func<IRandomSource, int> _sampleFn;
    readonly IRandomSource _rng;

    public double Max { get; }
    public bool Signed { get; }

    /// <summary>
    /// Construct with the given distribution and a new random source.
    /// </summary>
    /// <param name="max">Maximum absolute value.</param>
    /// <param name="signed">Indicates if the distribution interval includes negative values.</param>
    /// <param name="seed">Random source seed.</param>
    public Int32UniformDistributionSampler(int max, bool signed, ulong seed)
        : this(max, signed, RandomDefaults.CreateRandomSource(seed))
    {
    }

    /// <summary>
    /// Construct with the given distribution and a random source.
    /// </summary>
    /// <param name="max">Maximum absolute value.</param>
    /// <param name="signed">Indicates if the distribution interval includes negative values.</param>
    /// <param name="rng">Random source.</param>
    public Int32UniformDistributionSampler(int max, bool signed, IRandomSource rng)
    {
        Max = max;
        Signed = signed;
        _rng = rng;

        // Note. We predetermine which of these two function variants to use at construction time,
        // thus avoiding a branch on each invocation of Sample() (i.e. this is a micro-optimization).
        if(signed)
            _sampleFn = (r) => _rng.Next(-max, max);
        else
            _sampleFn = (r) => _rng.Next(max);
    }

    public void Sample(out int x)
    {
        x = _sampleFn(_rng);
    }

    public int Sample()
    {
        return _sampleFn(_rng);
    }

    public void Sample(Span<int> span)
    {
        for(int i=0; i < span.Length; i++)
            span[i] = _sampleFn(_rng);
    }
}
