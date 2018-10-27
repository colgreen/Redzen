using Redzen.Random;

namespace Redzen.Numerics.Distributions
{
    /// <summary>
    /// A discrete distribution sampler.
    /// </summary>
    public class DiscreteDistributionSampler : ISampler<int>
    {
        readonly DiscreteDistribution _dist;
        readonly IRandomSource _rng;

        #region Constructors

        /// <summary>
        /// Construct with the given distribution and a new random source.
        /// </summary>
        /// <param name="dist">Discrete distribution.</param>
        public DiscreteDistributionSampler(DiscreteDistribution dist)
        {
            _dist = dist;
            _rng = RandomDefaults.CreateRandomSource();
        }

        /// <summary>
        /// Construct with the given distribution and a new random source.
        /// </summary>
        /// <param name="dist">Discrete distribution.</param>
        /// <param name="seed">Random source seed.</param>
        public DiscreteDistributionSampler(DiscreteDistribution dist, ulong seed)
        {
            _dist = dist;
            _rng = RandomDefaults.CreateRandomSource(seed);
        }

        /// <summary>
        /// Construct with the given distribution and a random source.
        /// </summary>
        /// <param name="dist">Discrete distribution.</param>
        /// <param name="rng">Random source.</param>
        public DiscreteDistributionSampler(DiscreteDistribution dist, IRandomSource rng)
        {
            _dist = dist;
            _rng = rng;
        }

        #endregion

        #region IDistributionSampler

        /// <summary>
        /// Take a sample from the distribution.
        /// </summary>
        public int Sample()
        {
            return DiscreteDistribution.Sample(_rng, _dist);
        }

        /// <summary>
        /// Fill an array with samples from a distribution.
        /// </summary>
        /// <param name="buf">The array to fill with samples.</param>
        public void Sample(int[] buf)
        {
            DiscreteDistribution.Sample(_rng, _dist, buf);
        }

        #endregion
    }
}
