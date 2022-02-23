using Xunit;
using FluentAssertions;
using System;

namespace Redzen.Tests;

public class DoubleUtilsTests
{
    [Theory]
    [InlineData(-1.0f, false)]
    [InlineData(-0.000001f, false)]
    [InlineData(-double.Epsilon, false)]
    [InlineData(-1.23e16f, false)]
    [InlineData(double.NaN, false)]
    [InlineData(double.PositiveInfinity, false)]
    [InlineData(double.NegativeInfinity, false)]
    [InlineData(0f, true)]
    [InlineData(0.000001f, true)]
    [InlineData(double.Epsilon, true)]
    [InlineData(1.23e16f, true)]
    public void IsNonNegativeReal(double f, bool expected)
    {
        DoubleUtils.IsNonNegativeReal(f).Should().Be(expected);
    }

    [Theory]
    [InlineData(-1.0f, false)]
    [InlineData(-0.000001f, false)]
    [InlineData(-double.Epsilon, false)]
    [InlineData(-1.23e16f, false)]
    [InlineData(double.NaN, false)]
    [InlineData(double.PositiveInfinity, false)]
    [InlineData(double.NegativeInfinity, false)]
    [InlineData(0f, false)]
    [InlineData(0.000001f, true)]
    [InlineData(double.Epsilon, true)]
    [InlineData(1.23e16f, true)]
    public void IsPositiveReal(double f, bool expected)
    {
        DoubleUtils.IsPositiveReal(f).Should().Be(expected);
    }

    [Theory]
    [InlineData(-1.0f, false)]
    [InlineData(-0.000001f, false)]
    [InlineData(-double.Epsilon, false)]
    [InlineData(-1.23e16f, false)]
    [InlineData(double.NaN, false)]
    [InlineData(double.PositiveInfinity, false)]
    [InlineData(double.NegativeInfinity, false)]
    [InlineData(0f, true)]
    [InlineData(0.000001f, true)]
    [InlineData(double.Epsilon, true)]
    [InlineData(1.23e16f, true)]
    public void AllNonNegativeReal(double f, bool expected)
    {
        var vals = new double[100];

        vals[63] = f;
        DoubleUtils.AllNonNegativeReal(vals).Should().Be(expected);            
    }

    [Theory]
    [InlineData(-1.0f, false)]
    [InlineData(-0.000001f, false)]
    [InlineData(-double.Epsilon, false)]
    [InlineData(-1.23e16f, false)]
    [InlineData(double.NaN, false)]
    [InlineData(double.PositiveInfinity, false)]
    [InlineData(double.NegativeInfinity, false)]
    [InlineData(0f, false)]
    [InlineData(0.000001f, true)]
    [InlineData(double.Epsilon, true)]
    [InlineData(1.23e16f, true)]
    public void AllPositiveReal(double f, bool expected)
    {
        var vals = new double[100];
        Array.Fill(vals, 1);

        vals[63] = f;
        DoubleUtils.AllPositiveReal(vals).Should().Be(expected);
    }
}
