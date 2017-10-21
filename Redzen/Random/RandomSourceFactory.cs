using System;
using System.Security.Cryptography;

namespace Redzen.Random
{
    /// <summary>
    /// A source of IRandomSource instances that are guaranteed to have well distributed seeds.
    /// </summary>
    /// <remarks>
    /// This solves the problem of using classes such as System.Random with the default constructor that uses
    /// the current system time as a random seed. In modern systems it's possible to create a great many Random instances
    /// within the time of a single system tick, thus the RNGs all get the same seed.
    /// </remarks>
    public static class RandomSourceFactory
    {
        static readonly XorShiftRandom __seedRng;
        static readonly object __lockObj = new object();

        #region Static Initializer

        static RandomSourceFactory()
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

        #region Public Static Factory Method

        /// <summary>
        /// Create a new IRandomSource.
        /// </summary>
        /// <returns>A new instance of an IRandomSource.</returns>
        public static IRandomSource CreateRandomSource()
        {
            return new XorShiftRandom(GetNextSeed());
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
