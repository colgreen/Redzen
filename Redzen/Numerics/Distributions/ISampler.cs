
namespace Redzen.Numerics.Distributions
{
    /// <summary>
    /// For taking random samples from some underlying distribution, either a continuous or discrete distribution.
    /// </summary>
    /// <typeparam name="T">Data type of the samples.</typeparam>
    public interface ISampler<T> where T : struct
    {
        /// <summary>
        /// Take a sample from the distribution.
        /// </summary>
        /// <returns>A random sample.</returns>
        T Sample();

        /// <summary>
        /// Fill an array with samples from the distribution.
        /// </summary>
        /// <param name="buf">The array to fill with samples.</param>
        void Sample(T[] buf);
    }
}
