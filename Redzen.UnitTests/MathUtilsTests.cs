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
            Assert.AreEqual(0, MathUtils.Log2(0x1U));
            Assert.AreEqual(1, MathUtils.Log2(0x2U));
            Assert.AreEqual(1, MathUtils.Log2(0x3U));
            Assert.AreEqual(2, MathUtils.Log2(0x4U));
            Assert.AreEqual(2, MathUtils.Log2(0x5U));
            Assert.AreEqual(2, MathUtils.Log2(0x6U));
            Assert.AreEqual(2, MathUtils.Log2(0x7U));
            Assert.AreEqual(3, MathUtils.Log2(0x8U));
            Assert.AreEqual(3, MathUtils.Log2(0x9U));

            Assert.AreEqual(3, MathUtils.Log2(0xFU));
            Assert.AreEqual(4, MathUtils.Log2(0x10U));
            Assert.AreEqual(4, MathUtils.Log2(0x11U));

            Assert.AreEqual(4, MathUtils.Log2(0x1FU));
            Assert.AreEqual(5, MathUtils.Log2(0x20U));
            Assert.AreEqual(5, MathUtils.Log2(0x21U));

            Assert.AreEqual(29, MathUtils.Log2(0x3FFF_FFFFU));
            Assert.AreEqual(30, MathUtils.Log2(0x4000_0000U));
            Assert.AreEqual(30, MathUtils.Log2(0x4000_0001U));

            Assert.AreEqual(30, MathUtils.Log2(0x7FFF_FFFFU));
            Assert.AreEqual(31, MathUtils.Log2(0x8000_0000U));
            Assert.AreEqual(31, MathUtils.Log2(0xFFFF_FFFFU));
        }

        [TestMethod]
        [TestCategory("MathUtils")]
        public void Log2_Int64()
        {
            Assert.AreEqual(0, MathUtils.Log2(0x1UL));
            Assert.AreEqual(1, MathUtils.Log2(0x2UL));
            Assert.AreEqual(1, MathUtils.Log2(0x3UL));
            Assert.AreEqual(2, MathUtils.Log2(0x4UL));
            Assert.AreEqual(2, MathUtils.Log2(0x5UL));
            Assert.AreEqual(2, MathUtils.Log2(0x6UL));
            Assert.AreEqual(2, MathUtils.Log2(0x7UL));
            Assert.AreEqual(3, MathUtils.Log2(0x8UL));
            Assert.AreEqual(3, MathUtils.Log2(0x9UL));

            Assert.AreEqual(3, MathUtils.Log2(0xFUL));
            Assert.AreEqual(4, MathUtils.Log2(0x10UL));
            Assert.AreEqual(4, MathUtils.Log2(0x11UL));

            Assert.AreEqual(4, MathUtils.Log2(0x1FUL));
            Assert.AreEqual(5, MathUtils.Log2(0x20UL));
            Assert.AreEqual(5, MathUtils.Log2(0x21UL));

            Assert.AreEqual(29, MathUtils.Log2(0x3FFF_FFFFUL));
            Assert.AreEqual(30, MathUtils.Log2(0x4000_0000UL));
            Assert.AreEqual(30, MathUtils.Log2(0x4000_0001UL));

            Assert.AreEqual(61, MathUtils.Log2(0x3FFF_FFFF_FFFF_FFFFUL));
            Assert.AreEqual(62, MathUtils.Log2(0x4000_0000_0000_0000UL));
            Assert.AreEqual(62, MathUtils.Log2(0x4000_0000_0000_0001UL));

            Assert.AreEqual(62, MathUtils.Log2(0x7FFF_FFFF_FFFF_FFFFUL));
            Assert.AreEqual(63, MathUtils.Log2(0x8000_0000_0000_0000UL));
            Assert.AreEqual(63, MathUtils.Log2(0xFFFF_FFFF_FFFF_FFFFUL));
        }

        [TestMethod]
        [TestCategory("MathUtils")]
        public void Log2Ceiling_UInt32()
        {
            Assert.AreEqual(0, MathUtils.Log2Ceiling(0x1U));
            Assert.AreEqual(1, MathUtils.Log2Ceiling(0x2U));
            Assert.AreEqual(2, MathUtils.Log2Ceiling(0x3U));
            Assert.AreEqual(2, MathUtils.Log2Ceiling(0x4U));
            Assert.AreEqual(3, MathUtils.Log2Ceiling(0x5U));
            Assert.AreEqual(3, MathUtils.Log2Ceiling(0x6U));
            Assert.AreEqual(3, MathUtils.Log2Ceiling(0x7U));
            Assert.AreEqual(3, MathUtils.Log2Ceiling(0x8U));
            Assert.AreEqual(4, MathUtils.Log2Ceiling(0x9U));

            Assert.AreEqual(4, MathUtils.Log2Ceiling(0xFU));
            Assert.AreEqual(4, MathUtils.Log2Ceiling(0x10U));
            Assert.AreEqual(5, MathUtils.Log2Ceiling(0x11U));

            Assert.AreEqual(5, MathUtils.Log2Ceiling(0x1FU));
            Assert.AreEqual(5, MathUtils.Log2Ceiling(0x20U));
            Assert.AreEqual(6, MathUtils.Log2Ceiling(0x21U));

            Assert.AreEqual(30, MathUtils.Log2Ceiling(0x3FFF_FFFFU));
            Assert.AreEqual(30, MathUtils.Log2Ceiling(0x4000_0000U));
            Assert.AreEqual(31, MathUtils.Log2Ceiling(0x4000_0001U));

            Assert.AreEqual(31, MathUtils.Log2Ceiling(0x7FFF_FFFFU));
            Assert.AreEqual(32, MathUtils.Log2Ceiling(0x8000_0000U));
            Assert.AreEqual(32, MathUtils.Log2Ceiling(0x8000_0001U));
            Assert.AreEqual(32, MathUtils.Log2Ceiling(0xFFFF_FFFFU));
        }

        [TestMethod]
        [TestCategory("MathUtils")]
        public void Log2Ceiling_UInt64()
        {
            Assert.AreEqual(0, MathUtils.Log2Ceiling(0x1UL));
            Assert.AreEqual(1, MathUtils.Log2Ceiling(0x2UL));
            Assert.AreEqual(2, MathUtils.Log2Ceiling(0x3UL));
            Assert.AreEqual(2, MathUtils.Log2Ceiling(0x4UL));
            Assert.AreEqual(3, MathUtils.Log2Ceiling(0x5UL));
            Assert.AreEqual(3, MathUtils.Log2Ceiling(0x6UL));
            Assert.AreEqual(3, MathUtils.Log2Ceiling(0x7UL));
            Assert.AreEqual(3, MathUtils.Log2Ceiling(0x8UL));
            Assert.AreEqual(4, MathUtils.Log2Ceiling(0x9UL));

            Assert.AreEqual(4, MathUtils.Log2Ceiling(0xFUL));
            Assert.AreEqual(4, MathUtils.Log2Ceiling(0x10UL));
            Assert.AreEqual(5, MathUtils.Log2Ceiling(0x11UL));

            Assert.AreEqual(5, MathUtils.Log2Ceiling(0x1FUL));
            Assert.AreEqual(5, MathUtils.Log2Ceiling(0x20UL));
            Assert.AreEqual(6, MathUtils.Log2Ceiling(0x21UL));

            Assert.AreEqual(30, MathUtils.Log2Ceiling(0x3FFF_FFFFUL));
            Assert.AreEqual(30, MathUtils.Log2Ceiling(0x4000_0000UL));
            Assert.AreEqual(31, MathUtils.Log2Ceiling(0x4000_0001UL));

            Assert.AreEqual(62, MathUtils.Log2Ceiling(0x3FFF_FFFF_FFFF_FFFFUL));
            Assert.AreEqual(62, MathUtils.Log2Ceiling(0x4000_0000_0000_0000UL));
            Assert.AreEqual(63, MathUtils.Log2Ceiling(0x4000_0000_0000_0001UL));

            Assert.AreEqual(63, MathUtils.Log2Ceiling(0x7FFF_FFFF_FFFF_FFFFUL));
            Assert.AreEqual(63, MathUtils.Log2Ceiling(0x8000_0000_0000_0000UL));
            Assert.AreEqual(64, MathUtils.Log2Ceiling(0x8000_0000_0000_0001UL));
            Assert.AreEqual(64, MathUtils.Log2Ceiling(0xFFFF_FFFF_FFFF_FFFFUL));
        }

        [TestMethod]
        [TestCategory("MathUtils")]
        public void CountLeadingZeros_UInt32()
        {
            Assert.AreEqual(32, MathUtils.CountLeadingZeros(0x0u));

            Assert.AreEqual(31, MathUtils.CountLeadingZeros(0x1u));
            Assert.AreEqual(30, MathUtils.CountLeadingZeros(0x2u));
            Assert.AreEqual(30, MathUtils.CountLeadingZeros(0x3u));
            Assert.AreEqual(29, MathUtils.CountLeadingZeros(0x4u));
            Assert.AreEqual(29, MathUtils.CountLeadingZeros(0x5u));
            Assert.AreEqual(29, MathUtils.CountLeadingZeros(0x6u));
            Assert.AreEqual(29, MathUtils.CountLeadingZeros(0x7u));
            Assert.AreEqual(28, MathUtils.CountLeadingZeros(0x8u));
            Assert.AreEqual(28, MathUtils.CountLeadingZeros(0x9u));

            Assert.AreEqual(28, MathUtils.CountLeadingZeros(0xFu));
            Assert.AreEqual(27, MathUtils.CountLeadingZeros(0x10u));
            Assert.AreEqual(27, MathUtils.CountLeadingZeros(0x11u));

            Assert.AreEqual(27, MathUtils.CountLeadingZeros(0x1Fu));
            Assert.AreEqual(26, MathUtils.CountLeadingZeros(0x20u));
            Assert.AreEqual(26, MathUtils.CountLeadingZeros(0x21u));

            Assert.AreEqual(2, MathUtils.CountLeadingZeros(0x3FFF_FFFFu));
            Assert.AreEqual(1, MathUtils.CountLeadingZeros(0x4000_0000u));
            Assert.AreEqual(1, MathUtils.CountLeadingZeros(0x4000_0001u));

            Assert.AreEqual(1, MathUtils.CountLeadingZeros(0x7FFF_FFFFu));
            Assert.AreEqual(0, MathUtils.CountLeadingZeros(0x8000_0000u));
            Assert.AreEqual(0, MathUtils.CountLeadingZeros(0x8000_0001u));
            Assert.AreEqual(0, MathUtils.CountLeadingZeros(0xFFFF_FFFFu));
        }

        [TestMethod]
        [TestCategory("MathUtils")]
        public void CountLeadingZeros_UInt64()
        {
            Assert.AreEqual(64, MathUtils.CountLeadingZeros(0x0ul));

            Assert.AreEqual(63, MathUtils.CountLeadingZeros(0x1ul));
            Assert.AreEqual(62, MathUtils.CountLeadingZeros(0x2ul));
            Assert.AreEqual(62, MathUtils.CountLeadingZeros(0x3ul));
            Assert.AreEqual(61, MathUtils.CountLeadingZeros(0x4ul));
            Assert.AreEqual(61, MathUtils.CountLeadingZeros(0x5ul));
            Assert.AreEqual(61, MathUtils.CountLeadingZeros(0x6ul));
            Assert.AreEqual(61, MathUtils.CountLeadingZeros(0x7ul));
            Assert.AreEqual(60, MathUtils.CountLeadingZeros(0x8ul));
            Assert.AreEqual(60, MathUtils.CountLeadingZeros(0x9ul));

            Assert.AreEqual(60, MathUtils.CountLeadingZeros(0xFul));
            Assert.AreEqual(59, MathUtils.CountLeadingZeros(0x10ul));
            Assert.AreEqual(59, MathUtils.CountLeadingZeros(0x11ul));

            Assert.AreEqual(59, MathUtils.CountLeadingZeros(0x1Ful));
            Assert.AreEqual(58, MathUtils.CountLeadingZeros(0x20ul));
            Assert.AreEqual(58, MathUtils.CountLeadingZeros(0x21ul));

            Assert.AreEqual(34, MathUtils.CountLeadingZeros(0x3FFF_FFFFul));
            Assert.AreEqual(33, MathUtils.CountLeadingZeros(0x4000_0000ul));
            Assert.AreEqual(33, MathUtils.CountLeadingZeros(0x4000_0001ul));

            Assert.AreEqual(33, MathUtils.CountLeadingZeros(0x7FFF_FFFFul));
            Assert.AreEqual(32, MathUtils.CountLeadingZeros(0x8000_0000ul));
            Assert.AreEqual(32, MathUtils.CountLeadingZeros(0x8000_0001ul));
            Assert.AreEqual(32, MathUtils.CountLeadingZeros(0xFFFF_FFFFul));

            Assert.AreEqual(1, MathUtils.CountLeadingZeros(0x7FFF_FFFF_FFFF_FFFFul));
            Assert.AreEqual(0, MathUtils.CountLeadingZeros(0x8000_0000_0000_0000ul));
            Assert.AreEqual(0, MathUtils.CountLeadingZeros(0x8000_0000_0000_0001ul));
            Assert.AreEqual(0, MathUtils.CountLeadingZeros(0xFFFF_FFFF_FFFF_FFFFul));
        }
    }
}
