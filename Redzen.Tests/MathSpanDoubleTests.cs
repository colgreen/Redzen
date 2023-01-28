using FluentAssertions;
using Redzen.Numerics.Distributions.Double;
using Xunit;

namespace Redzen;

public class MathSpanDoubleTests
{
    #region Test Methods

    [Fact]
    public void Clip()
    {
        var sampler = new UniformDistributionSampler(20.0, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 40; len++)
            Clip_Inner(sampler, len);
    }

    [Fact]
    public void Min()
    {
        var sampler = new UniformDistributionSampler(100.0, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 40; len++)
            Min_Inner(sampler, len);
    }

    [Fact]
    public void Max()
    {
        var sampler = new UniformDistributionSampler(100.0, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 40; len++)
            Max_Inner(sampler, len);
    }

    [Fact]
    public void MinMax()
    {
        var sampler = new UniformDistributionSampler(100.0, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 40; len++)
            MinMax_Inner(sampler, len);
    }

    [Fact]
    public void MeanSquaredDelta()
    {
        var sampler = new UniformDistributionSampler(100.0, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 40; len++)
            MeanSquaredDelta_Inner(sampler, len);
    }

    [Fact]
    public void MedianOfSorted()
    {
        // Empty array.
        var arr = Array.Empty<double>();
        Assert.Throws<ArgumentException>(() => MathSpan.MedianOfSorted(arr));

        // Single element.
        arr = new double[] { 5 };
        double actual = MathSpan.MedianOfSorted(arr);
        Assert.Equal(5, actual);

        // Two elements.
        arr = new double[] { 2, 4 };
        actual = MathSpan.MedianOfSorted(arr);
        Assert.Equal(3.0, actual);

        // Three elements.
        arr = new double[] { 1, 2, 3 };
        actual = MathSpan.MedianOfSorted(arr);
        Assert.Equal(2, actual);

        // Five elements.
        arr = new double[] { 1, 2, 3, 4, 5 };
        actual = MathSpan.MedianOfSorted(arr);
        Assert.Equal(3, actual);

        // Six elements.
        arr = new double[] { 1, 2, 3, 4, 5, 6 };
        actual = MathSpan.MedianOfSorted(arr);
        Assert.Equal(3.5, actual);
    }

    [Fact]
    public void Multiply()
    {
        var arr = new double[65];
        Array.Fill(arr, 8.0);

        MathSpan.Multiply(arr, 2.0);

        arr.Should().OnlyContain(x => x == 16.0);
    }

    [Fact]
    public void Sum()
    {
        var sampler = new UniformDistributionSampler(20.0, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 20; len++)
            Sum_Inner(sampler, len);
    }

    [Fact]
    public void SumOfSquares()
    {
        var sampler = new UniformDistributionSampler(20.0, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 20; len++)
            SumOfSquares_Inner(sampler, len);
    }

    [Fact]
    public void SumSquaredDelta()
    {
        var sampler = new UniformDistributionSampler(100.0, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 40; len++)
            SumSquaredDelta_Inner(sampler, len);
    }

    #endregion

    #region Private Static Methods [Test Subroutines]

    private static void Clip_Inner(UniformDistributionSampler sampler, int len)
    {
        // Alloc array and fill with uniform random noise.
        double[] x = new double[len];
        sampler.Sample(x);

        // Clip the elements of the array with the safe routine.
        double[] expected = (double[])x.Clone();
        PointwiseClip(expected, -1.1, 18.8);

        // Clip the elements of the array.
        double[] actual = (double[])x.Clone();
        MathSpan.Clip(actual, -1.1, 18.8);

        // Compare expected with actual array.
        actual.Should().BeEquivalentTo(expected);
    }

    private static void Min_Inner(UniformDistributionSampler sampler, int len)
    {
        // Alloc arrays and fill with uniform random noise.
        double[] a = new double[len];
        sampler.Sample(a);

        // Calc results and compare.
        double expected = PointwiseMin(a);
        double actual = MathSpan.Min(a);

        actual.Should().Be(expected);
    }

