using Xunit;

namespace Redzen.Random.Tests
{
    public class DefaultRandomSeedSourceTests
    {
        [Fact]
        public void GetSeed()
        {
            ulong total = 0;

            // Run the code using a range of different concurrency levels.
            for(int minConcurrencyLevel=1; minConcurrencyLevel < 34; minConcurrencyLevel++)
            {
                DefaultRandomSeedSource seedSrc = new DefaultRandomSeedSource(minConcurrencyLevel);

                for(int i=0; i < 100_000; i++)
                {
                    ulong seed = seedSrc.GetSeed();
                    total += seed;
                }
            }
        }
    }
}
