/* ***************************************************************************
 * This file is part of the Redzen code library.
 * 
 * Copyright 2015-2019 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */
using System.Runtime.CompilerServices;

namespace Redzen
{
    // TODO: Consider deleting. Replace usages with System.Numerics.BitOperations.RotateLeft

    /// <summary>
    /// Bit manipulation utility methods.
    /// </summary>
    public static class BitwiseUtils
    {
        /// <summary>
        /// Rotate the bits of <paramref name="x"/> left, by the number of bits/positions defined by <paramref name="k"/>.
        /// </summary>
        /// <param name="x">The input value.</param>
        /// <param name="k">The number of bits/positions to rotate by.</param>
        /// <returns>The result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateLeft(ulong x, int k)
        {
            // Note. RyuJIT will compile this to a single rotate CPU instruction (as of about .NET 4.6.1 and dotnet core 2.0).
            return (x << k) | (x >> (64 - k));
        }
    }
}
