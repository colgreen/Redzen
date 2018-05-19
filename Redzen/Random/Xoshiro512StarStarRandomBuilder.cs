
namespace Redzen.Random
{
    /// <summary>
    /// A builder of Xoshiro512StarStarRandom instances.
    /// </summary>
    public class Xoshiro512StarStarRandomBuilder : IRandomSourceBuilder
    {
        readonly IRandomSeedSource _seedSource;

        #region Constructors

        /// <summary>
        /// Construct with a default seed source.
        /// </summary>
        public Xoshiro512StarStarRandomBuilder()
        {
            _seedSource = new DefaultRandomSeedSource();
        }

        /// <summary>
        /// Construct with the given seed source.
        /// </summary>
        public Xoshiro512StarStarRandomBuilder(
            IRandomSeedSource seedSource)
        {
            _seedSource = seedSource;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new instance of Xoshiro512StarStarRandom.
        /// </summary>
        public IRandomSource Create()
        {
            ulong seed = _seedSource.GetSeed();
            return new Xoshiro512StarStarRandom(seed);
        }

        /// <summary>
        /// Create a new instance of Xoshiro512StarStarRandom with the given PRNG seed.
        /// </summary>
        public IRandomSource Create(ulong seed)
        {
            return new Xoshiro512StarStarRandom(seed);
        }

        #endregion
    }
}
