
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
        /// Take a sample from the distribution.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <returns>A new random sample.</returns>
        T Sample(T mean, T stdDev);

        /// <summary>
        /// Take a sample from the standard gaussian distribution, i.e. with mean of 0 and standard deviation of 1.
        /// </summary>
        T SampleStandard();
    }
}
