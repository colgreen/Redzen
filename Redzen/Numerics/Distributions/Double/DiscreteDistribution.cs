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
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Double;

/// <summary>
/// Represents a distribution over a discrete set of possible states.
/// </summary>
public sealed class DiscreteDistribution
{
    /// <summary>
    /// A singleton instance that represents the special case of a discrete distribution with a single possible outcome with probability 1.
    /// </summary>
    public static readonly DiscreteDistribution SingleOutcome = new(new double[] { 1.0 });

    readonly double[] _probArr;
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
    public DiscreteDistribution(double[] probArr)
    {
        NormaliseProbabilities(probArr);
        _probArr = probArr;

        // Assign labels.
        _labelArr = GC.AllocateUninitializedArray<int>(probArr.Length);
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
    public DiscreteDistribution(double[] probArr, int[] labelArr)
    {
        if(probArr.Length != labelArr.Length) throw new ArgumentException("Array lengths are not equal.");

        NormaliseProbabilities(probArr);
        _probArr = probArr;
        _labelArr = labelArr;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the array of probabilities.
    /// </summary>
    public double[] Probabilities => _probArr;

    /// <summary>
    /// Gets the array of outcome labels.
    /// </summary>
    public int[] Labels => _labelArr;

    #endregion

    #region Public Methods

    /// <summary>
    /// Remove the specified item from the current discrete distribution.
    /// </summary>
    /// <param name="labelId">The label ID of the item to remove from the discrete distribution.</param>
    /// <returns>A new instance of <see cref="DiscreteDistribution"/> that is a copy of the current distribution, with the specified discrete item removed.</returns>
    public DiscreteDistribution RemoveOutcome(int labelId)
    {
        // Find the item with specified label.
        int idx = 0;
        for(; idx < _labelArr.Length && _labelArr[idx] != labelId; idx++);

        if(idx >= _probArr.Length)
            throw new ArgumentException("Invalid labelId");

        double[] probArr = GC.AllocateUninitializedArray<double>(_probArr.Length - 1);
        int[] labels = GC.AllocateUninitializedArray<int>(_probArr.Length - 1);
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
        return new DiscreteDistribution(probArr, labels);
    }

    #endregion

    #region Public Static Methods

    /// <summary>
    /// Sample from the provided discrete probability distribution.
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="dist">The discrete distribution to sample from.</param>
    /// <returns>A sample from the discrete distribution.</returns>
    public static int Sample(IRandomSource rng, DiscreteDistribution dist)
    {
        double[] pArr = dist.Probabilities;

        // Obtain a random threshold value by sampling uniformly from interval [0,1).
        double thresh = rng.NextDouble();

        // ENHANCEMENT: Precalc running sum over pArr, and use binary search over pArr if its length is > 10 (or thereabouts).
        // Loop through the discrete probabilities, accumulating as we go and stopping once
        // the accumulator is greater than the random sample.
        double acc = 0.0;
        for(int i=0; i < pArr.Length; i++)
        {
            acc += pArr[i];
            if(acc > thresh)
                return dist.Labels[i];
        }

        // We might get here through floating point arithmetic rounding issues.
        // e.g. accumulator == throwValue.

        // Find a nearby non-zero probability to select.
        // Wrap around to start of array.
        for(int i=0; i < pArr.Length; i++)
        {
            if(pArr[i] != 0.0)
                return dist.Labels[i];
        }

        // If we get here then we have an array of zero probabilities.
        throw new InvalidOperationException("Invalid operation. No non-zero probabilities to select.");
    }

    /// <summary>
    /// Fill a span with samples from the provided discrete probability distribution.
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="dist">The discrete distribution to sample from.</param>
    /// <param name="span">The span to fill with samples.</param>
    public static void Sample(IRandomSource rng, DiscreteDistribution dist, Span<int> span)
    {
        for(int i=0; i < span.Length; i++)
            span[i] = Sample(rng, dist);
    }

    /// <summary>
    /// Sample from a binary/Bernoulli distribution with the given probability of sampling 'true'.
    /// </summary>
    /// <param name="rng">Random number generator.</param>
    /// <param name="probability">Probability of sampling boolean true.</param>
    /// <returns>A boolean random sample.</returns>
    public static bool SampleBernoulli(IRandomSource rng, double probability)
    {
        return rng.NextDouble() < probability;
    }

    /// <summary>
    /// Fill a span with samples from a binary/Bernoulli distribution with the given probability of sampling 'true'.
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="probability">Probability of sampling 'true'.</param>
    /// <param name="span">The span to fill with samples.</param>
    public static void SampleBernoulli(IRandomSource rng, double probability, Span<bool> span)
    {
        for(int i=0; i < span.Length; i++)
            span[i] = rng.NextDouble() < probability;
    }

    /// <summary>
    /// Take multiple samples from a set of possible outcomes with equal probability (i.e., a uniform discrete distribution)
    /// without replacement, (i.e., any given value will only occur once at most in the set of samples).
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="numberOfOutcomes">The number of possible outcomes per sample.</param>
    /// <param name="numberOfSamples">The number of samples to take.</param>
    /// <returns>A new integer array containing the samples.</returns>
    public static int[] SampleUniformWithoutReplacement(IRandomSource rng, int numberOfOutcomes, int numberOfSamples)
    {
        int[] sampleArr = GC.AllocateUninitializedArray<int>(numberOfSamples);
        SampleUniformWithoutReplacement(rng, numberOfOutcomes, sampleArr);
        return sampleArr;
    }

    /// <summary>
    /// Take multiple samples from a set of possible outcomes with equal probability, i.e. a uniform discrete distribution,
    /// without replacement, i.e. any given value will only occur once at most in the set of samples.
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="numberOfOutcomes">The number of possible outcomes per sample.</param>
    /// <param name="sampleSpan">A span to fill with samples.</param>
    public static void SampleUniformWithoutReplacement(IRandomSource rng, int numberOfOutcomes, Span<int> sampleSpan)
    {
        if(sampleSpan.Length > numberOfOutcomes)
            throw new ArgumentException("sampleArr length must be less then or equal to numberOfOutcomes.");

        // Create a temp span containing all possible choices from 0 to numberOfOutcomes-1.
        Span<int> tmpSpan = stackalloc int[numberOfOutcomes];
        for(int i=0; i < numberOfOutcomes; i++)
            tmpSpan[i] = i;

        // Perform a Fisher–Yates shuffle over tmpSpan, but only for the number of items required for
        // sampleSpan. I.e., we could complete the shuffle over the full length of tmpSpan, but we can
        // improve performance by terminating early, once we have all the samples we require.
        for(int i=0; i < sampleSpan.Length; i++)
        {
            // Select an index at random.
            int idx = rng.Next(i, numberOfOutcomes);

            // Swap elements i and idx.
            int tmp = tmpSpan[i];
            tmpSpan[i] = tmpSpan[idx];
            tmpSpan[idx] = tmp;
        }

        // Copy the samples into sampleSpan.
        tmpSpan.Slice(0, sampleSpan.Length).CopyTo(sampleSpan);
    }

    #endregion

    #region Private Static Methods

    private static void NormaliseProbabilities(Span<double> pSpan)
    {
        if(pSpan.Length == 0)
            throw new ArgumentException("Invalid probabilities span. Zero length span.", nameof(pSpan));

        if(!DoubleUtils.AllNonNegativeReal(pSpan))
            throw new ArgumentException("Invalid probabilities span. One or more elements are either negative, NaN, or Infinity..", nameof(pSpan));

        // Sum the elements of pSpan.
        double sum = MathSpan.Sum(pSpan);

        // Handle special case where all provided probabilities are at or near zero;
        // in this case we evenly assign probabilities across all choices.
        if(sum <= 0.000_001)
        {
            pSpan.Fill(1.0 / pSpan.Length);
            return;
        }

        // Test if probabilities are already normalised (within reasonable limits of precision for floating
        // point variables).
        if(Math.Abs(1.0 - sum) < 0.000_001)
            return;

        // Normalise the probabilities.
        MathSpan.Multiply(pSpan, 1.0 / sum);
    }

    #endregion
}
