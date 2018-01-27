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
using Redzen.Random;

namespace Redzen.Numerics
{
    /// <summary>
    /// Represents a distribution over a discrete set of possible states.
    /// </summary>
    public class DiscreteDistribution
    {
        const double __MaxFloatError = 0.000001;
        readonly IRandomSource _rng;
        readonly double[] _probArr;
        readonly int[] _labelArr;

        #region Constructors

        /// <summary>
        /// Construct the layout with provided probabilities. The provided probabilities do not have to add 
        /// up to 1.0 as we implicitly normalise them when using the layout.
        /// </summary>
        public DiscreteDistribution(double[] probArr)
            : this(new XorShiftRandom(), probArr)
        {}

        /// <summary>
        /// Construct the layout with provided probabilities. The provided probabilities do not have to add 
        /// up to 1.0 as we implicitly normalise them when using the layout.
        /// </summary>
        public DiscreteDistribution(int seed, double[] probArr)
            : this(new XorShiftRandom(seed), probArr)
        {}

        /// <summary>
        /// Construct the layout with provided probabilities. The provided probabilities do not have to add 
        /// up to 1.0 as we implicitly normalise them when using the layout.
        /// </summary>
        public DiscreteDistribution(IRandomSource rng, double[] probArr)
        {
            _rng = rng;
            NormaliseProbabilities(probArr);
            _probArr = probArr;

            // Assign labels.
            _labelArr = new int[probArr.Length];
            for(int i=0; i<_probArr.Length; i++) {
                _labelArr[i] = i;
            }
        }

        /// <summary>
        /// Construct the layout with provided probabilities. The provided probabilities do not have to add 
        /// up to 1.0 as we implicitly normalise them when using the layout.
        /// </summary>
        public DiscreteDistribution(double[] probArr, int[] labelArr)
            : this(new XorShiftRandom(), probArr, labelArr)
        {}

        /// <summary>
        /// Construct the layout with provided probabilities. The provided probabilities do not have to add 
        /// up to 1.0 as we implicitly normalise them when using the layout.
        /// </summary>
        public DiscreteDistribution(int seed, double[] probArr, int[] labelArr)
            : this(new XorShiftRandom(seed), probArr, labelArr)
        {}

        /// <summary>
        /// Construct the layout with provided probabilities. The provided probabilities do not have to add 
        /// up to 1.0 as we implicitly normalise them when using the layout.
        /// </summary>
        public DiscreteDistribution(IRandomSource rng, double[] probArr, int[] labelArr)
        {
            Debug.Assert(probArr.Length == labelArr.Length);

            _rng = rng;
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
        /// Sample from the provided discrete probability distribution.
        /// </summary>
        public int Sample()
        {
            return Sample(_rng);
        }

        /// <summary>
        /// Sample from the provided discrete probability distribution.
        /// </summary>
        public int Sample(IRandomSource rng)
        {
            // Throw the ball and return an integer indicating the outcome.
            double sample = rng.NextDouble();
            double acc = 0.0;
            for(int i=0; i<_probArr.Length; i++)
            {
                acc += _probArr[i];
                if(sample < acc) {
                    return _labelArr[i];
                }
            }

            // We might get here through floating point arithmetic rounding issues. 
            // e.g. accumulator == throwValue. 

            // Find a nearby non-zero probability to select.
            // Wrap around to start of array.
            for(int i=0; i<_probArr.Length; i++)
            {
                if(0.0 != _probArr[i]) {
                    return _labelArr[i];
                }
            }

            // If we get here then we have an array of zero probabilities.
            throw new InvalidOperationException("Invalid operation. No non-zero probabilities to select.");
        }

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

            // Test if probabilities are already normalised (within reasonable limits of precision for floating point variables).
            if(Math.Abs(1.0 - total) < __MaxFloatError)
            {   // Close enough!!
                return;
            }

            // Normalise the probabilities.
            for(int i=0; i < probs.Length; i++) {
                probs[i] /= total;
            }
        }

        #endregion
    }
}
