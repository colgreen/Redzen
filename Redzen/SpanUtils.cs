/* ***************************************************************************
 * This file is part of the Redzen code library.
 *
 * Copyright 2015-2020 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Collections.Generic;

namespace Redzen
{
    /// <summary>
    /// Span static utility methods.
    /// </summary>
    public static class SpanUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Returns true if the contents of the two provided spans are equal.
        /// </summary>
        /// <param name="x">First span.</param>
        /// <param name="y">Second span.</param>
        public static bool Equal<T>(Span<T> x, Span<T> y)
        {
            // x and y are equal if they point to the same segment of memory, and have the same length.
            if(x == y) {
                return true;
            }

            // x and y and not equal if they have different lengths, regardless of whether they point to
            // the same segment of memory or not.
            if(x.Length != y.Length) {
                return false;
            }

            // x and y are *content* equals if their contained values are equal, regardless of whether they
            // point to the same segment of memory or not.
            var comp = EqualityComparer<T>.Default;

            for(int i=0; i < x.Length; i++)
            {
                if(!comp.Equals(x[i], y[i])) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if the elements of the specified span are equal to the given value.
        /// </summary>
        /// <param name="span">The span to test.</param>
        /// <param name="v">The test value.</param>
        public static bool Equal<T>(Span<T> span, T v)
        {
            var comp = EqualityComparer<T>.Default;
            for(int i=0; i < span.Length; i++)
            {
                if(!comp.Equals(span[i], v)){
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
