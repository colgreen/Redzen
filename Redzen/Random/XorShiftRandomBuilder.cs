using System;

namespace Redzen.Random
{
    /// <summary>
    /// A builder of XorShiftRandom instances.
    /// </summary>
    [Obsolete("Superseded by Xoshiro256StarStarRandomBuilder (comparable performance, but passes more statistical tests and has a longer period)")]
    public class XorShiftRandomBuilder : IRandomSourceBuilder
    {
        readonly IRandomSeedSource _seedSource;

        #region Constructors

        /// <summary>
        /// Construct with a default seed source.
        /// </summary>
        public XorShiftRandomBuilder()
        {
            _seedSource = new DefaultRandomSeedSource();
        }

        /// <summary>
        /// Construct with the given seed source.
        /// </summary>
        public XorShiftRandomBuilder(
            IRandomSeedSource seedSource)
        {
            _seedSource = seedSource;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a new instance of XorShiftRandom.
        /// </summary>
        public IRandomSource Create()
        {
            ulong seed = _seedSource.GetSeed();
            return new XorShiftRandom(seed);
        }

        /// <summary>
        /// Create a new instance of XorShiftRandom with the given PRNG seed.
        /// </summary>
        public IRandomSource Create(ulong seed)
        {
            return new XorShiftRandom(seed);
        }

        #endregion
    }
}
