namespace Redzen.Numerics;

/// <summary>
/// Represents the data for a single histogram bin.
/// </summary>
public readonly struct HistogramBin
{
    /// <summary>
    /// The lower bound of the histogram bin (inclusive).
    /// </summary>
    public double LowerBound { get; }

    /// <summary>
    /// The upper bound of the histogram bin (exclusive for all bins, except for the last/highest bin, which has an
    /// upper bound equal to the maximum x value represented in the histogram).
    /// </summary>
    public double UpperBound { get; }

    /// <summary>
    /// The number of data points in the histogram bin.
    /// </summary>
    public int Frequency { get; }

    /// <summary>
    /// Construct with the provided bin values.
    /// </summary>
    /// <param name="lowerBound">The lower bound of the histogram bin (inclusive).</param>
    /// <param name="upperBound">The upper bound of the histogram bin.</param>
    /// <param name="frequency">The number of data points in the histogram bin.</param>
    public HistogramBin(
        double lowerBound,
        double upperBound,
        int frequency)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(upperBound, lowerBound);
        ArgumentOutOfRangeException.ThrowIfNegative(frequency);

        LowerBound = lowerBound;
        UpperBound = upperBound;
        Frequency = frequency;
    }
}
