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
        readonly int[] _labelArr;

        #region Constructor

        /// <summary>
        /// Construct the layout with provided probabilities. The provided probabilites do not have to add 
        /// up to 1.0 as we implicitly normalise them when using the layout.
        /// </summary>
        public DiscreteDistribution(double[] probArr)
        {
            NormaliseProbabilities(probArr);
            _probArr = probArr;
            _labelArr = new int[probArr.Length];

            // Assign labels.
            for(int i=0; i<_probArr.Length; i++) {
                _labelArr[i] = i;
            }
        }

        /// <summary>
        /// Construct the layout with provided probabilities. The provided probabilites do not have to add 
        /// up to 1.0 as we implicitly normalise them when using the layout.
        /// </summary>
        public DiscreteDistribution(double[] probArr, int[] labelArr)
        {
            Debug.Assert(probArr.Length == labelArr.Length);

            NormaliseProbabilities(probArr);
            _probArr = probArr;
            _labelArr = labelArr;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        public DiscreteDistribution(DiscreteDistribution copyFrom)
        {
            _probArr = (double[])copyFrom._probArr.Clone();
            _labelArr = (int[])copyFrom._labelArr.Clone();
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
            int idx = -1;
            for(int i=0; i<_labelArr.Length; i++)
            {
                if(labelId == _labelArr[i]) {
                    idx = i;
                    break;
                }
            }

            Debug.Assert(idx > 0 && idx < _probArr.Length, "label not found");

            double[] probArr = new double[_probArr.Length-1];
            int[] labels = new int[_probArr.Length-1];
            for(int i=0; i<idx; i++)
            {
                probArr[i] = _probArr[i];
                labels[i] = _labelArr[i];
            }

            for(int i=idx+1, j=idx; i<_probArr.Length; i++, j++)
            {
                probArr[j] = _probArr[i];
                labels[j] = _labelArr[i];
            }

            // Note. The probabilities are not normalised here, however the constructor will normalise them.
            return new DiscreteDistribution(probArr, labels);
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
            if(total <= __MaxFloatError) 
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
