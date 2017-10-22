using System;

namespace Redzen.Random.Float
{
    /// <summary>
    /// For taking random samples from a uniform distribution.
    /// </summary>
    public class UniformDistribution : IContinuousDistribution<float>
    {
        #region Instance Fields

        readonly IRandomSource _rng;
        readonly double _scale = 1.0;
        readonly bool _signed = false;
        readonly Func<float> _sampleFn;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct with a default RNG source.
        /// </summary>
        public UniformDistribution() 
            : this(new XorShiftRandom(), 1f, false)
        { }

        /// <summary>
        /// Construct with the specified RNG seed.
        /// </summary>
        public UniformDistribution(int seed)
            : this(new XorShiftRandom(seed), 1f, false)
        { }

        /// <summary>
        /// Construct with the provided RNG source.
        /// </summary>
        public UniformDistribution(IRandomSource rng)
            : this(rng, 1f, false)
        { }

        /// <summary>
        /// Construct with a default RNG source.
        /// </summary>
        public UniformDistribution(float scale, bool signed) 
            : this(new XorShiftRandom(), scale, signed)
        { }

        /// <summary>
        /// Construct with the specified RNG seed.
        /// </summary>
        public UniformDistribution(int seed, float scale, bool signed)
            : this(new XorShiftRandom(seed), scale, signed)
        { }

        /// <summary>
        /// Construct with the provided RNG source.
        /// </summary>
        public UniformDistribution(IRandomSource rng, float scale, bool signed)
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
                    _sampleFn = () => { return (_rng.NextFloat() - 0.5f) * 2.0f; };
                } 
                else {
                    _sampleFn = () => { return (_rng.NextFloat() - 0.5f) * 2.0f * scale; };
                }
            }
            else
            {
                if(1.0 == scale) {
                    _sampleFn = () => { return _rng.NextFloat(); };
                }
                else {
                    _sampleFn = () => { return _rng.NextFloat() * scale; };
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get a sample from the configured uniform distribution, i.e. with interval [0, scale), or [-scale, scale) if 'signed' is true.
        /// </summary>
        public float Sample()
        {
            return _sampleFn();
        }

        /// <summary>
        /// Get a sample from the uniform distribution with interval [0, scale).
        /// </summary>
        public float Sample(float scale)
        {
            return _rng.NextFloat() * scale;
        }

        /// <summary>
        /// Get a sample from the uniform distribution with interval [0, scale), or [-scale, scale) if 'signed' is true.
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="signed"></param>
        /// <returns></returns>
        public float Sample(float scale, bool signed)
        {
            return (_rng.NextFloat() - 0.5f) * 2.0f * scale;
        }

        /// <summary>
        /// Get a sample from the unit uniform distribution, i.e. with interval [0,1).
        /// </summary>
        public float SampleUnit()
        {
            return _rng.NextFloat();
        }

        /// <summary>
        /// Get a sample from the signed unit uniform distribution, i.e. with interval [-1,1).
        /// </summary>
        public float SampleUnitSigned()
        {
            return (_rng.NextFloat() - 0.5f) * 2.0f;
        }

        #endregion
    }
}
