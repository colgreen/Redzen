// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;

namespace Redzen;

/// <summary>
/// Math utility methods for working with spans of numbers.
/// </summary>
public static class MathSpan
{
    /// <summary>
    /// Clip (limit) the values in a span to be within some defined interval.
    /// For example, if an interval of [0, 1] is specified, values less than 0 are set to 0, and values greater than 1 are set to 1.
    /// </summary>
    /// <typeparam name="T">Span numeric element type.</typeparam>
    /// <param name="s">Span containing the elements to clip.</param>
    /// <param name="min">Minimum value.</param>
    /// <param name="max">Maximum value.</param>
    public static void Clip<T>(Span<T> s, T min, T max)
        where T : struct, INumber<T>
    {
        // Run the vectorised code only if the hardware acceleration is available, and there are
        // enough span elements to utilise it.
        if(Vector.IsHardwareAccelerated && Vector<T>.IsSupported && (s.Length >= Vector<T>.Count))
        {
            var minVec = new Vector<T>(min);
            var maxVec = new Vector<T>(max);

            // Loop over vector sized slices.
            do
            {
                var vec = new Vector<T>(s);
                vec = Vector.Max(minVec, vec);
                vec = Vector.Min(maxVec, vec);
                vec.CopyTo(s);
                s = s.Slice(Vector<T>.Count);
            }
            while(s.Length >= Vector<T>.Count);
        }

        // Note. If the above vectorized code executed then this handles remaining elements,
        // otherwise it handles all elements.
        for(int i = 0; i < s.Length; i++)
        {
            if(s[i] < min)
                s[i] = min;
            else if(s[i] > max)
                s[i] = max;
        }
    }

    /// <summary>
    /// Determine the minimum value in the provided span.
    /// </summary>
    /// <typeparam name="T">Span numeric element type.</typeparam>
    /// <param name="s">The span.</param>
    /// <returns>The minimum value in the span.</returns>
    public static T Min<T>(ReadOnlySpan<T> s)
        where T : struct, INumber<T>
    {
        if(s.Length == 0) throw new ArgumentException("Empty span. Span must have one or elements.", nameof(s));

        T min;

        // Run the vectorised code only if the hardware acceleration is available, and there are
        // enough span elements to utilise it.
        if(Vector.IsHardwareAccelerated && Vector<T>.IsSupported && (s.Length >= Vector<T>.Count << 1))
        {
            var minVec = new Vector<T>(s);
            s = s.Slice(Vector<T>.Count);

            // Loop over vector sized slices.
            do
            {
                var vec = new Vector<T>(s);
                minVec = Vector.Min(minVec, vec);
                s = s.Slice(Vector<T>.Count);
            }
            while(s.Length >= Vector<T>.Count);

            // Calc min(minVec).
            min = minVec[0];
            for(int i = 1; i < Vector<T>.Count; i++)
            {
                if(minVec[i] < min)
                    min = minVec[i];
            }
        }
        else
        {
            min = s[0];
        }

        // Calc min.
        // Note. If the above vectorized code executed then this handles remaining elements,
        // otherwise it handles all elements.
        for(int i = 0; i < s.Length; i++)
        {
            if(s[i] < min)
                min = s[i];
        }

        return min;
    }

