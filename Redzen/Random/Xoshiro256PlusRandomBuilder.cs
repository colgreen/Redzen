
namespace Redzen.Random
{
    /// <summary>
    /// A builder of Xoshiro256PlusRandom instances.
    /// </summary>
    public class Xoshiro256PlusRandomBuilder : IRandomSourceBuilder
    {
        readonly IRandomSeedSource _seedSource;

        #region Constructors

        /// <summary>
        /// Construct with a default seed source.
        /// </summary>
        public Xoshiro256PlusRandomBuilder()
        {
            _seedSource = new DefaultRandomSeedSource();
        }

        /// <summary>
        /// Construct with the given seed source.
        /// </summary>
        public Xoshiro256PlusRandomBuilder(
            IRandomSeedSource seedSource)
        {
            _seedSource = seedSource;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new instance of Xoshiro256PlusRandom.
        /// </summary>
        public IRandomSource Create()
        {
            ulong seed = _seedSource.GetSeed();
            return new Xoshiro256PlusRandom(seed);
        }

        /// <summary>
        /// Create a new instance of Xoshiro256PlusRandom with the given PRNG seed.
        /// </summary>
        public IRandomSource Create(ulong seed)
        {
            return new Xoshiro256PlusRandom(seed);
        }

        #endregion
    }
}
