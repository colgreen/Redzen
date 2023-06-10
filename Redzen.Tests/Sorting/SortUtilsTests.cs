using System.Linq.Expressions;
using System.Reflection;
using FluentAssertions;
using Redzen.Random;
using Xunit;

namespace Redzen.Sorting;

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


    [Theory]
    [InlineData(100, 4, 6)]
    [InlineData(100, 0, 6)]
    [InlineData(100, 90, 10)]
    public void SortUnstable_IComparable(int length, int segStartIdx, int segLength)
    {
        // Arrange.
        Item[] arr = CreateTestItemArray(length, segStartIdx, segLength);
        var rng = RandomDefaults.CreateRandomSource(0);

        // Act.
        SortUtils.SortUnstable<Item>(arr, rng);

        // Assert.
        SortUtils.IsSortedAscending<Item>(arr).Should().BeTrue();

        // The segment of equal items should be shuffled (not sorted).
        var payloadSegment = arr.Skip(segStartIdx).Take(segLength).Select(x => x.Payload).ToArray();
        SortUtils.IsSortedAscending<int>(payloadSegment).Should().BeFalse();
    }

    [Theory]
    [InlineData(100, 4, 6)]
    [InlineData(100, 0, 6)]
    [InlineData(100, 90, 10)]
    public void SortUnstable_IComparer(int length, int segStartIdx, int segLength)
    {
        // Arrange.
        Item[] arr = CreateTestItemArray(length, segStartIdx, segLength);
        var comparer = Comparer<Item>.Create((x, y) => x.Value.CompareTo(y.Value));
        var rng = RandomDefaults.CreateRandomSource(0);

        // Act.
        SortUtils.SortUnstable(arr, comparer, rng);

        // Assert.
        SortUtils.IsSortedAscending(arr, comparer).Should().BeTrue();

        // The segment of equal items should be shuffled (not sorted).
        var payloadSegment = arr.Skip(segStartIdx).Take(segLength).Select(x => x.Payload).ToArray();
        SortUtils.IsSortedAscending<int>(payloadSegment).Should().BeFalse();

    }

    delegate bool TryFindSegmentSpanDelegate_IComparable<T>(Span<T> span, ref int startIdx, out int length);
    delegate bool TryFindSegmentSpanDelegate_IComparer<T>(ReadOnlySpan<T> span, IComparer<T> comparer, ref int startIdx, out int length);

    [Theory]
    [InlineData(100, 30, 10)]
    [InlineData(100, 0, 10)]
    [InlineData(100, 0, 2)]
    [InlineData(100, 1, 2)]
    [InlineData(100, 98, 2)]
    public void TestTryFindSegment_IComparable(int spanLength, int segStartIdx, int segLength)
    {
        // This is highly convoluted method calling by reflection *and* dynamic building and compiling of an expression tree.
        // The reflection is required because the method being tested is private; the dynamic compilation is required because
        // one of the method parameters is a span, i.e. a by ref struct which can therefore not be placed on the heap, so we
        // can't use the usual method of passing an object[] of method arguments.
        // See: https://stackoverflow.com/a/63127075/15703

        // Old method when there was a single implementation of TryFindSegment().
        MethodInfo methodInfo = typeof(SortUtils).GetMethod(
            "TryFindSegment_IComparable",
            BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(typeof(int));

        var spanParamExpr = Expression.Parameter(typeof(Span<int>), "s");
        var startIdxParamExpr = Expression.Parameter(typeof(int).MakeByRefType(), "i");
        var lengthParamExpr = Expression.Parameter(typeof(int).MakeByRefType(), "l");

        var methodCallExpr = Expression.Call(
            methodInfo, spanParamExpr,
            startIdxParamExpr,
            lengthParamExpr);

        Expression<TryFindSegmentSpanDelegate_IComparable<int>> expr =
            Expression.Lambda<TryFindSegmentSpanDelegate_IComparable<int>>(
                methodCallExpr,
                spanParamExpr,
                startIdxParamExpr,
                lengthParamExpr);

        TryFindSegmentSpanDelegate_IComparable<int> TryFindSegmentFunc = expr.Compile();

        int startIdx = 0;

        bool success = TryFindSegmentFunc(
            CreateIntArrayWithSegment(spanLength, segStartIdx, segLength).AsSpan(),
            ref startIdx,
            out int length);

        success.Should().BeTrue();
        startIdx.Should().Be(segStartIdx);
        length.Should().Be(segLength);
    }

    [Theory]
    [InlineData(100, 30, 10)]
    [InlineData(100, 0, 10)]
    [InlineData(100, 0, 2)]
    [InlineData(100, 1, 2)]
    [InlineData(100, 98, 2)]
    public void TestTryFindSegment_IComparer(int spanLength, int segStartIdx, int segLength)
    {
        // This is highly convoluted method calling by reflection *and* dynamic building and compiling of an expression tree.
        // The reflection is required because the method being tested is private; the dynamic compilation is required because
        // one of the method parameters is a span, i.e. a by ref struct which can therefore not be placed on the heap, so we
        // can't use the usual method of passing an object[] of method arguments.
        // See: https://stackoverflow.com/a/63127075/15703

        // Old method when there was a single implementation of TryFindSegment().
        MethodInfo methodInfo = typeof(SortUtils).GetMethod(
            "TryFindSegment",
            BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(typeof(int));

        var spanParamExpr = Expression.Parameter(typeof(ReadOnlySpan<int>), "s");
        var comparerParamExpr = Expression.Parameter(typeof(IComparer<int>), "c");
        var startIdxParamExpr = Expression.Parameter(typeof(int).MakeByRefType(), "i");
        var lengthParamExpr = Expression.Parameter(typeof(int).MakeByRefType(), "l");

        var methodCallExpr = Expression.Call(
            methodInfo, spanParamExpr,
            comparerParamExpr, startIdxParamExpr,
            lengthParamExpr);

        Expression<TryFindSegmentSpanDelegate_IComparer<int>> expr =
            Expression.Lambda<TryFindSegmentSpanDelegate_IComparer<int>>(
                methodCallExpr,
                spanParamExpr,
                comparerParamExpr,
                startIdxParamExpr,
                lengthParamExpr);

        TryFindSegmentSpanDelegate_IComparer<int> TryFindSegmentFunc = expr.Compile();

        int startIdx = 0;

        bool success = TryFindSegmentFunc(
            CreateIntArrayWithSegment(spanLength, segStartIdx, segLength).AsSpan(),
            Comparer<int>.Default,
            ref startIdx,
            out int length);

        success.Should().BeTrue();
        startIdx.Should().Be(segStartIdx);
        length.Should().Be(segLength);
    }

    private static int[] CreateIntArrayWithSegment(int length, int segStartIdx, int segLength)
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

    private static Item[] CreateTestItemArray(int length, int segStartIdx, int segLength)
    {
        var arr = new Item[length];

        for(int i=0; i < length; i++)
            arr[i] = new Item(i, i);

        for(int i = segStartIdx; i < segStartIdx + segLength; i++)
            arr[i] = new Item(segStartIdx, i + 100);

        return arr;
    }

    struct Item : IComparable<Item>
    {
        public int Value;
        public int Payload;

        public Item(int val, int payload)
        {
            Value = val;
            Payload = payload;
        }

        public readonly int CompareTo(Item other)
        {
            return Value.CompareTo(other.Value);
        }

        public static bool operator <(Item left, Item right)
        {
            return left.CompareTo(right)<0;
        }

        public static bool operator >(Item left, Item right)
        {
            return left.CompareTo(right)>0;
        }

        public static bool operator <=(Item left, Item right)
        {
            return left.CompareTo(right)<=0;
        }

        public static bool operator >=(Item left, Item right)
        {
            return left.CompareTo(right)>=0;
        }
    }
}
