using System;
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
        public void TestRangeRandomOrder_Simple()
        {
            var rng = RandomDefaults.CreateRandomSource(0);

            TestRangeRandomOrderInner(0, 100, rng);
            TestRangeRandomOrderInner(20, 100, rng);

            for(int i=0; i < 100; i++)
            {
                int start = rng.Next(100_000);
                int count = rng.Next(100_000);

                // Enumerate the sequence and store the result in an array.
                int[] arr = EnumerableUtils.RangeRandomOrder(start, count, rng).ToArray();

                // Perform some simple tests.
                AssertSimpleTests(start, count, arr);
            }  
        }

        private static void TestRangeRandomOrderInner(int start, int count, IRandomSource rng)
        {
            // Enumerate the sequence and store the result in an array.
            int[] arr = EnumerableUtils.RangeRandomOrder(start, count, rng).ToArray();

            // Perform some simple tests.
            AssertSimpleTests(start, count, arr);
                
            // Basic randomness test.
            // As we progress through the sequence, count the number of times and entry is higher and lower than the previous entry.
            // For an ordered sequence the 'lower' count will be zero. For a truly random sequence the two counts have an expected 
            // 50/50 distribution, although any given random sequence may differ substantially from the expected value. By fixing the 
            // random seed we at least ensure that the unit test will pass rather than passing most of the time(!)
            CountLowHighTransitions(arr, out int lo, out int hi);

            Assert.IsTrue(hi > 46);
            Assert.IsTrue(lo > 46);
        }

        [TestMethod]
        [TestCategory("EnumerableUtils")]
        public void TestRangeRandomOrder_Randomness()
        {
            var rng = RandomDefaults.CreateRandomSource();

            // Accumulators for the low and high transition counts.
            long loAcc = 0;
            long hiAcc = 0;

            for(int i=0; i < 100; i++)
            {
                int start = rng.Next(100_000);
                int count = rng.Next(100_000);

                // Enumerate the sequence and store the result in an array.
                int[] arr = EnumerableUtils.RangeRandomOrder(start, count, rng).ToArray();

                // Perform some simple tests.
                AssertSimpleTests(start, count, arr);

                // Count low and high transitions, and accumulate counts for all tests.
                CountLowHighTransitions(arr, out int lo, out int hi);
                loAcc += lo;
                hiAcc += hi;
            }

            // Calc proportion of all transitions that where from high to low.
            long loHiTotal = loAcc + hiAcc;
            double loProportion = (double)loAcc / (double)loHiTotal;

            // Calc the delta from the expected value of 0.5.
            double errorAbs = Math.Abs(loProportion - 0.5);

            // Note. For large numbers of tests any outliers are averaged down, thus the error will
            // generally be small here.
            Assert.IsTrue(errorAbs < 0.001);
        }

        private static void AssertSimpleTests(int start, int count, int[] arr)
        {
            // Simple tests.
            Assert.AreEqual(count, arr.Length);
            Assert.AreEqual(start, arr.Min());
            Assert.AreEqual(start+count-1, arr.Max());

            // Test for dupes.
            Assert.AreEqual(0, arr.GroupBy(x => x)
                                .Where(g => g.Count() > 1)
                                .Select(y => y.Key).Count());
        }

        private static void CountLowHighTransitions(int[] arr, out int lo, out int hi)
        {
            lo = 0;
            hi = 0;

            for(int i=1; i < arr.Length; i++)
            {
                if(arr[i] > arr[i-1]) {
                    hi++;
                }
                else {
                    lo++;    
                }
            }
        }
    }
}