    private static void Max_Inner(UniformDistributionSampler sampler, int len)
    {
        // Alloc arrays and fill with uniform random noise.
        double[] a = new double[len];
        sampler.Sample(a);

        // Calc results and compare.
        double expected = PointwiseMax(a);
        double actual = MathSpan.Max(a);

        actual.Should().Be(expected);
    }

    private static void MinMax_Inner(UniformDistributionSampler sampler, int len)
    {
        // Alloc arrays and fill with uniform random noise.
        double[] a = new double[len];
        sampler.Sample(a);

        // Calc results and compare.
        PointwiseMinMax(a, out double expectedMin, out double expectedMax);
        MathSpan.MinMax(a, out double actualMin, out double actualMax);

        actualMin.Should().BeApproximately(expectedMin, 10);
        actualMax.Should().BeApproximately(expectedMax, 10);
    }

    private static void MeanSquaredDelta_Inner(UniformDistributionSampler sampler, int len)
    {
        // Alloc arrays and fill with uniform random noise.
        double[] a = new double[len];
        double[] b = new double[len];
        sampler.Sample(a);
        sampler.Sample(b);

        // Calc results and compare.
        double expected = PointwiseSumSquaredDelta(a, b) / a.Length;
        double actual = MathSpan.MeanSquaredDelta(a, b);
        actual.Should().BeApproximately(expected, 9);
    }

    private static void Sum_Inner(UniformDistributionSampler sampler, int len)
    {
        // Alloc array and fill with uniform random noise.
        double[] x = new double[len];
        sampler.Sample(x);

        // Sum the array elements.
        double expected = PointwiseSum(x);
        double actual = MathSpan.Sum(x);

        // Compare expected and actual sum.
        actual.Should().BeApproximately(expected, 12);
    }

    private static void SumOfSquares_Inner(UniformDistributionSampler sampler, int len)
    {
        // Alloc array and fill with uniform random noise.
        double[] x = new double[len];
        sampler.Sample(x);

        // Sum the array elements.
        double expected = PointwiseSumOfSquares(x);
        double actual = MathSpan.SumOfSquares(x);

        // Compare expected and actual sum.
        actual.Should().BeApproximately(expected, 11);
    }

    private static void SumSquaredDelta_Inner(UniformDistributionSampler sampler, int len)
    {
        // Alloc arrays and fill with uniform random noise.
        double[] a = new double[len];
        double[] b = new double[len];
        sampler.Sample(a);
        sampler.Sample(b);

        // Calc results and compare.
        double expected = PointwiseSumSquaredDelta(a, b);
        double actual = MathSpan.SumSquaredDelta(a, b);
        actual.Should().BeApproximately(expected, 8);
    }

    #endregion

    #region Private Static Methods [Scalar Math Routines]

    private static void PointwiseClip(double[] x, double min, double max)
    {
        for(int i = 0; i < x.Length; i++)
        {
            if(x[i] < min)
                x[i] = min;
            else if(x[i] > max)
                x[i] = max;
        }
    }

    private static double PointwiseMin(double[] a)
    {
        double min = a[0];
        for(int i=1; i < a.Length; i++)
        {
            if(a[i] < min)
                min = a[i];
        }
        return min;
    }

    private static double PointwiseMax(double[] a)
    {
        double max = a[0];
        for(int i=1; i < a.Length; i++)
        {
            if(a[i] > max)
                max = a[i];
        }
        return max;
    }

    private static void PointwiseMinMax(double[] a, out double min, out double max)
    {
        min = max = a[0];
        for(int i=1; i < a.Length; i++)
        {
            double val = a[i];
            if(val < min)
                min = val;
            else if(val > max)
                max = val;
        }
    }

    private static double PointwiseSum(double[] x)
    {
        double sum = 0.0;
        for(int i=0; i < x.Length; i++)
            sum += x[i];

        return sum;
    }

    private static double PointwiseSumOfSquares(double[] x)
    {
        double sum = 0.0;
        for(int i=0; i < x.Length; i++)
            sum += x[i] * x[i];

        return sum;
    }

    private static double PointwiseSumSquaredDelta(double[] a, double[] b)
    {
        double total = 0.0;

        // Calc sum(squared error).
        for(int i=0; i < a.Length; i++)
        {
            double err = a[i] - b[i];
            total += err * err;
        }

        return total;
    }

    #endregion
}
