using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Redzen.Sorting;

/// <summary>
/// TimSort static utility methods.
/// </summary>
/// <typeparam name="T">The sort span element type.</typeparam>
internal static class TimSortUtils<T>
    where T : IComparable<T>
{
    /// <summary>
    /// Returns the minimum acceptable run length for an array of the specified
    /// length. Natural runs shorter than this will be extended with
    /// BinarySort(K[], int, int, int).
    ///
    /// Roughly speaking, the computation is:
    ///
    ///  If n &lt; MIN_MERGE, return n (it's too small to bother with fancy stuff).
    ///  Else if n is an exact power of 2, return MIN_MERGE/2.
    ///  Else return an int k, MIN_MERGE/2 &lt;= k &lt;= MIN_MERGE, such that n/k
    ///   is close to, but strictly less than, an exact power of 2.
    ///
    /// For the rationale, see timsort.txt.
    /// </summary>
    /// <param name="n">The length of the array to be sorted.</param>
    /// <param name="minMerge">The minimum merge length.</param>
    /// <returns>The length of the minimum run to be merged.</returns>
    public static int MinRunLength(int n, int minMerge)
    {
        Debug.Assert(n >= 0);

        int r = 0;  // Becomes 1 if any 1 bits are shifted off.
        while(n >= minMerge)
        {
            r |= (n & 1);
            n >>= 1;
        }
        return n + r;
    }

    /// <summary>
    /// Locates the position at which to insert the specified key into the
    /// specified sorted range. If the range contains an element equal to key,
    /// returns the index of the leftmost equal element.
    /// </summary>
    /// <param name="key">The key whose insertion point to search for.</param>
    /// <param name="s">The array in which to search.</param>
    /// <param name="baseIdx">The index of the first element in the range.</param>
    /// <param name="len">The length of the range; must be &gt; 0.</param>
    /// <param name="hint">Hint the index at which to begin the search, 0 &lt;= hint &lt; n.
    /// The closer hint is to the result, the faster this method will run.</param>
    /// <returns>The position in <paramref name="s"/> in which to insert <paramref name="key"/>.</returns>
    public static int GallopLeft(
        T key,
        Span<T> s,
        int baseIdx,
        int len,
        int hint)
    {
        Debug.Assert(len > 0 && hint >= 0 && hint < len);

        int lastOfs = 0;
        int ofs = 1;
        if(LessThan(ref s[baseIdx + hint], ref key))
        {
            // Gallop right until a[baseIdx+hint+lastOfs] < key <= a[baseIdx+hint+ofs]
            int maxOfs = len - hint;
            while(ofs < maxOfs && LessThan(ref s[baseIdx + hint + ofs], ref key))
            {
                lastOfs = ofs;
                ofs = (ofs << 1) + 1;
                if(ofs <= 0) // int overflow.
                    ofs = maxOfs;
            }

            if(ofs > maxOfs)
                ofs = maxOfs;

            // Make offsets relative to baseIdx.
            lastOfs += hint;
            ofs += hint;
        }
        else
        {
            // key <= a[baseIdx + hint]
            // Gallop left until a[baseIdx+hint-ofs] < key <= a[baseIdx+hint-lastOfs]
            int maxOfs = hint + 1;
            while(ofs < maxOfs && !LessThan(ref s[baseIdx + hint - ofs], ref key))
            {
                lastOfs = ofs;
                ofs = (ofs << 1) + 1;
                if(ofs <= 0) // int overflow.
                    ofs = maxOfs;
            }
            if(ofs > maxOfs)
                ofs = maxOfs;

            // Make offsets relative to baseIdx.
            int tmp = lastOfs;
            lastOfs = hint - ofs;
            ofs = hint - tmp;
        }
        Debug.Assert(lastOfs >= -1 && lastOfs < ofs && ofs <= len);

        // Now a[baseIdx+lastOfs] < key <= a[baseIdx+ofs], so key belongs somewhere
        // to the right of lastOfs but no farther right than ofs.  Do a binary
        // search, with invariant a[baseIdx + lastOfs - 1] < key <= a[baseIdx + ofs].
        lastOfs++;
        while(lastOfs < ofs)
        {
            int m = lastOfs + ((ofs - lastOfs) >> 1);

            if(LessThan(ref s[baseIdx + m], ref key))
                lastOfs = m + 1;    // a[baseIdx + m] < key
            else
                ofs = m;            // key <= a[baseIdx + m]
        }
        Debug.Assert(lastOfs == ofs);   // so a[baseIdx + ofs - 1] < key <= a[baseIdx + ofs]
        return ofs;
    }

    /// <summary>
    /// Like gallopLeft, except that if the range contains an element equal to
    /// key, gallopRight returns the index after the rightmost equal element.
    /// </summary>
    /// <param name="key">The key whose insertion point to search for.</param>
    /// <param name="s">The array in which to search.</param>
    /// <param name="baseIdx">The index of the first element in the range.</param>
    /// <param name="len">The length of the range; must be &gt; 0.</param>
    /// <param name="hint">The index at which to begin the search, 0 &lt;= hint &lt; n.
    /// The closer hint is to the result, the faster this method will run.</param>
    /// <returns>The int k,  0 &lt;= k &lt;= n such that a[b + k - 1] &lt;= key &lt; a[b + k].</returns>
    public static int GallopRight(
        T key,
        Span<T> s,
        int baseIdx,
        int len,
        int hint)
    {
        Debug.Assert(len > 0 && hint >= 0 && hint < len);

        int ofs = 1;
        int lastOfs = 0;
        if(LessThan(ref key, ref s[baseIdx + hint]))
        {
            // Gallop left until a[baseIdx + hint - ofs] <= key < a[baseIdx + hint - lastOfs]
            int maxOfs = hint + 1;
            while(ofs < maxOfs && LessThan(ref key, ref s[baseIdx + hint - ofs]))
            {
                lastOfs = ofs;
                ofs = (ofs << 1) + 1;
                if(ofs <= 0) // int overflow.
                    ofs = maxOfs;
            }
            if(ofs > maxOfs)
                ofs = maxOfs;

            // Make offsets relative to baseIdx.
            int tmp = lastOfs;
            lastOfs = hint - ofs;
            ofs = hint - tmp;
        }
        else
        {
            // a[baseIdx + hint] <= key
            // Gallop right until a[baseIdx + hint + lastOfs] <= key < a[baseIdx + hint + ofs]
            int maxOfs = len - hint;
            while(ofs < maxOfs && !LessThan(ref key, ref s[baseIdx + hint + ofs]))
            {
                lastOfs = ofs;
                ofs = (ofs << 1) + 1;
                if(ofs <= 0) // int overflow.
                    ofs = maxOfs;
            }
            if(ofs > maxOfs)
                ofs = maxOfs;

            // Make offsets relative to baseIdx.
            lastOfs += hint;
            ofs += hint;
        }
        Debug.Assert(lastOfs >= -1 && lastOfs < ofs && ofs <= len);

        // Now a[baseIdx + lastOfs] <= key < a[baseIdx + ofs], so key belongs somewhere to
        // the right of lastOfs but no farther right than ofs.  Do a binary
        // search, with invariant a[baseIdx + lastOfs - 1] <= key < a[baseIdx + ofs].
        lastOfs++;
        while(lastOfs < ofs)
        {
            int m = lastOfs + ((ofs - lastOfs) >> 1);

            if(LessThan(ref key, ref s[baseIdx + m]))
                ofs = m;            // key < a[baseIdx + m]
            else
                lastOfs = m + 1;    // a[baseIdx + m] <= key
        }
        Debug.Assert(lastOfs == ofs);   // so a[baseIdx + ofs - 1] <= key < a[baseIdx + ofs]
        return ofs;
    }

#pragma warning disable IDE0075 // Simplify conditional expression

    // - These methods exist for use in sorting, where the additional operations present in
    //   the CompareTo methods that would otherwise be used on these primitives add non-trivial overhead,
    //   in particular for floating point where the CompareTo methods need to factor in NaNs.
    // - The floating-point comparisons here assume no NaNs, hence we do not support sorting on floating point values containing NaNs.
    //   The core framework sorting algorithm contains additional logic for pre-sorting NaNs to the head of the span/array, and calling
    //   the sort routine proper on the remaining sub-span. Here we take the view that the vast majority of time in numeric computing
    //   you do not plan to include NaNs in the data, i.e. if they are present then it's due to a problem in an upstream calculation.
    // - The `? true : false` is to work-around poor codegen: https://github.com/dotnet/runtime/issues/37904#issuecomment-644180265.
    // - These are duplicated here rather than being on a helper type due to current limitations around generic inlining.

    /// <summary>
    /// Less than comparison.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>True, if left is less than right.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // compiles to a single comparison or method call
    public static bool LessThan(ref T left, ref T right)
    {
        if(typeof(T) == typeof(byte)) return (byte)(object)left < (byte)(object)right ? true : false;
        if(typeof(T) == typeof(sbyte)) return (sbyte)(object)left < (sbyte)(object)right ? true : false;
        if(typeof(T) == typeof(ushort)) return (ushort)(object)left < (ushort)(object)right ? true : false;
        if(typeof(T) == typeof(short)) return (short)(object)left < (short)(object)right ? true : false;
        if(typeof(T) == typeof(uint)) return (uint)(object)left < (uint)(object)right ? true : false;
        if(typeof(T) == typeof(int)) return (int)(object)left < (int)(object)right ? true : false;
        if(typeof(T) == typeof(ulong)) return (ulong)(object)left < (ulong)(object)right ? true : false;
        if(typeof(T) == typeof(long)) return (long)(object)left < (long)(object)right ? true : false;
        if(typeof(T) == typeof(nuint)) return (nuint)(object)left < (nuint)(object)right ? true : false;
        if(typeof(T) == typeof(nint)) return (nint)(object)left < (nint)(object)right ? true : false;
        if(typeof(T) == typeof(float)) return (float)(object)left < (float)(object)right ? true : false;
        if(typeof(T) == typeof(double)) return (double)(object)left < (double)(object)right ? true : false;
        if(typeof(T) == typeof(Half)) return (Half)(object)left < (Half)(object)right ? true : false;
        return left.CompareTo(right) < 0 ? true : false;
    }

#pragma warning restore IDE0075 // Simplify conditional expression
}
