using System;

namespace Redzen.Random.Double
{
    /// <summary>
    /// For taking random samples from a uniform distribution.
    /// </summary>
    public class UniformDistribution : IUniformDistribution<double>
    {
        #region Instance Fields

        readonly IRandomSource _rng;
        readonly double _scale = 1.0;
        readonly bool _signed = false;
        readonly Func<double> _sampleFn;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct with a default RNG source.
        /// </summary>
        public UniformDistribution() 
            : this(new XorShiftRandom(), 1.0, false)
        { }

        /// <summary>
        /// Construct with the specified RNG seed.
        /// </summary>
        public UniformDistribution(int seed)
            : this(new XorShiftRandom(seed), 1.0, false)
        { }

        /// <summary>
        /// Construct with the provided RNG source.
        /// </summary>
        public UniformDistribution(IRandomSource rng)
            : this(rng, 1.0, false)
        { }

        /// <summary>
        /// Construct with a default RNG source.
        /// </summary>
        public UniformDistribution(double scale, bool signed) 
            : this(new XorShiftRandom(), scale, signed)
        { }

        /// <summary>
        /// Construct with the specified RNG seed.
        /// </summary>
        public UniformDistribution(int seed, double scale, bool signed)
            : this(new XorShiftRandom(seed), scale, signed)
        { }

        /// <summary>
        /// Construct with the provided RNG source.
        /// </summary>
        public UniformDistribution(IRandomSource rng, double scale, bool signed)
        {
            _rng = rng;
            _scale = scale;
            _signed = signed;

            // Note. We predetermine which of these four function variants to use at construction time,
            // thus avoiding the two condition tests on each invocation of Sample(). 
            // I.e. this is a micro-optimization.
            if(signed)
            {
                if(1.0 == scale) {
                    _sampleFn = () => { return (_rng.NextDouble() - 0.5) * 2.0; };
                } 
                else {
                    _sampleFn = () => { return (_rng.NextDouble() - 0.5) * 2.0 * scale; };
                }
            }
            else
            {
                if(1.0 == scale) {
                    _sampleFn = () => { return _rng.NextDouble(); };
                }
                else {
                    _sampleFn = () => { return _rng.NextDouble() * scale; };
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get a sample from the configured uniform distribution, i.e. with interval [0, scale), or [-scale, scale) if 'signed' is true.
        /// </summary>
        public double Sample()
        {
            return _sampleFn();
        }

        /// <summary>
        /// Get a sample from the uniform distribution with interval [0, scale).
        /// </summary>
        public double Sample(double scale)
        {
            return _rng.NextDouble() * scale;
        }

        /// <summary>
        /// Get a sample from the uniform distribution with interval [0, scale), or [-scale, scale) if 'signed' is true.
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="signed"></param>
        /// <returns></returns>
        public double Sample(double scale, bool signed)
        {
            return (_rng.NextDouble() - 0.5) * 2.0 * scale;
        }

        /// <summary>
        /// Get a sample from the unit uniform distribution, i.e. with interval [0,1).
        /// </summary>
        public double SampleUnit()
        {
            return _rng.NextDouble();
        }

        /// <summary>
        /// Get a sample from the signed unit uniform distribution, i.e. with interval [-1,1).
        /// </summary>
        public double SampleUnitSigned()
        {
            return (_rng.NextDouble() - 0.5) * 2.0;
        }

        #endregion
    }
}
