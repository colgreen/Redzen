using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Redzen.Sorting.Tests
{
    public class ListSortUtilsTests
    {
        [Theory]
        [InlineData(new int[] { 2, 5, 7, 8, 12, 16, 32 })]
        [InlineData(new int[] { 2, 2, 5, 7, 8, 12, 16, 32 })]
        [InlineData(new int[] { 2, 5, 5, 7, 8, 12, 16, 32 })]
        [InlineData(new int[] { 2, 5, 7, 8, 12, 16, 32, 32 })]
        [InlineData(new int[] { 0, 2, 5, 7, 8, 12, 16, 32, 32, int.MaxValue})]
        [InlineData(new int[] { -10, -9, -8, -7, -6, -2 })]
        [InlineData(new int[] { -10, -9, -8, -7, -6, -2, 0 })]
        [InlineData(new int[] { -10, -9, -8, -7, -6, -2, 0, 100 })]
        [InlineData(new int[] { -10, -9, -8, -7, -6, -2, 0, int.MaxValue })]
        [InlineData(new int[] { int.MinValue, -10, -9, -8, -7, -6, -2, 0, int.MaxValue })]
        public void IsSortedAscending_Int_Sorted(int[] arr)
        {
            Assert.True(ListSortUtils.IsSortedAscending<int>(arr));
        }

        [Theory]
        [InlineData(new int[] { 2, 5, 8, 7, 12, 16, 32 })]
        [InlineData(new int[] { 5, 8, 2, 16, 32, 12, 7 })]
        [InlineData(new int[] { 5, 8, 2, 16, 32, 12, int.MaxValue })]
        public void IsSortedAscending_Int_NotSorted(int[] arr)
        {
            Assert.False(ListSortUtils.IsSortedAscending<int>(arr));
        }

        [Theory]
        [InlineData("a", "b", "c", "d", "e")]
        [InlineData("a", "a", "c", "d", "e")]
        public void IsSortedAscending_String_Sorted(params string[] arr)
        {
            Assert.True(ListSortUtils.IsSortedAscending<string>(arr));
        }

        [Theory]
        [InlineData("b", "a", "c", "d", "e")]
        [InlineData("a", "c", "e", "d")]
        public void IsSortedAscending_String_NotSorted(params string[] arr)
        {
            Assert.False(ListSortUtils.IsSortedAscending<string>(arr));
        }

        [Theory]
        [InlineData(new int[] { 2, 5, 7, 8, 12, 16, 32 })]
        [InlineData(new int[] { 2, 2, 5, 7, 8, 12, 16, 32 })]
        [InlineData(new int[] { 2, 5, 5, 7, 8, 12, 16, 32 })]
        [InlineData(new int[] { 2, 5, 7, 8, 12, 16, 32, 32 })]
        [InlineData(new int[] { 0, 2, 5, 7, 8, 12, 16, 32, 32, int.MaxValue})]
        [InlineData(new int[] { -10, -9, -8, -7, -6, -2 })]
        [InlineData(new int[] { -10, -9, -8, -7, -6, -2, 0 })]
        [InlineData(new int[] { -10, -9, -8, -7, -6, -2, 0, 100 })]
        [InlineData(new int[] { -10, -9, -8, -7, -6, -2, 0, int.MaxValue })]
        [InlineData(new int[] { int.MinValue, -10, -9, -8, -7, -6, -2, 0, int.MaxValue })]
        public void IsSortedAscending_Comparer_Int_Sorted(int[] arr)
        {
            Assert.True(ListSortUtils.IsSortedAscending(arr, Comparer<int>.Default));
        }

        [Theory]
        [InlineData(new int[] { 2, 5, 8, 7, 12, 16, 32 })]
        [InlineData(new int[] { 5, 8, 2, 16, 32, 12, 7 })]
        [InlineData(new int[] { 5, 8, 2, 16, 32, 12, int.MaxValue })]
        public void IsSortedAscending_Comparer_Int_NotSorted(int[] arr)
        {
            Assert.False(ListSortUtils.IsSortedAscending(arr, Comparer<int>.Default));
        }

        [Theory]
        [InlineData("a", "b", "c", "d", "e")]
        [InlineData("a", "a", "c", "d", "e")]
        public void IsSortedAscending_Comparer_String_Sorted(params string[] arr)
        {
            Assert.True(ListSortUtils.IsSortedAscending(arr, Comparer<string>.Default));
        }

        [Theory]
        [InlineData("b", "a", "c", "d", "e")]
        [InlineData("a", "c", "e", "d")]
        public void IsSortedAscending_Comparer_String_NotSorted(params string[] arr)
        {
            Assert.False(ListSortUtils.IsSortedAscending(arr, Comparer<string>.Default));
        }

        [Fact]
        public void TestTryFindSegment()
        {
            MethodInfo methodInfo = typeof(ListSortUtils).GetMethod("TryFindSegment", BindingFlags.Static | BindingFlags.NonPublic);
            object[] args = new object[4];

            args[0] = CreateIntListWithSegment(100, 30, 10);
            args[1] = Comparer<int>.Default;
            args[2] = 0;

            MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(typeof(int));
            object result = genericMethodInfo.Invoke(null, args);

            Assert.Equal(true, result);
            Assert.Equal(30, args[2]);
            Assert.Equal(39, args[3]);
        }

        private static List<int> CreateIntListWithSegment(int length, int segStartIdx, int segLength)
        {
            List<int> list = new List<int>(length);
            int i=0;
            for(; i < segStartIdx; i++) {
                list.Add(i);
            }

            int val = i;
            int segEndIdx = segStartIdx + segLength;
            for(; i < segEndIdx; i++) {
                list.Add(val);
            }

            for(val++; i < length; i++, val++) {
                list.Add(val);
            }

            return list;
        }
    }
}
