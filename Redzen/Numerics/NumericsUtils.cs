// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using Redzen.Random;

namespace Redzen.Numerics;

/// <summary>
/// General purpose numeric helper methods.
/// </summary>
public static class NumericsUtils
{
    /// <summary>
    /// Rounds up or down to a whole number by using the fractional part of the input value
    /// as the probability that the value will be rounded up.
    /// </summary>
    /// <param name="val">The value to round.</param>
    /// <param name="rng">Random source.</param>
    /// <returns>The rounded value.</returns>
    /// <remarks>
    /// This is useful if we wish to round values and then sum them without generating a rounding bias.
    /// For monetary rounding this problem is solved with rounding to e.g. the nearest even number,
    /// which then causes a bias towards even numbers. As such, this solution is more appropriate for
    /// certain types of scientific calculations.
    /// </remarks>
    public static double StochasticRound(double val, IRandomSource rng)
    {
        double integerPart = Math.Floor(val);
        double fractionalPart = val - integerPart;
        return rng.NextDouble() < fractionalPart ? integerPart + 1.0 : integerPart;
    }

    /// <summary>
    /// Rounds up or down to a whole number by using the fractional part of the input value
    /// as the probability that the value will be rounded up.
    /// </summary>
    /// <typeparam name="T">Numeric type.</typeparam>
    /// <param name="val">The value to round.</param>
    /// <param name="rng">Random source.</param>
    /// <returns>The rounded value.</returns>
    /// <remarks>
    /// This is useful if we wish to round values and then sum them without generating a rounding bias.
    /// For monetary rounding this problem is solved with rounding to e.g. the nearest even number,
    /// which then causes a bias towards even numbers. As such, this solution is more appropriate for
    /// certain types of scientific calculations.
    /// </remarks>
    public static T StochasticRound<T>(T val, IRandomSource rng)
        where T : struct, IBinaryFloatingPointIeee754<T>
    {
        T integerPart = T.Floor(val);
        T fractionalPart = val - integerPart;
        return rng.NextUnitInterval<T>() < fractionalPart ? integerPart + T.One : integerPart;
    }
}
