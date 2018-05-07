using System;
using System.Security.Cryptography;
using System.Threading;

namespace Redzen.Random
{
    /// <summary>
    /// A source of IRandomSource instances that are guaranteed to have well distributed seeds.
    /// </summary>
    /// <remarks>
    /// This solves the problem of using classes such as System.Random with the default constructor that uses
    /// the current system time as a random seed. In modern systems it's possible to create a great many Random instances
    /// within the time of a single system tick, thus the RNGs all get the same seed.
    /// 
    /// This implementation uses multiple seed PRNGs initialises with high quality crypto random seed state. This factory
    /// class rotates through the seed PNGs to generate seed values for constructing new IRandomSource instances. 
    /// Using multiple seed PRNGs increases the state space we are sampling seeds from, and also improves thread
    /// concurrency by allowing multiple seed PRNGs to be sync locked and generating values simultaneously.
    /// </remarks>
    public static class RandomSourceFactory
    {
        #region Consts / Statics

        const int __seedRngCount = 8;

        static int _seedRngSwitch = 0;
        static Xoshiro256StarStarRandom[] __seedRngArr;
        static readonly object[] __lockObjArr;

        #endregion

        #region Static Initializer

        static RandomSourceFactory()
        {
            // Create high quality random bytes to init the seed PRNGs.
            byte[] buf = GetCryptoRandomBytes(__seedRngCount * 8);

            // Init the seed PRNGs and associated sync lock objects.
            __seedRngArr = new Xoshiro256StarStarRandom[__seedRngCount];
            __lockObjArr = new object[__seedRngCount];

            for(int i=0; i < __seedRngCount; i++)
            {
                // Init rng.
                ulong seed = BitConverter.ToUInt64(buf, i * 8);
                __seedRngArr[i] = new Xoshiro256StarStarRandom(seed);

                // Create an associated lock object.
                __lockObjArr[i] = new object();
            }
        }

        private static byte[] GetCryptoRandomBytes(int count)
        {
            // Note. Generating crypto random bytes can be very slow, relative to a PRNG; we may even have to wait
            // for the OS to have sufficient entropy for generating the bytes.
            byte[] buf = new byte[count];
            using(RNGCryptoServiceProvider cryptoRng = new RNGCryptoServiceProvider())
            {
                cryptoRng.GetBytes(buf);
            }
            return buf;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Create a new IRandomSource.
        /// </summary>
        /// <returns>A new instance of an IRandomSource.</returns>
        public static IRandomSource Create()
        {
            return new Xoshiro256StarStarRandom(GetNextSeed());
        }

        /// <summary>
        /// Create a new IRandomSource using the provided ulong seed.
        /// </summary>
        /// <returns>A new instance of an IRandomSource.</returns>
        public static IRandomSource Create(ulong seed)
        {
            return new Xoshiro256StarStarRandom(seed);
        }

        /// <summary>
        /// Get a new random seed.
        /// </summary>
        public static ulong GetNextSeed()
        {
            // Rotate through the seed rng array.
            int idx = Interlocked.Increment(ref _seedRngSwitch) % __seedRngCount;

            // Obtain the sync clock for the chosen seed rng, and use it to generate a new seed.
            lock(__seedRngArr[idx])
            {
                return __seedRngArr[idx].NextUInt() + ((ulong)__seedRngArr[idx].NextUInt() << 32);
            }
        }

        #endregion
    }
}
