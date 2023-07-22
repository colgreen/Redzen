using FluentAssertions;
using Xunit;

namespace Redzen;

public class FloatUtilsTestsHalf
{
    [Theory]
    [InlineData(-1.0f, false)]
    [InlineData(-0.000001f, false)]
    [InlineData(-6e-8, false)]
    [InlineData(-1.23e4f, false)]
    [InlineData(float.NaN, false)]
    [InlineData(float.PositiveInfinity, false)]
    [InlineData(float.NegativeInfinity, false)]
    [InlineData(0f, true)]
    [InlineData(0.000001f, true)]
    [InlineData(6e-8, true)]
    [InlineData(1.23e4f, true)]
    public void IsNonNegativeReal(float x, bool expected)
    {
        FloatUtils.IsNonNegativeReal((Half)x).Should().Be(expected);
    }

    [Theory]
    [InlineData(-1.0f, false)]
    [InlineData(-0.000001f, false)]
    [InlineData(-6e-8, false)]
    [InlineData(-1.23e4f, false)]
    [InlineData(float.NaN, false)]
    [InlineData(float.PositiveInfinity, false)]
    [InlineData(float.NegativeInfinity, false)]
    [InlineData(0f, false)]
    [InlineData(0.000001f, true)]
    [InlineData(6e-8, true)]
    [InlineData(1.23e4f, true)]
    public void IsPositiveReal(float x, bool expected)
    {
        FloatUtils.IsPositiveReal((Half)x).Should().Be(expected);
    }

    [Theory]
    [InlineData(-1.0f, false)]
    [InlineData(-0.000001f, false)]
    [InlineData(-6e-8, false)]
    [InlineData(-1.23e16f, false)]
    [InlineData(float.NaN, false)]
    [InlineData(float.PositiveInfinity, false)]
    [InlineData(float.NegativeInfinity, false)]
    [InlineData(0f, true)]
    [InlineData(0.000001f, true)]
    [InlineData(6e-8, true)]
    [InlineData(1.23e4f, true)]
    public void AllNonNegativeReal(float x, bool expected)
    {
        var vals = new Half[100];

        vals[63] = (Half)x;
        FloatUtils.AllNonNegativeReal<Half>(vals).Should().Be(expected);
    }

    [Theory]
    [InlineData(-1.0f, false)]
    [InlineData(-0.000001f, false)]
    [InlineData(-6e-8, false)]
    [InlineData(-1.23e4f, false)]
    [InlineData(float.NaN, false)]
    [InlineData(float.PositiveInfinity, false)]
    [InlineData(float.NegativeInfinity, false)]
    [InlineData(0f, false)]
    [InlineData(0.000001f, true)]
    [InlineData(6e-8, true)]
    [InlineData(1.23e4f, true)]
    public void AllPositiveReal(float x, bool expected)
    {
        var vals = new Half[100];
        Array.Fill(vals, (Half)1);

        vals[63] = (Half)x;
        FloatUtils.AllPositiveReal<Half>(vals).Should().Be(expected);
    }
}
