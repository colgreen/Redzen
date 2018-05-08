using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Redzen.UnitTests
{
    [TestClass]
    public class MathUtilsTests
    {
        [TestMethod]
        [TestCategory("MathUtils")]
        public void CeilingToPowerOfTwo_Int32()
        {
            // Note. Zero isn't really a power of two!
            Assert.AreEqual(0, MathUtils.CeilingToPowerOfTwo(0));
            Assert.AreEqual(1, MathUtils.CeilingToPowerOfTwo(1));
            Assert.AreEqual(2, MathUtils.CeilingToPowerOfTwo(2));
            Assert.AreEqual(4, MathUtils.CeilingToPowerOfTwo(3));
            Assert.AreEqual(4, MathUtils.CeilingToPowerOfTwo(4));
            Assert.AreEqual(8, MathUtils.CeilingToPowerOfTwo(5));
            Assert.AreEqual(8, MathUtils.CeilingToPowerOfTwo(7));
            Assert.AreEqual(8, MathUtils.CeilingToPowerOfTwo(8));

            Assert.AreEqual(16, MathUtils.CeilingToPowerOfTwo(9));
            Assert.AreEqual(16, MathUtils.CeilingToPowerOfTwo(15));
            Assert.AreEqual(16, MathUtils.CeilingToPowerOfTwo(16));

            Assert.AreEqual(32, MathUtils.CeilingToPowerOfTwo(17));
            Assert.AreEqual(32, MathUtils.CeilingToPowerOfTwo(31));
            Assert.AreEqual(32, MathUtils.CeilingToPowerOfTwo(32));

            Assert.AreEqual(0x2000_0000, MathUtils.CeilingToPowerOfTwo(0x1000_0001));
            Assert.AreEqual(0x2000_0000, MathUtils.CeilingToPowerOfTwo(0x1FFF_FFFF));
            Assert.AreEqual(0x2000_0000, MathUtils.CeilingToPowerOfTwo(0x2000_0000));

            Assert.AreEqual(0x4000_0000, MathUtils.CeilingToPowerOfTwo(0x2000_0001));
            Assert.AreEqual(0x4000_0000, MathUtils.CeilingToPowerOfTwo(0x3FFF_FFFF));
            Assert.AreEqual(0x4000_0000, MathUtils.CeilingToPowerOfTwo(0x4000_0000));
        }

        [TestMethod]
        [TestCategory("MathUtils")]
        public void CeilingToPowerOfTwo_Int64()
        {
            // Note. Zero isn't really a power of two!
            Assert.AreEqual(0, MathUtils.CeilingToPowerOfTwo(0L));
            Assert.AreEqual(1, MathUtils.CeilingToPowerOfTwo(1L));
            Assert.AreEqual(2, MathUtils.CeilingToPowerOfTwo(2L));
            Assert.AreEqual(4, MathUtils.CeilingToPowerOfTwo(3L));
            Assert.AreEqual(4, MathUtils.CeilingToPowerOfTwo(4L));
            Assert.AreEqual(8, MathUtils.CeilingToPowerOfTwo(5L));
            Assert.AreEqual(8, MathUtils.CeilingToPowerOfTwo(7L));
            Assert.AreEqual(8, MathUtils.CeilingToPowerOfTwo(8L));

            Assert.AreEqual(16, MathUtils.CeilingToPowerOfTwo(9L));
            Assert.AreEqual(16, MathUtils.CeilingToPowerOfTwo(15L));
            Assert.AreEqual(16, MathUtils.CeilingToPowerOfTwo(16L));

            Assert.AreEqual(32, MathUtils.CeilingToPowerOfTwo(17L));
            Assert.AreEqual(32, MathUtils.CeilingToPowerOfTwo(31L));
            Assert.AreEqual(32, MathUtils.CeilingToPowerOfTwo(32L));

            Assert.AreEqual(0x2000_0000, MathUtils.CeilingToPowerOfTwo(0x1000_0001L));
            Assert.AreEqual(0x2000_0000, MathUtils.CeilingToPowerOfTwo(0x1FFF_FFFFL));
            Assert.AreEqual(0x2000_0000, MathUtils.CeilingToPowerOfTwo(0x2000_0000L));

            Assert.AreEqual(0x4000_0000, MathUtils.CeilingToPowerOfTwo(0x2000_0001L));
            Assert.AreEqual(0x4000_0000, MathUtils.CeilingToPowerOfTwo(0x3FFF_FFFFL));
            Assert.AreEqual(0x4000_0000, MathUtils.CeilingToPowerOfTwo(0x4000_0000L));

            Assert.AreEqual(0x2000_0000_0000_0000, MathUtils.CeilingToPowerOfTwo(0x1000_0000_0000_0001L));
            Assert.AreEqual(0x2000_0000_0000_0000, MathUtils.CeilingToPowerOfTwo(0x1FFF_FFFF_FFFF_FFFFL));
            Assert.AreEqual(0x2000_0000_0000_0000, MathUtils.CeilingToPowerOfTwo(0x2000_0000_0000_0000L));

            Assert.AreEqual(0x4000_0000_0000_0000, MathUtils.CeilingToPowerOfTwo(0x2000_0000_0000_0001L));
            Assert.AreEqual(0x4000_0000_0000_0000, MathUtils.CeilingToPowerOfTwo(0x3FFF_FFFF_FFFF_FFFFL));
            Assert.AreEqual(0x4000_0000_0000_0000, MathUtils.CeilingToPowerOfTwo(0x4000_0000_0000_0000L));
        }

        [TestMethod]
        [TestCategory("MathUtils")]
        public void Log2_Int32()
        {
            Assert.AreEqual(0, MathUtils.Log2(0x1));
            Assert.AreEqual(1, MathUtils.Log2(0x2));
            Assert.AreEqual(1, MathUtils.Log2(0x3));
            Assert.AreEqual(2, MathUtils.Log2(0x4));
            Assert.AreEqual(2, MathUtils.Log2(0x5));
            Assert.AreEqual(2, MathUtils.Log2(0x6));
            Assert.AreEqual(2, MathUtils.Log2(0x7));
            Assert.AreEqual(3, MathUtils.Log2(0x8));
            Assert.AreEqual(3, MathUtils.Log2(0x9));

            Assert.AreEqual(3, MathUtils.Log2(0xF));
            Assert.AreEqual(4, MathUtils.Log2(0x10));
            Assert.AreEqual(4, MathUtils.Log2(0x11));

            Assert.AreEqual(4, MathUtils.Log2(0x1F));
            Assert.AreEqual(5, MathUtils.Log2(0x20));
            Assert.AreEqual(5, MathUtils.Log2(0x21));

            Assert.AreEqual(29, MathUtils.Log2(0x3FFF_FFFF));
            Assert.AreEqual(30, MathUtils.Log2(0x4000_0000));
            Assert.AreEqual(30, MathUtils.Log2(0x4000_0001));
        }

        [TestMethod]
        [TestCategory("MathUtils")]
        public void Log2_Int64()
        {
            Assert.AreEqual(0, MathUtils.Log2(0x1L));
            Assert.AreEqual(1, MathUtils.Log2(0x2L));
            Assert.AreEqual(1, MathUtils.Log2(0x3L));
            Assert.AreEqual(2, MathUtils.Log2(0x4L));
            Assert.AreEqual(2, MathUtils.Log2(0x5L));
            Assert.AreEqual(2, MathUtils.Log2(0x6L));
            Assert.AreEqual(2, MathUtils.Log2(0x7L));
            Assert.AreEqual(3, MathUtils.Log2(0x8L));
            Assert.AreEqual(3, MathUtils.Log2(0x9L));

            Assert.AreEqual(3, MathUtils.Log2(0xFL));
            Assert.AreEqual(4, MathUtils.Log2(0x10L));
            Assert.AreEqual(4, MathUtils.Log2(0x11L));

            Assert.AreEqual(4, MathUtils.Log2(0x1FL));
            Assert.AreEqual(5, MathUtils.Log2(0x20L));
            Assert.AreEqual(5, MathUtils.Log2(0x21L));

            Assert.AreEqual(29, MathUtils.Log2(0x3FFF_FFFFL));
            Assert.AreEqual(30, MathUtils.Log2(0x4000_0000L));
            Assert.AreEqual(30, MathUtils.Log2(0x4000_0001L));
        }
    }
}
