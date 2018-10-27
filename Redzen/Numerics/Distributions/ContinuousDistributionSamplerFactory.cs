using System;
using Redzen.Random;

namespace Redzen.Numerics.Distributions
{
    /// <summary>
    /// Factory class for creating instances of IUniformDistribution{T} and IGaussianDistribution{T}.
    /// </summary>
    public static class ContinuousDistributionSamplerFactory
    {
        #region Public Static Methods [CreateUniformDistribution]

        /// <summary>
        /// Create a sampler for the uniform distribution with interval [0,1).
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateUniformDistribution<T>() 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource();
            return CreateUniformDistribution<T>(1.0, false, rng);
        }

        /// <summary>
        /// Create a uniform distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <param name="seed">Random source seed.</param>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateUniformDistribution<T>(ulong seed) 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(seed);
            return CreateUniformDistribution<T>(1.0, false, rng);
        }

        /// <summary>
        /// Create a sampler for the uniform distribution with interval [0,1).
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <param name="rng">Random source.</param>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateUniformDistribution<T>(IRandomSource rng)
            where T : struct
        {
            return CreateUniformDistribution<T>(1.0, false, rng);
        }

        /// <summary>
        /// Create a sampler for the uniform distribution with interval [0,max) or (-max,max).
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <param name="max">Maximum value (exclusive).</param>
        /// <param name="signed">If true the distribution has interval (-max,max); otherwise [0,max).</param>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateUniformDistribution<T>(double max, bool signed) 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource();
            return CreateUniformDistribution<T>(max, signed, rng);
        }

        /// <summary>
        /// Create a sampler for the uniform distribution with interval [0,max) or (-max,max).
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <param name="max">Maximum value (exclusive).</param>
        /// <param name="signed">If true the distribution has interval (-max,max); otherwise [0,max).</param>
        /// <param name="seed">Random source seed.</param>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateUniformDistribution<T>(double max, bool signed, ulong seed) 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(seed);
            return CreateUniformDistribution<T>(max, signed, rng);
        }

        /// <summary>
        /// Create a sampler for the uniform distribution with interval [0,max) or (-max,max).
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <param name="max">Maximum value (exclusive).</param>
        /// <param name="signed">If true the distribution has interval (-max,max); otherwise [0,max).</param>
        /// <param name="seed">Random source.</param>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateUniformDistribution<T>(double max, bool signed, IRandomSource rng) 
            where T : struct
        {
            if(typeof(T) == typeof(double)) 
            {
                return (ISampler<T>)new Redzen.Numerics.Distributions.Double.UniformDistributionSampler(max, signed, rng);
            }
            else if(typeof(T) == typeof(float)) 
            {
                return (ISampler<T>)new Redzen.Numerics.Distributions.Float.UniformDistributionSampler((float)max, signed, rng);
            }
            else {
                throw new ArgumentException("Unsupported type argument");
            }
        }

        #endregion

        #region Public Static Methods [CreateGaussianDistribution]

        /// <summary>
        /// Create a sampler for the standard Gaussian distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateGaussianDistribution<T>() 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource();
            return CreateGaussianDistribution<T>(0.0, 1.0, rng);
        }

        /// <summary>
        /// Create a sampler for the standard Gaussian distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <param name="seed">Random source seed.</param>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateGaussianDistribution<T>(ulong seed) 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(seed);
            return CreateGaussianDistribution<T>(0.0, 1.0, rng);
        }

        /// <summary>
        /// Create a sample for the standard Gaussian distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateGaussianDistribution<T>(IRandomSource rng) 
            where T : struct
        {
            return CreateGaussianDistribution<T>(0.0, 1.0, rng);
        }

        /// <summary>
        /// Create a sample for the Gaussian distribution with the specified mean and standard deviation.
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateGaussianDistribution<T>(double mean, double stdDev) 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource();
            return CreateGaussianDistribution<T>(mean, stdDev, rng);
        }

        /// <summary>
        /// Create a sample for the Gaussian distribution with the specified mean and standard deviation.
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <param name="seed">Random source seed.</param>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateGaussianDistribution<T>(double mean, double stdDev, ulong seed) 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(seed);
            return CreateGaussianDistribution<T>(mean, stdDev, rng);
        }

        /// <summary>
        /// Create a sample for the Gaussian distribution with the specified mean and standard deviation.
        /// </summary>
        /// <typeparam name="T">Data type of the samples.</typeparam>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <param name="rng">Random source.</param>
        /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
        public static ISampler<T> CreateGaussianDistribution<T>(double mean, double stdDev, IRandomSource rng) 
            where T : struct
        {
            if(typeof(T) == typeof(double)) 
            {
                return (ISampler<T>)new Redzen.Numerics.Distributions.Double.ZigguratGaussianSampler(mean, stdDev, rng);
            }
            else if(typeof(T) == typeof(float)) 
            {
                return (ISampler<T>)new Redzen.Numerics.Distributions.Float.ZigguratGaussianSampler((float)mean, (float)stdDev, rng);   
            }
            else {
                throw new ArgumentException("Unsupported type argument");
            }
        }

        #endregion
    }
}
