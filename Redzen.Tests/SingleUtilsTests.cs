using Xunit;
using FluentAssertions;
using System;

namespace Redzen.Tests;

public class SingleUtilsTests
{
    [Theory]
    [InlineData(-1.0f, false)]
    [InlineData(-0.000001f, false)]
    [InlineData(-float.Epsilon, false)]
    [InlineData(-1.23e16f, false)]
    [InlineData(float.NaN, false)]
    [InlineData(float.PositiveInfinity, false)]
    [InlineData(float.NegativeInfinity, false)]
    [InlineData(0f, true)]
    [InlineData(0.000001f, true)]
    [InlineData(float.Epsilon, true)]
    [InlineData(1.23e16f, true)]
    public void IsNonNegativeReal(float f, bool expected)
    {
        SingleUtils.IsNonNegativeReal(f).Should().Be(expected);
    }

    [Theory]
    [InlineData(-1.0f, false)]
    [InlineData(-0.000001f, false)]
    [InlineData(-float.Epsilon, false)]
    [InlineData(-1.23e16f, false)]
    [InlineData(float.NaN, false)]
    [InlineData(float.PositiveInfinity, false)]
    [InlineData(float.NegativeInfinity, false)]
    [InlineData(0f, false)]
    [InlineData(0.000001f, true)]
    [InlineData(float.Epsilon, true)]
    [InlineData(1.23e16f, true)]
    public void IsPositiveReal(float f, bool expected)
    {
        SingleUtils.IsPositiveReal(f).Should().Be(expected);
    }

    [Theory]
    [InlineData(-1.0f, false)]
    [InlineData(-0.000001f, false)]
    [InlineData(-float.Epsilon, false)]
    [InlineData(-1.23e16f, false)]
    [InlineData(float.NaN, false)]
    [InlineData(float.PositiveInfinity, false)]
    [InlineData(float.NegativeInfinity, false)]
    [InlineData(0f, true)]
    [InlineData(0.000001f, true)]
    [InlineData(float.Epsilon, true)]
    [InlineData(1.23e16f, true)]
    public void AllNonNegativeReal(float f, bool expected)
    {
        var vals = new float[100];

        vals[63] = f;
        SingleUtils.AllNonNegativeReal(vals).Should().Be(expected);            
    }

    [Theory]
    [InlineData(-1.0f, false)]
    [InlineData(-0.000001f, false)]
    [InlineData(-float.Epsilon, false)]
    [InlineData(-1.23e16f, false)]
    [InlineData(float.NaN, false)]
    [InlineData(float.PositiveInfinity, false)]
    [InlineData(float.NegativeInfinity, false)]
    [InlineData(0f, false)]
    [InlineData(0.000001f, true)]
    [InlineData(float.Epsilon, true)]
    [InlineData(1.23e16f, true)]
    public void AllPositiveReal(float f, bool expected)
    {
        var vals = new float[100];
        Array.Fill(vals, 1);

        vals[63] = f;
        SingleUtils.AllPositiveReal(vals).Should().Be(expected);
    }
}
