/* ***************************************************************************
 * This file is part of the Redzen code library.
 * 
 * Copyright 2015-2019 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Diagnostics;

namespace Redzen.Collections
{
    /// <summary>
    /// A stack of int32 values.
    /// A simpler alternative to Stack<int> that provides additional Poke() and TryPoke() methods.
    /// </summary>
    public sealed class IntStack
    {
        #region Fields

        const int __defaultCapacity = 4;
        int[] _array; 
        int _size; 
        
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public IntStack()
        {
            _array = new int[__defaultCapacity];
        }

        /// <summary>
        /// Construct with the given initial capacity.
        /// </summary>
        /// <param name="capacity">Initial capacity.</param>
        public IntStack(int capacity)
        {
            if (capacity < 0) {
                throw new ArgumentOutOfRangeException("Capacity must be non-negative.");
            }
            _array = new int[capacity];
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of items on the stack.
        /// </summary>
        public int Count => _size;

        #endregion

        #region Public Methods

        /// <summary>
        /// Pushes a value onto the top of the stack.
        /// </summary>
        /// <param name="val">The value to push.</param>
        public void Push(int val)
        {
            if (_size == _array.Length) {
                Array.Resize(ref _array, (_array.Length == 0) ? __defaultCapacity : 2 * _array.Length);
            }
            _array[_size++] = val;
        }

        /// <summary>
        /// Pop a value from the top of the stack.
        /// </summary>
        /// <returns>The popped value from the top of the stack.</returns>
        public int Pop()
        {
            if(0 == _size) {
                ThrowForEmptyStack();
            }
            
            return _array[--_size];
        }

        /// <summary>
        /// Attempt to pop a value from the top of the stack.
        /// </summary>
        /// <param name="result">The value from the top of the stack.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool TryPop(out int result)
        {
            if(0 == _size)
            {
                result = default;
                return false;
            }

            result = _array[--_size];
            return true;
        }

        /// <summary>
        /// Returns the value at the top of the stack without popping it.
        /// </summary>
        /// <returns>The value at the top of the stack.</returns>
        public int Peek()
        {
            if(0 == _size) {
                ThrowForEmptyStack();
            }
            return _array[_size - 1];
        }

        /// <summary>
        /// Returns the value at the top of the stack without popping it, if the stack is not empty.
        /// </summary>
        /// <param name="result">The value at the top of the stack.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool TryPeek(out int result)
        {
            if(0 == _size) 
            {
                result = default;
                return false;
            }
            result = _array[_size - 1];
            return true;
        }

        /// <summary>
        /// Sets/overwrites he value at the top of the stack.
        /// </summary>
        /// <param name="val">The value to set.</param>
        public void Poke(int val)
        {
            if(0 == _size) {
                ThrowForEmptyStack();
            }
            _array[_size - 1] = val;
        }

        /// <summary>
        /// Sets/overwrites he value at the top of the stack, if the stack is not empty.
        /// </summary>
        /// <param name="val">The value to set.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool TryPoke(int val)
        {
            if(0 == _size) {
                return false;
            }
            _array[_size - 1] = val;
            return true;
        }

        /// <summary>
        /// Removes all items from the stack; i.e. moves the stack pointer to the bottom of the stack.
        /// </summary>
        public void Clear()
        {
            // Note. For efficiency the elements of _array are not reset.
            _size = 0;   
        }

        #endregion

        #region Private Methods

        private void ThrowForEmptyStack()
        {
            Debug.Assert(_size == 0);
            throw new InvalidOperationException("Attempt to obtain an item from an empty stack.");
        }

        #endregion
    }
}
