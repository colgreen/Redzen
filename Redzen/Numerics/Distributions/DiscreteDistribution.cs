// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using Redzen.Random;

namespace Redzen.Numerics.Distributions;

/// <summary>
/// Represents a distribution over a discrete set of possible states.
/// </summary>
/// <typeparam name="T">Floating point data type.</typeparam>
public sealed class DiscreteDistribution<T>
    where T : struct, IBinaryFloatingPointIeee754<T>
{
    /// <summary>
    /// A singleton instance that represents the special case of a discrete distribution with a single possible outcome with probability 1.
    /// </summary>
    public static readonly DiscreteDistribution<T> SingleOutcome = new([T.One]);

    readonly T[] _probArr;
    readonly int[] _labelArr;

    #region Constructors

    /// <summary>
    /// Construct with the provided distribution probabilities.
    /// </summary>
    /// <param name="probArr">An array of discrete distribution probabilities.</param>
    /// <remarks>
    /// The provided probabilities do not have to sum 1.0, as they will be normalised during construction.
    /// There is no check for negative values, therefore behaviour is undefined if one or more negative probabilities are supplied.
    /// </remarks>
    public DiscreteDistribution(T[] probArr)
    {
        NormaliseProbabilities(probArr);
        _probArr = probArr;

        // Assign labels.
        _labelArr = new int[probArr.Length];
        for(int i=0; i < _probArr.Length; i++)
            _labelArr[i] = i;
    }

    /// <summary>
    /// Construct with the provided distribution probabilities and associated labels.
    /// </summary>
    /// <param name="probArr">An array of discrete distribution probabilities.</param>
    /// <param name="labelArr">An array of integer labels to assign to each discrete item.</param>
    /// <remarks>
    /// The provided probabilities do not have to sum 1.0, as they will be normalised during construction.
    /// There is no check for negative values, therefore behaviour is undefined if one or more negative probabilities are supplied.
    /// </remarks>
    public DiscreteDistribution(T[] probArr, int[] labelArr)
    {
        if(probArr.Length != labelArr.Length) throw new ArgumentException("Array lengths are not equal.");

        NormaliseProbabilities(probArr);
        _probArr = probArr;
        _labelArr = labelArr;
    }

    #endregion

    #region Public Methods / Properties

    /// <summary>
    /// Gets the array of probabilities.
    /// </summary>
    public T[] Probabilities => _probArr;

    /// <summary>
    /// Gets the array of outcome labels.
    /// </summary>
    public int[] Labels => _labelArr;

    /// <summary>
    /// Sample from the provided discrete probability distribution.
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <returns>A sample from the discrete distribution.</returns>
    public int Sample(IRandomSource rng)
    {
        T[] pArr =_probArr;

        // Obtain a random threshold value by sampling uniformly from interval [0,1).
        T thresh = rng.NextUnitInterval<T>();

        // ENHANCEMENT: Precalc running sum over pArr, and use binary search over pArr if its length is > 10 (or thereabouts).
        // Loop through the discrete probabilities, accumulating as we go and stopping once
        // the accumulator is greater than the random sample.
        T acc = T.Zero;
        for(int i = 0; i < pArr.Length; i++)
        {
            acc += pArr[i];
            if(acc > thresh)
                return _labelArr[i];
        }

        // We might get here through floating point arithmetic rounding issues.
        // e.g. accumulator == throwValue.

        // Find a nearby non-zero probability to select.
        // Wrap around to start of array.
        for(int i = 0; i < pArr.Length; i++)
        {
            if(pArr[i] != T.Zero)
                return _labelArr[i];
        }

        // If we get here then we have an array of zero probabilities.
        throw new InvalidOperationException("Invalid operation. No non-zero probabilities to select.");
    }

    /// <summary>
    /// Fill a span with samples from the discrete probability distribution.
    /// </summary>
    /// <param name="span">The span to fill with samples.</param>
    /// <param name="rng">Random source.</param>
    public void Sample(
        Span<int> span,
        IRandomSource rng)
    {
        for(int i = 0; i < span.Length; i++)
            span[i] = Sample(rng);
    }

    /// <summary>
    /// Remove the specified item from the current discrete distribution.
    /// </summary>
    /// <param name="labelId">The label ID of the item to remove from the discrete distribution.</param>
    /// <returns>A new instance of <see cref="DiscreteDistribution{T}"/> that is a copy of the current distribution, with the specified discrete item removed.</returns>
    public DiscreteDistribution<T> RemoveOutcome(int labelId)
    {
        // Find the item with specified label.
        int idx = 0;
        for(; idx < _labelArr.Length && _labelArr[idx] != labelId; idx++);

        if(idx >= _probArr.Length)
            throw new ArgumentException("Invalid labelId");

        T[] probArr = new T[_probArr.Length - 1];
        int[] labels = new int[_probArr.Length - 1];
        for(int i=0; i < idx; i++)
        {
            probArr[i] = _probArr[i];
            labels[i] = _labelArr[i];
        }

        for(int i = idx+1, j = idx; i < _probArr.Length; i++, j++)
        {
            probArr[j] = _probArr[i];
            labels[j] = _labelArr[i];
        }

        // Note. The probabilities are not normalised here, however the constructor will normalise them.
        return new DiscreteDistribution<T>(probArr, labels);
    }

    #endregion

    #region Private Static Methods

    private static readonly T __nearZeroThreshold = T.CreateChecked(0.000_001);

    private static void NormaliseProbabilities(Span<T> pSpan)
    {
        if(pSpan.Length == 0)
            throw new ArgumentException("Invalid probabilities span. Zero length span.", nameof(pSpan));

        if(!FloatUtils.AllNonNegativeReal<T>(pSpan))
            throw new ArgumentException("Invalid probabilities span. One or more elements are either negative, NaN, or Infinity..", nameof(pSpan));

        // Sum the elements of pSpan.
        T sum = MathSpan.Sum<T>(pSpan);

        // Handle special case where all provided probabilities are at or near zero;
        // in this case we evenly assign probabilities across all choices.
        if(sum <= __nearZeroThreshold)
        {
            pSpan.Fill(T.One / T.CreateChecked(pSpan.Length));
            return;
        }

        // Test if probabilities are already normalised (within reasonable limits of precision for floating
        // point variables).
        if(T.Abs(T.One - sum) < __nearZeroThreshold)
            return;

        // Normalise the probabilities.
        MathSpan.Multiply(pSpan, T.One / sum);
    }

    #endregion
}
