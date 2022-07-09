// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.using System.Numerics;
using System.Numerics;

namespace Redzen.Sorting;

/// <summary>
/// For sorting a span of key values, and accompanying value spans, using Introsort
/// (see https://en.wikipedia.org/wiki/Introsort).
/// </summary>
/// <remarks>
/// This helper exists because Array.Sort() has an overload that takes a single additional value array only,
/// whereas this helper will sort two additional arrays.
///
/// In addition, this class does not support:
/// (1) Null key values.
/// (2) Floating key values with a value of NaN.
///
/// If you want to use this class in those two scenarios, then you may simply move all of the null (or NaN)
/// values to the head of the spans, and then call span.Slice() on the three spans to obtain all of the
/// remaining items, which can then be sorted by this helper.
/// </remarks>
public static class IntroSort
{
    /// <summary>
    /// Sort the elements of <paramref name="keys"/>, keeping the corresponding elements of two value arrays
    /// aligned with the key elements.
    /// </summary>
    /// <typeparam name="K">Key item type.</typeparam>
    /// <typeparam name="V">Value item type.</typeparam>
    /// <typeparam name="W">Value item type, for the second values array.</typeparam>
    /// <param name="keys">The key values to sort.</param>
    /// <param name="values">The secondary values span..</param>
    /// <param name="values2">The tertiary values span.</param>
    public static void Sort<K,V,W>(
        Span<K> keys,
        Span<V> values,
        Span<W> values2)
        where K : IComparable<K>
    {
        if(keys.Length > 1)
        {
            IntroSort<K,V,W>.IntroSortInner(
                keys, values, values2,
                2 * (BitOperations.Log2((uint)keys.Length) + 1));
        }
    }
}
