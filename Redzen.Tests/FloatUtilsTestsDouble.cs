using FluentAssertions;
using Xunit;

namespace Redzen;

public class FloatUtilsTestsDouble
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
    public void IsNonNegativeReal(double x, bool expected)
    {
        FloatUtils.IsNonNegativeReal(x).Should().Be(expected);
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
    public void IsPositiveReal(double x, bool expected)
    {
        FloatUtils.IsPositiveReal(x).Should().Be(expected);
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
    public void AllNonNegativeReal(double x, bool expected)
    {
        var vals = new double[100];

        vals[63] = x;
        FloatUtils.AllNonNegativeReal<double>(vals).Should().Be(expected);
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
    public void AllPositiveReal(double x, bool expected)
    {
        var vals = new double[100];
        Array.Fill(vals, 1);

        vals[63] = x;
        FloatUtils.AllPositiveReal<double>(vals).Should().Be(expected);
    }
}
