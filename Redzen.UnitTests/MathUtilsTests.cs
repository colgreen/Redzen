using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Redzen.UnitTests
{
    [TestClass]
    public class MathUtilsTests
    {
        [TestMethod]
        [TestCategory("MathUtils")]
        public void IsPowerOfTwo_Int32()
        {
            Assert.IsFalse(MathUtils.IsPowerOfTwo(0));
            Assert.IsTrue(MathUtils.IsPowerOfTwo(1));
            Assert.IsTrue(MathUtils.IsPowerOfTwo(2));
            Assert.IsFalse(MathUtils.IsPowerOfTwo(3));

            // Loop over integer exponents.
            for(int i=4; i < 31; i++)
            {
                // Test exact power of two.
                Assert.IsTrue(MathUtils.IsPowerOfTwo(1 << i));

                // Test boundary and near boundary cases.
                Assert.IsFalse(MathUtils.IsPowerOfTwo((1 << i) - 3));
                Assert.IsFalse(MathUtils.IsPowerOfTwo((1 << i) - 2));
                Assert.IsFalse(MathUtils.IsPowerOfTwo((1 << i) - 1));
                Assert.IsFalse(MathUtils.IsPowerOfTwo((1 << i) + 1));
                Assert.IsFalse(MathUtils.IsPowerOfTwo((1 << i) + 2));
                Assert.IsFalse(MathUtils.IsPowerOfTwo((1 << i) + 3));
            }

            Assert.IsFalse(MathUtils.IsPowerOfTwo(int.MaxValue)); // 2^31 - 1
        }

        [TestMethod]
        [TestCategory("MathUtils")]
        public void IsPowerOfTwo_Int64()
        {
            Assert.IsFalse(MathUtils.IsPowerOfTwo(0L));
            Assert.IsTrue(MathUtils.IsPowerOfTwo(1L));
            Assert.IsTrue(MathUtils.IsPowerOfTwo(2L));
            Assert.IsFalse(MathUtils.IsPowerOfTwo(3L));

            // Loop over integer exponents.
            for(int i=4; i < 63; i++)
            {
                // Test exact power of two.
                Assert.IsTrue(MathUtils.IsPowerOfTwo(1L << i));

                // Test boundary and near boundary cases.
                Assert.IsFalse(MathUtils.IsPowerOfTwo((1L << i) - 3));
                Assert.IsFalse(MathUtils.IsPowerOfTwo((1L << i) - 2));
                Assert.IsFalse(MathUtils.IsPowerOfTwo((1L << i) - 1));
                Assert.IsFalse(MathUtils.IsPowerOfTwo((1L << i) + 1));
                Assert.IsFalse(MathUtils.IsPowerOfTwo((1L << i) + 2));
                Assert.IsFalse(MathUtils.IsPowerOfTwo((1L << i) + 3));
            }

            Assert.IsFalse(MathUtils.IsPowerOfTwo(uint.MaxValue)); // 2^63 - 1
        }

        [TestMethod]
        [TestCategory("MathUtils")]
        public void PowerOfTwo_Int32()
        {
            Assert.AreEqual(1, MathUtils.PowerOfTwo(0));
            Assert.AreEqual(2, MathUtils.PowerOfTwo(1));
            Assert.AreEqual(4, MathUtils.PowerOfTwo(2));
            Assert.AreEqual(8, MathUtils.PowerOfTwo(3));

            // Loop over integer exponents.
            int expected = 16;
            for(int i=4; i < 31; i++, expected *=2) {
                Assert.AreEqual(expected, MathUtils.PowerOfTwo(i));
            }
        }

        [TestMethod]
        [TestCategory("MathUtils")]
        public void PowerOfTwo_Int64()
        {
            Assert.AreEqual(1, MathUtils.PowerOfTwo(0L));
            Assert.AreEqual(2, MathUtils.PowerOfTwo(1L));
            Assert.AreEqual(4, MathUtils.PowerOfTwo(2L));
            Assert.AreEqual(8, MathUtils.PowerOfTwo(3L));

            // Loop over integer exponents.
            long expected = 16;
            for(long i=4; i < 63; i++, expected *=2) {
                Assert.AreEqual(expected, MathUtils.PowerOfTwo(i));
            }
        }

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

            Assert.AreEqual(61, MathUtils.Log2(0x3FFF_FFFF_FFFF_FFFFL));
            Assert.AreEqual(62, MathUtils.Log2(0x4000_0000_0000_0000L));
            Assert.AreEqual(62, MathUtils.Log2(0x4000_0000_0000_0001L));
        }

        [TestMethod]
        [TestCategory("MathUtils")]
        public void Log2Ceiling_Int32()
        {
            Assert.AreEqual(0, MathUtils.Log2Ceiling(0x1));
            Assert.AreEqual(1, MathUtils.Log2Ceiling(0x2));
            Assert.AreEqual(2, MathUtils.Log2Ceiling(0x3));
            Assert.AreEqual(2, MathUtils.Log2Ceiling(0x4));
            Assert.AreEqual(3, MathUtils.Log2Ceiling(0x5));
            Assert.AreEqual(3, MathUtils.Log2Ceiling(0x6));
            Assert.AreEqual(3, MathUtils.Log2Ceiling(0x7));
            Assert.AreEqual(3, MathUtils.Log2Ceiling(0x8));
            Assert.AreEqual(4, MathUtils.Log2Ceiling(0x9));

            Assert.AreEqual(4, MathUtils.Log2Ceiling(0xF));
            Assert.AreEqual(4, MathUtils.Log2Ceiling(0x10));
            Assert.AreEqual(5, MathUtils.Log2Ceiling(0x11));

            Assert.AreEqual(5, MathUtils.Log2Ceiling(0x1F));
            Assert.AreEqual(5, MathUtils.Log2Ceiling(0x20));
            Assert.AreEqual(6, MathUtils.Log2Ceiling(0x21));

            Assert.AreEqual(30, MathUtils.Log2Ceiling(0x3FFF_FFFF));
            Assert.AreEqual(30, MathUtils.Log2Ceiling(0x4000_0000));
            Assert.AreEqual(31, MathUtils.Log2Ceiling(0x4000_0001));
        }

        [TestMethod]
        [TestCategory("MathUtils")]
        public void Log2Ceiling_Int64()
        {
            Assert.AreEqual(0, MathUtils.Log2Ceiling(0x1L));
            Assert.AreEqual(1, MathUtils.Log2Ceiling(0x2L));
            Assert.AreEqual(2, MathUtils.Log2Ceiling(0x3L));
            Assert.AreEqual(2, MathUtils.Log2Ceiling(0x4L));
            Assert.AreEqual(3, MathUtils.Log2Ceiling(0x5L));
            Assert.AreEqual(3, MathUtils.Log2Ceiling(0x6L));
            Assert.AreEqual(3, MathUtils.Log2Ceiling(0x7L));
            Assert.AreEqual(3, MathUtils.Log2Ceiling(0x8L));
            Assert.AreEqual(4, MathUtils.Log2Ceiling(0x9L));

            Assert.AreEqual(4, MathUtils.Log2Ceiling(0xFL));
            Assert.AreEqual(4, MathUtils.Log2Ceiling(0x10L));
            Assert.AreEqual(5, MathUtils.Log2Ceiling(0x11L));

            Assert.AreEqual(5, MathUtils.Log2Ceiling(0x1FL));
            Assert.AreEqual(5, MathUtils.Log2Ceiling(0x20L));
            Assert.AreEqual(6, MathUtils.Log2Ceiling(0x21L));

            Assert.AreEqual(30, MathUtils.Log2Ceiling(0x3FFF_FFFFL));
            Assert.AreEqual(30, MathUtils.Log2Ceiling(0x4000_0000L));
            Assert.AreEqual(31, MathUtils.Log2Ceiling(0x4000_0001L));

            Assert.AreEqual(62, MathUtils.Log2Ceiling(0x3FFF_FFFF_FFFF_FFFFL));
            Assert.AreEqual(62, MathUtils.Log2Ceiling(0x4000_0000_0000_0000L));
            Assert.AreEqual(63, MathUtils.Log2Ceiling(0x4000_0000_0000_0001L));
        }
    }
}
