/* ***************************************************************************
 * This file is part of the Redzen code library.
 * 
 * Copyright 2015-2019 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */

namespace Redzen.Random
{
    /// <summary>
    /// A factory of IRandomSource instances.
    /// </summary>
    public interface IRandomSourceFactory
    {
        /// <summary>
        /// Create a new IRandomSource.
        /// </summary>
        IRandomSource Create();

        /// <summary>
        /// Create a new IRandomSource with the given PRNG seed.
        /// </summary>
        IRandomSource Create(ulong seed);
    }
}
