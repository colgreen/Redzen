using System.Collections.Generic;
using System.Reflection;
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

            // Not sorted.
            Assert.IsFalse(SortUtils.IsSortedAscending(new int[] { 2, 5, 8, 7, 12, 16, 32 }));
            Assert.IsFalse(SortUtils.IsSortedAscending(new int[] { 5, 8, 2, 16, 32, 12, 7 }));
            Assert.IsFalse(SortUtils.IsSortedAscending(new string[] { "b", "a", "c", "d", "e" }));
            Assert.IsFalse(SortUtils.IsSortedAscending(new string[] { "a", "c", "e", "d" }));
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

        [TestMethod]
        [TestCategory("SortUtils")]
        public void TestIsSortedNullableAscending()
        {
            // Sorted.
            Assert.IsTrue(SortUtils.IsSortedNullableAscending(new int[] { 2, 5, 7, 8, 12, 16, 32 }));
            Assert.IsTrue(SortUtils.IsSortedNullableAscending(new int[] { 2, 2, 5, 7, 8, 12, 16, 32 }));
            Assert.IsTrue(SortUtils.IsSortedNullableAscending(new int[] { 2, 5, 5, 7, 8, 12, 16, 32 }));
            Assert.IsTrue(SortUtils.IsSortedNullableAscending(new int[] { 2, 5, 7, 8, 12, 16, 32, 32 }));
            Assert.IsTrue(SortUtils.IsSortedNullableAscending(new string[] { "a", "b", "c", "d", "e" }));
            Assert.IsTrue(SortUtils.IsSortedNullableAscending(new string[] { "a", "a", "c", "d", "e" }));
            Assert.IsTrue(SortUtils.IsSortedNullableAscending(new string[] { null, "a", "a", "c", "d", "e" }));
            Assert.IsTrue(SortUtils.IsSortedNullableAscending(new string[] { null, null, "a", "a", "c", "d", "e" }));

            // Not sorted.
            Assert.IsFalse(SortUtils.IsSortedNullableAscending(new int[] { 2, 5, 8, 7, 12, 16, 32 }));
            Assert.IsFalse(SortUtils.IsSortedNullableAscending(new int[] { 5, 8, 2, 16, 32, 12, 7 }));
            Assert.IsFalse(SortUtils.IsSortedNullableAscending(new string[] { "b", "a", "c", "d", "e" }));
            Assert.IsFalse(SortUtils.IsSortedNullableAscending(new string[] { "a", "c", "e", "d" }));
            Assert.IsFalse(SortUtils.IsSortedNullableAscending(new string[] { "a", null, "c", "d", "e" }));
            Assert.IsFalse(SortUtils.IsSortedNullableAscending(new string[] { null, "b", "a", "c", "d", "e" }));
        }

        [TestMethod]
        [TestCategory("SortUtils")]
        public void TestIsSortedNullableAscendingComparer()
        {
            // Sorted.
            Assert.IsTrue(SortUtils.IsSortedNullableAscending(new int[] { 2, 5, 7, 8, 12, 16, 32 }, Comparer<int>.Default));
            Assert.IsTrue(SortUtils.IsSortedNullableAscending(new int[] { 2, 2, 5, 7, 8, 12, 16, 32 }, Comparer<int>.Default));
            Assert.IsTrue(SortUtils.IsSortedNullableAscending(new int[] { 2, 5, 5, 7, 8, 12, 16, 32 }, Comparer<int>.Default));
            Assert.IsTrue(SortUtils.IsSortedNullableAscending(new int[] { 2, 5, 7, 8, 12, 16, 32, 32 }, Comparer<int>.Default));
            Assert.IsTrue(SortUtils.IsSortedNullableAscending(new string[] { "a", "b", "c", "d", "e" }, Comparer<string>.Default));
            Assert.IsTrue(SortUtils.IsSortedNullableAscending(new string[] { "a", "a", "c", "d", "e" }, Comparer<string>.Default));
            Assert.IsTrue(SortUtils.IsSortedNullableAscending(new string[] { null, "a", "a", "c", "d", "e" }, Comparer<string>.Default));
            Assert.IsTrue(SortUtils.IsSortedNullableAscending(new string[] { null, null, "a", "a", "c", "d", "e" }, Comparer<string>.Default));

            // Not sorted.
            Assert.IsFalse(SortUtils.IsSortedNullableAscending(new int[] { 2, 5, 8, 7, 12, 16, 32 }, Comparer<int>.Default));
            Assert.IsFalse(SortUtils.IsSortedNullableAscending(new int[] { 5, 8, 2, 16, 32, 12, 7 }, Comparer<int>.Default));
            Assert.IsFalse(SortUtils.IsSortedNullableAscending(new string[] { "b", "a", "c", "d", "e" }, Comparer<string>.Default));
            Assert.IsFalse(SortUtils.IsSortedNullableAscending(new string[] { "a", "c", "e", "d" }, Comparer<string>.Default));
            Assert.IsFalse(SortUtils.IsSortedNullableAscending(new string[] { "a", null, "c", "d", "e" }, Comparer<string>.Default));
            Assert.IsFalse(SortUtils.IsSortedNullableAscending(new string[] { null, "b", "a", "c", "d", "e" }, Comparer<string>.Default));
        }

        [TestMethod]
        [TestCategory("SortUtils")]
        public void TestTryFindSegment()
        {
            MethodInfo methodInfo = typeof(SortUtils).GetMethod("TryFindSegment", BindingFlags.Static | BindingFlags.NonPublic);
            object[] args = new object[4];

            args[0] = CreateIntListWithSegment(100, 30, 10);
            args[1] = Comparer<int>.Default;
            args[2] = 0;

            MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(typeof(int));
            object result = genericMethodInfo.Invoke(null, args);

            Assert.AreEqual(true, result);
            Assert.AreEqual(30, args[2]);
            Assert.AreEqual(39, args[3]);
        }

        private static List<int> CreateIntListWithSegment(int length, int segStartIdx, int segLength)
        {
            List<int> list = new List<int>(length);
            int i=0;
            for(; i < segStartIdx; i++) {
                list.Add(i);
            }

            int val = i;
            int segEndIdx = segStartIdx + segLength;
            for(; i < segEndIdx; i++) {
                list.Add(val);
            }

            for(val++; i < length; i++, val++) {
                list.Add(val);
            }

            return list;
        }
    }
}