    /// <summary>
    /// Determine the maximum value in the provided span.
    /// </summary>
    /// <typeparam name="T">Span numeric element type.</typeparam>
    /// <param name="s">The span.</param>
    /// <returns>The minimum value in the span.</returns>
    public static T Max<T>(ReadOnlySpan<T> s)
        where T : struct, INumber<T>
    {
        if(s.Length == 0) throw new ArgumentException("Empty span. Span must have one or elements.", nameof(s));

        T max;

        // Run the vectorised code only if the hardware acceleration is available, and there are
        // enough span elements to utilise it.
        if(Vector.IsHardwareAccelerated && Vector<T>.IsSupported && (s.Length >= Vector<T>.Count << 1))
        {
            var maxVec = new Vector<T>(s);
            s = s.Slice(Vector<T>.Count);

            // Loop over vector sized slices.
            do
            {
                var vec = new Vector<T>(s);
                maxVec = Vector.Max(maxVec, vec);
                s = s.Slice(Vector<T>.Count);
            }
            while(s.Length >= Vector<T>.Count);

            // Calc max(maxVec).
            max = maxVec[0];
            for(int i = 1; i < Vector<T>.Count; i++)
            {
                if(maxVec[i] > max)
                    max = maxVec[i];
            }
        }
        else
        {
            max = s[0];
        }

        // Calc max.
        // Note. If the above vectorized code executed then this handles remaining elements,
        // otherwise it handles all elements.
        for(int i = 0; i < s.Length; i++)
        {
            if(s[i] > max)
                max = s[i];
        }

        return max;
    }

    /// <summary>
    /// Determine the minimum and maximum values in the provided span.
    /// </summary>
    /// <typeparam name="T">Span numeric element type.</typeparam>
    /// <param name="s">The span.</param>
    /// <param name="min">Returns the minimum value in the span.</param>
    /// <param name="max">Returns the maximum value in the span.</param>
    public static void MinMax<T>(ReadOnlySpan<T> s, out T min, out T max)
        where T : struct, INumber<T>
    {
        if(s.Length == 0) throw new ArgumentException("Empty span. Span must have one or elements.", nameof(s));

        // Run the vectorised code only if the hardware acceleration is available, and there are
        // enough span elements to utilise it.
        if(Vector.IsHardwareAccelerated && Vector<T>.IsSupported && (s.Length >= Vector<T>.Count << 1))
        {
            var minVec = new Vector<T>(s);
            var maxVec = new Vector<T>(s);
            s = s.Slice(Vector<T>.Count);

            // Loop over vector sized slices.
            do
            {
                var vec = new Vector<T>(s);
                minVec = Vector.Min(minVec, vec);
                maxVec = Vector.Max(maxVec, vec);
                s = s.Slice(Vector<T>.Count);
            }
            while(s.Length >= Vector<T>.Count);

            // Calc min(minVec) and max(maxVec).
            min = minVec[0];
            max = maxVec[0];
            for(int i = 1; i < Vector<T>.Count; i++)
            {
                if(minVec[i] < min) min = minVec[i];
                if(maxVec[i] > max) max = maxVec[i];
            }
        }
        else
        {
            min = max = s[0];
        }

        // Calc min/max.
        // Note. If the above vectorized code executed then this handles remaining elements,
        // otherwise it handles all elements.
        for(int i = 0; i < s.Length; i++)
        {
            T val = s[i];
            if(val < min)
                min = val;
            else if(val > max)
                max = val;
        }
    }

    /// <summary>
    /// Calculate the arithmetic mean of the span elements.
    /// </summary>
    /// <typeparam name="T">Span numeric element type.</typeparam>
    /// <param name="s">The span.</param>
    /// <returns>The sum of the elements.</returns>
    public static T Mean<T>(ReadOnlySpan<T> s)
        where T : struct, IFloatingPoint<T>
    {
        return s.Length != 0 ? Sum(s) / T.CreateChecked(s.Length) : T.Zero;
    }

    /// <summary>
    /// Calculate the mean of the squared difference for the elements of spans a and b.
    /// </summary>
    /// <typeparam name="T">Span numeric element type.</typeparam>
    /// <param name="a">Span a.</param>
    /// <param name="b">Span b.</param>
    /// <returns>The result.</returns>
    /// <remarks>Spans a and b must have the same length.</remarks>
    public static T MeanSquaredDelta<T>(ReadOnlySpan<T> a, ReadOnlySpan<T> b)
        where T : struct, INumber<T>
    {
        return SumSquaredDelta(a, b) / T.CreateChecked(a.Length);
    }

