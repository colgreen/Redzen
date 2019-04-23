
namespace Redzen.Random
{
    /// <summary>
    /// A factory of Xoshiro256StarStarRandom instances.
    /// </summary>
    public class Xoshiro256StarStarRandomFactory : IRandomSourceFactory
    {
        readonly IRandomSeedSource _seedSource;

        #region Constructors

        /// <summary>
        /// Construct with a default seed source.
        /// </summary>
        public Xoshiro256StarStarRandomFactory()
        {
            _seedSource = new DefaultRandomSeedSource();
        }

        /// <summary>
        /// Construct with the given seed source.
        /// </summary>
        public Xoshiro256StarStarRandomFactory(
            IRandomSeedSource seedSource)
        {
            _seedSource = seedSource;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new instance of Xoshiro256StarStarRandom.
        /// </summary>
        public IRandomSource Create()
        {
            ulong seed = _seedSource.GetSeed();
            return new Xoshiro256StarStarRandom(seed);
        }

        /// <summary>
        /// Create a new instance of Xoshiro256StarStarRandom with the given PRNG seed.
        /// </summary>
        public IRandomSource Create(ulong seed)
        {
            return new Xoshiro256StarStarRandom(seed);
        }

        #endregion
    }
}
