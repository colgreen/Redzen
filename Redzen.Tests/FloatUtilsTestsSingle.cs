using FluentAssertions;
using Xunit;

namespace Redzen;

public class FloatUtilsTestsSingle
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
    public void IsNonNegativeReal(float x, bool expected)
    {
        FloatUtils.IsNonNegativeReal(x).Should().Be(expected);
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
    public void IsPositiveReal(float x, bool expected)
    {
        FloatUtils.IsPositiveReal(x).Should().Be(expected);
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
    public void AllNonNegativeReal(float x, bool expected)
    {
        var vals = new float[100];

        vals[63] = x;
        FloatUtils.AllNonNegativeReal<float>(vals).Should().Be(expected);
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
    public void AllPositiveReal(float x, bool expected)
    {
        var vals = new float[100];
        Array.Fill(vals, 1);

        vals[63] = x;
        FloatUtils.AllPositiveReal<float>(vals).Should().Be(expected);
    }
}
