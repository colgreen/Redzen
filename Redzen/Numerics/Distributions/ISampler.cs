/* ***************************************************************************
 * This file is part of the Redzen code library.
 *
 * Copyright 2015-2021 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */

using System;

namespace Redzen.Numerics.Distributions
{
    /// <summary>
    /// Represents some underlying probability distribution (either a continuous or discrete distribution)
    /// from which random samples can be taken.
    /// </summary>
    /// <typeparam name="T">Data type of the samples.</typeparam>
    public interface ISampler<T> where T : struct
    {
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
}
