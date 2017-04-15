/* ***************************************************************************
 * This file is part of the Redzen code library.
 * 
 * Copyright 2015-2017 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */

using System;

namespace Redzen.Numerics
{
    /// <summary>
    /// Static methods for roulette wheel selection from a set of choices with predefined probabilities.
    /// </summary>
    public static class DiscreteDistributionUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Sample from a binary distribution with the specified probability split between state false and true.
        /// </summary>
        /// <param name="probability">A probability between 0..1 that describes the probbaility of sampling boolean true.</param>
        /// <param name="rng">Random number generator.</param>
        public static bool SampleBinaryDistribution(double probability, IRandomSource rng)
        {
            return rng.NextDouble() < probability;
        }
        
        /// <summary>
        /// Sample from a set of possible outcomes with equal probability, i.e. a uniform discrete distribution.
        /// </summary>
        /// <param name="numberOfOutcomes">The number of possible outcomes per sample.</param>
        /// <param name="rng">Random number generator.</param>
        /// <returns>An integer between 0..numberOfOutcomes-1.</returns>
        public static int SampleUniformDistribution(int numberOfOutcomes, IRandomSource rng)
        {
            return (int)(rng.NextDouble() * numberOfOutcomes);
        }

        /// <summary>
        /// Take multiple samples from a set of possible outcomes with equal probability, i.e. a uniform discrete distribution,
        /// with replacement, i.e. any given value will only occur once at most in the set of samples
        /// </summary>
        /// <param name="numberOfOutcomes">The number of possible outcomes per sample.</param>
        /// <param name="sampleCount">The number of samples to take. </param>
        /// <param name="rng">A source of randomness.</param>
        /// <returns>An array containing the numbers of the selected samples.</returns>
        public static int[] SampleWithoutReplacement(int numberOfOutcomes, int sampleCount, IRandomSource rng)
        {
            if(sampleCount > numberOfOutcomes) {
                throw new ArgumentException("sampleCount must be less then or equal to numberOfChoices.");
            }

            // Create an array of indexes, one index per possible choice.
            int[] indexArr = new int[numberOfOutcomes];
            for(int i=0; i<numberOfOutcomes; i++) {
                indexArr[i] = i;
            }

            // Sample loop.
            for(int i=0; i<sampleCount; i++)
            {
                // Select an index at random.
                int idx = rng.Next(i, numberOfOutcomes);

                // Swap elements i and idx.
                Swap(indexArr, i, idx);
            }

            // Copy the samples into an array.
            int[] samplesArr = new int[sampleCount];
            for(int i=0; i<sampleCount; i++) {
                samplesArr[i] = indexArr[i];
            }

            return samplesArr;
        }

        #endregion

        #region Private Static Methods

        private static void Swap(int[] arr, int x, int y)
        {
            int tmp = arr[x];
            arr[x] = arr[y];
            arr[y] = tmp;
        }

        #endregion

    }
}