    /// <summary>
    /// Returns the median value in a span of sorted values.
    /// </summary>
    /// <typeparam name="T">Span numeric element type.</typeparam>
    /// <param name="s">The span.</param>
    /// <returns>The median of the provided values.</returns>
    /// <remarks>
    /// The span elements must be sorted such that the median element(s) are in the middle of the span. The
    /// sort order can be ascending or descending. If the elements are not sorted then this method will not
    /// throw an exception, but it will give an arbitrary result that is not the median.
    /// </remarks>
    public static double MedianOfSortedIntegers<T>(ReadOnlySpan<T> s)
        where T : struct, IBinaryInteger<T>
    {
        if(s.Length == 0) throw new ArgumentException("Empty span. Span must have one or elements.", nameof(s));

        if(s.Length == 1)
            return double.CreateChecked(s[0]);

        if(s.Length % 2 == 0)
        {
            // There are an even number of values. The values are already sorted so we
            // simply take the mean of the two central values.
            int idx = s.Length >> 1;
            return double.CreateChecked(s[idx - 1] + s[idx]) *  0.5;
        }

        // Odd number of values. Return the middle value.
        // Note. bit shift right by one bit results in integer division by two with the fraction part truncated, e.g. 3/2 = 1.
        return double.CreateChecked(s[s.Length >> 1]);
    }

    /// <summary>
    /// Returns the median value in a span of sorted values.
    /// </summary>
    /// <typeparam name="T">Span numeric element type.</typeparam>
    /// <param name="s">The span.</param>
    /// <returns>The median of the provided values.</returns>
    /// <remarks>
    /// The span elements must be sorted such that the median element(s) are in the middle of the span. The
    /// sort order can be ascending or descending. If the elements are not sorted then this method will not
    /// throw an exception, but it will give an arbitrary result that is not the median.
    /// </remarks>
    public static T MedianOfSorted<T>(ReadOnlySpan<T> s)
        where T : struct, IFloatingPoint<T>
    {
        if(s.Length == 0) throw new ArgumentException("Empty span. Span must have one or elements.", nameof(s));

        if(s.Length == 1)
            return s[0];

        if(s.Length % 2 == 0)
        {
            // There are an even number of values. The values are already sorted so we
            // simply take the mean of the two central values.
            int idx = s.Length >> 1;
            return (s[idx - 1] + s[idx]) * T.CreateChecked(0.5);
        }

        // Odd number of values. Return the middle value.
        // Note. bit shift right by one bit results in integer division by two with the fraction part truncated, e.g. 3/2 = 1.
        return s[s.Length >> 1];
    }

    /// <summary>
    /// Multiply each element of a span by a single scalar value.
    /// </summary>
    /// <typeparam name="T">Span numeric element type.</typeparam>
    /// <param name="s">The span with the elements to multiply.</param>
    /// <param name="x">The scalar value to multiply each span element by.</param>
    public static void Multiply<T>(Span<T> s, T x)
        where T : struct, INumber<T>
    {
        // Run the vectorised code only if hardware acceleration is available, and there are enough span
        // elements to justify its use.
        if(Vector.IsHardwareAccelerated && Vector<T>.IsSupported && (s.Length >= Vector<T>.Count << 1))
        {
            // Loop over vector sized slices.
            do
            {
                var vec = new Vector<T>(s);
                vec *= x;
                vec.CopyTo(s);
                s = s.Slice(Vector<T>.Count);
            }
            while(s.Length >= Vector<T>.Count);
        }

        // Note. If the above vectorized code executed then this handles remaining elements,
        // otherwise it handles all elements.
        for(int i = 0; i < s.Length; i++)
            s[i] *= x;
    }

