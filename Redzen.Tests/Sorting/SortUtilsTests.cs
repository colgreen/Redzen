using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FluentAssertions;
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
        [InlineData(new int[] { 0, 2, 5, 7, 8, 12, 16, 32, 32, int.MaxValue })]
        [InlineData(new int[] { -10, -9, -8, -7, -6, -2 })]
        [InlineData(new int[] { -10, -9, -8, -7, -6, -2, 0 })]
        [InlineData(new int[] { -10, -9, -8, -7, -6, -2, 0, 100 })]
        [InlineData(new int[] { -10, -9, -8, -7, -6, -2, 0, int.MaxValue })]
        [InlineData(new int[] { int.MinValue, -10, -9, -8, -7, -6, -2, 0, int.MaxValue })]
        public void IsSortedAscending_Int_Sorted(int[] arr)
        {
            SortUtils.IsSortedAscending<int>(arr).Should().BeTrue();
        }

        [Theory]
        [InlineData(new int[] { 2, 5, 8, 7, 12, 16, 32 })]
        [InlineData(new int[] { 5, 8, 2, 16, 32, 12, 7 })]
        [InlineData(new int[] { 5, 8, 2, 16, 32, 12, int.MaxValue })]
        public void IsSortedAscending_Int_NotSorted(int[] arr)
        {
            SortUtils.IsSortedAscending<int>(arr).Should().BeFalse();
        }

        [Theory]
        [InlineData("a", "b", "c", "d", "e")]
        [InlineData("a", "a", "c", "d", "e")]
        public void IsSortedAscending_String_Sorted(params string[] arr)
        {
            SortUtils.IsSortedAscending<string>(arr).Should().BeTrue();
        }

        [Theory]
        [InlineData("b", "a", "c", "d", "e")]
        [InlineData("a", "c", "e", "d")]
        public void IsSortedAscending_String_NotSorted(params string[] arr)
        {
            SortUtils.IsSortedAscending<string>(arr).Should().BeFalse();
        }

        [Theory]
        [InlineData(new int[] { 2, 5, 7, 8, 12, 16, 32 })]
        [InlineData(new int[] { 2, 2, 5, 7, 8, 12, 16, 32 })]
        [InlineData(new int[] { 2, 5, 5, 7, 8, 12, 16, 32 })]
        [InlineData(new int[] { 2, 5, 7, 8, 12, 16, 32, 32 })]
        [InlineData(new int[] { 0, 2, 5, 7, 8, 12, 16, 32, 32, int.MaxValue })]
        [InlineData(new int[] { -10, -9, -8, -7, -6, -2 })]
        [InlineData(new int[] { -10, -9, -8, -7, -6, -2, 0 })]
        [InlineData(new int[] { -10, -9, -8, -7, -6, -2, 0, 100 })]
        [InlineData(new int[] { -10, -9, -8, -7, -6, -2, 0, int.MaxValue })]
        [InlineData(new int[] { int.MinValue, -10, -9, -8, -7, -6, -2, 0, int.MaxValue })]
        public void IsSortedAscending_Comparer_Int_Sorted(int[] arr)
        {
            SortUtils.IsSortedAscending(arr, Comparer<int>.Default).Should().BeTrue();

        }

        [Theory]
        [InlineData(new int[] { 2, 5, 8, 7, 12, 16, 32 })]
        [InlineData(new int[] { 5, 8, 2, 16, 32, 12, 7 })]
        [InlineData(new int[] { 5, 8, 2, 16, 32, 12, int.MaxValue })]
        public void IsSortedAscending_Comparer_Int_NotSorted(int[] arr)
        {
            SortUtils.IsSortedAscending(arr, Comparer<int>.Default).Should().BeFalse();
        }

        [Theory]
        [InlineData("a", "b", "c", "d", "e")]
        [InlineData("a", "a", "c", "d", "e")]
        public void IsSortedAscending_Comparer_String_Sorted(params string[] arr)
        {
            SortUtils.IsSortedAscending(arr, Comparer<string>.Default).Should().BeTrue();
        }

        [Theory]
        [InlineData("b", "a", "c", "d", "e")]
        [InlineData("a", "c", "e", "d")]
        public void IsSortedAscending_Comparer_String_NotSorted(params string[] arr)
        {
            SortUtils.IsSortedAscending(arr, Comparer<string>.Default).Should().BeFalse();
        }

        delegate bool TryFindSegmentSpanDelegate<T>(ReadOnlySpan<T> span, IComparer<T> comparer, ref int startIdx, out int length);

        [Theory]
        [InlineData(100, 30, 10)]
        [InlineData(100, 0, 10)]
        [InlineData(100, 0, 2)]
        [InlineData(100, 1, 2)]
        [InlineData(100, 98, 2)]
        public void TestTryFindSegment(int spanLength, int segStartIdx, int segLength)
        {
            // This is highly convoluted method calling by refletion *and* dynamic building and compiling of an expression tree.
            // The reflection is required because the method being tested is private; the dynamic compilation is required because
            // one of the method parameters is a span, i.e. a by ref struct which can therefore not be placed on the heap, so we
            // can't use the usual method of passing an object[] of method arguments.
            // See: https://stackoverflow.com/a/63127075/15703

            // Old method when there was a single implementation of TryFindSegment().
            MethodInfo methodInfo = typeof(SortUtils).GetMethod("TryFindSegment", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(typeof(int));

            var spanParamExpr = Expression.Parameter(typeof(ReadOnlySpan<int>), "s");
            var comparerParamExpr = Expression.Parameter(typeof(IComparer<int>), "c");
            var startIdxParamExpr = Expression.Parameter(typeof(int).MakeByRefType(), "i");
            var lengthParamExpr = Expression.Parameter(typeof(int).MakeByRefType(), "l");

            var methodCallExpr = Expression.Call(
                methodInfo, spanParamExpr,
                comparerParamExpr, startIdxParamExpr,
                lengthParamExpr);

            Expression<TryFindSegmentSpanDelegate<int>> expr = 
                Expression.Lambda<TryFindSegmentSpanDelegate<int>>(
                    methodCallExpr,
                    spanParamExpr,
                    comparerParamExpr,
                    startIdxParamExpr,
                    lengthParamExpr);

            TryFindSegmentSpanDelegate<int> TryFindSegmentFunc = expr.Compile();

            int startIdx = 0;

            bool success = TryFindSegmentFunc(
                CreateIntListWithSegment(spanLength, segStartIdx, segLength).AsSpan(),
                Comparer<int>.Default,
                ref startIdx,
                out int length);

            success.Should().BeTrue();
            startIdx.Should().Be(segStartIdx);
            length.Should().Be(segLength);
        }

        private static int[] CreateIntListWithSegment(int length, int segStartIdx, int segLength)
        {
            List<int> list = new(length);
            int i=0;
            for(; i < segStartIdx; i++)
                list.Add(i);

            int val = i;
            int segEndIdx = segStartIdx + segLength;
            for(; i < segEndIdx; i++)
                list.Add(val);

            for(val++; i < length; i++, val++)
                list.Add(val);

            return list.ToArray();
        }
    }
}
