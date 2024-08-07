﻿using FluentAssertions;
using Xunit;

namespace Redzen;

public class MathSpanInt32Tests
{
    #region Test Methods

    [Fact]
    public void Clip()
    {
        var sampler = new Int32UniformDistributionSampler(200, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 40; len++)
            Clip_Inner(sampler, len);
    }

    [Fact]
    public void Min()
    {
        var sampler = new Int32UniformDistributionSampler(100, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 40; len++)
            Min_Inner(sampler, len);
    }

    [Fact]
    public void Max()
    {
        var sampler = new Int32UniformDistributionSampler(100, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 40; len++)
            Max_Inner(sampler, len);
    }

    [Fact]
    public void MinMax()
    {
        var sampler = new Int32UniformDistributionSampler(100, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 40; len++)
            MinMax_Inner(sampler, len);
    }

    [Fact]
    public void MedianOfSorted()
    {
        // Empty array.
        int[] arr = [];
        Assert.Throws<ArgumentException>(() => MathSpan.MedianOfSortedIntegers<int>(arr));

        // Single element.
        arr = [5];
        double actual = MathSpan.MedianOfSortedIntegers<int>(arr);
        actual.Should().Be(5.0);

        // Two elements.
        arr = [2, 4];
        actual = MathSpan.MedianOfSortedIntegers<int>(arr);
        actual.Should().Be(3.0);

        // Three elements.
        arr = [1, 2, 3];
        actual = MathSpan.MedianOfSortedIntegers<int>(arr);
        actual.Should().Be(2.0);

        // Five elements.
        arr = [1, 2, 3, 4, 5];
        actual = MathSpan.MedianOfSortedIntegers<int>(arr);
        actual.Should().Be(3.0);

        // Six elements.
        arr = [1, 2, 3, 4, 5, 6];
        actual = MathSpan.MedianOfSortedIntegers<int>(arr);
        actual.Should().Be(3.5);
    }

    [Fact]
    public void Multiply()
    {
        var arr = new int[65];
        Array.Fill(arr, 8);

        MathSpan.Multiply(arr, 2);

        arr.Should().OnlyContain(x => x == 16);
    }

    [Fact]
    public void Sum()
    {
        var sampler = new Int32UniformDistributionSampler(200, true, 0);

        // Test with a range of array lengths;
        // the vectorised code has edge cases related to array length, so this is a sensible test to do.
        for(int len = 1; len < 20; len++)
            Sum_Inner(sampler, len);
    }

    #endregion

    #region Private Static Methods [Test Subroutines]

    private static void Clip_Inner(Int32UniformDistributionSampler sampler, int len)
    {
        // Alloc array and fill with uniform random noise.
        int[] x = new int[len];
        sampler.Sample(x);

        // Clip the elements of the array with the safe routine.
        int[] expected = (int[])x.Clone();
        PointwiseClip(expected, -1, 18);

        // Clip the elements of the array.
        int[] actual = (int[])x.Clone();
        MathSpan.Clip(actual, -1, 18);

        // Compare expected with actual array.
        actual.Should().BeEquivalentTo(expected);
    }

    private static void Min_Inner(Int32UniformDistributionSampler sampler, int len)
    {
        // Alloc arrays and fill with uniform random noise.
        int[] a = new int[len];
        sampler.Sample(a);

        // Calc results and compare.
        int expected = PointwiseMin(a);
        int actual = MathSpan.Min<int>(a);

        actual.Should().Be(expected);
    }

    private static void Max_Inner(Int32UniformDistributionSampler sampler, int len)
    {
        // Alloc arrays and fill with uniform random noise.
        int[] a = new int[len];
        sampler.Sample(a);

        // Calc results and compare.
        int expected = PointwiseMax(a);
        int actual = MathSpan.Max<int>(a);

        actual.Should().Be(expected);
    }

    private static void MinMax_Inner(Int32UniformDistributionSampler sampler, int len)
    {
        // Alloc arrays and fill with uniform random noise.
        int[] a = new int[len];
        sampler.Sample(a);

        // Calc results and compare.
        PointwiseMinMax(a, out int expectedMin, out int expectedMax);
        MathSpan.MinMax(a, out int actualMin, out int actualMax);

        actualMin.Should().Be(expectedMin);
        actualMax.Should().Be(expectedMax);
    }

    private static void Sum_Inner(Int32UniformDistributionSampler sampler, int len)
    {
        // Alloc array and fill with uniform random noise.
        int[] x = new int[len];
        sampler.Sample(x);

        // Sum the array elements.
        int expected = PointwiseSum(x);
        int actual = MathSpan.Sum<int>(x);

        // Compare expected and actual sum.
        actual.Should().Be(expected);
    }

    #endregion

    #region Private Static Methods [Scalar Math Routines]

    private static void PointwiseClip(int[] x, int min, int max)
    {
        for(int i=0; i < x.Length; i++)
        {
            if(x[i] < min)
                x[i] = min;
            else if(x[i] > max)
                x[i] = max;
        }
    }

    private static int PointwiseMin(int[] a)
    {
        int min = a[0];
        for(int i=1; i < a.Length; i++)
        {
            if(a[i] < min)
                min = a[i];
        }

        return min;
    }

    private static int PointwiseMax(int[] a)
    {
        int max = a[0];
        for(int i=1; i < a.Length; i++)
        {
            if(a[i] > max)
                max = a[i];
        }

        return max;
    }

    private static void PointwiseMinMax(int[] a, out int min, out int max)
    {
        min = max = a[0];
        for(int i=1; i < a.Length; i++)
        {
            int val = a[i];
            if(val < min)
                min = val;
            else if(val > max)
                max = val;
        }
    }

    private static int PointwiseSum(int[] x)
    {
        int sum = 0;
        for(int i=0; i < x.Length; i++)
            sum += x[i];

        return sum;
    }

    #endregion
}