    /// <summary>
    /// Calculate the sum of the span elements.
    /// </summary>
    /// <typeparam name="T">Span numeric element type.</typeparam>
    /// <param name="s">The span.</param>
    /// <returns>The sum of the elements.</returns>
    public static T Sum<T>(ReadOnlySpan<T> s)
        where T : struct, INumber<T>
    {
        T sum = T.Zero;

        // Run the vectorised code only if hardware acceleration is available, and there are enough span
        // elements to justify its use.
        if(Vector.IsHardwareAccelerated && Vector<T>.IsSupported && (s.Length >= Vector<T>.Count << 1))
        {
            var sumVec = new Vector<T>(s);
            s = s.Slice(Vector<T>.Count);

            // Loop over vector sized slices.
            do
            {
                var vec = new Vector<T>(s);
                sumVec += vec;
                s = s.Slice(Vector<T>.Count);
            }
            while(s.Length >= Vector<T>.Count);

            // Sum the elements of sumVec.
            sum = Vector.Sum(sumVec);
        }

        // Note. If the above vectorized code executed then this handles remaining elements,
        // otherwise it handles all elements.
        for(int i = 0; i < s.Length; i++)
            sum += s[i];

        return sum;
    }

    /// <summary>
    /// Calculate the sum of the square of the span elements.
    /// </summary>
    /// <typeparam name="T">Span numeric element type.</typeparam>
    /// <param name="s">The span.</param>
    /// <returns>The sum of the elements.</returns>
    public static T SumOfSquares<T>(ReadOnlySpan<T> s)
        where T : struct, INumber<T>
    {
        T sum = T.Zero;

        // Run the vectorised code only if hardware acceleration is available, and there are enough span
        // elements to justify its use.
        if(Vector.IsHardwareAccelerated && Vector<T>.IsSupported && (s.Length >= Vector<T>.Count))
        {
            var sumVec = Vector<T>.Zero;

            // Loop over vector sized slices.
            do
            {
                var vec = new Vector<T>(s);
                sumVec += vec * vec;
                s = s.Slice(Vector<T>.Count);
            }
            while(s.Length >= Vector<T>.Count);

            // Sum the elements of sumVec.
            sum = Vector.Sum(sumVec);
        }

        // Note. If the above vectorized code executed then this handles remaining elements,
        // otherwise it handles all elements.
        for(int i = 0; i < s.Length; i++)
            sum += s[i] * s[i];

        return sum;
    }

    /// <summary>
    /// Calculate the sum of the squared difference for the elements of spans a and b.
    /// </summary>
    /// <typeparam name="T">Span numeric element type.</typeparam>
    /// <param name="a">Span a.</param>
    /// <param name="b">Span b.</param>
    /// <returns>The result.</returns>
    /// <remarks>Spans a and b must be the same length.</remarks>
    public static T SumSquaredDelta<T>(ReadOnlySpan<T> a, ReadOnlySpan<T> b)
        where T : struct, INumber<T>
    {
        if(a.Length != b.Length) throw new ArgumentException("Array lengths are not equal.");

        T total = T.Zero;

        // Run the vectorised code only if the hardware acceleration is available, and there are
        // enough span elements to utilise it.
        if(Vector.IsHardwareAccelerated && Vector<T>.IsSupported && (a.Length >= Vector<T>.Count))
        {
            var sumVec = Vector<T>.Zero;

            // Loop over vector sized slices, calc the squared error for each, and accumulate in sumVec.
            do
            {
                var av = new Vector<T>(a);
                var bv = new Vector<T>(b);
                var cv = av - bv;
                sumVec += cv * cv;

                a = a.Slice(Vector<T>.Count);
                b = b.Slice(Vector<T>.Count);
            }
            while(a.Length >= Vector<T>.Count);

            // Sum the elements of sumVec.
            total = Vector.Sum(sumVec);
        }

        // Note. If the above vectorized code executed then this handles remaining elements,
        // otherwise it handles all elements.
        for(int i = 0; i < a.Length; i++)
        {
            T err = a[i] - b[i];
            total += err * err;
        }

        return total;
    }
}
