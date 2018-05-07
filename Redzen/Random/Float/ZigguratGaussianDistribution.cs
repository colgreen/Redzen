
namespace Redzen.Random.Float
{
    // TODO: Re-implement once System.MathF is available in a .NET Standard.
    // This class is merely a wrapper over a double precision sampler in order to provide an instance of IGaussianDistribution<float>. 
    // When MathF becomes available this wrapper can be replaced with a proper implementation based on the single precision floating point
    // data type, thus affording some performance improvement.
    /// <summary>
    /// For taking random samples from a Gaussian distribution.
    /// </summary>
    public class ZigguratGaussianDistribution : IGaussianDistribution<float>
    {
        Double.ZigguratGaussianDistribution _gaussianDouble;

        #region Constructors

        /// <summary>
        /// Construct a gaussian generator.
        /// The distribution has a zero mean and standard deviation of 1.0.
        /// </summary>
        public ZigguratGaussianDistribution() 
            : this(0.0, 1.0, RandomSourceFactory.Create())
        {}

        /// <summary>
        /// Construct a gaussian generator with the provided random seed.
        /// a zero mean and standard deviation of 1.0.
        /// </summary>
        public ZigguratGaussianDistribution(ulong seed) 
            : this(0.0, 1.0, RandomSourceFactory.Create(seed))
        {}

        /// <summary>
        /// Construct with the provided RNG source.
        /// </summary>
        public ZigguratGaussianDistribution(IRandomSource rng)
            : this(0.0, 1.0, rng)
        {}

        /// <summary>
        /// Construct a gaussian generator with the specified distribution mean and standard deviation
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        public ZigguratGaussianDistribution(double mean, double stdDev) 
            : this(mean, stdDev, RandomSourceFactory.Create())
        {}

        /// <summary>
        /// Construct a gaussian generator with the specified distribution mean, standard deviation,
        /// and random seed.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <param name="seed">Random seed.</param>
        public ZigguratGaussianDistribution(double mean, double stdDev, ulong seed) 
            : this(mean, stdDev, RandomSourceFactory.Create(seed))
        {}

        /// <summary>
        /// Construct a gaussian generator with the specified distribution mean, standard deviation,
        /// and the provided random source.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <param name="rng">Random source.</param>
        public ZigguratGaussianDistribution(double mean, double stdDev, IRandomSource rng)
        {
            _gaussianDouble = new Double.ZigguratGaussianDistribution(mean, stdDev, rng);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Take a sample from the distribution.
        /// </summary>
        public float Sample()
        {
            return (float)_gaussianDouble.Sample();
        }

        /// <summary>
        /// Take a sample from the distribution.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <returns>A new random sample.</returns>
        public float Sample(float mean, float stdDev)
        {
            return (float)_gaussianDouble.Sample(mean, stdDev);
        }

        /// <summary>
        /// Take a sample from the standard gaussian distribution, i.e. with mean of 0 and standard deviation of 1.
        /// </summary>
        public float SampleStandard()
        {
            return (float)_gaussianDouble.SampleStandard();
        }

        #endregion
    }
}
