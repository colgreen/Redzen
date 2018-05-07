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
        /// Construct a uniform distribution generator over the interval [0,1).
        /// </summary>
        public UniformDistribution() 
            : this(1f, false, RandomSourceFactory.Create())
        { }

        /// <summary>
        /// Construct a uniform distribution generator over the interval [0,1) with the provided random seed.
        /// </summary>
        public UniformDistribution(ulong seed)
            : this(1f, false, RandomSourceFactory.Create(seed))
        { }

        /// <summary>
        /// Construct a uniform distribution generator over the interval [0,1) with the provided random source.
        /// </summary>
        public UniformDistribution(IRandomSource rng)
            : this(1f, false, rng)
        { }

        /// <summary>
        /// Construct a uniform distribution generator with the provided random source.
        /// If {signed} is false the distribution interval is [0, scale), otherwise it is (-scale, +scale).
        /// </summary>
        public UniformDistribution(float scale, bool signed) 
            : this(scale, signed, RandomSourceFactory.Create())
        { }

        /// <summary>
        /// Construct a uniform distribution generator with the provided random seed.
        /// If {signed} is false the distribution interval is [0, scale), otherwise it is (-scale, +scale).
        /// </summary>
        public UniformDistribution(float scale, bool signed, ulong seed)
            : this(scale, signed, RandomSourceFactory.Create(seed))
        { }

        /// <summary>
        /// Construct a uniform distribution generator with the provided random source.
        /// If {signed} is false the distribution interval is [0, scale), otherwise it is (-scale, +scale).
        /// </summary>
        public UniformDistribution(float scale, bool signed, IRandomSource rng)
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
