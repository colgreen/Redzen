
namespace Redzen.Random
{
    /// <summary>
    /// For taking samples from a continuous distribution.
    /// </summary>
    /// <typeparam name="T">Data type of the individual samples.</typeparam>
    public interface IContinuousDistribution<T> where T : struct
    {
        /// <summary>
        /// Get a sample from the distribution.
        /// </summary>
        T Sample();
    }
}
