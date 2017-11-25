using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Sorting;

namespace Redzen.UnitTests.Sorting
{
    [TestClass]
    public class SortUtilsTests
    {
        [TestMethod]
        [TestCategory("SortUtils")]
        public void TestIsSorted()
        {
            // Sorted.
            Assert.IsTrue(SortUtils.IsSorted(new int[] { 2, 5, 7, 8, 12, 16, 32 }));
            Assert.IsTrue(SortUtils.IsSorted(new int[] { 2, 2, 5, 7, 8, 12, 16, 32 }));
            Assert.IsTrue(SortUtils.IsSorted(new int[] { 2, 5, 5, 7, 8, 12, 16, 32 }));
            Assert.IsTrue(SortUtils.IsSorted(new int[] { 2, 5, 7, 8, 12, 16, 32, 32 }));
            Assert.IsTrue(SortUtils.IsSorted(new string[] { "a", "b", "c", "d", "e" }));
            Assert.IsTrue(SortUtils.IsSorted(new string[] { "a", "a", "c", "d", "e" }));
            Assert.IsTrue(SortUtils.IsSorted(new string[] { null, "a", "a", "c", "d", "e" }));
            Assert.IsTrue(SortUtils.IsSorted(new string[] { null, null, "a", "a", "c", "d", "e" }));

            // Not sorted.
            Assert.IsFalse(SortUtils.IsSorted(new int[] { 2, 5, 8, 7, 12, 16, 32 }));
            Assert.IsFalse(SortUtils.IsSorted(new int[] { 5, 8, 2, 16, 32, 12, 7 }));
            Assert.IsFalse(SortUtils.IsSorted(new string[] { "b", "a", "c", "d", "e" }));
            Assert.IsFalse(SortUtils.IsSorted(new string[] { "a", "c", "e", "d" }));
            Assert.IsFalse(SortUtils.IsSorted(new string[] { "a", null,  "c", "d", "e" }));
            Assert.IsFalse(SortUtils.IsSorted(new string[] { null, "b", "a", "c", "d", "e" }));
        }
    }
}
