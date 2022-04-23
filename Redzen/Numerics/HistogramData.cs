// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace Redzen.Numerics;

// TODO: Check if there is an equivalent class in MathNet.Numerics.
/// <summary>
/// Histogram data. Frequency counts arranged into bins..
/// </summary>
public sealed class HistogramData
{
    #region Auto Properties

    /// <summary>
    /// The minimum value in the data series the distribution represents.
    /// </summary>
    public double Min { get; }

    /// <summary>
    /// The maximum value in the data series the distribution represents.
    /// </summary>
    public double Max { get; }

    /// <summary>
    /// The range of a single category bucket.
    /// </summary>
    public double Increment { get; }

    /// <summary>
    /// The array of category frequency counts.
    /// </summary>
    public int[] FrequencyArray { get; }

    #endregion

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
        this.Min = min;
        this.Max = max;
        this.Increment = increment;
        this.FrequencyArray = frequencyArr;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the index of the histogram bin that the given value falls within.
    /// </summary>
    /// <param name="x">The value to obtain a bin index for.</param>
    /// <returns>An histogram bin index.</returns>
    /// <remarks>
    /// Throws an exception if x is outside the range of represented by the histogram's bins.
    /// </remarks>
    public int GetBucketIndex(double x)
    {
        if(x < Min || x > Max)
            throw new ApplicationException("x is outside the range represented by the distribution data.");

        return (int)((x - Min) / Increment);
    }

    #endregion
}
