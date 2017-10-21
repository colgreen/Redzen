using Redzen.Numerics;

namespace Redzen.Random.Double
{
    /// <summary>
    /// For taking random samples from a uniform distribution.
    /// Samples are in the interval [0,1).
    /// </summary>
    public class UniformDistribution : IContinuousDistribution<double>
    {
        IRandomSource _rng;

        #region Constructors

        /// <summary>
        /// Construct with a default RNG source.
        /// </summary>
        public UniformDistribution() 
            : this(new XorShiftRandom())
        { }

        /// <summary>
        /// Construct with the specified RNG seed.
        /// </summary>
        public UniformDistribution(int seed)
            : this(new XorShiftRandom(seed))
        { }

        /// <summary>
        /// Construct with the provided RNG source.
        /// </summary>
        public UniformDistribution(IRandomSource rng)
        {
            _rng = rng;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get a sample from the distribution.
        /// </summary>
        public double Sample()
        {
            return _rng.NextDouble();
        }

        #endregion
    }
}
