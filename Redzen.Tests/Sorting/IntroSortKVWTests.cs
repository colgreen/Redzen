using FluentAssertions;
using Redzen.Random;
using Xunit;

namespace Redzen.Sorting;

#pragma warning disable CA1861 // Avoid constant arrays as arguments

public class IntroSortKVWTests
{
    #region Public Test Methods

    [Fact]
    public void Sort_ShortArray()
    {
        int[] keys = [5,   8,  2, 16, 32, 12,  7];
        int[] v =    [45, 42, 48, 24,  8, 28, 43];
        int[] w =    [0,   1,  2,  3,  4,  5,  6];
        IntroSort.Sort<int,int,int>(keys, v, w);

        keys.Should().BeEquivalentTo(new int[] {  2,  5,  7,  8, 12, 16, 32 });
        v.Should().BeEquivalentTo(   new int[] { 48, 45, 43, 42, 28, 24,  8 });
        w.Should().BeEquivalentTo(   new int[] {  2,  0,  6,  1,  5,  3,  4 });
    }

    [Fact]
    public void Sort_LongRandomArrays()
    {
        IRandomSource rng = RandomDefaults.CreateRandomSource(0);

        int length = rng.Next(200_000);
        for(int i=0; i < 100; i++)
            LongRandomArraysInner(length, rng);
    }

    #endregion

    #region Private Static Methods

    private static void LongRandomArraysInner(int len, IRandomSource rng)
    {
        // Create random array.
        int[] keys = CreateRandomArray(len, rng);
        int[] v = (int[])keys.Clone();
        int[] w = (int[])keys.Clone();

        // Sort array.
        IntroSort.Sort<int,int,int>(keys, v, w);

        // Check array is sorted.
        SortUtils.IsSortedAscending<int>(keys).Should().BeTrue();
        SortUtils.IsSortedAscending<int>(v).Should().BeTrue();
        SortUtils.IsSortedAscending<int>(w).Should().BeTrue();
    }

    private static int[] CreateRandomArray(int len, IRandomSource rng)
    {
        var arr = new int[len];
        for(int i=0; i < len; i++)
            arr[i] = rng.Next(int.MinValue, int.MaxValue);

        return arr;
    }

    #endregion
}
