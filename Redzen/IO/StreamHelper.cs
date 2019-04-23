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
using System.IO;

namespace Redzen.IO
{
    /// <summary>
    /// General purpose helper methods for working with streams.
    /// </summary>
    public static class StreamHelper
    {
        /// <summary>
        /// Reads data from a stream into a provided array. Reads up to the length of array and returns
        /// the number of bytes read.
        /// </summary>
        /// <param name="stream">The stream to read data from.</param>
        /// <param name="data">The array to read bytes into.</param>
        /// <returns>Returns the number of bytes read into the data byte array.</returns>
        /// <remarks>Unlike Stream.Read(), this method guarantees to read bytes until the end of stream is reached.</remarks>
        public static int Read(Stream stream, byte[] data)
        {
            int offset=0;
            int remaining = data.Length;
            while(remaining > 0)
            {
                int read = stream.Read(data, offset, remaining);
                if(read <= 0) 
                {   // End of stream reached.
                    return data.Length - remaining;
                }
                remaining -= read;
                offset += read;
            }
            return data.Length;
        }

        /// <summary>
        /// Reads data from a stream into a provided array, filling the array. If the end of 
        /// the stream is reached before the array is filled then an EndOfStreamException is thrown.
        /// </summary>
        /// <param name="stream">The stream to read data from.</param>
        /// <param name="data">The array to read bytes into.</param>
        /// <remarks>Unlike Stream.Read(), this method guarantees to fill the byte array if the stream has 
        /// sufficient bytes.</remarks>
        public static void ReadFill(Stream stream, byte[] data)
        {
            int offset=0;
            int remaining = data.Length;
            while(remaining > 0)
            {
                int count = stream.Read(data, offset, remaining);
                if(count <= 0) {
                    throw new EndOfStreamException(string.Format("End of stream reached with [{0}] bytes left to read.", remaining));
                }
                remaining -= count;
                offset += count;
            }
        }

        /// <summary>
        /// Read stream into byte array. Reads until the end of stream is reached and returns entire stream contents
        /// as a new byte array.
        /// </summary>
        /// <param name="stream">The stream to read data from.</param>
        /// <returns>Returns a new byte array containing the read data.</returns>
        public static byte[] ReadToByteArray(Stream stream)
        {
            using(MemoryBlockStream ms = new MemoryBlockStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
