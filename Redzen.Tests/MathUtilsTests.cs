﻿using FluentAssertions;
using Xunit;

namespace Redzen;

public class MathUtilsTests
{
    [Fact]
    public void IsPowerOfTwo_Int32()
    {
        MathUtils.IsPowerOfTwo(0).Should().BeFalse();
        MathUtils.IsPowerOfTwo(1).Should().BeTrue();
        MathUtils.IsPowerOfTwo(2).Should().BeTrue();
        MathUtils.IsPowerOfTwo(3).Should().BeFalse();

        // Loop over integer exponents.
        for(int i=4; i < 31; i++)
        {
            // Test exact power of two.
            MathUtils.IsPowerOfTwo(1 << i).Should().BeTrue();

            // Test boundary and near boundary cases.
            MathUtils.IsPowerOfTwo((1 << i) - 3).Should().BeFalse();
            MathUtils.IsPowerOfTwo((1 << i) - 2).Should().BeFalse();
            MathUtils.IsPowerOfTwo((1 << i) - 1).Should().BeFalse();
            MathUtils.IsPowerOfTwo((1 << i) + 1).Should().BeFalse();
            MathUtils.IsPowerOfTwo((1 << i) + 2).Should().BeFalse();
            MathUtils.IsPowerOfTwo((1 << i) + 3).Should().BeFalse();
        }

        Assert.False(MathUtils.IsPowerOfTwo(int.MaxValue)); // 2^31 - 1
    }

    [Fact]
    public void IsPowerOfTwo_Int64()
    {
        MathUtils.IsPowerOfTwo(0L).Should().BeFalse();
        MathUtils.IsPowerOfTwo(1L).Should().BeTrue();
        MathUtils.IsPowerOfTwo(2L).Should().BeTrue();
        MathUtils.IsPowerOfTwo(3L).Should().BeFalse();

        // Loop over integer exponents.
        for(int i=4; i < 63; i++)
        {
            // Test exact power of two.
            Assert.True(MathUtils.IsPowerOfTwo(1L << i));

            // Test boundary and near boundary cases.
            MathUtils.IsPowerOfTwo((1L << i) - 3).Should().BeFalse();
            MathUtils.IsPowerOfTwo((1L << i) - 2).Should().BeFalse();
            MathUtils.IsPowerOfTwo((1L << i) - 1).Should().BeFalse();
            MathUtils.IsPowerOfTwo((1L << i) + 1).Should().BeFalse();
            MathUtils.IsPowerOfTwo((1L << i) + 2).Should().BeFalse();
            MathUtils.IsPowerOfTwo((1L << i) + 3).Should().BeFalse();
        }

        Assert.False(MathUtils.IsPowerOfTwo(uint.MaxValue)); // 2^63 - 1
    }

    [Fact]
    public void PowerOfTwo_Int32()
    {
        MathUtils.PowerOfTwo(0).Should().Be(1);
        MathUtils.PowerOfTwo(1).Should().Be(2);
        MathUtils.PowerOfTwo(2).Should().Be(4);
        MathUtils.PowerOfTwo(3).Should().Be(8);

        // Loop over integer exponents.
        int expected = 16;
        for(int i=4; i < 31; i++, expected *=2)
            MathUtils.PowerOfTwo(i).Should().Be(expected);
    }

