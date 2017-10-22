
namespace Redzen.Random.Float
{
    // TODO: Re-implement once System.MathF is available in a .NET Standard.
    // This class is merely a wrapper over a double precision sampler in order to provide an instance of IGaussianDistribution<float>, once MathF is available
    // this wrapper can be replaced with a proper implementation based on the single precision floating point data type, thus affording some performance improvement.
    /// <summary>
    /// For taking random samples from a Gaussian distribution.
    /// </summary>
    public class ZigguratGaussianDistribution : IGaussianDistribution<float>
    {
        Double.ZigguratGaussianDistribution _gaussianDouble;

        #region Constructor

        /// <summary>
        /// Construct with a default RNG source.
        /// </summary>
        public ZigguratGaussianDistribution() 
            : this(new XorShiftRandom())
        { }

        /// <summary>
        /// Construct with the specified RNG seed.
        /// </summary>
        public ZigguratGaussianDistribution(int seed)
            : this(new XorShiftRandom(seed))
        { }

        /// <summary>
        /// Construct with the provided RNG source.
        /// </summary>
        public ZigguratGaussianDistribution(IRandomSource rng)
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
