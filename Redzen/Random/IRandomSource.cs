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

namespace Redzen.Random
{
    /// <summary>
    /// A source of random values.
    /// </summary>
    public interface IRandomSource
    {
        /// <summary>
        /// Re-initialises the random number generator state using the provided seed value.
        /// </summary>
        void Reinitialise(ulong seed);

        /// <summary>
        /// Generates a random Int32 over the interval [0, int.MaxValue), i.e. exclusive of Int32.MaxValue.
        /// </summary>
        int Next();

        /// <summary>
        /// Generate a random Int32 over the interval [0 to maxValue), i.e. excluding maxValue.
        /// </summary>
        int Next(int maxValue);

        /// <summary>
        /// Generate a random Int32 over the interval [minValue, maxValue), i.e. excluding maxValue.
        /// maxValue must be >= minValue. minValue may be negative.
        /// </summary>
        int Next(int minValue, int maxValue);

        /// <summary>
        /// Generates a random double over the interval [0, 1), i.e. inclusive of 0.0 and exclusive of 1.0.
        /// </summary>
        double NextDouble();

        /// <summary>
        /// Generate a random double over the interval [0, 1), i.e. inclusive of 0.0 and exclusive of 1.0.
        /// </summary>
        /// <remarks>
        /// Uses an alternative sampling method that is capable of generating all possible values in the
        /// interval [0,1) that can be represented by a double precision float. Note however that this method 
        /// is significantly slower than NextDouble().
        /// </remarks>
        double NextDoubleHighRes();

        /// <summary>
        /// Fills the provided byte array with random bytes.
        /// </summary>
        /// <param name="buffer">The byte array to fill with random values.</param>
        void NextBytes(byte[] buffer);

        /// <summary>
        /// Generates a random float over the interval [0, 1), i.e. inclusive of 0.0 and exclusive of 1.0.
        /// </summary>
        float NextFloat();

        /// <summary>
        /// Generates a random UInt32 over the interval [0, 2^32-1], i.e. over the full 
        /// range of a UInt32.
        /// </summary>
        uint NextUInt();

        /// <summary>
        /// Generates a random Int32 over interval [0 to 2^31-1], i.e. inclusive of Int32.MaxValue.
        /// </summary>
        int NextInt();

        /// <summary>
        /// Generates a random UInt64 over the interval [0, 2^64-1], i.e. over the full 
        /// range of a UInt64.
        /// </summary>
        ulong NextULong();

        /// <summary>
        /// Generates a random double over the interval (0, 1), i.e. exclusive of both 0.0 and 1.0
        /// </summary>
        double NextDoubleNonZero();

        /// <summary>
        /// Generates a single random bit.
        /// </summary>
        bool NextBool();

        /// <summary>
        /// Generates a single random byte over the interval [0,255].
        /// </summary>
        byte NextByte();
    }
}
