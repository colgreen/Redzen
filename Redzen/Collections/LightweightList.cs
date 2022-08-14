// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Collections;
using System.Runtime.CompilerServices;

namespace Redzen.Collections;

/// <summary>
/// A simple/lightweight alternative to List{T}.
/// </summary>
/// <typeparam name="T">The list item type.</typeparam>
/// <remarks>
/// A list type that doesn't implement IList{T}, and thus ICollection{T}, and all of the associated API
/// surface and logic that comes with those interfaces.
///
/// The list wraps an inner array that grows as items are added, as per the behaviour of List{T}. However,
/// unlike List{T} the inner array is exposed via <see cref="GetInternalArray()"/> and <see cref="AsSpan()"/>;
/// this allows many operations on the items of the list to be achieved more efficiently by operating directly
/// on the inner array, which is more efficient for operations such as sorting or binary search.
///
/// The internal array can also be set using <see cref="SetInternalArray(T[])"/>, so in some senses this class
/// could be considered to be an array builder class, whereby a List like API can be used to add, remove, and
/// get/set items, and the resulting array can be obtained for use outside of List.
///
/// Note that starting with .NET 6 it is possible to obtain a Span{T} over the elements of a List{T} by use of
/// helper method CollectionsMarshal.AsSpan(); as such, this class may be considered redundant. However, it
/// does present a much simpler API surface (and less API baggage) than List{T} and therefore it remains part
/// of the Redzen project.
/// </remarks>
public class LightweightList<T> : IEnumerable<T>
{
    const int __DefaultCapacity = 4;
    const int __MaxArrayLength = 0X7FEF_FFFF;
    static readonly T[] __emptyArray = Array.Empty<T>();

    T[] _items;
    int _size;

    #region Construction

    /// <summary>
    /// Initializes a new empty instance of <see cref="LightweightList{T}"/>.
    /// </summary>
    public LightweightList()
    {
        _items = __emptyArray;
    }

    /// <summary>
    /// Initializes a new empty instance of <see cref="LightweightList{T}"/>, with the specified capacity.
    /// </summary>
    /// <param name="capacity">The list's initial capacity.</param>
    public LightweightList(int capacity)
    {
        if(capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));

        if(capacity == 0)
            _items = __emptyArray;
        else
            _items = new T[capacity];
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the list's capacity.
    /// </summary>
    public int Capacity
    {
        get => _items.Length;
        set
        {
            if(value < _size) throw new ArgumentOutOfRangeException(nameof(value));

            if(value != _items.Length)
            {
                if(value > 0)
                {
                    T[] newItems = new T[value];
                    if(_size > 0)
                        Array.Copy(_items, newItems, _size);

                    _items = newItems;
                }
                else
                {
                    _items = __emptyArray;
                }
            }
        }
    }

    /// <summary>
    /// Gets the number of items currently contained within the list.
    /// </summary>
    public int Count => _size;

