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

namespace Redzen.Structures
{
    /// <summary>
    /// A circular buffer of double precision floating point values.
    ///
    /// A circular buffer must be assigned a capacity at construction time.
    ///
    /// Values can be enqueued indefinitely, but when the buffer's capacity is reached the oldest values
    /// in it are overwritten, thus the buffer is best thought of as a circular array or buffer.
    ///
    /// In addition to normal circular buffer behaviour, this class has a 'sum' variable that
    /// maintains the sum of all values currently in the buffer. Therefore when the buffer
    /// reaches capacity and new values overwrite old ones, the sum is reduced by the value being
    /// overwritten and increased by the new value. This allows us to cheaply (in computational terms)
    /// maintain a sum and mean for all values in the buffer.
    ///
    /// Note that this class isn't made generic because of the lack of operator constraints required
    /// to maintain the sum over current buffer items.
    /// </summary>
    public sealed class CircularBufferWithStats
    {
        /// <summary>
        /// Internal array that stores the circular buffer's values.
        /// </summary>
        readonly double[] _buff;

        /// <summary>
        /// The index of the previously enqueued item. -1 if buffer is empty.
        /// </summary>
        int _headIdx;

        /// <summary>
        /// The index of the next item to be dequeued. -1 if buffer is empty.
        /// </summary>
        int _tailIdx;

        /// <summary>
        /// The sum of all current values in the buffer.
        /// </summary>
        double _sum = 0.0;

        #region Constructors

        /// <summary>
        /// Constructs a circular buffer with the specified capacity.
        /// </summary>
        /// <param name="capacity">Circular buffer capacity.</param>
        public CircularBufferWithStats(int capacity)
        {
            if(capacity < 2) { throw new ArgumentException("Must be 2 or higher.", nameof(capacity)); }

            _buff = new double[capacity];
            _headIdx = _tailIdx = -1;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of values in the buffer. Returns the buffer's capacity if it is full.
        /// </summary>
        public int Length
        {
            get
            {
                if(_headIdx == -1)
                    return 0;

                if(_headIdx > _tailIdx)
                    return (_headIdx-_tailIdx)+1;

                if(_tailIdx > _headIdx)
                    return (_buff.Length - _tailIdx) + _headIdx + 1;

                return 1;
            }
        }

        /// <summary>
        /// Gets the sum of all values on in the buffer.
        /// </summary>
        public double Sum => _sum;

        /// <summary>
        /// Gets the arithmetic mean of all values in the buffer.
        /// </summary>
        public double Mean
        {
            get
            {
                if(_headIdx == -1)
                    return 0.0;

                return _sum / Length;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clear the buffer and reset the sum.
        /// </summary>
        public void Clear()
        {
            _headIdx = _tailIdx = -1;
            _sum = 0.0;
        }

        /// <summary>
        /// Enqueue a new item.
        /// </summary>
        /// <param name="item">The item to enqueue.</param>
        /// <remarks>
        /// Enqueuing a new item overwrites the oldest item in the buffer if the buffer is at maximum capacity.
        /// </remarks>
        public void Enqueue(double item)
        {
            if(_headIdx == -1)
            {
                // buffer is currently empty.
                _headIdx = _tailIdx = 0;
                _buff[0] = item;
                _sum = item;
                return;
            }

            // Determine the index to write to.
            if(++_headIdx == _buff.Length)
            {
                // Wrap around.
                _headIdx = 0;
            }

            if(_headIdx == _tailIdx)
            {
                // Buffer overflow. Increment tailIdx.
                _sum -= _buff[_headIdx];
                if(++_tailIdx == _buff.Length)
                {
                    // Wrap around.
                    _tailIdx = 0;
                }
            }

            _buff[_headIdx] = item;

            // If the buffer head has just wrapped around, then we elect to use this as a convenient time/event for
            // recalculating the sum based on the current set of items in the buffer. This period recalc avoids any
            // potential accumulated drift in the _sum variable over time as items are added and removed from the
            // buffer, i.e. due to the inherent inexactness of floating point arithmetic.
            if(_headIdx != 0)
            {
                // Maintain the running sum.
                _sum += item;
            }
            else
            {
                // Wrap-around event; recalc the sum based on current buffer items.
                _sum = item;

                for(int i = _tailIdx; i < _buff.Length; i++)
                    _sum += _buff[i];
            }

            return;
        }

        /// <summary>
        /// Removes the oldest item from the tail end of the buffer, and returns it.
        /// </summary>
        /// <returns>The dequeued item.</returns>
        /// <exception cref="InvalidOperationException">If the buffer is empty.</exception>
        public double Dequeue()
        {
            // Test for empty buffer.
            if(_headIdx == -1) { throw new InvalidOperationException("buffer is empty."); }

            double d = _buff[_tailIdx];
            _sum -= d;

            if(_tailIdx == _headIdx)
            {
                // Reset the head and tail indexes.
                _headIdx = _tailIdx = -1;

                // Reset sum, as rounding errors may cause its value to drift.
                _sum = 0.0;
                return d;
            }

            if(++_tailIdx == _buff.Length)
            {
                // Wrap around.
                _tailIdx = 0;
            }

            return d;
        }

        /// <summary>
        /// Pops the most recently added item from the head of the buffer, and returns it.
        /// </summary>
        /// <returns>The popped item.</returns>
        /// <exception cref="InvalidOperationException">If the buffer is empty.</exception>
        public double Pop()
        {
            // Test for empty buffer.
            if(_headIdx == -1) { throw new InvalidOperationException("buffer is empty."); }

            double d = _buff[_headIdx];
            _sum -= d;

            if(_tailIdx == _headIdx)
            {
                // The buffer is now empty.
                // Reset the head and tail indexes.
                _headIdx = _tailIdx = -1;

                // Reset sum, as rounding errors may cause its value to drift.
                _sum = 0.0;
                return d;
            }

            if(--_headIdx == -1)
            {
                // Wrap around.
                _headIdx = _buff.Length - 1;
            }

            return d;
        }

        #endregion
    }
}
