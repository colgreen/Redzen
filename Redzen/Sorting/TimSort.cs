// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace Redzen.Sorting;

/// <summary>
/// Timsort public API.
/// </summary>
public static class TimSort
{
    /// <summary>
    /// Sort the items of the given span.
    /// </summary>
    /// <typeparam name="T">The span element type.</typeparam>
    /// <param name="span">The span to be sorted.</param>
    public static void Sort<T>(Span<T> span)
        where T : IComparable<T>
    {
        T[]? work = null;
        TimSort<T>.Sort(span, ref work);
    }

    /// <summary>
    /// Sort the items of the given span, using the optional working array when provided.
    /// </summary>
    /// <typeparam name="T">The span element type.</typeparam>
    /// <param name="span">The span to be sorted.</param>
    /// <param name="work">An optional working array.</param>
    /// <remarks>
    /// Timsort requires additional memory to work with, and this can optionally be supplied by the
    /// <paramref name="work"/> array parameter. If the supplied array is null or too short, then a new array will
    /// be allocated, and <paramref name="work"/> will reference the new array when the method returns. Further
    /// calls to this method may then re-use the same array reference, and therefore the working array will
    /// naturally grow to whatever size is required.
    /// </remarks>
    public static void Sort<T>(Span<T> span, ref T[]? work)
        where T : IComparable<T>
    {
        TimSort<T>.Sort(span, ref work);
    }

    /// <summary>
    /// Sort a pair of spans, based on the sort order of elements in <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="K">The key span element type.</typeparam>
    /// <typeparam name="V">The secondary values span element type.</typeparam>
    /// <param name="keys">The key values span.</param>
    /// <param name="vals">The secondary values span.</param>
    /// <remarks>
    /// The elements of <paramref name="keys"/> are sorted normally.
    /// The elements of <paramref name="vals"/> are re-ordered in parallel with the keys, such that each key-value
    /// pair is maintained.
    /// </remarks>
    public static void Sort<K,V>(Span<K> keys, Span<V> vals)
        where K : IComparable<K>
    {
        K[]? work = null;
        V[]? workv = null;
        TimSort<K,V>.Sort(
            keys, vals,
            ref work,
            ref workv);
    }

    /// <summary>
    /// Sort a pair of spans, based on the sort order of elements in <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="K">The key span element type.</typeparam>
    /// <typeparam name="V">The secondary values span element type.</typeparam>
    /// <param name="keys">The key values span.</param>
    /// <param name="vals">The secondary values span.</param>
    /// <param name="work">An optional working array.</param>
    /// <param name="workv">An optional working array, for the secondary values.</param>
    /// <remarks>
    /// Timsort requires additional memory to work with, and this can optionally be supplied by the
    /// <paramref name="work"/> and <paramref name="workv"/> array parameters. If the supplied work arrays are
    /// null or too short, then new arrays will be allocated and returned via the reference parameters when the
    /// method returns. Further calls to this method may then re-use the same array references, and therefore the
    /// working arrays will naturally grow to whatever size is required.
    /// The work array parameters must either both be supplied, or not supplied. If supplied they must have the
    /// same length.
    /// </remarks>
    public static void Sort<K,V>(
        Span<K> keys,
        Span<V> vals,
        ref K[]? work,
        ref V[]? workv)
        where K : IComparable<K>
    {
        TimSort<K, V>.Sort(
            keys, vals,
            ref work,
            ref workv);
    }

    /// <summary>
    /// Sort three spans, based on the sort order of elements in <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="K">The key span element type.</typeparam>
    /// <typeparam name="V">The secondary values span element type.</typeparam>
    /// <typeparam name="W">The tertiary values span element type.</typeparam>
    /// <param name="keys">The key values span.</param>
    /// <param name="vals">The secondary values span.</param>
    /// <param name="wals">The tertiary values span.</param>
    /// <remarks>
    /// The elements of <paramref name="keys"/> are sorted normally.
    /// The elements of <paramref name="vals"/> and <paramref name="wals"/> are re-ordered in parallel with the
    /// keys, such that each key-value-value tuple is maintained.
    /// </remarks>
    public static void Sort<K, V, W>(
        Span<K> keys,
        Span<V> vals,
        Span<W> wals)
        where K : IComparable<K>
    {
        K[]? work = null;
        V[]? workv = null;
        W[]? workw = null;
        TimSort<K,V,W>.Sort(
            keys, vals, wals,
            ref work,
            ref workv,
            ref workw);
    }

    /// <summary>
    /// Sort three spans, based on the sort order of elements in <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="K">The key span element type.</typeparam>
    /// <typeparam name="V">The secondary values span element type.</typeparam>
    /// <typeparam name="W">The tertiary values span element type.</typeparam>
    /// <param name="keys">The key values span.</param>
    /// <param name="vals">The secondary values span.</param>
    /// <param name="wals">The tertiary values span.</param>
    /// <param name="work">An optional working array.</param>
    /// <param name="workv">An optional working array, for the secondary values.</param>
    /// <param name="workw">An optional working array, for the tertiary values.</param>
    /// <remarks>
    /// Timsort requires additional memory to work with, and this can optionally be supplied by the
    /// <paramref name="work"/>, <paramref name="workv"/> and <paramref name="workw"/> array parameters. If the
    /// supplied work arrays are null or too short, then new arrays will be allocated and returned via the
    /// reference parameters when the method returns. Further calls to this method may then re-use the same array
    /// references, and therefore the working arrays will naturally grow to whatever size is required. The work
    /// array parameters must either all be supplied, or not supplied. If supplied they must all have the same
    /// length.
    /// </remarks>
    public static void Sort<K, V, W>(
        Span<K> keys,
        Span<V> vals,
        Span<W> wals,
        ref K[]? work,
        ref V[]? workv,
        ref W[]? workw)
        where K : IComparable<K>
    {
        TimSort<K, V, W>.Sort(
            keys, vals, wals,
            ref work,
            ref workv,
            ref workw);
    }
}