    /// <summary>
    /// Gets or sets the item at the specified index.
    /// </summary>
    /// <param name="index">Item index.</param>
    public T this[int index]
    {
        get
        {
            // Note. the cast to uint is a trick to allow checking of both the high and low bounds of index
            // with a single comparison.
            if((uint)index >= (uint)_size) throw new ArgumentOutOfRangeException(nameof(index));
            return _items[index];
        }

        set
        {
            if((uint)index >= (uint)_size) throw new ArgumentOutOfRangeException(nameof(index));
            _items[index] = value;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Adds a new item to the end of the list.
    /// </summary>
    /// <param name="item">The item to add.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        T[] arr = _items;
        int size = _size;
        if(size < arr.Length)
        {
            _size = size + 1;
            arr[size] = item;
        }
        else
        {
            AddWithResize(item);
        }
    }

    /// <summary>
    /// Adds new items to the end of the list.
    /// </summary>
    /// <param name="span">The span of items to add.</param>
    public void AddRange(ReadOnlySpan<T> span)
        => InsertRange(_size, span);

    /// <summary>
    /// Inserts an item into the list at the specified index.
    /// </summary>
    /// <param name="index">The insertion index.</param>
    /// <param name="item">The item to insert.</param>
    public void Insert(int index, T item)
    {
        // Note. the cast to uint is a trick to allow checking of both the high and low bounds of index
        // with a single comparison.
        // Note that insertions at the end are legal.
        if((uint)index > (uint)_size) throw new ArgumentOutOfRangeException(nameof(index));

        // Ensure there is capacity for the new item.
        if(_size == _items.Length)
            EnsureCapacity(_size + 1);

        // If the insertion is not at the end of the list, then move the existing items up to make space for
        // the insertion.
        if(index < _size)
            Array.Copy(_items, index, _items, index + 1, _size - index);

        // Set the new item.
        _items[index] = item;
        _size++;
    }

    /// <summary>
    /// Inserts new into the list at the specified index.
    /// </summary>
    /// <param name="index">The insertion index.</param>
    /// <param name="span">The span of the items to insert.</param>
    public void InsertRange(int index, ReadOnlySpan<T> span)
    {
        // Note. the cast to uint is a trick to allow checking of both the high and low bounds of index
        // with a single comparison.
        // Note that insertions at the end are legal.
        if((uint)index > (uint)_size) throw new ArgumentOutOfRangeException(nameof(index));

        // We don't allow insertion of items from a span that points to the current list's internal data array.
        if(span.Overlaps(_items)) throw new ArgumentException("span represents a segment of memory within the current List's internal items array; this is not allowed.", nameof(span));

        // Do the insertion of there are items to insert.
        int count = span.Length;
        if(count > 0)
        {
            // Ensure there is capacity for the new item.
            EnsureCapacity(_size + count);

            // If the insertion is not at the end of the list, then move the existing items up to make space for
            // the insertion.
            if(index < _size)
                Array.Copy(_items, index, _items, index + count, _size - index);

            // Insert the new items into the empty section we have just created.
            span.CopyTo(_items.AsSpan(index));
            _size += count;
        }
    }

    /// <summary>
    /// Clears the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        if(RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            // For reference types, we clear the references to allow the garbage collector to reclaim unused objects.
            int size = _size;
            _size = 0;
            if(size > 0)
                Array.Clear(_items, 0, size);
        }
        else
        {
            _size = 0;
        }
    }

    /// <summary>
    /// Removes the item at the specified index.
    /// </summary>
    /// <param name="index">Index of the item to remove.</param>
    /// <remarks>
    /// If there are items after the removed item, then these are moved up the list to close the gap.
    /// </remarks>
    public void RemoveAt(int index)
    {
        // Note. the cast to uint is a trick to allow checking of both the high and low bounds of index
        // with a single comparison.
        if((uint)index >= (uint)_size) throw new ArgumentOutOfRangeException(nameof(index));

        _size--;
        if(index < _size)
            Array.Copy(_items, index + 1, _items, index, _size - index);

        if(RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            _items[_size] = default!;
    }

    /// <summary>
    /// Removes a range of items.
    /// </summary>
    /// <param name="index">Index of the first item to remove.</param>
    /// <param name="count">The number of items to remove, starting at <paramref name="index"/>.</param>
    /// <remarks>
    /// If there are items after the removed items, then these are moved up the list to close the gap.
    /// </remarks>
    public void RemoveRange(int index, int count)
    {
        if(index < 0) throw new ArgumentOutOfRangeException(nameof(index));
        if(count < 0) throw new ArgumentOutOfRangeException(nameof(count));
        if(count > (_size - index)) throw new AggregateException("Invalid arguments.");

        if(count > 0)
        {
            _size -= count;
            if(index < _size)
                Array.Copy(_items, index + count, _items, index, _size - index);

            // For reference types, we clear the references to allow the garbage collector to reclaim unused objects.
            if(RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                Array.Clear(_items, _size, count);
        }
    }

    /// <summary>
    /// Copies the elements of the list to a new array.
    /// </summary>
    /// <returns>An array containing copies of the elements in the list.</returns>
    public T[] ToArray()
    {
        if(_size == 0)
            return __emptyArray;

        T[] arr = new T[_size];
        Array.Copy(_items, arr, _size);
        return arr;
    }

    /// <summary>
    /// Sets the capacity to the actual number of elements in the list, if that number is less than 90% of
    /// the current capacity.
    /// </summary>
    public void TrimExcess()
    {
        // Avoid trimming if less than 10% of the list length would be recovered.
        int threshold = (int)(_items.Length * 0.9);
        if(_size < threshold)
            Capacity = _size;
    }

    /// <summary>
    /// Gets the internal items array that the list is wrapping.
    /// </summary>
    /// <returns>The list's internal data items array.</returns>
    /// <remarks>
    /// The array may have a length that is higher than the number of items in the list.
    /// </remarks>
    public T[] GetInternalArray()
    {
        return _items;
    }

    /// <summary>
    /// Sets the internal items array that the list is wrapping.
    /// </summary>
    /// <param name="items">An array to be used as the list's internal data items array.</param>
    public void SetInternalArray(T[] items)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
        _size = items.Length;
    }

    /// <summary>
    /// Sets the internal items array that the list is wrapping.
    /// </summary>
    /// <param name="items">An array to be used as the list's internal data items array.</param>
    /// <param name="count">The new list count. Must be less than or equal to the length of <paramref name="items"/>.</param>
    public void SetInternalArray(T[] items, int count)
    {
        if(items is null) throw new ArgumentNullException(nameof(items));

        // Note. the cast to uint is a trick to allow checking of both the high and low bounds of index
        // with a single comparison.
        if((uint)count >= (uint)items.Length) throw new ArgumentOutOfRangeException(nameof(count));

        _items = items;
        _size = count;
    }

    /// <summary>
    /// Gets a span over the list items in the internal items array.
    /// </summary>
    /// <returns>A new instance of <see cref="Span{T}"/>.</returns>
    /// <remarks>
    /// The Span length is equal to the number of items in the list.
    /// If mutation operations (such as Add or Clear) are called on the List during the Span's lifetime, then
    /// the Span my be pointing to data that is no longer being used by the List.
    /// </remarks>
    public Span<T> AsSpan()
    {
        return _items.AsSpan(0, _size);
    }

    /// <summary>
    /// Gets a span over a portion of the list items in the internal items array, starting at a
    /// specified position to the end of the array.
    /// </summary>
    /// <param name="start">The index of the first item to cover with the returned Span.</param>
    /// <returns>A new instance of <see cref="Span{T}"/>.</returns>
    /// <remarks>
    /// If mutation operations (such as Add or Clear) are called on the List during the Span's lifetime, then
    /// the Span my be pointing to data that is no longer being used by the List.
    /// </remarks>
    public Span<T> AsSpan(int start)
    {
        // Note. the cast to uint is a trick to allow checking of both the high and low bounds of index
        // with a single comparison.
        if((uint)start >= (uint)_size) throw new ArgumentOutOfRangeException(nameof(start));

        return _items.AsSpan(start, _size - start);
    }

    /// <summary>
    /// Gets a span over a portion of the list items in the internal items array, starting at a
    /// specified position, and for the specified length.
    /// </summary>
    /// <param name="start">The index of the first item to cover with the returned span.</param>
    /// <param name="length">The length of the returned span.</param>
    /// <returns>A new instance of <see cref="Span{T}"/>.</returns>
    /// <remarks>
    /// If mutation operations (such as Add or Clear) are called on the List during the Span's lifetime, then
    /// the Span my be pointing to data that is no longer being used by the List.
    /// </remarks>
    public Span<T> AsSpan(int start, int length)
    {
        if(start < 0) throw new ArgumentOutOfRangeException(nameof(start));
        if(length < 0) throw new ArgumentOutOfRangeException(nameof(length));
        if(length > (_size - start)) throw new AggregateException("Invalid arguments.");

        return _items.AsSpan(start, length);
    }

    #endregion

    #region IEnumerable<T>

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(_items, _size);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(_items, _size);
    }

    #endregion

    #region Private Methods

    // Prevent inlining, as calls to this method are relatively uncommon.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddWithResize(T item)
    {
        int size = _size;
        EnsureCapacity(size + 1);
        _size = size + 1;
        _items[size] = item;
    }

    private void EnsureCapacity(int min)
    {
        if(_items.Length < min)
        {
            int newCapacity = _items.Length == 0 ? __DefaultCapacity : _items.Length * 2;

            // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if((uint)newCapacity > __MaxArrayLength) newCapacity = __MaxArrayLength;
            if(newCapacity < min) newCapacity = min;
            Capacity = newCapacity;
        }
    }

    #endregion

    #region Inner Struct [Enumerator]

    /// <summary>
    /// Enumerates the elements of a <see cref="LightweightList{T}"/>.
    /// </summary>
    public struct Enumerator : IEnumerator<T>, IEnumerator
    {
        readonly T[] _items;
        readonly int _size;
        int _idx;
        T? _curr;

        /// <summary>
        /// Construct an enumerator over the given items array.
        /// </summary>
        /// <param name="items">The items to enumerate over.</param>
        /// <param name="size">A bound on the items to stop enumerating at.</param>
        internal Enumerator(T[] items, int size)
        {
            _items = items;
            _size = size;
            _idx = 0;
            _curr = default;
        }

        /// <inheritdoc/>
        public T Current => _curr!;

        /// <inheritdoc/>
        object? IEnumerator.Current => _curr;

        /// <inheritdoc/>
        public bool MoveNext()
        {
            if(_idx < _size)
            {
                _curr = _items[_idx];
                _idx++;
                return true;
            }
            return MoveNextRare();
        }

        /// <inheritdoc/>
        void IEnumerator.Reset()
        {
            _idx = 0;
            _curr = default;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        private bool MoveNextRare()
        {
            _idx = _size + 1;
            _curr = default;
            return false;
        }
    }

    #endregion
}
