using System;
using System.Runtime.CompilerServices;

namespace Redzen.Structures
{
    /// <summary>
    /// A leaner faster alternative to System.Collections.BitArray.
    /// </summary>
    public sealed class BoolArray
    {
        readonly int[] _dataArr;
        readonly int _bitLength;

        #region Constructor

        /// <summary>
        /// Construct with the given length in bits, and default value for all bits.
        /// </summary>
        /// <param name="length">Array length in bits.</param>
        public BoolArray(int length)
        {
            if (length < 0) { throw new ArgumentOutOfRangeException(nameof(length)); }

            _dataArr = new int[GetDataArrayLength(length)];
            _bitLength = length;
        }

        /// <summary>
        /// Construct with the given length in bits, and default value for all bits.
        /// </summary>
        /// <param name="length">Array length in bits.</param>
        /// <param name="defaultValue">Default value for all bits.</param>
        public BoolArray(int length, bool defaultValue)
        {
            if (length < 0) { throw new ArgumentOutOfRangeException(nameof(length)); }

            _dataArr = new int[GetDataArrayLength(length)];
            _bitLength = length;

            if(defaultValue)
            {
                int fillValue = unchecked(((int)0xffffffff));
                for (int i = 0; i < _dataArr.Length; i++) {
                    _dataArr[i] = fillValue;
                }
            }
        }

        #endregion

        #region Indexer

        /// <summary>
        /// Gets or sets the bit at the specified index.
        /// </summary>
        /// <param name="index">The bit index.</param>
        /// <returns>The value of the bit at the specified index.</returns>
        public bool this[int index]
        {
            get
            {
                if (index < 0 || index >= _bitLength) {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                CalcIndexes(index, out int byteIdx, out int bitIdx);
                return (_dataArr[byteIdx] & (1 << bitIdx)) != 0;
            }
            set
            {
                if (index < 0 || index >= _bitLength) {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                CalcIndexes(index, out int byteIdx, out int bitIdx);

                if (value) {
                    _dataArr[byteIdx] |= (1 << bitIdx);
                } 
                else {
                    _dataArr[byteIdx] &= ~(1 << bitIdx);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reset all bits in the array to the specified default value.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        public void Reset(bool defaultValue)
        {
            if(defaultValue)
            {
                int fillValue = unchecked(((int)0xffffffff));
                for (int i = 0; i < _dataArr.Length; i++) {
                    _dataArr[i] = fillValue;
                }
            }
            else
            {
                Array.Clear(_dataArr, 0, _dataArr.Length);
            }
        }

        #endregion

        #region Private Static Methods

        private static int GetDataArrayLength(int bitLen)
        {
            return bitLen > 0 ? (((bitLen - 1) / 32) + 1) : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CalcIndexes(int idx, out int byteIdx, out int bitIdx)
        {
            // TODO: Review when https://github.com/dotnet/coreclr/issues/3439 is resolved.
            // This is a faster alternative to Math.DivRem(); faster because the divisor is a constant,
            // and RyuJIT can produce optimizations for this.
            byteIdx = idx / 32;
            bitIdx = idx - (byteIdx * 32);
        }

        #endregion
    }
}
