/* ***************************************************************************
 * This file is part of the Redzen code library.
 * 
 * Copyright 2015-2020 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */
using System;
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
        /// Fill a span with samples from the distribution, using the provided <see cref="IRandomSource"/> as the source of entropy.
        /// </summary>
        /// <param name="span">The span to fill with samples.</param>
        /// <param name="rng">Random source.</param>
        void Sample(Span<T> span, IRandomSource rng);
    }
}
