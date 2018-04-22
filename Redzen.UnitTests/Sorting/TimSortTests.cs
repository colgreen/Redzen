using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;
using Redzen.Sorting;

namespace Redzen.UnitTests.Sorting
{
    [TestClass]
    public class TimSortTests
    {
        #region Public Test Methods

        [TestMethod]
        [TestCategory("TimSort")]
        public void ShortArray()
        {
            int[] keys = new int[] { 5,   8,  2, 16, 32, 12,  7};
            TimSort<int>.Sort(keys, 0, keys.Length);
            Assert.IsTrue(ArrayUtils.Equals(new int[]{  2,  5,  7,  8, 12, 16, 32 }, keys));
        }

        [TestMethod]
        [TestCategory("TimSort")]
        public void LongRandomArrays()
        {
            XorShiftRandom rng = new XorShiftRandom(0);

            int length = rng.Next(200000);
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

            // Sort array.
            TimSort<int>.Sort(keys);

            // Check array is sorted.
            Assert.IsTrue(SortUtils.IsSortedAscending(keys));
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
