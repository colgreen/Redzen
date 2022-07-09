using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Redzen.Sorting;

#pragma warning disable SA1649 // File name should match first type name

/// <summary>
/// For sorting a span of key values, and two accompanying value spans.
/// </summary>
/// <remarks>
/// This class exists because Array.Sort() has overloads for one additional array only, whereas this class
/// will sort two additional arrays.
///
/// In addition, this class does not support:
/// (1) Null key values.
/// (2) Floating key values with a value of NaN.
///
/// If you want to use this class in those two scenarios, then you may simply move all of the null (or NaN)
/// values to the head of the span, and then call span.Slice() on the three spans to obtain all of the
/// remaining items, which can then be passed to Sort() on this class.
///
/// This class is a modification of ArraySortHelper in the core framework:
///    https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Collections/Generic/ArraySortHelper.cs.
/// </remarks>
/// <typeparam name="K">Key item type.</typeparam>
/// <typeparam name="V">Value item type.</typeparam>
/// <typeparam name="W">Value item type, for the secondary values array.</typeparam>
public static class IntroSort<K, V, W>
    where K : IComparable<K>
{
    #region Statics / Consts

    // This is the threshold where Introspective sort switches to Insertion sort.
    // Empirically, 16 seems to speed up most cases without slowing down others, at least for integers.
    // Large value types may benefit from a smaller number.
    const int __introsortSizeThreshold = 24;

    #endregion

    #region Public Static Methods

    /// <summary>
    /// Sort the elements of <paramref name="keys"/>, keeping the corresponding elements of type value arrays aligned with the key elements.
    /// </summary>
    /// <param name="keys">The key values to sort.</param>
    /// <param name="vspan">The secondary values span..</param>
    /// <param name="wspan">The tertiary values span.</param>
    public static void Sort(
        Span<K> keys,
        Span<V> vspan,
        Span<W> wspan)
    {
        if(keys.Length > 1)
        {
            IntroSortInner(
                keys, vspan, wspan,
                2 * (BitOperations.Log2((uint)keys.Length) + 1));
        }
    }

    #endregion

    #region Private Static Methods [Intro Sort]

    private static void IntroSortInner(
        Span<K> keys,
        Span<V> values,
        Span<W> wspan,
        int depthLimit)
    {
        Debug.Assert(!keys.IsEmpty);
        Debug.Assert(values.Length == keys.Length);
        Debug.Assert(depthLimit >= 0);

        int partitionSize = keys.Length;
        while(partitionSize > 1)
        {
            if(partitionSize <= __introsortSizeThreshold)
            {
                if(partitionSize == 2)
                {
                    SwapIfGreater(keys, values, wspan, 0, 1);
                    return;
                }

                if(partitionSize == 3)
                {
                    SwapIfGreater(keys, values, wspan, 0, 1);
                    SwapIfGreater(keys, values, wspan, 0, 2);
                    SwapIfGreater(keys, values, wspan, 1, 2);
                    return;
                }

                InsertionSort(
                    keys.Slice(0, partitionSize),
                    values.Slice(0, partitionSize),
                    wspan.Slice(0, partitionSize));
                return;
            }

            if(depthLimit == 0)
            {
                HeapSort(
                    keys.Slice(0, partitionSize),
                    values.Slice(0, partitionSize),
                    wspan.Slice(0, partitionSize));
                return;
            }
            depthLimit--;

            int p = PickPivotAndPartition(
                keys.Slice(0, partitionSize),
                values.Slice(0, partitionSize),
                wspan.Slice(0, partitionSize));

            // Note we've already partitioned around the pivot and do not have to move the pivot again.
            IntroSortInner(
                keys[(p+1)..partitionSize],
                values[(p+1)..partitionSize],
                wspan[(p+1)..partitionSize],
                depthLimit);

            partitionSize = p;
        }
    }

    private static int PickPivotAndPartition(
        Span<K> keys,
        Span<V> values,
        Span<W> wspan)
    {
        Debug.Assert(keys.Length >= __introsortSizeThreshold);

        int hi = keys.Length - 1;

        // Compute median-of-three.  But also partition them, since we've done the comparison.
        int middle = hi >> 1;

        // Sort lo, mid and hi appropriately, then pick mid as the pivot.
        SwapIfGreater(keys, values, wspan, 0, middle);  // swap the low with the mid point.
        SwapIfGreater(keys, values, wspan, 0, hi);      // swap the low with the high.
        SwapIfGreater(keys, values, wspan, middle, hi); // swap the middle with the high.

        K pivot = keys[middle];
        Swap(keys, values, wspan, middle, hi - 1);
        int left = 0, right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

        while(left < right)
        {
            while(GreaterThan(ref pivot, ref keys[++left]));
            while(LessThan(ref pivot, ref keys[--right]));

            if(left >= right)
                break;

            Swap(keys, values, wspan, left, right);
        }

        // Put pivot in the right location.
        if(left != hi - 1)
        {
            Swap(keys, values, wspan, left, hi - 1);
        }
        return left;
    }

    #endregion

    #region Private Static Methods [Heap Sort]

    private static void HeapSort(
        Span<K> keys,
        Span<V> vspan,
        Span<W> wspan)
    {
        Debug.Assert(!keys.IsEmpty);

        int n = keys.Length;
        for(int i = n >> 1; i >= 1; i--)
        {
            DownHeap(keys, vspan, wspan, i, n);
        }

        for(int i = n; i > 1; i--)
        {
            Swap(keys, vspan, wspan, 0, i - 1);
            DownHeap(keys, vspan, wspan, 1, i - 1);
        }
    }

    private static void DownHeap(
        Span<K> keys,
        Span<V> vspan,
        Span<W> wspan,
        int i, int n)
    {
        K d = keys[i - 1];
        V dv = vspan[i - 1];
        W dw = wspan[i - 1];

        while(i <= n >> 1)
        {
            int child = 2 * i;
            if(child < n && LessThan(ref keys[child - 1], ref keys[child]))
            {
                child++;
            }

            if(!LessThan(ref d, ref keys[child - 1]))
                break;

            keys[i - 1] = keys[child - 1];
            vspan[i - 1] = vspan[child - 1];
            wspan[i - 1] = wspan[child - 1];
            i = child;
        }

        keys[i - 1] = d;
        vspan[i - 1] = dv;
        wspan[i - 1] = dw;
    }

    #endregion

    #region Private Static Methods [Insertion Sort]

    private static void InsertionSort(
        Span<K> keys,
        Span<V> vspan,
        Span<W> wspan)
    {
        for(int i = 0; i < keys.Length - 1; i++)
        {
            K k = keys[i + 1];
            V v = vspan[i + 1];
            W w = wspan[i + 1];

            int j = i;
            while(j >= 0 && LessThan(ref k, ref keys[j]))
            {
                keys[j + 1] = keys[j];
                vspan[j + 1] = vspan[j];
                wspan[j + 1] = wspan[j];
                j--;
            }

            keys[j + 1] = k;
            vspan[j + 1] = v;
            wspan[j + 1] = w;
        }
    }

    #endregion

    #region Private Static Methods [Misc]

    private static void SwapIfGreater(
        Span<K> keys,
        Span<V> vspan,
        Span<W> wspan,
        int i, int j)
    {
        Debug.Assert(i != j);

        if(GreaterThan(ref keys[i], ref keys[j]))
        {
            Swap(keys, vspan, wspan, i, j);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Swap(
        Span<K> keys,
        Span<V> vspan,
        Span<W> wspan,
        int i, int j)
    {
        Debug.Assert(i != j);

        K key = keys[i];
        keys[i] = keys[j];
        keys[j] = key;

        V v = vspan[i];
        vspan[i] = vspan[j];
        vspan[j] = v;

        W w = wspan[i];
        wspan[i] = wspan[j];
        wspan[j] = w;
    }

#pragma warning disable IDE0075 // Simplify conditional expression

    // - These methods exist for use in sorting, where the additional operations present in
    //   the CompareTo methods that would otherwise be used on these primitives add non-trivial overhead,
    //   in particular for floating point where the CompareTo methods need to factor in NaNs.
    // - The floating-point comparisons here assume no NaNs, which is valid only because the sorting routines
    //   themselves special-case NaN with a pre-pass that ensures none are present in the values being sorted
    //   by moving them all to the front first and then sorting the rest.
    // - The `? true : false` is to work-around poor codegen: https://github.com/dotnet/runtime/issues/37904#issuecomment-644180265.
    // - These are duplicated here rather than being on a helper type due to current limitations around generic inlining.

    [MethodImpl(MethodImplOptions.AggressiveInlining)] // compiles to a single comparison or method call
    private static bool LessThan(ref K left, ref K right)
    {
        if(typeof(K) == typeof(byte)) return (byte)(object)left < (byte)(object)right ? true : false;
        if(typeof(K) == typeof(sbyte)) return (sbyte)(object)left < (sbyte)(object)right ? true : false;
        if(typeof(K) == typeof(ushort)) return (ushort)(object)left < (ushort)(object)right ? true : false;
        if(typeof(K) == typeof(short)) return (short)(object)left < (short)(object)right ? true : false;
        if(typeof(K) == typeof(uint)) return (uint)(object)left < (uint)(object)right ? true : false;
        if(typeof(K) == typeof(int)) return (int)(object)left < (int)(object)right ? true : false;
        if(typeof(K) == typeof(ulong)) return (ulong)(object)left < (ulong)(object)right ? true : false;
        if(typeof(K) == typeof(long)) return (long)(object)left < (long)(object)right ? true : false;
        if(typeof(K) == typeof(nuint)) return (nuint)(object)left < (nuint)(object)right ? true : false;
        if(typeof(K) == typeof(nint)) return (nint)(object)left < (nint)(object)right ? true : false;
        if(typeof(K) == typeof(float)) return (float)(object)left < (float)(object)right ? true : false;
        if(typeof(K) == typeof(double)) return (double)(object)left < (double)(object)right ? true : false;
        if(typeof(K) == typeof(Half)) return (Half)(object)left < (Half)(object)right ? true : false;
        return left.CompareTo(right) < 0 ? true : false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] // compiles to a single comparison or method call
    private static bool GreaterThan(ref K left, ref K right)
    {
        if(typeof(K) == typeof(byte)) return (byte)(object)left > (byte)(object)right ? true : false;
        if(typeof(K) == typeof(sbyte)) return (sbyte)(object)left > (sbyte)(object)right ? true : false;
        if(typeof(K) == typeof(ushort)) return (ushort)(object)left > (ushort)(object)right ? true : false;
        if(typeof(K) == typeof(short)) return (short)(object)left > (short)(object)right ? true : false;
        if(typeof(K) == typeof(uint)) return (uint)(object)left > (uint)(object)right ? true : false;
        if(typeof(K) == typeof(int)) return (int)(object)left > (int)(object)right ? true : false;
        if(typeof(K) == typeof(ulong)) return (ulong)(object)left > (ulong)(object)right ? true : false;
        if(typeof(K) == typeof(long)) return (long)(object)left > (long)(object)right ? true : false;
        if(typeof(K) == typeof(nuint)) return (nuint)(object)left > (nuint)(object)right ? true : false;
        if(typeof(K) == typeof(nint)) return (nint)(object)left > (nint)(object)right ? true : false;
        if(typeof(K) == typeof(float)) return (float)(object)left > (float)(object)right ? true : false;
        if(typeof(K) == typeof(double)) return (double)(object)left > (double)(object)right ? true : false;
        if(typeof(K) == typeof(Half)) return (Half)(object)left > (Half)(object)right ? true : false;
        return left.CompareTo(right) > 0 ? true : false;
    }

#pragma warning restore IDE0075 // Simplify conditional expression

    #endregion
}
