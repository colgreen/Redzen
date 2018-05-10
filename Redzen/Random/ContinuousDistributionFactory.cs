using System;

namespace Redzen.Random
{
    /// <summary>
    /// Factory class for creating instances of IUniformDistribution{T} and IGaussianDistribution{T}.
    /// </summary>
    public static class ContinuousDistributionFactory
    {
        #region Public Static Methods [CreateUniformDistribution]

        /// <summary>
        /// Create a uniform distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IContinuousDistribution{T}"/>.</returns>
        public static IUniformDistribution<T> CreateUniformDistribution<T>() 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource();
            return CreateUniformDistribution<T>(1.0, false, rng);
        }

        /// <summary>
        /// Create a uniform distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IContinuousDistribution{T}"/>.</returns>
        public static IUniformDistribution<T> CreateUniformDistribution<T>(ulong seed) 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(seed);
            return CreateUniformDistribution<T>(1.0, false, rng);
        }

        /// <summary>
        /// Create a uniform distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IContinuousDistribution{T}"/>.</returns>
        public static IUniformDistribution<T> CreateUniformDistribution<T>(IRandomSource rng)
            where T : struct
        {
            return CreateUniformDistribution<T>(1.0, false, rng);
        }

        /// <summary>
        /// Create a uniform distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IContinuousDistribution{T}"/>.</returns>
        public static IUniformDistribution<T> CreateUniformDistribution<T>(double scale, bool signed) 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource();
            return CreateUniformDistribution<T>(scale, signed, rng);
        }

        /// <summary>
        /// Create a uniform distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IContinuousDistribution{T}"/>.</returns>
        public static IUniformDistribution<T> CreateUniformDistribution<T>(double scale, bool signed, ulong seed) 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(seed);
            return CreateUniformDistribution<T>(scale, signed, rng);
        }

        /// <summary>
        /// Create a uniform distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IContinuousDistribution{T}"/>.</returns>
        public static IUniformDistribution<T> CreateUniformDistribution<T>(double scale, bool signed, IRandomSource rng) 
            where T : struct
        {
            if(typeof(T) == typeof(double)) {
                return (IUniformDistribution<T>)new Double.UniformDistribution(scale, signed, rng);
            }
            else if(typeof(T) == typeof(float)) {
                return (IUniformDistribution<T>)new Float.UniformDistribution((float)scale, signed, rng);
            }
            else {
                throw new ArgumentException("Unsupported type argument");
            }
        }

        #endregion

        #region Public Static Methods [CreateGaussianDistribution]

        /// <summary>
        /// Create a Gaussian distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IGaussianDistribution{T}"/>.</returns>
        public static IGaussianDistribution<T> CreateGaussianDistribution<T>() 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource();
            return CreateGaussianDistribution<T>(0.0, 1.0, rng);
        }

        /// <summary>
        /// Create a Gaussian distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IGaussianDistribution{T}"/>.</returns>
        public static IGaussianDistribution<T> CreateGaussianDistribution<T>(ulong seed) 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(seed);
            return CreateGaussianDistribution<T>(0.0, 1.0, rng);
        }

        /// <summary>
        /// Create a Gaussian distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IGaussianDistribution{T}"/>.</returns>
        public static IGaussianDistribution<T> CreateGaussianDistribution<T>(IRandomSource rng) 
            where T : struct
        {
            return CreateGaussianDistribution<T>(0.0, 1.0, rng);
        }

        /// <summary>
        /// Create a Gaussian distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IGaussianDistribution{T}"/>.</returns>
        public static IGaussianDistribution<T> CreateGaussianDistribution<T>(double mean, double stdDev) 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource();
            return CreateGaussianDistribution<T>(mean, stdDev, rng);
        }

        /// <summary>
        /// Create a Gaussian distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IGaussianDistribution{T}"/>.</returns>
        public static IGaussianDistribution<T> CreateGaussianDistribution<T>(double mean, double stdDev, ulong seed) 
            where T : struct
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(seed);
            return CreateGaussianDistribution<T>(mean, stdDev, rng);
        }

        /// <summary>
        /// Create a Gaussian distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IGaussianDistribution{T}"/>.</returns>
        public static IGaussianDistribution<T> CreateGaussianDistribution<T>(double mean, double stdDev, IRandomSource rng) 
            where T : struct
        {
            if(typeof(T) == typeof(double)) {
                return (IGaussianDistribution<T>)new Double.ZigguratGaussianDistribution(mean, stdDev, rng);
            }
            else if(typeof(T) == typeof(float)) 
            {
                return (IGaussianDistribution<T>)new Float.ZigguratGaussianDistribution(mean, stdDev, rng);
            }
            else {
                throw new ArgumentException("Unsupported type argument");
            }
        }

        #endregion
    }
}
