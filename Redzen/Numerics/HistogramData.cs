// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Buffers;

namespace Redzen.Numerics;

/// <summary>
/// Histogram data. Frequency counts arranged into bins..
/// </summary>
public sealed class HistogramData : IDisposable
{
    private readonly HistogramBin[] _binArr;

    /// <summary>
    /// Gets the number of bins in the histogram.
    /// </summary>
    public int BinCount { get; }

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

    private HistogramData(
        HistogramBin[] binArr,
        int binCount,
        double min, double max,
        double increment)
    {
        _binArr = binArr ?? throw new ArgumentNullException(nameof(binArr));

        if(binCount > binArr.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(binCount),
                $"Value must be greater than or equal to the length of the {nameof(binArr)} array.");
        }

        BinCount = binCount;
        Min = min;
        Max = max;
        Increment = increment;
    }

    /// <summary>
    /// Get a span that contains the histogram bin data.
    /// Each element of the span represents a single bin.
    /// </summary>
    /// <returns>
    /// A <see cref="Span{T}"/> that contains the histogram bin data.
    /// </returns>
    public Span<HistogramBin> GetBinSpan()
    {
        return _binArr.AsSpan(0, BinCount);
    }

    /// <summary>
    /// Lookup the index of the histogram bin that the given value falls within.
    /// </summary>
    /// <param name="x">The value to obtain a bin index for.</param>
    /// <returns>An histogram bin index.</returns>
    /// <remarks>
    /// Throws an exception if x is outside the range of represented by the histogram's bins.
    /// </remarks>
    public int LookupBinIndex(double x)
    {
        if(x < Min || x > Max)
            throw new ArgumentOutOfRangeException(nameof(x), "Value is outside the range represented by the distribution data.");

        return (int)((x - Min) / Increment);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ArrayPool<HistogramBin>.Shared.Return(_binArr);
    }

    /// <summary>
    /// Calculate histogram data for the provided span of values, and a histogram bin count calculated using the Rice
    /// rule (see http://en.wikipedia.org/wiki/Histogram).
    /// </summary>
    /// <param name="vals">The values to calculate a histogram for.</param>
    /// <returns>A new instance of <see cref="HistogramData"/>.</returns>
    public static HistogramData BuildHistogramData(
        Span<double> vals)
    {
        // Calc histogram bin count using the Rice rule; see http://en.wikipedia.org/wiki/Histogram
        int binCount = (int)(2.0 * Math.Pow(vals.Length, 1.0/3.0));

        return HistogramData.BuildHistogramData(vals, binCount);
    }

    /// <summary>
    /// Calculate histogram data for the provided span of values, and the specified number of histogram bins.
    /// </summary>
    /// <param name="vals">The values to calculate a histogram for.</param>
    /// <param name="binCount">The number of histogram bins to use.</param>
    /// <returns>A new instance of <see cref="HistogramData"/>.</returns>
    public static HistogramData BuildHistogramData(
        Span<double> vals,
        int binCount)
    {
        // Determine the minimum and maximum values in the provided values.
        MathSpan.MinMax(vals, out double min, out double max);

        // Note. each bin's range has interval [low,high), i.e. samples exactly equal to 'high' will fall
        // into the next highest bin. Except for the last bin which has interval [low, high].
        double range = max - min;

        // Handle special case where the data series contains a single value.
        HistogramBin[] binArr;
        if(range == 0.0)
        {
            binArr = ArrayPool<HistogramBin>.Shared.Rent(1);
            binArr[0] = new HistogramBin(min, max, vals.Length);

            return new HistogramData(
                binArr, 1,
                min, max,
                0.0);
        }

        // Alloc an array of histogram bin frequencies.
        int[] frequencyArr = ArrayPool<int>.Shared.Rent(binCount);

        try
        {
            // For extra safety, get a span of the correct length, as the rented array will usually be longer than
            // the required length.
            var frequencySpan = frequencyArr.AsSpan(0, binCount);

            double incr = range / binCount;

            // Loop values; for each one, increment the relevant bin's frequency/count.
            for(int i = 0; i < vals.Length; i++)
            {
                // Determine which bin the value falls within.
                int binIdx = (int)((vals[i] - min) / incr);

                // Values that equal max, are placed into the last bin.
                if(binIdx == binCount) binIdx--;

                frequencySpan[binIdx]++;
            }

            // Rent an array of HistogramBin structs, and populate it with histogram data.
            binArr = ArrayPool<HistogramBin>.Shared.Rent(binCount);

            double lowerBound = min;
            for(int i=0; i < binCount; i++, lowerBound += incr)
            {
                binArr[i] = new HistogramBin(
                    lowerBound,
                    upperBound: lowerBound + incr,
                    frequencySpan[i]);
            }

            return new HistogramData(
                binArr, binCount,
                min, max, incr);
        }
        finally
        {
            // Return the temporary rented array.
            ArrayPool<int>.Shared.Return(frequencyArr);
        }
    }
}
