using System;
using System.Security.Cryptography;
using Redzen.Numerics;

namespace Redzen.Random
{
    /// <summary>
    /// Factory class for creating instances of IContinuousDistribution{T} and IGaussianDistribution{T}.
    /// </summary>
    public static class ContinuousDistributionFactory
    {
        static readonly XorShiftRandom __seedRng;
        static readonly object __lockObj = new object();

        #region Static Initializer

        static ContinuousDistributionFactory()
        {
            using (RNGCryptoServiceProvider cryptoRng = new RNGCryptoServiceProvider())
            {
                // Create a random seed. Note. this uses system entropy as a source of external randomness.
                byte[] buf = new byte[4];
                cryptoRng.GetBytes(buf);
                int seed = BitConverter.ToInt32(buf, 0);

                // Init a single pseudo-random RNG for generating seeds for other RNGs.
                __seedRng = new XorShiftRandom(seed);
            }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Create a uniform distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IContinuousDistribution{T}"/>.</returns>
        public static IUniformDistribution<T> CreateUniformDistribution<T>() where T : struct
        {
            if(typeof(T) == typeof(double)) {
                return (IUniformDistribution<T>)new Double.UniformDistribution(GetNextSeed());
            }
            else if(typeof(T) == typeof(float)) {
                return (IUniformDistribution<T>)new Float.UniformDistribution(GetNextSeed());
            }
            else {
                throw new ArgumentException("Unsupported type argument");
            }
        }

        /// <summary>
        /// Create a Gaussian distribution.
        /// </summary>
        /// <typeparam name="T">Data type of the individual samples.</typeparam>
        /// <returns>A new instance of <see cref="IContinuousDistribution{T}"/>.</returns>
        public static IGaussianDistribution<T> IGaussianDistribution<T>() where T : struct
        {
            if(typeof(T) == typeof(double)) {
                return (IGaussianDistribution<T>)new Double.ZigguratGaussianDistribution(GetNextSeed());
            }
            else if(typeof(T) == typeof(float)) 
            {
                return (IGaussianDistribution<T>)new Float.ZigguratGaussianDistribution(GetNextSeed());
            }
            else {
                throw new ArgumentException("Unsupported type argument");
            }
        }
            
        #endregion

        #region Private Static Methods

        private static int GetNextSeed()
        {
            // Get a new seed. 
            // Calls to __seedRng need to be sync locked because it has state and is not re-entrant; as such 
            // we get and release the lock as quickly as possible.
            lock(__lockObj){
                return __seedRng.NextInt();
            }
        }

        #endregion
    }
}
