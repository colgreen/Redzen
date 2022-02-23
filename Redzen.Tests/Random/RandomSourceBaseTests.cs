using FluentAssertions;
using Xunit;

namespace Redzen.Random.Tests;

public class RandomSourceBaseTests
{
    // Constants.
    const double INCR_DOUBLE = 1.0 / (1UL << 53);
    const float INCR_FLOAT = 1f / (1U << 24);

    [Fact]
    public void Next_MinVal()
    {
        var rng = new ConstantRandomSource(0UL);
        int x = rng.Next();
        x.Should().Be(0);
    }

    [Fact]
    public void Next_MaxVal()
    {
        var rng = new ConstantRandomSource(0xffff_fffc_0000_0000);
        int x = rng.Next();
        x.Should().Be(int.MaxValue-1);
    }

    [Fact]
    public void NextMax_MinVal()
    {
        var rng = new ConstantRandomSource(0UL);
        int x = rng.Next(1_234_567);
        x.Should().Be(0);
    }

    [Fact]
    public void NextMax_MaxVal()
    {
        const int max = 1_234_567;
        var rng = new ConstantRandomSource(((ulong)(max-1)) << 43);
        int x = rng.Next(max);
        x.Should().Be(max-1);
    }

    [Fact]
    public void NextMinMax_MinVal()
    {
        var rng = new ConstantRandomSource(0UL);
        int x = rng.Next(123, 1_234_567);
        x.Should().Be(123);
    }

    [Fact]
    public void NextMinMax_MaxVal()
    {
        const int min = 123;
        const int max = 1_234_567;

        var rng = new ConstantRandomSource(((ulong)((max - min) - 1)) << 43);
        int x = rng.Next(min, max);
        x.Should().Be(max-1);
    }

    [Fact]
    public void NextMinMax_LongRange_MinVal()
    {
        const int maxValHalf = int.MaxValue / 2;
        const int lowerBound = -maxValHalf;
        const int upperBound = maxValHalf + 123;

        var rng = new ConstantRandomSource(0UL);
        int x = rng.Next(lowerBound, upperBound);
        x.Should().Be(lowerBound);
    }

    [Fact]
    public void NextMinMax_LongRange_MaxVal()
    {
        const int maxValHalf = int.MaxValue / 2;
        const int lowerBound = -maxValHalf;
        const int upperBound = maxValHalf + 123;

        var rng = new ConstantRandomSource(0x8000_0078UL << 32);
        int x = rng.Next(lowerBound, upperBound);
        x.Should().Be(upperBound-1);
    }

    [Fact]
    public void NextDouble_MinVal()
    {
        var rng = new ConstantRandomSource(0UL);
        double x = rng.NextDouble();
        x.Should().Be(0.0);
    }

    [Fact]
    public void NextDouble_MaxVal()
    {
        var rng = new ConstantRandomSource(ulong.MaxValue);
        double x = rng.NextDouble();
        x.Should().Be(1.0 - INCR_DOUBLE);
    }

    [Fact]
    public void NextInt_MinVal()
    {
        var rng = new ConstantRandomSource(0UL);
        int x = rng.Next();
        x.Should().Be(0);
    }

    [Fact]
    public void NextInt_MaxVal()
    {
        var rng = new ConstantRandomSource(ulong.MaxValue);
        int x = rng.NextInt();
        x.Should().Be(int.MaxValue);
    }

    [Fact]
    public void NextUInt_MinVal()
    {
        var rng = new ConstantRandomSource(0UL);
        uint x = rng.NextUInt();
        x.Should().Be(0u);
    }

    [Fact]
    public void NextUInt_MaxVal()
    {
        var rng = new ConstantRandomSource(ulong.MaxValue);
        uint x = rng.NextUInt();
        x.Should().Be(uint.MaxValue);
    }

    [Fact]
    public void NextULong_MinVal()
    {
        var rng = new ConstantRandomSource(0UL);
        ulong x = rng.NextULong();
        x.Should().Be(0UL);
    }

    [Fact]
    public void NextULong_MaxVal()
    {
        var rng = new ConstantRandomSource(ulong.MaxValue);
        ulong x = rng.NextULong();
        x.Should().Be(ulong.MaxValue);
    }

    [Fact]
    public void NextBool_False()
    {
        var rng = new ConstantRandomSource(0UL);
        bool x = rng.NextBool();
        x.Should().BeFalse();
    }

    [Fact]
    public void NextBool_True()
    {
        var rng = new ConstantRandomSource(0x8000_0000_0000_0000UL);
        bool x = rng.NextBool();
        x.Should().BeTrue();
    }

    [Fact]
    public void NextByte_MinVal()
    {
        var rng = new ConstantRandomSource(0UL);
        byte x = rng.NextByte();
        x.Should().Be(0);
    }

    [Fact]
    public void NextByte_MaxVal()
    {
        var rng = new ConstantRandomSource(0xFFUL << 56);
        byte x = rng.NextByte();
        x.Should().Be(255);
    }

    [Fact]
    public void NextFloat_MinVal()
    {
        var rng = new ConstantRandomSource(0UL);
        float x = rng.NextFloat();
        x.Should().Be(0f);
    }

    [Fact]
    public void NextFloat_MaxVal()
    {
        var rng = new ConstantRandomSource(ulong.MaxValue);
        float x = rng.NextFloat();
        x.Should().Be(1f - INCR_FLOAT);
    }

    [Fact]
    public void NextFloatNonZero_Min()
    {
        var rng = new ConstantRandomSource(0UL);
        double x = rng.NextFloatNonZero();
        Assert.Equal(INCR_FLOAT, x);
        x.Should().Be(INCR_FLOAT);
    }

    [Fact]
    public void NextFloatNonZero_Max()
    {
        var rng = new ConstantRandomSource(ulong.MaxValue);
        double x = rng.NextFloatNonZero();
        x.Should().Be(1f);
    }

    [Fact]
    public void NextDoubleNonZero_Min()
    {
        var rng = new ConstantRandomSource(0UL);
        double x = rng.NextDoubleNonZero();
        x.Should().Be(INCR_DOUBLE);
    }

    [Fact]
    public void NextDoubleNonZero_Max()
    {
        var rng = new ConstantRandomSource(ulong.MaxValue);
        double x = rng.NextDoubleNonZero();
        x.Should().Be(1.0);
    }

    [Fact]
    public void NextDoubleHighRes_Min()
    {
        var rng = new ConstantRandomSource(0UL);
        double x = rng.NextDoubleHighRes();
        x.Should().Be(0.0);
    }

    [Fact]
    public void NextDoubleHighRes_Max()
    {
        var rng = new ConstantRandomSource(ulong.MaxValue);
        double x = rng.NextDoubleHighRes();
        x.Should().Be(1.0);
    }
}
