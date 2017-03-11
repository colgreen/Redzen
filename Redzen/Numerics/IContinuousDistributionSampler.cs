

namespace Redzen.Numerics
{
    /// <summary>
    /// Provides random samples from a continuous distribution.
    /// </summary>
    public interface IContinuousDistributionSampler
    {
        /// <summary>
        /// Get a random sample from the distribution.
        /// </summary>
        double NextDouble();
    }
}
