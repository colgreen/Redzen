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

namespace Redzen.Numerics
{
    public interface IRandomSource
    {
        /// <summary>
        /// Reinitialises using an int value as a seed.
        /// </summary>
        void Reinitialise(int seed);

        /// <summary>
        /// Generates a random int over the range 0 to int.MaxValue-1.
        /// MaxValue is not generated in order to remain functionally equivalent to System.Random.Next().
        /// This does slightly eat into some of the performance gain over System.Random, but not much.
        /// For better performance see:
        /// 
        /// Call NextInt() for an int over the range 0 to int.MaxValue.
        /// 
        /// Call NextUInt() and cast the result to an int to generate an int over the full Int32 value range
        /// including negative values. 
        /// </summary>
        int Next();

        /// <summary>
        /// Generates a random int over the range 0 to upperBound-1, and not including upperBound.
        /// </summary>
        int Next(int upperBound);

        /// <summary>
        /// Generates a random int over the range lowerBound to upperBound-1, and not including upperBound.
        /// upperBound must be >= lowerBound. lowerBound may be negative.
        /// </summary>
        int Next(int lowerBound, int upperBound);

        /// <summary>
        /// Generates a random double. Values returned are over the range [0, 1). That is, inclusive of 0.0 and exclusive of 1.0.
        /// </summary>
        double NextDouble();

        /// <summary>
        /// Fills the provided byte array with random bytes.
        /// This method is functionally equivalent to System.Random.NextBytes(). 
        /// </summary>
        void NextBytes(byte[] buffer);

        /// <summary>
        /// Generates a random float. Values returned are over the range [0, 1). That is, inclusive of 0.0 and exclusive of 1.0.
        /// </summary>
        float NextFloat();

        /// <summary>
        /// Generates a uint. Values returned are over the full range of a uint, 
        /// uint.MinValue to uint.MaxValue, inclusive.
        /// 
        /// This is the fastest method for generating a single random number because the underlying
        /// random number generator algorithm generates 32 random bits that can be cast directly to 
        /// a uint.
        /// </summary>
        uint NextUInt();

        /// <summary>
        /// Generates a random int over the range 0 to int.MaxValue, inclusive. 
        /// This method differs from Next() only in that the range is 0 to int.MaxValue
        /// and not 0 to int.MaxValue-1.
        /// 
        /// The slight difference in range means this method is slightly faster than Next()
        /// but is not functionally equivalent to System.Random.Next().
        /// </summary>
        int NextInt();

        /// <summary>
        /// Generates a random double. Values returned are over the range (0, 1). That is, exclusive of both 0.0 and 1.0.
        /// </summary>
        double NextDoubleNonZero();

        /// <summary>
        /// Generates a single random bit.
        /// This method's performance is improved by generating 32 bits in one operation and storing them
        /// ready for future calls.
        /// </summary>
        bool NextBool();

        /// <summary>
        /// Generates a signle random byte with range [0,255].
        /// This method's performance is improved by generating 4 bytes in one operation and storing them
        /// ready for future calls.
        /// </summary>
        byte NextByte();
    }
}
