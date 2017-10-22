
namespace Redzen.Random
{
    /// <summary>
    /// For taking samples from a Gaussian distribution.
    /// </summary>
    /// <typeparam name="T">Data type of the individual samples.</typeparam>
    public interface IGaussianDistribution<T> : IContinuousDistribution<T>
        where T : struct
    {
        /// <summary>
        /// Get a sample from the standard gaussian distribution, i.e. with mean of zero and standard deviation of one.
        /// </summary>
        T SampleStandard();

        /// <summary>
        /// Get a sample value from the gaussian distribution.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <returns>A new random sample.</returns>
        T Sample(T mean, T stdDev);
    }
}
