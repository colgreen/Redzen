
namespace Redzen.Random
{
    /// <summary>
    /// A factory of IRandomSource instances.
    /// </summary>
    public interface IRandomSourceFactory
    {
        /// <summary>
        /// Create a new IRandomSource.
        /// </summary>
        IRandomSource Create();

        /// <summary>
        /// Create a new IRandomSource with the given PRNG seed.
        /// </summary>
        IRandomSource Create(ulong seed);
    }
}
