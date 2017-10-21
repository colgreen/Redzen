using Redzen.Numerics;

namespace Redzen.Random.Float
{
    /// <summary>
    /// For taking random samples from a Gaussian distribution.
    /// </summary>
    public class DefaultGaussianDistribution : IGaussianDistribution<float>
    {
        Double.ZigguratGaussianDistribution _gaussianDouble;


        #region Constructor

        /// <summary>
        /// Construct with a default RNG source.
        /// </summary>
        public DefaultGaussianDistribution() 
            : this(new XorShiftRandom())
        { }

        /// <summary>
        /// Construct with the specified RNG seed.
        /// </summary>
        public DefaultGaussianDistribution(int seed)
            : this(new XorShiftRandom(seed))
        { }

        /// <summary>
        /// Construct with the provided RNG source.
        /// </summary>
        public DefaultGaussianDistribution(IRandomSource rng)
        {
            _gaussianDouble = new Double.ZigguratGaussianDistribution(rng);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get a sample from the distribution.
        /// </summary>
        public float Sample()
        {
            return (float)_gaussianDouble.Sample();
        }

        /// <summary>
        /// Get a sample value from the gaussian distribution.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <returns>A new random sample.</returns>
        public float Sample(float mean, float stdDev)
        {
            return (float)_gaussianDouble.Sample(mean, stdDev);
        }

        #endregion
    }
}
