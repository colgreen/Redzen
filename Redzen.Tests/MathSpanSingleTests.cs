using FluentAssertions;
using Redzen.Numerics.Distributions.Float;
using Xunit;

namespace Redzen;

public class MathSpanSingleTests
{
    #region Test Methods

    [Fact]
    public void Clip()
    {
        var sampler = new UniformDistributionSampler(20f, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 40; len++)
            Clip_Inner(sampler, len);
    }

    [Fact]
    public void Min()
    {
        var sampler = new UniformDistributionSampler(100f, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 40; len++)
            Min_Inner(sampler, len);
    }

    [Fact]
    public void Max()
    {
        var sampler = new UniformDistributionSampler(100f, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 40; len++)
            Max_Inner(sampler, len);
    }

    [Fact]
    public void MinMax()
    {
        var sampler = new UniformDistributionSampler(100f, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 40; len++)
            MinMax_Inner(sampler, len);
    }

    [Fact]
    public void MeanSquaredDelta()
    {
        var sampler = new UniformDistributionSampler(10f, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 40; len++)
            MeanSquaredDelta_Inner(sampler, len);
    }

    [Fact]
    public void MedianOfSorted()
    {
        // Empty array.
        var arr = Array.Empty<float>();
        ((Action)(() => MathSpan.MedianOfSorted<float>(arr))).Should().Throw<ArgumentException>();

        // Single element.
        arr = new float[] { 5 };
        double actual = MathSpan.MedianOfSorted<float>(arr);
        actual.Should().Be(5.0);

        // Two elements.
        arr = new float[] { 2, 4 };
        actual = MathSpan.MedianOfSorted<float>(arr);
        actual.Should().Be(3.0);

        // Three elements.
        arr = new float[] { 1, 2, 3 };
        actual = MathSpan.MedianOfSorted<float>(arr);
        actual.Should().Be(2.0);

        // Five elements.
        arr = new float[] { 1, 2, 3, 4, 5 };
        actual = MathSpan.MedianOfSorted<float>(arr);
        actual.Should().Be(3.0);

        // Six elements.
        arr = new float[] { 1, 2, 3, 4, 5, 6 };
        actual = MathSpan.MedianOfSorted<float>(arr);
        actual.Should().Be(3.5);
    }

    [Fact]
    public void Multiply()
    {
        var arr = new float[65];
        Array.Fill(arr, 8f);

        MathSpan.Multiply(arr, 2f);

        arr.Should().OnlyContain(x => x == 16f);
    }

    [Fact]
    public void Sum()
    {
        var sampler = new UniformDistributionSampler(20f, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 40; len++)
            Sum_Inner(sampler, len);
    }

    [Fact]
    public void SumOfSquares()
    {
        var sampler = new UniformDistributionSampler(20f, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 20; len++)
            SumOfSquares_Inner(sampler, len);
    }

    [Fact]
    public void SumSquaredDelta()
    {
        var sampler = new UniformDistributionSampler(10f, true, 0);

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
        float[] x = new float[len];
        sampler.Sample(x);

        // Clip the elements of the array with the safe routine.
        float[] expected = (float[])x.Clone();
        PointwiseClip(expected, -1.1f, 18.8f);

        // Clip the elements of the array.
        float[] actual = (float[])x.Clone();
        MathSpan.Clip(actual, -1.1f, 18.8f);

        // Compare expected with actual array.
        actual.Should().BeEquivalentTo(expected);
    }

    private static void Min_Inner(UniformDistributionSampler sampler, int len)
    {
        // Alloc arrays and fill with uniform random noise.
        float[] a = new float[len];
        sampler.Sample(a);

        // Calc results and compare.
        float expected = PointwiseMin(a);
        float actual = MathSpan.Min<float>(a);

        actual.Should().Be(expected);
    }

    private static void Max_Inner(UniformDistributionSampler sampler, int len)
    {
        // Alloc arrays and fill with uniform random noise.
        float[] a = new float[len];
        sampler.Sample(a);

        // Calc results and compare.
        float expected = PointwiseMax(a);
        float actual = MathSpan.Max<float>(a);

        actual.Should().Be(expected);
    }

