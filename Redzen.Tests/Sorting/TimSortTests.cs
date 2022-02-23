using FluentAssertions;
using Redzen.Random;
using Xunit;

namespace Redzen.Sorting.Tests;

public class TimSortTests
{
    #region Public Test Methods

    [Fact]
    public void Sort_ShortArray()
    {
        int[] keys = new int[] { 5, 8, 2, 16, 32, 12, 7 };
        TimSort<int>.Sort(keys);
        keys.Should().BeEquivalentTo(new int[] { 2, 5, 7, 8, 12, 16, 32 });
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

        // Sort array.
        TimSort<int>.Sort(keys);

        // Check array is sorted.
        SortUtils.IsSortedAscending<int>(keys).Should().BeTrue();
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
