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
using System;

namespace Redzen.Numerics
{
    /// <summary>
    /// Static methods for roulette wheel selection from a set of choices with predefined probabilities.
    /// </summary>
    public static class DiscreteDistributionUtils
    {
        /// <summary>
        /// Sample from a binary distribution with the specified probability split between state false and true.
        /// </summary>
        /// <param name="probability">A probability between 0..1 that describes the probbaility of sampling boolean true.</param>
        /// <param name="rng">Random number generator.</param>
        public static bool SampleBinaryDistribution(double probability, XorShiftRandom rng)
        {
            return rng.NextDouble() < probability;
        }
        
        /// <summary>
        /// Sample from a set of possible outcomes with equal probability, i.e. a uniform discrete distribution.
        /// </summary>
        /// <param name="numberOfOutcomes">The number of possible outcomes.</param>
        /// <param name="rng">Random number generator.</param>
        /// <returns>An integer between 0..numberOfOutcomes-1.</returns>
        public static int SampleUniformDistribution(int numberOfOutcomes, XorShiftRandom rng)
        {
            return (int)(rng.NextDouble() * numberOfOutcomes);
        }

        /// <summary>
        /// Sample from the provided discrete probability distribution.
        /// </summary>
        /// <param name="dist">The discrete distribution to sample from.</param>
        /// <param name="rng">Random number generator.</param>
        public static int Sample(DiscreteDistribution dist, XorShiftRandom rng)
        {
            // Throw the ball and return an integer indicating the outcome.
            double sample = rng.NextDouble();
            double acc = 0.0;
            for(int i=0; i<dist.Probabilities.Length; i++)
            {
                acc += dist.Probabilities[i];
                if(sample < acc) {
                    return dist.Labels[i];
                }
            }

            // We might get here through floating point arithmetic rounding issues. 
            // e.g. accumulator == throwValue. 

            // Find a nearby non-zero probability to select.
            // Wrap around to start of array.
            for(int i=0; i<dist.Probabilities.Length; i++)
            {
                if(0.0 != dist.Probabilities[i]) {
                    return dist.Labels[i];
                }
            }

            // If we get here then we have an array of zero probabilities.
            throw new InvalidOperationException("Invalid operation. No non-zero probabilities to select.");
        }
    }
}
