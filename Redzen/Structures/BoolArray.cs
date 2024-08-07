﻿// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Runtime.CompilerServices;

namespace Redzen.Structures;

#pragma warning disable CA2201 // Do not raise reserved exception types

/// <summary>
/// A leaner faster alternative to System.Collections.BitArray.
/// </summary>
/// <remarks>
/// The underlying storage is an array of Int32.
///
/// Indexed access to elements is allowed for all bits in the underlying Int32 array, thus
/// the array length is always a multiple of 32. Doing this eliminates the need for some array index
/// bounds checks, thus simplifying the implementation and improving performance a little. This is
/// the main way in which this class differs from BitArray class in the dotnet framework.
/// </remarks>
public sealed class BoolArray
{
    readonly int[] _dataArr;

    #region Constructor

    /// <summary>
    /// Construct with the given minimum length in bits, and default value for all bits.
    /// </summary>
    /// <param name="minLength">Minimum array length in bits, i.e. the array will be allocated with at least this many elements.</param>
    /// <remarks>The actual length will be the smallest multiple of 32 that is greater than or equal to minLength.</remarks>
    public BoolArray(int minLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minLength);

        _dataArr = new int[GetDataArrayLength(minLength)];
    }

    /// <summary>
    /// Construct with the given length in bits, and default value for all bits.
    /// </summary>
    /// <param name="minLength">Minimum array length in bits, i.e. the array will be allocated with at least this many elements.</param>
    /// <param name="defaultValue">Default value for all bits.</param>
    public BoolArray(int minLength, bool defaultValue)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minLength);

        _dataArr = new int[GetDataArrayLength(minLength)];

        if(defaultValue)
        {
            int fillValue = unchecked((int)0xffffffff);
            for(int i = 0; i < _dataArr.Length; i++)
                _dataArr[i] = fillValue;
        }
    }

    /// <summary>
    /// Construct with the given data array.
    /// </summary>
    /// <param name="dataArray">The data array.</param>
    public BoolArray(int[] dataArray)
    {
        _dataArr = dataArray;
    }

    #endregion

    #region Public Methods / Properties

    /// <summary>
    /// Gets the array length in bits.
    /// </summary>
    public int Length
    {
        get => _dataArr.Length * 32;
    }

    /// <summary>
    /// Gets or sets the bit at the specified index.
    /// </summary>
    /// <param name="index">The bit index.</param>
    /// <returns>The value of the bit at the specified index.</returns>
    public bool this[int index]
    {
        get
        {
            if(index < 0)
                throw new IndexOutOfRangeException();

            CalcIndexes(index, out int byteIdx, out int bitIdx);
            return (_dataArr[byteIdx] & (1 << bitIdx)) != 0;
        }

        set
        {
            if(index < 0)
                throw new IndexOutOfRangeException();

            CalcIndexes(index, out int byteIdx, out int bitIdx);

            if(value)
                _dataArr[byteIdx] |= (1 << bitIdx);
            else
                _dataArr[byteIdx] &= ~(1 << bitIdx);
        }
    }

    /// <summary>
    /// Reset all bits in the array to the specified default value.
    /// </summary>
    /// <param name="defaultValue">The default value.</param>
    public void Reset(bool defaultValue)
    {
        if(defaultValue)
        {
            int fillValue = unchecked((int)0xffff_ffff);
            for(int i = 0; i < _dataArr.Length; i++)
                _dataArr[i] = fillValue;
        }
        else
        {
            Array.Clear(_dataArr, 0, _dataArr.Length);
        }
    }

    /// <summary>
    /// Gets the underlying data array.
    /// </summary>
    /// <returns>The underlying Int32 array.</returns>
    public int[] GetDataArray()
    {
        return _dataArr;
    }

    #endregion

    #region Private Static Methods

    private static int GetDataArrayLength(int bitLength)
    {
        return bitLength > 0 ? (((bitLength - 1) / 32) + 1) : 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CalcIndexes(int idx, out int byteIdx, out int bitIdx)
    {
        // TODO: Review when https://github.com/dotnet/runtime/issues/5213 is resolved.
        // This is a faster alternative to Math.DivRem(); faster because the divisor is a constant,
        // and RyuJIT can produce optimizations for this.
        byteIdx = idx / 32;
        bitIdx = idx - (byteIdx * 32);
    }

    #endregion
}
