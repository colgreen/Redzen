using Redzen.Random;
using System;
using Xunit;

namespace Redzen.Sorting.Tests
{
    public class TimSortKVWTests
    {
        #region Public Test Methods

        [Fact]
        public void Sort_ShortArray()
        {
            int[] keys = new int[] { 5, 8, 2, 16, 32, 12,  7 };
            int[] vals = new int[] { 0, 1, 2,  3,  4,  5,  6 };
            int[] wals = new int[] { 6, 5, 4,  3,  2,  1,  0 };

            TimSort<int,int,int>.Sort(keys, vals, wals);

            Assert.Equal(new int[] { 2,  5,  7,  8, 12, 16, 32 }, keys);
            Assert.Equal(new int[] { 2,  0,  6,  1, 5,   3,  4 }, vals);
            Assert.Equal(new int[] { 4,  6,  0,  5, 1,   3,  2 }, wals);
        }

        [Fact]
        public void Sort_LongRandomArrays()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(0);

            for (int i=0; i < 100; i++)
            {
                int length = rng.Next(200_000);
                Sort_LongRandomArrays_Inner(length, rng);
            }
        }

        #endregion

        #region Private Static Methods

        private void Sort_LongRandomArrays_Inner(int len, IRandomSource rng)
        {
            // Create random array.
            int[] keys = CreateRandomArray(len, rng);

            // For the vals array, use a copy of the keys, but add a large constant so that we
            // can be sure keys weren't just copied by accident into vals.
            const int offsetv = 1_000_000;
            int[] vals = (int[])keys.Clone();
            for(int i=0; i < vals.Length; i++) {
                vals[i] += offsetv;
            }

            // Repeat the same procedure for wals.
            const int offsetw = 10_000_000;
            int[] wals = (int[])keys.Clone();
            for(int i=0; i < wals.Length; i++) {
                wals[i] += offsetw;
            }

            // Sort array.
            TimSort<int,int,int>.Sort(keys, vals, wals);

            // Check array is sorted.
            Assert.True(SortUtils.IsSortedAscending((Span<int>)keys));

            // Checks vals.
            for(int i=0; i < keys.Length; i++) {
                Assert.Equal(keys[i] + offsetv, vals[i]);
            }

            // Checks wals.
            for(int i=0; i < keys.Length; i++) {
                Assert.Equal(keys[i] + offsetw, wals[i]);
            }
        }

        private static int[] CreateRandomArray(int len, IRandomSource rng)
        {
            var arr = new int[len];
            for(int i=0; i < len; i++) {
                arr[i] = rng.Next(int.MinValue, int.MaxValue);
            }
            return arr;
        }

        #endregion
    }
}
