// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace Redzen.Structures.Compact;

/// <summary>
/// A compact list of sequential integer values.
///
/// Each integer is represented by a single bit in a bitmap. The bitmap is broken into a series of
/// bitmap chunks, each of which has a base value which is the integer value represented by the first bit
/// in the chunk.
///
/// This class is useful when caching the results of a search over a list. The search results can be represented
/// as a list on indexes into the list, the resulting integer list can then be compactly stored using an instance of this
/// class.
///
/// E.g. 50,000 results would normally require 50,000 * 4 = 200,000 bytes.
/// This class requires 50000/8 = 6,250 bytes for a contiguous list of values, and increasing memory requirements
/// for non-contiguous lists.
/// </summary>
public sealed class CompactIntegerList : IEnumerable<int>
{
    /// <summary>
    /// A singleton that represents an empty <see cref="CompactIntegerList"/> list.
    /// </summary>
    public static readonly CompactIntegerList EmptyList = new(null);

    readonly BitmapChunk[]? _bitmapChunks;

    /// <summary>
    /// The number of integers in the list. Calculating this requires a walk of the bitmap chunks, thus we
    /// just store the value directly.
    /// </summary>
    readonly int _count;

    #region Constructor

    /// <summary>
    /// Construct a CompactIntegerList from the provided list of integers.
    /// </summary>
    /// <param name="intList">A collection of integers to populate with.</param>
    public CompactIntegerList(IList<int>? intList)
    {
        if(intList is null || intList.Count == 0)
        {
            _count = 0;
            _bitmapChunks = null;
        }
        else
        {
            _count = intList.Count;
            _bitmapChunks = BuildChunks(intList, 1024);
        }
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Returns the number of integers in the list.
    /// </summary>
    public int Count => _count;

    #endregion

    #region IEnumerable<int> Members

    /// <summary>
    /// Gets an enumerator over the compacted integer list.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{T}"/> that can be used to iterate over the list.</returns>
    public IEnumerator<int> GetEnumerator()
    {
        if(_bitmapChunks is null)
            yield break;

        // Loop over each chunk.
        for(int i=0; i < _bitmapChunks.Length; i++)
        {
            // loop over bits in chunk and yield integer values represented by set bits.
            BitmapChunk chunk = _bitmapChunks[i];

            // For speed we first test for non-zero uints within the chunk's bitmap array.
            for(int j=0; j < chunk._bitmap.Length; j++)
            {
                // Skip uints with no bits set.
                if(chunk._bitmap[j] == 0u)
                    continue;

                // Loop over bits in the current uint and yield the set bits.
                uint block = chunk._bitmap[j];
                for(int k=0, l = chunk._baseValue + (j<<5); k<32; k++, l++)
                {
                    if((block & 0x1) != 0)
                        yield return l;

                    block >>= 1;
                }
            }
        }
    }

    /// <summary>
    /// Gets a reverse enumerator over the compacted integer list.
    /// </summary>
    /// <returns>An <see cref="IEnumerator{T}"/> that can be used to iterate over the list.</returns>
    public IEnumerator<int> GetReverseEnumerator()
    {
        if(_bitmapChunks is null)
            yield break;

        // Loop over each chunk.
        for(int i = _bitmapChunks.Length-1; i > -1; i--)
        {
            // loop over bits in chunk and yield integer values represented by set bits.
            BitmapChunk chunk = _bitmapChunks[i];

            // For speed we first test for non-zero uints within the chunk's bitmap array.
            for(int j = chunk._bitmap.Length-1; j > -1; j--)
            {
                // Skip uints with no bits set.
                if(chunk._bitmap[j] == 0u)
                    continue;

                // Loop over bits in the current uint and yield the set bits.
                uint block = chunk._bitmap[j];
                for(int k=0, l = chunk._baseValue + (j<<5) + 31; k<32; k++, l--)
                {
                    if((block & 0x80000000) != 0)
                        yield return l;

                    block <<= 1;
                }
            }
        }
    }

    #endregion

    #region IEnumerable Members

    /// <summary>
    /// Gets an enumerator over the compacted integer list.
    /// </summary>
    /// <returns>
    /// An <see cref="System.Collections.IEnumerator"/> that can be used to iterate through the list items.
    /// </returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    #region Private Static Methods

    /// <summary>
    /// Builds the bitmap chunks given the integer list to compact and the bitmap chunk size
    /// (number of bits in each chunk).
    /// </summary>
    /// <param name="intList">A collection of integers to populate with.</param>
    /// <param name="chunkSize">The chunk size.</param>
    /// <returns>An array of <see cref="BitmapChunk"/> containing the <paramref name="intList"/>.</returns>
    private static BitmapChunk[] BuildChunks(IList<int> intList, int chunkSize)
    {
        List<BitmapChunk> chunkList = new(20);

        // Index into indexList.
        int idx = 0;
        int count = intList.Count;

        // Build chunks until we exhaust intList.
        while(idx < count)
        {
            int val = intList[idx];
            BitmapChunk chunk = new(val, chunkSize);
            chunkList.Add(chunk);
            int chunkBound = val + chunkSize;

            // Iterate through all values that are represented by the current chunk.
            do
            {
                chunk.SetBitForValue(val);
            }
            while(++idx < count &&  (val = intList[idx]) < chunkBound);
        }

        // Return the chunks as an array.
        return chunkList.ToArray();
    }

    #endregion

    #region Inner Class

    readonly struct BitmapChunk
    {
        public readonly int _baseValue;
        public readonly uint[] _bitmap;

        #region Constructor

        /// <summary>
        /// Constructs with the specified integer base value for the chunk and the number
        /// of bits allocated in _bitmap.
        /// </summary>
        /// <param name="baseValue">The base value for the chunk.</param>
        /// <param name="chunkSizeInBits">The chunk size in bits.</param>
        public BitmapChunk(int baseValue, int chunkSizeInBits)
        {
            _baseValue = baseValue;

            // Uses fast divide by 32.
            _bitmap = new uint[chunkSizeInBits>>5];
        }

        #endregion

        #region Public Methods

        public void SetBitForValue(int val)
        {
            SetBit(val - _baseValue);
        }

        public void SetBit(int bitIdx)
        {
            // Determine index of uint that holds our bit.
            // Fast integer divide by 32. (32 bits in a uint)
            int bitmapIdx = bitIdx >> 5;

            // Determine index of our bit in its home uint/slot.
            int slotBitIdx = bitIdx - (bitmapIdx<<5);

            // Set the bit.
            _bitmap[bitmapIdx] |= 1u << slotBitIdx;
        }

        #endregion
    }

    #endregion
}
