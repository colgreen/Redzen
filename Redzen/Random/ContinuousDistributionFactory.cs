using System;

namespace Redzen.Random
{
    /// <summary>
    /// Factory class for creating instances of IUniformDistribution{T} and IGaussianDistribution{T}.
    /// </summary>
    public static class ContinuousDistributionFactory
    {
        #region Public Static Methods

        /// <summary>
        /// Create a uniform distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IContinuousDistribution{T}"/>.</returns>
        public static IUniformDistribution<T> CreateUniformDistribution<T>() where T : struct
        {
            if(typeof(T) == typeof(double)) {
                return (IUniformDistribution<T>)new Double.UniformDistribution();
            }
            else if(typeof(T) == typeof(float)) {
                return (IUniformDistribution<T>)new Float.UniformDistribution();
            }
            else {
                throw new ArgumentException("Unsupported type argument");
            }
        }

        /// <summary>
        /// Create a uniform distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IContinuousDistribution{T}"/>.</returns>
        public static IUniformDistribution<T> CreateUniformDistribution<T>(double scale, bool signed) where T : struct
        {
            if(typeof(T) == typeof(double)) {
                return (IUniformDistribution<T>)new Double.UniformDistribution(scale, signed);
            }
            else if(typeof(T) == typeof(float)) {
                return (IUniformDistribution<T>)new Float.UniformDistribution((float)scale, signed);
            }
            else {
                throw new ArgumentException("Unsupported type argument");
            }
        }

        /// <summary>
        /// Create a Gaussian distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IGaussianDistribution{T}"/>.</returns>
        public static IGaussianDistribution<T> CreateGaussianDistribution<T>() where T : struct
        {
            if(typeof(T) == typeof(double)) {
                return (IGaussianDistribution<T>)new Double.ZigguratGaussianDistribution();
            }
            else if(typeof(T) == typeof(float)) 
            {
                return (IGaussianDistribution<T>)new Float.ZigguratGaussianDistribution();
            }
            else {
                throw new ArgumentException("Unsupported type argument");
            }
        }

        /// <summary>
        /// Create a Gaussian distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IGaussianDistribution{T}"/>.</returns>
        public static IGaussianDistribution<T> CreateGaussianDistribution<T>(double mean, double stdDev) where T : struct
        {
            if(typeof(T) == typeof(double)) {
                return (IGaussianDistribution<T>)new Double.ZigguratGaussianDistribution(mean, stdDev);
            }
            else if(typeof(T) == typeof(float)) 
            {
                return (IGaussianDistribution<T>)new Float.ZigguratGaussianDistribution(mean, stdDev);
            }
            else {
                throw new ArgumentException("Unsupported type argument");
            }
        }

        #endregion
    }
}
