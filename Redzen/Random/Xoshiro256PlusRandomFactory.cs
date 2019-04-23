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
    /// A factory of Xoshiro256PlusFactory instances.
    /// </summary>
    public class Xoshiro256PlusRandomFactory : IRandomSourceFactory
    {
        readonly IRandomSeedSource _seedSource;

        #region Constructors

        /// <summary>
        /// Construct with a default seed source.
        /// </summary>
        public Xoshiro256PlusRandomFactory()
        {
            _seedSource = new DefaultRandomSeedSource();
        }

        /// <summary>
        /// Construct with the given seed source.
        /// </summary>
        public Xoshiro256PlusRandomFactory(
            IRandomSeedSource seedSource)
        {
            _seedSource = seedSource;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new instance of Xoshiro256PlusRandom.
        /// </summary>
        public IRandomSource Create()
        {
            ulong seed = _seedSource.GetSeed();
            return new Xoshiro256PlusRandom(seed);
        }

        /// <summary>
        /// Create a new instance of Xoshiro256PlusRandom with the given PRNG seed.
        /// </summary>
        public IRandomSource Create(ulong seed)
        {
            return new Xoshiro256PlusRandom(seed);
        }

        #endregion
    }
}
