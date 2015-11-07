/* ****************************************************************************
 * This file is part of the Redzen code library.
 *
 * Copyright 2015 Colin D. Green (colin.green1@gmail.com)
 *
 * This software is issued under the MIT License.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System.Collections.Generic;
using Redzen.Numerics;

namespace Redzen.Sorting
{
    public static class SortUtils
    {
        /// <summary>
        /// Indicates if a list of doubles is sorted into ascending order.
        /// </summary>
        public static bool IsSorted(IList<double> valueList)
        {
            if (0 == valueList.Count) {
                return true;
            }

            double prev = valueList[0];
            int count = valueList.Count;
            for (int i = 1; i < count; i++)
            {
                if (valueList[i] < prev) {
                    return false;
                }
                prev = valueList[i];
            }
            return true;
        }

        /// <summary>
        /// Randomly shuffles items within a list.
        /// </summary>
        /// <param name="list">The list to shuffle.</param>
        /// <param name="rng">Random number generator.</param>
        public static void Shuffle<T>(IList<T> list, XorShiftRandom rng)
        {
            // This approach was suggested by Jon Skeet in a dotNet newsgroup post and
            // is also the technique used by the OpenJDK. The use of rnd.Next(i+1) introduces
            // the possibility of swapping an item with itself, I suspect the reasoning behind this
            // has to do with ensuring the probability of each possible permutation is approximately equal.
            for(int i = list.Count - 1; i > 0; i--)
            {
                int swapIndex = rng.Next(i + 1);
                T tmp = list[swapIndex];
                list[swapIndex] = list[i];
                list[i] = tmp;
            }
        }
    }
}
