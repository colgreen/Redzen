using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redzen.Numerics
{
    public static class SamplingWithoutReplacement
    {
        #region Public Static Methods

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
