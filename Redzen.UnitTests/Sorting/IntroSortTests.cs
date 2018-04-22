using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Sorting;

namespace Redzen.UnitTests.Sorting
{
    [TestClass]
    public class IntroSortTests
    {
        [TestMethod]
        [TestCategory("IntroSort")]
        public void Test1()
        {
            int[] keys = new int[] { 5,   8,  2, 16, 32, 12,  7};
            int[] v = new int[]    { 45, 42, 48, 24,  8, 28, 43};
            int[] w = new int[]    { 0,   1,  2,  3,  4,  5,  6};
            IntroSort<int,int,int>.Sort(keys, v, w);

            Assert.IsTrue(ArrayUtils.Equals(new int[]{  2,  5,  7,  8, 12, 16, 32 }, keys));
            Assert.IsTrue(ArrayUtils.Equals(new int[]{ 48, 45, 43, 42, 28, 24,  8 }, v));
            Assert.IsTrue(ArrayUtils.Equals(new int[]{  2,  0,  6,  1,  5,  3,  4 }, w));
        }
    }
}
