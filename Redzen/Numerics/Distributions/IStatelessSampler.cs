// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;

namespace Redzen.Numerics.Distributions;

/// <summary>
/// Represents some underlying probability distribution (a continuous or discrete distribution)
/// from which random samples can be taken.
///
/// The source of entropy for the random samples is an <see cref="IRandomSource"/> provided on
/// each method call, hence the sampler instance itself is stateless.
/// </summary>
/// <typeparam name="T">Data type of the individual samples.</typeparam>
public interface IStatelessSampler<T>
    where T : struct
{
    /// <summary>
    /// Gets a random sample from the distribution.
    /// </summary>
    /// <param name="x">Reference to a variable to store the new sample value in.</param>
    /// <param name="rng">Random source.</param>
    void Sample(out T x, IRandomSource rng);

    /// <summary>
    /// Returns a random sample from the distribution, using the provided <see cref="IRandomSource"/> as the source of entropy.
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <returns>A new random sample.</returns>
    T Sample(IRandomSource rng);

    /// <summary>
    /// Fills the provided span with random samples from the distribution,
    /// using the provided <see cref="IRandomSource"/> as the source of entropy.
    /// </summary>
    /// <param name="span">The span to fill with samples.</param>
    /// <param name="rng">Random source.</param>
    void Sample(Span<T> span, IRandomSource rng);
}
