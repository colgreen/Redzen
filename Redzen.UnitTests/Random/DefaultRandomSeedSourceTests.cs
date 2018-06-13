using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;

namespace Redzen.UnitTests.Random
{
    [TestClass]
    public class DefaultRandomSeedSourceTests
    {
        [TestMethod]
        [TestCategory("DefaultRandomSeedSource")]
        public void GetSeed()
        {
            ulong total = 0;

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
