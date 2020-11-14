using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace Redzen.Sorting.Tests
{
    public class SortUtilsTests
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
            Assert.True(SortUtils.IsSortedAscending<int>(arr));
        }

        [Theory]
        [InlineData(new int[] { 2, 5, 8, 7, 12, 16, 32 })]
        [InlineData(new int[] { 5, 8, 2, 16, 32, 12, 7 })]
        [InlineData(new int[] { 5, 8, 2, 16, 32, 12, int.MaxValue })]
        public void IsSortedAscending_Int_NotSorted(int[] arr)
        {
            Assert.False(SortUtils.IsSortedAscending<int>(arr));
        }

        [Theory]
        [InlineData("a", "b", "c", "d", "e")]
        [InlineData("a", "a", "c", "d", "e")]
        public void IsSortedAscending_String_Sorted(params string[] arr)
        {
            Assert.True(SortUtils.IsSortedAscending<string>(arr));
        }

        [Theory]
        [InlineData("b", "a", "c", "d", "e")]
        [InlineData("a", "c", "e", "d")]
        public void IsSortedAscending_String_NotSorted(params string[] arr)
        {
            Assert.False(SortUtils.IsSortedAscending<string>(arr));
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
            Assert.True(SortUtils.IsSortedAscending(arr, Comparer<int>.Default));
        }

        [Theory]
        [InlineData(new int[] { 2, 5, 8, 7, 12, 16, 32 })]
        [InlineData(new int[] { 5, 8, 2, 16, 32, 12, 7 })]
        [InlineData(new int[] { 5, 8, 2, 16, 32, 12, int.MaxValue })]
        public void IsSortedAscending_Comparer_Int_NotSorted(int[] arr)
        {
            Assert.False(SortUtils.IsSortedAscending(arr, Comparer<int>.Default));
        }

        [Theory]
        [InlineData("a", "b", "c", "d", "e")]
        [InlineData("a", "a", "c", "d", "e")]
        public void IsSortedAscending_Comparer_String_Sorted(params string[] arr)
        {
            Assert.True(SortUtils.IsSortedAscending(arr, Comparer<string>.Default));
        }

        [Theory]
        [InlineData("b", "a", "c", "d", "e")]
        [InlineData("a", "c", "e", "d")]
        public void IsSortedAscending_Comparer_String_NotSorted(params string[] arr)
        {
            Assert.False(SortUtils.IsSortedAscending(arr, Comparer<string>.Default));
        }

        delegate bool TryFindSegmentDelegate<T>(Span<T> span, IComparer<T> comparer, ref int startIdx, out int length);

        [Fact]
        public void TestTryFindSegment()
        {
            // This is highly convoluted method calling by reflation *and* dynamic building and and compiling of an expression tree.
            // The reflection is required because the metho dbeign tested is private; the dynamic comilation is required because
            // of the method parameters is a span, i.e. a by ref struct which can thereore not be placed on the head, so we can't 
            // use the usual method of passing an object[] of method arguments.
            // See: https://stackoverflow.com/a/63127075/15703

            MethodInfo methodInfo = typeof(SortUtils).GetMethod("TryFindSegment", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(typeof(int));

            var spanParamExpr = Expression.Parameter(typeof(Span<int>), "s");
            var comparerParamExpr = Expression.Parameter(typeof(IComparer<int>), "c");
            var startIdxParamExpr = Expression.Parameter(typeof(int).MakeByRefType(), "i");
            var lengthParamExpr = Expression.Parameter(typeof(int).MakeByRefType(), "l");
            
            var methodCallExpr = Expression.Call(
                methodInfo, spanParamExpr,
                comparerParamExpr, startIdxParamExpr,
                lengthParamExpr);

            Expression<TryFindSegmentDelegate<int>> expr = Expression.Lambda<TryFindSegmentDelegate<int>>(methodCallExpr, spanParamExpr, comparerParamExpr, startIdxParamExpr, lengthParamExpr);

            TryFindSegmentDelegate<int> TryFindSegmentFunc = expr.Compile();

            int startIdx = 0;

            bool success = TryFindSegmentFunc(
                CreateIntListWithSegment(100,30,10).AsSpan(),
                Comparer<int>.Default,
                ref startIdx,
                out int length);

            Assert.True(success);
            Assert.Equal(30, startIdx);
            Assert.Equal(10, length);
        }

        private static int[] CreateIntListWithSegment(int length, int segStartIdx, int segLength)
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

            return list.ToArray();
        }
    }
}