    private static void MinMax_Inner(UniformDistributionSampler sampler, int len)
    {
        // Alloc arrays and fill with uniform random noise.
        float[] a = new float[len];
        sampler.Sample(a);

        // Calc results and compare.
        PointwiseMinMax(a, out float expectedMin, out float expectedMax);
        MathSpan.MinMax(a, out float actualMin, out float actualMax);

        actualMin.Should().BeApproximately(expectedMin, 10);
        actualMax.Should().BeApproximately(expectedMax, 10);
    }

    private static void MeanSquaredDelta_Inner(UniformDistributionSampler sampler, int len)
    {
        // Alloc arrays and fill with uniform random noise.
        float[] a = new float[len];
        float[] b = new float[len];
        sampler.Sample(a);
        sampler.Sample(b);

        // Calc results and compare.
        float expected = PointwiseSumSquaredDelta(a, b) / a.Length;
        float actual = MathSpan.MeanSquaredDelta<float>(a, b);
        actual.Should().BeApproximately(expected, 3);
    }

    private static void Sum_Inner(UniformDistributionSampler sampler, int len)
    {
        // Alloc array and fill with uniform random noise.
        float[] x = new float[len];
        sampler.Sample(x);

        // Sum the array elements.
        float expected = PointwiseSum(x);
        float actual = MathSpan.Sum<float>(x);

        // Compare expected and actual sum.
        actual.Should().BeApproximately(expected, 4);
    }

    private static void SumOfSquares_Inner(UniformDistributionSampler sampler, int len)
    {
        // Alloc array and fill with uniform random noise.
        float[] x = new float[len];
        sampler.Sample(x);

        // Sum the array elements.
        float expected = PointwiseSumOfSquares(x);
        float actual = MathSpan.SumOfSquares<float>(x);

        // Compare expected and actual sum.
        actual.Should().BeApproximately(expected, 3);
    }

    private static void SumSquaredDelta_Inner(UniformDistributionSampler sampler, int len)
    {
        // Alloc arrays and fill with uniform random noise.
        float[] a = new float[len];
        float[] b = new float[len];
        sampler.Sample(a);
        sampler.Sample(b);

        // Calc results and compare.
        float expected = PointwiseSumSquaredDelta(a, b);
        float actual = MathSpan.SumSquaredDelta<float>(a, b);
        actual.Should().BeApproximately(expected, 2);
    }

    #endregion

    #region Private Static Methods [Scalar Math Routines]

    private static void PointwiseClip(float[] x, float min, float max)
    {
        for(int i=0; i < x.Length; i++)
        {
            if(x[i] < min)
                x[i] = min;
            else if(x[i] > max)
                x[i] = max;
        }
    }

    private static float PointwiseMin(float[] a)
    {
        float min = a[0];
        for(int i=1; i < a.Length; i++)
        {
            if(a[i] < min)
                min = a[i];
        }
        return min;
    }

    private static float PointwiseMax(float[] a)
    {
        float max = a[0];
        for(int i=1; i < a.Length; i++)
        {
            if(a[i] > max)
                max = a[i];
        }
        return max;
    }

    private static void PointwiseMinMax(float[] a, out float min, out float max)
    {
        min = max = a[0];
        for(int i=1; i < a.Length; i++)
        {
            float val = a[i];
            if(val < min)
                min = val;
            else if(val > max)
                max = val;
        }
    }

    private static float PointwiseSum(float[] x)
    {
        float sum = 0f;
        for(int i=0; i < x.Length; i++)
            sum += x[i];

        return sum;
    }

    private static float PointwiseSumOfSquares(float[] x)
    {
        float sum = 0f;
        for(int i=0; i < x.Length; i++)
            sum += x[i] * x[i];

        return sum;
    }

    private static float PointwiseSumSquaredDelta(float[] a, float[] b)
    {
        float total = 0f;

        // Calc sum(squared error).
        for(int i=0; i < a.Length; i++)
        {
            float err = a[i] - b[i];
            total += err * err;
        }

        return total;
    }

    #endregion
}
