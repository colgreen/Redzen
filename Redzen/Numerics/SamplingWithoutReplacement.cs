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
    /// Helper methods for performing sampling with replacement.
    /// </summary>
    public static class SamplingWithoutReplacement
    {
        #region Public Static Methods

        /// <summary>
        /// Take a number of samples from a set of possible choices, without replacement, i.e. any given selection will only
        /// occur once at most in the set of samples
        /// </summary>
        /// <param name="numberOfChoices">The total number of possible selections</param>
        /// <param name="sampleCount">The number of samples to take. </param>
        /// <param name="rng">A source of randomness.</param>
        /// <returns>An array containing the numbers of the selected samples.</returns>
        public static int[] TakeSamples(int numberOfChoices, int sampleCount, IRandomSource rng)
        {
            if(sampleCount > numberOfChoices) {
                throw new ArgumentException("sampleCount must be less then or equal to numberOfChoices.");
            }

            // Create an array of indexes, one index per possible choice.
            int[] indexArr = new int[numberOfChoices];
            for(int i=0; i<numberOfChoices; i++) {
                indexArr[i] = i;
            }

            // Sample loop.
            for(int i=0; i<sampleCount; i++)
            {
                // Select an index at random.
                int idx = rng.Next(i, numberOfChoices);

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
