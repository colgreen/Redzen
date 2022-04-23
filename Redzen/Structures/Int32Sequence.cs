// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace Redzen.Structures;

/// <summary>
/// Conveniently encapsulates a single Int32, which is incremented to produce a sequence of integers.
/// </summary>
public sealed class Int32Sequence
{
    int _next;

    #region Constructors

    /// <summary>
    /// Construct, setting the initial sequence value to zero.
    /// </summary>
    public Int32Sequence()
    {
        _next = 0;
    }

    /// <summary>
    /// Construct, setting the initial sequence value to the given value.
    /// </summary>
    /// <param name="next">The initial sequence value.</param>
    public Int32Sequence(int next)
    {
        _next = next;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Get the next ID without incrementing (peek the ID).
    /// </summary>
    public int Peek => _next;

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the next integer.
    /// </summary>
    /// <returns>The next integer in the sequence.</returns>
    /// <remarks>
    /// The sequence 'wraps around' to zero when int.MaxValue is reached.
    /// </remarks>
    public int Next()
    {
        if(_next == int.MaxValue)
            throw new InvalidOperationException("Last ID has been reached.");

        return _next++;
    }

    /// <summary>
    /// Resets the next value to zero.
    /// </summary>
    public void Reset()
    {
        _next = 0;
    }

    /// <summary>
    /// Sets the next value to a given value.
    /// </summary>
    /// <param name="next">The next value in the sequence.</param>
    public void Reset(int next)
    {
        _next = next;
    }

    #endregion
}
