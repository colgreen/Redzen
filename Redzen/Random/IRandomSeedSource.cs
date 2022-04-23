// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace Redzen.Random;

/// <summary>
/// A source of seed values for use by pseudo-random number generators (PRNGs).
/// </summary>
public interface IRandomSeedSource
{
    /// <summary>
    /// Get a new seed value.
    /// </summary>
    /// <returns>A random <see cref="ulong"/> suitable for seeding a PRNG.</returns>
    ulong GetSeed();
}