    [Fact]
    public void PowerOfTwo_Int64()
    {
        MathUtils.PowerOfTwo(0L).Should().Be(1L);
        MathUtils.PowerOfTwo(1L).Should().Be(2L);
        MathUtils.PowerOfTwo(2L).Should().Be(4L);
        MathUtils.PowerOfTwo(3L).Should().Be(8L);

        // Loop over integer exponents.
        long expected = 16;
        for(long i=4; i < 63; i++, expected *=2)
            MathUtils.PowerOfTwo(i).Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 4)]
    [InlineData(4, 4)]
    [InlineData(5, 8)]
    [InlineData(7, 8)]
    [InlineData(8, 8)]

    [InlineData(9, 16)]
    [InlineData(15, 16)]
    [InlineData(16, 16)]

    [InlineData(17, 32)]
    [InlineData(31, 32)]
    [InlineData(32, 32)]

    [InlineData(0x1000_0001, 0x2000_0000)]
    [InlineData(0x1FFF_FFFF, 0x2000_0000)]
    [InlineData(0x2000_0000, 0x2000_0000)]

    [InlineData(0x2000_0001, 0x4000_0000)]
    [InlineData(0x3FFF_FFFF, 0x4000_0000)]
    [InlineData(0x4000_0000, 0x4000_0000)]
    public void CeilingToPowerOfTwo_Int32(int x, int result)
    {
        MathUtils.CeilingToPowerOfTwo(x).Should().Be(result);
    }

    [Theory]
    [InlineData(0L, 1L)]
    [InlineData(1L, 1L)]
    [InlineData(2L, 2L)]
    [InlineData(3L, 4L)]
    [InlineData(4L, 4L)]
    [InlineData(5L, 8L)]
    [InlineData(7L, 8L)]
    [InlineData(8L, 8L)]

    [InlineData(9L, 16L)]
    [InlineData(15L, 16L)]
    [InlineData(16L, 16L)]

    [InlineData(17L, 32L)]
    [InlineData(31L, 32L)]
    [InlineData(32L, 32L)]

    [InlineData(0x1000_0001L, 0x2000_0000L)]
    [InlineData(0x1FFF_FFFFL, 0x2000_0000L)]
    [InlineData(0x2000_0000L, 0x2000_0000L)]

    [InlineData(0x2000_0001L, 0x4000_0000L)]
    [InlineData(0x3FFF_FFFFL, 0x4000_0000L)]
    [InlineData(0x4000_0000L, 0x4000_0000L)]

    [InlineData(0x1000_0000_0000_0001L, 0x2000_0000_0000_0000L)]
    [InlineData(0x1FFF_FFFF_FFFF_FFFFL, 0x2000_0000_0000_0000L)]
    [InlineData(0x2000_0000_0000_0000L, 0x2000_0000_0000_0000L)]

    [InlineData(0x2000_0000_0000_0001L, 0x4000_0000_0000_0000L)]
    [InlineData(0x3FFF_FFFF_FFFF_FFFFL, 0x4000_0000_0000_0000L)]
    [InlineData(0x4000_0000_0000_0000L, 0x4000_0000_0000_0000L)]
    public void CeilingToPowerOfTwo_Int64(long x, long result)
    {
        MathUtils.CeilingToPowerOfTwo(x).Should().Be(result);
    }

    [Theory]
    [InlineData(0x0U, 0)]
    [InlineData(0x1U, 0)]
    [InlineData(0x2U, 1)]
    [InlineData(0x3U, 2)]
    [InlineData(0x4U, 2)]
    [InlineData(0x5U, 3)]
    [InlineData(0x6U, 3)]
    [InlineData(0x7U, 3)]
    [InlineData(0x8U, 3)]
    [InlineData(0x9U, 4)]

    [InlineData(0xFU, 4)]
    [InlineData(0x10U, 4)]
    [InlineData(0x11U, 5)]

    [InlineData(0x1FU, 5)]
    [InlineData(0x20U, 5)]
    [InlineData(0x21U, 6)]

    [InlineData(0x3FFF_FFFFU, 30)]
    [InlineData(0x4000_0000U, 30)]
    [InlineData(0x4000_0001U, 31)]

    [InlineData(0x7FFF_FFFFU, 31)]
    [InlineData(0x8000_0000U, 31)]
    [InlineData(0x8000_0001U, 32)]
    [InlineData(0xFFFF_FFFFU, 32)]
    public void Log2Ceiling_UInt32(uint x, int result)
    {
        MathUtils.Log2Ceiling(x).Should().Be(result);
    }

    [Theory]
    [InlineData(0x0UL, 0)]
    [InlineData(0x1UL, 0)]
    [InlineData(0x2UL, 1)]
    [InlineData(0x3UL, 2)]
    [InlineData(0x4UL, 2)]
    [InlineData(0x5UL, 3)]
    [InlineData(0x6UL, 3)]
    [InlineData(0x7UL, 3)]
    [InlineData(0x8UL, 3)]
    [InlineData(0x9UL, 4)]

    [InlineData(0xFUL, 4)]
    [InlineData(0x10UL, 4)]
    [InlineData(0x11UL, 5)]

    [InlineData(0x1FUL, 5)]
    [InlineData(0x20UL, 5)]
    [InlineData(0x21UL, 6)]

    [InlineData(0x3FFF_FFFFUL, 30)]
    [InlineData(0x4000_0000UL, 30)]
    [InlineData(0x4000_0001UL, 31)]

    [InlineData(0x3FFF_FFFF_FFFF_FFFFUL, 62)]
    [InlineData(0x4000_0000_0000_0000UL, 62)]
    [InlineData(0x4000_0000_0000_0001UL, 63)]

    [InlineData(0x7FFF_FFFF_FFFF_FFFFUL, 63)]
    [InlineData(0x8000_0000_0000_0000UL, 63)]
    [InlineData(0x8000_0000_0000_0001UL, 64)]
    [InlineData(0xFFFF_FFFF_FFFF_FFFFUL, 64)]
    public void Log2Ceiling_UInt64(ulong x, int result)
    {
        MathUtils.Log2Ceiling(x).Should().Be(result);
    }
}
