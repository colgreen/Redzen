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
/// A factory of IRandomSource instances.
/// </summary>
public interface IRandomSourceFactory
{
    /// <summary>
    /// Creates a new <see cref="IRandomSource"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="IRandomSource"/>.</returns>
    IRandomSource Create();

    /// <summary>
    /// Creates a new <see cref="IRandomSource"/> with the given PRNG seed.
    /// </summary>
    /// <param name="seed">Seed value.</param>
    /// <returns>A new instance of <see cref="IRandomSource"/>.</returns>
    IRandomSource Create(ulong seed);
}
