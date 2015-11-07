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
    /// Histogram data. Frequency counts arranged into bins..
    /// </summary>
    public class HistogramData
    {
        double _min;
        double _max;
        double _incr;
        int[] _frequencyArr;

        #region Constructor

        /// <summary>
        /// Construct with the provided frequency distribution data.
        /// </summary>
        /// <param name="min">The minimum value in the data series the distribution represents.</param>
        /// <param name="max">The maximum value in the data series the distribution represents.</param>
        /// <param name="increment">The range of a single category bucket.</param>
        /// <param name="frequencyArr">The array of category frequency counts.</param>
        public HistogramData(double min, double max, double increment, int[] frequencyArr)
        {
            _min = min;
            _max = max;
            _incr = increment;
            _frequencyArr = frequencyArr;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The minimum value in the data series the distribution represents.
        /// </summary>
        public double Min
        {
            get { return _min; }
        }

        /// <summary>
        /// The maximum value in the data series the distribution represents.
        /// </summary>
        public double Max
        {
            get { return _max; }
        }

        /// <summary>
        /// The range of a single category bucket.
        /// </summary>
        public double Increment
        {
            get { return _incr; }
        }

        /// <summary>
        /// The array of category frequency counts.
        /// </summary>
        public int[] FrequencyArray
        {
            get { return _frequencyArr; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the index of the bucket that covers the specified x value. Throws an exception if x is 
        /// outside the range of represented by the distribution buckets.
        /// </summary>
        public int GetBucketIndex(double x)
        {
            if(x < _min || x > _max) {
                throw new ApplicationException("x is outide the range represented by the distribution data.");
            }
            return (int)((x- _min) / _incr);
        }

        #endregion
    }
}
