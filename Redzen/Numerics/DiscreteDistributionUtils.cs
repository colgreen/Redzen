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
        /// Take multiple samples from a set of possible outcomes with equal probability, i.e. a uniform discrete distribution,
        /// with replacement, i.e. any given value will only occur once at most in the set of samples
        /// </summary>
        /// <param name="numberOfOutcomes">The number of possible outcomes per sample.</param>
        /// <param name="sampleArr">An array to fill with samples.</param>
        /// <param name="rng">A source of randomness.</param>
        public static void SampleUniformWithoutReplacement(int numberOfOutcomes, int[] sampleArr, IRandomSource rng)
        {
            if(sampleArr.Length > numberOfOutcomes) {
                throw new ArgumentException("sampleArr length must be less then or equal to numberOfOutcomes.");
            }

            // Create an array of indexes, one index per possible choice.
            int[] indexArr = new int[numberOfOutcomes];
            for(int i=0; i<numberOfOutcomes; i++) {
                indexArr[i] = i;
            }

            // Sample loop.
            for(int i=0; i<sampleArr.Length; i++)
            {
                // Select an index at random.
                int idx = rng.Next(i, numberOfOutcomes);

                // Swap elements i and idx.
                Swap(indexArr, i, idx);
            }

            // Copy the samples into the result array.
            for(int i=0; i<sampleArr.Length; i++) {
                sampleArr[i] = indexArr[i];
            }
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
