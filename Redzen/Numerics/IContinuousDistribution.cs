/* ***************************************************************************
 * This file is part of the Redzen code library.
 * 
 * Copyright 2015-2017 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */

namespace Redzen.Numerics
{
    /// <summary>
    /// Provides random samples from a continuous distribution.
    /// </summary>
    public interface IContinuousDistribution
    {
        /// <summary>
        /// Get a random sample from the distribution.
        /// </summary>
        double NextDouble();
    }
}
