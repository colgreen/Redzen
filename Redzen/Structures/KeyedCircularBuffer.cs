/* ***************************************************************************
 * This file is part of the Redzen code library.
 *
 * Copyright 2015-2022 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Redzen.Structures
{
    /// <summary>
    /// A generic circular buffer of key-value pairs. The values are retrievable by their
    /// key. Old key-value pairs are overwritten when the circular buffer runs out of
    /// empty elements to place items into; as this happens the internal dictionary that
    /// maintains the lookup ability is also updated to reflect only the items in the
    /// circular buffer.
    /// </summary>
    /// <typeparam name="K">Key type.</typeparam>
    /// <typeparam name="V">Value type.</typeparam>
    public sealed class KeyedCircularBuffer<K,V> : CircularBuffer<ValueTuple<K,V>>
        where K : notnull
    {
        readonly Dictionary<K,V> _dictionary;

        #region Constructor

        /// <summary>
        /// Constructs a circular buffer with the specified capacity.
        /// </summary>
        /// <param name="capacity">Circular buffer capacity.</param>
        public KeyedCircularBuffer(int capacity) : base(capacity)
        {
            _dictionary = new Dictionary<K,V>(capacity);
        }

        #endregion

        #region Public Methods [Circular Buffer]

        /// <summary>
        /// Clear the buffer.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            _dictionary.Clear();
        }

        /// <summary>
        /// Enqueue a new item.
        /// </summary>
        /// <param name="key">The key of the new item to enqueue.</param>
        /// <param name="value">The value of the new item to enqueue.</param>
        /// <remarks>
        /// Enqueuing a new item overwrites the oldest item in the buffer if the buffer is at maximum capacity.
        /// </remarks>
        public void Enqueue(in K key, in V value)
        {
            Enqueue(new ValueTuple<K,V>(key, value));
        }

        /// <summary>
        /// Enqueue a new item.
        /// </summary>
        /// <param name="item">The item (i.e., key-value pair) to enqueue.</param>
        /// <remarks>
        /// Enqueuing a new item overwrites the oldest item in the buffer if the buffer is at maximum capacity.
        /// </remarks>
        public override void Enqueue(in ValueTuple<K,V> item)
        {
            if(_headIdx == -1)
            {   // buffer is currently empty.
                _headIdx = _tailIdx = 0;
                _buff[0] = item;
                _dictionary.Add(item.Item1, item.Item2);
                return;
            }

            // Determine the index to write to.
            if(++_headIdx == _buff.Length)
            {   // Wrap around.
                _headIdx = 0;
            }

            if(_headIdx == _tailIdx)
            {   // Buffer overflow. Increment tailIdx and remove the overwritten item from the dictionary.
                _dictionary.Remove(_buff[_headIdx].Item1);
                if(++_tailIdx == _buff.Length)
                {   // Wrap around.
                    _tailIdx=0;
                }

                // Overwrite the existing item. And add the new one. (below)
            }

            _buff[_headIdx] = item;
            _dictionary.Add(item.Item1, item.Item2);
            return;
        }

        /// <summary>
        /// Removes the oldest item from the tail end of the buffer, and returns it.
        /// </summary>
        /// <returns>The dequeued item.</returns>
        /// <exception cref="InvalidOperationException">If the buffer is empty.</exception>
        public override ValueTuple<K,V> Dequeue()
        {
            ValueTuple<K,V> kvPair = base.Dequeue();
            _dictionary.Remove(kvPair.Item1);
            return kvPair;
        }

        /// <summary>
        /// Pops the most recently added item from the head of the buffer, and returns it.
        /// </summary>
        /// <returns>The popped item.</returns>
        /// <exception cref="InvalidOperationException">If the buffer is empty.</exception>
        public override ValueTuple<K,V> Pop()
        {
            ValueTuple<K,V> kvPair = base.Pop();
            _dictionary.Remove(kvPair.Item1);
            return kvPair;
        }

        #endregion

        #region Public Methods/Properties [Dictionary]

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">Item key.</param>
        /// <exception cref="KeyNotFoundException">If an item with the specified key does not exist..</exception>
        public V this[K key] => _dictionary[key];

        /// <summary>
        /// Determines whether the buffer contains an item with the specified key.
        /// </summary>
        /// <param name="key">Item key.</param>
        /// <returns>True if the buffer contains an item with the specified key; otherwise false.</returns>
        public bool ContainsKey(in K key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">Item key.</param>
        /// <param name="value">Returns the value associated with the key; or null if no such item exists.</param>
        /// <returns>True if an item with the specified key was found; otherwise false.</returns>
        public bool TryGetValue(
            in K key,
            [MaybeNullWhen(false)] out V value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        #endregion
    }
}
