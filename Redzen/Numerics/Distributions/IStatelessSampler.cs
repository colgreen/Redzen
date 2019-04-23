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
using Redzen.Random;

namespace Redzen.Numerics.Distributions
{
    /// <summary>
    /// For taking random samples from some underlying distribution, either a continuous or discrete distribution.
    /// 
    /// The source of entropy for the random samples is an <see cref="IRandomSource"/> provided on each call to 
    /// <see cref="Sample(IRandomSource)"/>, hence the sampler instance itself is stateless.
    /// </summary>
    /// <typeparam name="T">Data type of the individual samples.</typeparam>
    public interface IStatelessSampler<T> where T : struct
    {
        /// <summary>
        /// Take a sample from the distribution, using the provided <see cref="IRandomSource"/> as the source of entropy.
        /// </summary>
        T Sample(IRandomSource rng);

        /// <summary>
        /// Fill an array with samples from a distribution, using the provided <see cref="IRandomSource"/> as the source of entropy.
        /// </summary>
        /// <param name="buf">The array to fill with samples.</param>
        void Sample(IRandomSource rng, T[] buf);
    }
}
