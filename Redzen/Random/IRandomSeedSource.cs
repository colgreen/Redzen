
namespace Redzen.Random
{
    /// <summary>
    /// A source of seed values for use by pseudo-random number generators (PRNGs).
    /// </summary>
    public interface IRandomSeedSource
    {
        /// <summary>
        /// Get a new seed value.
        /// </summary>
        ulong GetSeed();
    }
}
