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
using System.Diagnostics;

namespace Redzen.Numerics
{
    /// <summary>
    /// Represents a distribution over a discrete set of possible states.
    /// Total probability over all states must add up to 1.0
    /// 
    /// This class was previously called RouletteWheelLayout.
    /// </summary>
    public class DiscreteDistribution
    {
        const double __MaxFloatError = 0.000001;
        readonly double[] _probArr;

        #region Constructor

        /// <summary>
        /// Construct the layout with provided probabilities. The provided probabilites do not have to add 
        /// up to 1.0 as implicitly normalise them when using the layout.
        /// </summary>
        public DiscreteDistribution(double[] probArr)
        {
            NormaliseProbabilities(probArr);
            _probArr = probArr;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public DiscreteDistribution(DiscreteDistribution copyFrom)
        {
            _probArr = (double[])copyFrom._probArr.Clone();
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

        #endregion

        #region Public Methods

        /// <summary>
        /// Remove the specified outcome from the set of probabilities and return as a new DiscreteDistribution object.
        /// </summary>
        public DiscreteDistribution RemoveOutcome(int idx)
        {
            Debug.Assert(idx > 0 && idx < _probArr.Length, "label not found");

            double[] probArr = new double[_probArr.Length-1];
            for(int i=0; i<idx; i++) {
                probArr[i] = _probArr[i];
            }

            for(int i=idx+1, j=idx; i<_probArr.Length; i++, j++) {
                probArr[j] = _probArr[i];
            }

            // Note. The probabilities are not normalised here, however the constructor will normalise them.
            return new DiscreteDistribution(probArr);
        }

        #endregion

        #region Private Static Methods

        private static void NormaliseProbabilities(double[] probs)
        {
            // TODO/FIXME: There may be corner cases in which floating point precision issues might cause 
            // post normalised distributions that don't total anything close to 1.0.

            // Total up probabilities.
            double total = 0.0;
            for(int i=0; i < probs.Length; i++) {
                total += probs[i];
            }

            // Handle special case where all provided probabilities are zero.
            // In this case we evenly assign probabilities across all choices.
            if(0.0 == total) 
            {
                double p = 1.0 / probs.Length;
                for(int i=0; i < probs.Length; i++) {
                    probs[i] = p;
                }
                return;
            }

            // Test if probs are already normalised (within reasonable limits of precision for floating point variables).
            if(Math.Abs(1.0 - total) < __MaxFloatError)
            {   // Close enough!!
                return;
            }

            // Normailise the probabilities.
            for(int i=0; i < probs.Length; i++) {
                probs[i] /= total;
            }
        }

        #endregion
    }
}
