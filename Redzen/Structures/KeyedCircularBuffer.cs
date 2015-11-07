/* ****************************************************************************
 * This file is part of the Redzen code library.
 *
 * Copyright 2015 Colin D. Green (colin.green1@gmail.com)
 *
 * This software is issued under the MIT License.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System.Collections.Generic;

namespace Redzen.Structures
{
    /// <summary>
    /// A generic circular buffer of KeyValuePairs. The values are retrievable by their
    /// key. Old key-value pairs are overwritten when the circular buffer runs out of
    /// empty elements to place items into, as this happens the internal dictionary that 
    /// maintains the lookup ability is also updated to reflect only the items in the 
    /// circular buffer.
    /// </summary>
    public class KeyedCircularBuffer<K,V> : CircularBuffer<KeyValuePair<K,V>>
    {
        readonly Dictionary<K,V> _dictionary;

        #region Constructor

        /// <summary>
        /// Constructs a circular buffer with the specified capacity.
        /// </summary>
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
        /// Enqueue a new item. This overwrites the oldest item in the buffer if the buffer
        /// has reached capacity.
        /// </summary>
        public void Enqueue(K key, V value)
        {
            Enqueue(new KeyValuePair<K,V>(key, value));
        }

        /// <summary>
        /// Enqueue a new item. This overwrites the oldest item in the buffer if the buffer
        /// has reached capacity.
        /// </summary>
        public override void Enqueue(KeyValuePair<K, V> item)
        {
            if(_headIdx == -1)
            {   // buffer is currently empty.
                _headIdx = _tailIdx = 0;
                _buff[0] = item;
                _dictionary.Add(item.Key, item.Value);
                return;
            }

            // Determine the index to write to.
            if(++_headIdx == _buff.Length)
            {   // Wrap around.
                _headIdx = 0;
            }

            if(_headIdx == _tailIdx)
            {   // Buffer overflow. Increment tailIdx and remove the overwritten item from the dictionary.
                _dictionary.Remove(_buff[_headIdx].Key);
                if(++_tailIdx == _buff.Length) 
                {   // Wrap around.
                    _tailIdx=0;
                }

                // Overwrite the existing item. And add the new one. (below)
            }

            _buff[_headIdx] = item;
            _dictionary.Add(item.Key, item.Value);
            return;
        }

        /// <summary>
        /// Remove the oldest item from the back end of the buffer and return it.
        /// </summary>
        public override KeyValuePair<K,V> Dequeue()
        {
            KeyValuePair<K,V> kvPair = base.Dequeue();
            _dictionary.Remove(kvPair.Key);
            return kvPair;
        }

        /// <summary>
        /// Pop the most recently added item from the front end of the buffer and return it.
        /// </summary>
        public override KeyValuePair<K,V> Pop()
        {
            KeyValuePair<K,V> kvPair = base.Pop();
            _dictionary.Remove(kvPair.Key);
            return kvPair;
        }

        #endregion

        #region Public Methods/Properties [Dictionary]
        
        /// <summary>
        /// Gets the value associated with the specified key. If the specified key is not found,
        /// a get operation throws a KeyNotFoundException.
        /// </summary>
        public V this[K key]
        {
            get { return _dictionary[key]; }
        }
 
        /// <summary>
        /// Determines whether the KeyedCircularBuffer contains the specified key.
        /// </summary>
        public bool ContainsKey(K key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key. 
        /// </summary>
        public bool TryGetValue(K key, out V value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        #endregion
    }
}
