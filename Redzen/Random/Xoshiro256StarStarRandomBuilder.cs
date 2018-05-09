
namespace Redzen.Random
{
    /// <summary>
    /// A builder of Xoshiro256StarStarRandomBuilder instances.
    /// </summary>
    public class Xoshiro256StarStarRandomBuilder : IRandomSourceBuilder
    {
        readonly IRandomSeedSource _seedSource;

        #region Constructors

        /// <summary>
        /// Construct with a default seed source.
        /// </summary>
        public Xoshiro256StarStarRandomBuilder()
        {
            _seedSource = new DefaultRandomSeedSource();
        }

        /// <summary>
        /// Construct with the given seed source.
        /// </summary>
        public Xoshiro256StarStarRandomBuilder(
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
