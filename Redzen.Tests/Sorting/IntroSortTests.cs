using Redzen.Random;
using Xunit;

namespace Redzen.Sorting.Tests
{
    public class IntroSortTests
    {
        #region Public Test Methods

        [Fact]
        public void Sort_ShortArray()
        {
            int[] keys = new int[] { 5,   8,  2, 16, 32, 12,  7};
            int[] v = new int[]    { 45, 42, 48, 24,  8, 28, 43};
            int[] w = new int[]    { 0,   1,  2,  3,  4,  5,  6};
            IntroSort<int,int,int>.Sort(keys, v, w);

            Assert.True(SpanUtils.Equals<int>(new int[]{  2,  5,  7,  8, 12, 16, 32 }, keys));
            Assert.True(SpanUtils.Equals<int>(new int[]{ 48, 45, 43, 42, 28, 24,  8 }, v));
            Assert.True(SpanUtils.Equals<int>(new int[]{  2,  0,  6,  1,  5,  3,  4 }, w));
        }

        [Fact]
        public void Sort_LongRandomArrays()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(0);

            int length = rng.Next(200_000);
            for(int i=0; i < 100; i++)
            {
                LongRandomArraysInner(length, rng);
            }
        }

        #endregion

        #region Private Static Methods

        private void LongRandomArraysInner(int len, IRandomSource rng)
        {
            // Create random array.
            int[] keys = CreateRandomArray(len, rng);
            int[] v = (int[])keys.Clone();
            int[] w = (int[])keys.Clone();

            // Sort array.
            IntroSort<int,int,int>.Sort(keys, v, w);

            // Check array is sorted.
            Assert.True(SortUtils.IsSortedAscending(keys));
            Assert.True(SortUtils.IsSortedAscending(v));
            Assert.True(SortUtils.IsSortedAscending(w));
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
