using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Linq;
using Redzen.Random;

namespace Redzen.UnitTests.Linq
{
    [TestClass]
    public class EnumerableUtilsTests
    {
        [TestMethod]
        [TestCategory("EnumerableUtils")]
        public void TestRangeRandomOrder()
        {
            var rng = new XorShiftRandom(0);

            RunTest(0, 100, rng);
            RunTest(20, 100, rng);
        }

        private static void RunTest(int start, int count, IRandomSource rng)
        {
            // Enumerate the sequence and store the result in an array.
            int[] arr = EnumerableUtils.RangeRandomOrder(start, count, rng).ToArray();

            // Simple tests.
            Assert.AreEqual(count, arr.Length);
            Assert.AreEqual(start, arr.Min());
            Assert.AreEqual(start+count-1, arr.Max());

            // Test for dupes.
            Assert.AreEqual(0, arr.GroupBy(x => x)
                                .Where(g => g.Count() > 1)
                                .Select(y => y.Key).Count());
                            
            // Basic randomness test.
            // As we progress through the sequence, count the number of times and entry is higher and lower than the previous entry.
            // For an ordered sequence the 'lower' count will be zero. For pure random the two counts have an expected value of 50/50, but
            // in any given sequence hey may not be anywhere near that. By fixing the random seed (above) we at least ensure that the unit test 
            // will pass rather than mostly passing!
            int highCount = 0, lowCount = 0;
            
            for(int i=1; i < arr.Length; i++)
            {
                if(arr[i] > arr[i-1]) {
                    highCount++;
                }
                else {
                    lowCount++;    
                }
            }

            Assert.IsTrue(highCount > 30);
            Assert.IsTrue(lowCount > 30);
        }
    }
}
