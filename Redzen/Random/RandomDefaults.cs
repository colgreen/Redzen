
namespace Redzen.Random
{
    public static class RandomDefaults
    {
        /// <summary>
        /// A static default IRandomSeedSource instance for use anywhere within the current process.
        /// </summary>
        public static readonly IRandomSeedSource DefaultRandomSeedSource;

        /// <summary>
        /// A static default instance for use anywhere within the current process.
        /// </summary>
        public static readonly IRandomSourceBuilder DefaultRandomSourceBuilder;

        #region Static Initialiser

        static RandomDefaults()
        {
            DefaultRandomSeedSource = new DefaultRandomSeedSource();
            DefaultRandomSourceBuilder = new Xoshiro256StarStarRandomBuilder(DefaultRandomSeedSource);
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Get a new seed value.
        /// </summary>
        public static ulong GetSeed()
        {
            return DefaultRandomSeedSource.GetSeed();
        }

        /// <summary>
        /// Create a new IRandomSource.
        /// </summary>
        public static IRandomSource CreateRandomSource()
        {
            return DefaultRandomSourceBuilder.Create();
        }

        /// <summary>
        /// Create a new IRandomSource with the given PRNG seed.
        /// </summary>
        public static IRandomSource CreateRandomSource(ulong seed)
        {
            return DefaultRandomSourceBuilder.Create(seed);
        }

        #endregion
    }
}
