
namespace Redzen.Random
{
    /// <summary>
    /// A builder of IRandomSource instances.
    /// </summary>
    public interface IRandomSourceBuilder
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
