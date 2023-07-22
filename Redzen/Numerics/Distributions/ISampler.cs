// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;

namespace Redzen.Numerics.Distributions;

/// <summary>
/// Represents some underlying probability distribution (either a continuous or discrete distribution)
/// from which random samples can be taken.
/// </summary>
/// <typeparam name="T">Data type of the samples.</typeparam>
public interface ISampler<T>
    where T : struct, INumber<T>
{
    /// <summary>
    /// Gets a random sample from the distribution.
    /// </summary>
    /// <param name="x">Reference to a variable to store the new sample value in.</param>
    void Sample(out T x);

    /// <summary>
    /// Returns a random sample from the distribution.
    /// </summary>
    /// <returns>A new random sample.</returns>
    T Sample();

    /// <summary>
    /// Fills the provided span with random samples from the distribution.
    /// </summary>
    /// <param name="span">The span to fill with samples.</param>
    void Sample(Span<T> span);
}
