using FluentAssertions;
using Redzen.Random;
using Xunit;

namespace Redzen.Sorting.Tests;

public class TimSortKVTests
{
    #region Public Test Methods

    [Fact]
    public void Sort_ShortArray()
    {
        int[] keys = new int[] { 5, 8, 2, 16, 32, 12,  7 };
        int[] vals = new int[] { 0, 1, 2,  3,  4,  5,  6 };

        TimSort<int,int>.Sort(keys, vals);

        keys.Should().BeEquivalentTo(new int[] { 2,  5,  7,  8, 12, 16, 32 });
        vals.Should().BeEquivalentTo(new int[] { 2,  0,  6,  1, 5,   3,  4 });
    }

    [Fact]
    public void Sort_LongRandomArrays()
    {
        IRandomSource rng = RandomDefaults.CreateRandomSource(0);

        for(int i=0; i < 100; i++)
        {
            int length = rng.Next(200_000);
            Sort_LongRandomArrays_Inner(length, rng);
        }
    }

    #endregion

    #region Private Static Methods

    private static void Sort_LongRandomArrays_Inner(int len, IRandomSource rng)
    {
        // Create random array.
        int[] keys = CreateRandomArray(len, rng);

        // For the vals array, use a copy of the keys, but add a large constant so that we
        // can be sure keys weren't just copied by accident into vals.
        const int offset = 1_000_000;
        int[] vals = (int[])keys.Clone();
        for(int i=0; i < vals.Length; i++)
            vals[i] += offset;

        // Sort array.
        TimSort<int,int>.Sort(keys, vals);

        // Check array is sorted.
        SortUtils.IsSortedAscending<int>(keys).Should().BeTrue();

        // Checks vals.
        for(int i=0; i < keys.Length; i++)
            vals[i].Should().Be(keys[i] + offset);
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
