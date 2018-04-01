using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Sorting;

namespace Redzen.UnitTests.Sorting
{
    [TestClass]
    public class SortUtilsTests
    {
        [TestMethod]
        [TestCategory("SortUtils")]
        public void TestIsSortedAscending()
        {
            // Sorted.
            Assert.IsTrue(SortUtils.IsSortedAscending(new int[] { 2, 5, 7, 8, 12, 16, 32 }));
            Assert.IsTrue(SortUtils.IsSortedAscending(new int[] { 2, 2, 5, 7, 8, 12, 16, 32 }));
            Assert.IsTrue(SortUtils.IsSortedAscending(new int[] { 2, 5, 5, 7, 8, 12, 16, 32 }));
            Assert.IsTrue(SortUtils.IsSortedAscending(new int[] { 2, 5, 7, 8, 12, 16, 32, 32 }));
            Assert.IsTrue(SortUtils.IsSortedAscending(new string[] { "a", "b", "c", "d", "e" }));
            Assert.IsTrue(SortUtils.IsSortedAscending(new string[] { "a", "a", "c", "d", "e" }));
            Assert.IsTrue(SortUtils.IsSortedAscending(new string[] { null, "a", "a", "c", "d", "e" }));
            Assert.IsTrue(SortUtils.IsSortedAscending(new string[] { null, null, "a", "a", "c", "d", "e" }));

            // Not sorted.
            Assert.IsFalse(SortUtils.IsSortedAscending(new int[] { 2, 5, 8, 7, 12, 16, 32 }));
            Assert.IsFalse(SortUtils.IsSortedAscending(new int[] { 5, 8, 2, 16, 32, 12, 7 }));
            Assert.IsFalse(SortUtils.IsSortedAscending(new string[] { "b", "a", "c", "d", "e" }));
            Assert.IsFalse(SortUtils.IsSortedAscending(new string[] { "a", "c", "e", "d" }));
            Assert.IsFalse(SortUtils.IsSortedAscending(new string[] { "a", null,  "c", "d", "e" }));
            Assert.IsFalse(SortUtils.IsSortedAscending(new string[] { null, "b", "a", "c", "d", "e" }));
        }

        [TestMethod]
        [TestCategory("SortUtils")]
        public void TestIsSortedAscendingComparer()
        {
            // Sorted.
            Assert.IsTrue(SortUtils.IsSortedAscending(new int[] { 2, 5, 7, 8, 12, 16, 32 }, Comparer<int>.Default));
            Assert.IsTrue(SortUtils.IsSortedAscending(new int[] { 2, 2, 5, 7, 8, 12, 16, 32 }, Comparer<int>.Default));
            Assert.IsTrue(SortUtils.IsSortedAscending(new int[] { 2, 5, 5, 7, 8, 12, 16, 32 }, Comparer<int>.Default));
            Assert.IsTrue(SortUtils.IsSortedAscending(new int[] { 2, 5, 7, 8, 12, 16, 32, 32 }, Comparer<int>.Default));
            Assert.IsTrue(SortUtils.IsSortedAscending(new string[] { "a", "b", "c", "d", "e" }, Comparer<string>.Default));
            Assert.IsTrue(SortUtils.IsSortedAscending(new string[] { "a", "a", "c", "d", "e" }, Comparer<string>.Default));
            Assert.IsTrue(SortUtils.IsSortedAscending(new string[] { null, "a", "a", "c", "d", "e" }, Comparer<string>.Default));
            Assert.IsTrue(SortUtils.IsSortedAscending(new string[] { null, null, "a", "a", "c", "d", "e" }, Comparer<string>.Default));

            // Not sorted.
            Assert.IsFalse(SortUtils.IsSortedAscending(new int[] { 2, 5, 8, 7, 12, 16, 32 }, Comparer<int>.Default));
            Assert.IsFalse(SortUtils.IsSortedAscending(new int[] { 5, 8, 2, 16, 32, 12, 7 }, Comparer<int>.Default));
            Assert.IsFalse(SortUtils.IsSortedAscending(new string[] { "b", "a", "c", "d", "e" }, Comparer<string>.Default));
            Assert.IsFalse(SortUtils.IsSortedAscending(new string[] { "a", "c", "e", "d" }, Comparer<string>.Default));
            Assert.IsFalse(SortUtils.IsSortedAscending(new string[] { "a", null,  "c", "d", "e" }, Comparer<string>.Default));
            Assert.IsFalse(SortUtils.IsSortedAscending(new string[] { null, "b", "a", "c", "d", "e" }, Comparer<string>.Default));
        }

    }
}
