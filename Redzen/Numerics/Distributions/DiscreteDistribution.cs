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
using Redzen.Random;

namespace Redzen.Numerics.Distributions
{
    // TODO: Unit tests.

    /// <summary>
    /// Represents a distribution over a discrete set of possible states.
    /// </summary>
    public sealed class DiscreteDistribution
    {
        const double __MaxFloatError = 0.000001;
        readonly double[] _probArr;
        readonly int[] _labelArr;

        #region Constructors

        /// <summary>
        /// Construct with the provided distribution probabilities.
        /// </summary>
        /// <remarks>
        /// The provided probabilities do not have to sum 1.0 as they will be normalised during construction.
        /// </remarks>
        public DiscreteDistribution(double[] probArr)
        {
            NormaliseProbabilities(probArr);
            _probArr = probArr;

            // Assign labels.
            _labelArr = new int[probArr.Length];
            for(int i=0; i<_probArr.Length; i++) {
                _labelArr[i] = i;
            }
        }

        /// <summary>
        /// Construct with the provided distribution probabilities and associated labels.
        /// </summary>
        /// <remarks>
        /// The provided probabilities do not have to sum 1.0 as they will be normalised during construction.
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
        public double[] Probabilities
        {
            get { return _probArr; }
        }

        /// <summary>
        /// Gets the array of outcome labels.
        /// </summary>
        public int[] Labels
        {
            get { return _labelArr; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Remove the specified outcome from the set of probabilities and return as a new DiscreteDistribution object.
        /// </summary>
        public DiscreteDistribution RemoveOutcome(int labelId)
        {
            // Find the item with specified label.
            int idx = 0;
            for(; idx < _labelArr.Length && _labelArr[idx] != labelId; idx++);

            if(idx >= _probArr.Length) {
                throw new ArgumentException("Invalid labelId");
            }

            double[] probArr = new double[_probArr.Length-1];
            int[] labels = new int[_probArr.Length-1];
            for(int i=0; i < idx; i++)
            {
                probArr[i] = _probArr[i];
                labels[i] = _labelArr[i];
            }

            for(int i=idx+1, j=idx; i < _probArr.Length; i++, j++)
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
        public static int Sample(IRandomSource rng, DiscreteDistribution dist)
        {
            double[] pArr = dist.Probabilities;

            // Obtain a random threshold value by sampling uniformly from interval [0,1).
            double thresh = rng.NextDouble();

            // Loop through the discrete probabilities, accumulating as we go and stopping once 
            // the accumulator is greater than the random sample.
            double acc = 0.0;
            for(int i=0; i < pArr.Length; i++)
            {
                acc += pArr[i];
                if(acc > thresh) {
                    return dist.Labels[i];
                }
            }

            // We might get here through floating point arithmetic rounding issues. 
            // e.g. accumulator == throwValue. 

            // Find a nearby non-zero probability to select.
            // Wrap around to start of array.
            for(int i=0; i < pArr.Length; i++)
            {
                if(0.0 != pArr[i]) {
                    return dist.Labels[i];
                }
            }

            // If we get here then we have an array of zero probabilities.
            throw new InvalidOperationException("Invalid operation. No non-zero probabilities to select.");
        }

        /// <summary>
        /// Fill an array with samples from the provided discrete probability distribution.
        /// </summary>
        /// <param name="rng">Random source.</param>
        /// <param name="dist">The discrete distribution to sample from.</param>
        /// <param name="buf">The array to fill with samples.</param>
        public static void Sample(IRandomSource rng, DiscreteDistribution dist, int[] buf)
        {
            for(int i=0; i < buf.Length; i++) {
                buf[i] = Sample(rng, dist);
            }
        }

        /// <summary>
        /// Sample from a binary/Bernoulli distribution with the specified boolean true probability.
        /// </summary>
        /// <param name="probability">Probability of sampling boolean true.</param>
        /// <param name="rng">Random number generator.</param>
        public static bool SampleBernoulli(IRandomSource rng, double probability)
        {
            return rng.NextDouble() < probability;
        }

        /// <summary>
        /// Fill an array with samples from a binary/Bernoulli distribution with the specified boolean true probability.
        /// </summary>
        /// <param name="rng">Random source.</param>
        /// <param name="probability">Probability of sampling boolean true.</param>
        /// <param name="buf">The array to fill with samples.</param>
        public static void SampleBernoulli(IRandomSource rng, double probability, bool[] buf)
        {
            for(int i=0; i < buf.Length; i++) {
                buf[i] = (rng.NextDouble() < probability);
            }
        }

        /// <summary>
        /// Take multiple samples from a set of possible outcomes with equal probability, i.e. a uniform discrete distribution,
        /// without replacement, i.e. any given value will only occur once at most in the set of samples
        /// </summary>
        /// <param name="rng">Random source.</param>
        /// <param name="numberOfOutcomes">The number of possible outcomes per sample.</param>
        /// <param name="numberOfSamples">The number of samples to take.</param>
        public static int[] SampleUniformWithoutReplacement(IRandomSource rng, int numberOfOutcomes, int numberOfSamples)
        {
            int[] sampleArr = new int[numberOfSamples];
            SampleUniformWithoutReplacement(rng, numberOfOutcomes, sampleArr);
            return sampleArr;
        }

        /// <summary>
        /// Take multiple samples from a set of possible outcomes with equal probability, i.e. a uniform discrete distribution,
        /// without replacement, i.e. any given value will only occur once at most in the set of samples
        /// </summary>
        /// <param name="rng">Random source.</param>
        /// <param name="numberOfOutcomes">The number of possible outcomes per sample.</param>
        /// <param name="sampleArr">An array to fill with samples.</param>
        public static void SampleUniformWithoutReplacement(IRandomSource rng, int numberOfOutcomes, int[] sampleArr)
        {
            if(sampleArr.Length > numberOfOutcomes) {
                throw new ArgumentException("sampleArr length must be less then or equal to numberOfOutcomes.");
            }

            // Use stack allocated temp array to avoid overhead of heap allocation and garbage collection.
            unsafe
            {
                // Create an array of indexes, one index per possible choice.
                int* indexArr = stackalloc int[numberOfOutcomes];
                for(int i=0; i < numberOfOutcomes; i++) {
                    indexArr[i] = i;
                }

                // Sample loop.
                for(int i=0; i < sampleArr.Length; i++)
                {
                    // Select an index at random.
                    int idx = rng.Next(i, numberOfOutcomes);

                    // Swap elements i and idx.
                    int tmp = indexArr[i];
                    indexArr[i] = indexArr[idx];
                    indexArr[idx] = tmp;
                }

                // Copy the samples into the result array.
                for(int i=0; i < sampleArr.Length; i++) {
                    sampleArr[i] = indexArr[i];
                }
            }
        }

        #endregion

        #region Private Static Methods

        private static void NormaliseProbabilities(double[] pArr)
        {
            if(pArr.Length == 0) {
                throw new ArgumentException("Invalid probabilities array (zero length).", nameof(pArr));
            }

            // TODO/FIXME: There may be pathological corner cases in which floating point precision issues might 
            // cause post normalised distributions that don't total anything close to 1.0.

            // Calc sum(pArr).
            double total = 0.0;
            for(int i=0; i < pArr.Length; i++) {
                total += pArr[i];
            }

            // Handle special case where all provided probabilities are at or near zero;
            // in this case we evenly assign probabilities across all choices.
            if(total <= __MaxFloatError) 
            {
                double p = 1.0 / pArr.Length;
                for(int i=0; i < pArr.Length; i++) {
                    pArr[i] = p;
                }
                return;
            }

            // Test if probabilities are already normalised (within reasonable limits of precision for floating point variables).
            if(Math.Abs(1.0 - total) < __MaxFloatError)
            {   // Close enough!!
                return;
            }

            // Normalise the probabilities.
            double factor = 1.0 / total;
            for(int i=0; i < pArr.Length; i++) {
                pArr[i] *= factor;
            }
        }

        #endregion
    }
}
