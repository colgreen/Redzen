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
    /// A factory of Xoshiro512StarStarRandom instances.
    /// </summary>
    public class Xoshiro512StarStarRandomFactory : IRandomSourceFactory
    {
        readonly IRandomSeedSource _seedSource;

        #region Constructors

        /// <summary>
        /// Construct with a default seed source.
        /// </summary>
        public Xoshiro512StarStarRandomFactory()
        {
            _seedSource = new DefaultRandomSeedSource();
        }

        /// <summary>
        /// Construct with the given seed source.
        /// </summary>
        public Xoshiro512StarStarRandomFactory(
            IRandomSeedSource seedSource)
        {
            _seedSource = seedSource;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new instance of Xoshiro512StarStarRandom.
        /// </summary>
        public IRandomSource Create()
        {
            ulong seed = _seedSource.GetSeed();
            return new Xoshiro512StarStarRandom(seed);
        }

        /// <summary>
        /// Create a new instance of Xoshiro512StarStarRandom with the given PRNG seed.
        /// </summary>
        public IRandomSource Create(ulong seed)
        {
            return new Xoshiro512StarStarRandom(seed);
        }

        #endregion
    }
}
