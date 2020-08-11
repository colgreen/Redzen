using System;
using System.Linq;
using Redzen.Random;
using Xunit;

namespace Redzen.Linq.Tests
{
    public class EnumerableUtilsTests
    {
        #region Public Test Methods

        [Theory]
        [InlineData(0, 100, 0u)]
        [InlineData(20, 100, 123u)]
        public static void RangeRandomOrder_Simple(int start, int count, ulong seed)
        {
            var rng = RandomDefaults.CreateRandomSource(seed);

            // Enumerate the sequence and store the result in an array.
            int[] arr = EnumerableUtils.RangeRandomOrder(start, count, rng).ToArray();

            // Perform some basic tests.
            AssertBasicStats(arr, start, count);
                
            // Basic randomness test.
            // As we progress through the sequence, count the number of times and entry is higher and lower than the previous entry.
            // For an ordered sequence the 'lower' count will be zero. For a truly random sequence the two counts have an expected 
            // 50/50 distribution, although any given random sequence may differ substantially from the expected value. By fixing the 
            // random seed we at least ensure that the unit test will pass rather than passing most of the time(!)
            CountLowHighTransitions(arr, out int lo, out int hi);

            Assert.True(hi > 46);
            Assert.True(lo > 46);
        }

        [Fact]
        public void RangeRandomOrder_RandomLargeTestCases()
        {
            var rng = RandomDefaults.CreateRandomSource(0);

            for(int i=0; i < 100; i++)
            {
                int start = rng.Next(100_000);
                int count = rng.Next(100_000);

                // Enumerate the sequence and store the result in an array.
                int[] arr = EnumerableUtils.RangeRandomOrder(start, count, rng).ToArray();

                // Perform some simple tests.
                AssertBasicStats(arr, start, count);
            }  
        }

        [Fact]
        public void RangeRandomOrder_QualityOfRandomness()
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

                // Perform some basic tests.
                AssertBasicStats(arr, start, count);

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
            Assert.True(errorAbs < 0.001);
        }

        #endregion

        #region Private Static Methods

        private static void AssertBasicStats(int[] arr, int start, int count)
        {
            // Simple tests.
            Assert.Equal(count, arr.Length);
            Assert.Equal(start, arr.Min());
            Assert.Equal(start + count - 1, arr.Max());

            // Test for dupes.
            Assert.Empty(      
                arr.GroupBy(x => x)
                   .Where(g => g.Count() > 1)
                   .Select(y => y.Key)
                );
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

        #endregion
    }
}
