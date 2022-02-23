/* ***************************************************************************
 * This file is part of the Redzen code library.
 *
 * Copyright 2015-2022 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */
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
