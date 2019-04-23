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
using System;
using Redzen.Random;

namespace Redzen.Numerics.Distributions
{
    /// <summary>
    /// Static factory methods for creating instances of ISampler{T} and IStatelessSampler{T} for sampling from Gaussian distributions.
    /// </summary>
    public class GaussianDistributionSamplerFactory
    {
        #region Public Static Methods [ISampler Factory Methods]

        /// <summary>
        /// Create a sampler for the standard Gaussian distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateSampler<T>() 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource();
            return CreateSampler<T>(0.0, 1.0, rng);
        }

        /// <summary>
        /// Create a sampler for the standard Gaussian distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <param name="seed">Random source seed.</param>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateSampler<T>(ulong seed) 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(seed);
            return CreateSampler<T>(0.0, 1.0, rng);
        }

        /// <summary>
        /// Create a sampler for the standard Gaussian distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateSampler<T>(IRandomSource rng) 
            where T : struct
        {
            return CreateSampler<T>(0.0, 1.0, rng);
        }

        /// <summary>
        /// Create a sampler for the Gaussian distribution with the specified mean and standard deviation.
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateSampler<T>(double mean, double stdDev) 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource();
            return CreateSampler<T>(mean, stdDev, rng);
        }

        /// <summary>
        /// Create a sampler for the Gaussian distribution with the specified mean and standard deviation.
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <param name="seed">Random source seed.</param>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateSampler<T>(double mean, double stdDev, ulong seed) 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(seed);
            return CreateSampler<T>(mean, stdDev, rng);
        }

        /// <summary>
        /// Create a sampler for the Gaussian distribution with the specified mean and standard deviation.
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <param name="rng">Random source.</param>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateSampler<T>(double mean, double stdDev, IRandomSource rng) 
            where T : struct
        {
            if(typeof(T) == typeof(double)) 
            {
                return (ISampler<T>)new Double.ZigguratGaussianSampler(mean, stdDev, rng);
            }
            else if(typeof(T) == typeof(float)) 
            {
                return (ISampler<T>)new Float.ZigguratGaussianSampler((float)mean, (float)stdDev, rng);   
            }
            else 
            {
                throw new ArgumentException("Unsupported type argument");
            }
        }

        #endregion

        #region Public Static Methods [IStatelessSampler Factory Methods]

        /// <summary>
        /// Create a stateless sampler for the standard Gaussian distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <returns>A new instance of <see cref="IStatelessSampler{T}"/>.</returns>
        public static IStatelessSampler<T> CreateStatelessSampler<T>() 
            where T : struct
        {
            return CreateStatelessSampler<T>(0.0, 1.0);
        }

        /// <summary>
        /// Create a stateless sampler for the Gaussian distribution with the specified mean and standard deviation.
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <returns>A new instance of <see cref="IStatelessSampler{T}"/>.</returns>
        public static IStatelessSampler<T> CreateStatelessSampler<T>(double mean, double stdDev) 
            where T : struct
        {
            if(typeof(T) == typeof(double)) 
            {
                return (IStatelessSampler<T>)new Double.ZigguratGaussianStatelessSampler(mean, stdDev);
            }
            else if(typeof(T) == typeof(float)) 
            {
                return (IStatelessSampler<T>)new Float.ZigguratGaussianStatelessSampler((float)mean, (float)stdDev);
            }
            else 
            {
                throw new ArgumentException("Unsupported type argument");
            }
        }

        #endregion
    }
}
