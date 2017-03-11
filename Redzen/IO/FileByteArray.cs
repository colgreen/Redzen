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

using System;
using System.IO;

namespace Redzen.IO
{
    /// <summary>
    /// A byte array backed by a file on disk.
    /// 
    /// The byte array has fixed length and random accessible as per a normal byte array, but is backed by a 
    /// file rather than memory.
    /// </summary>
    public class FileByteArray : IDisposable
    {
        readonly string _filePath;
        readonly FileStream _fileStream;
        readonly int _length;

        #region Constructor

        /// <summary>
        /// Open an existing byte array file.
        /// </summary>
        /// <param name="filePath">The full path to the file.</param>
        public FileByteArray(string filePath)
        {
            // Open file.
            _filePath = filePath;
            _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

            // Read file/array length (in bytes).
            _length = (int)_fileStream.Length;
        }

        /// <summary>
        /// Create a new byte array file with the specified length (in bytes).
        /// </summary>
        /// <param name="filePath">The full path to the file.</param>
        /// <param name="length">The length of the byte array.</param>
        public FileByteArray(string filePath, int length)
        {
            // Open file.
            _filePath = filePath;
            _fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            // Init file (write zeros).
            byte[] ba = new byte[length];;
            _fileStream.Write(ba, 0, length);
            _length = length;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the full file path of the file that is backing the byte array.
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
        }

        #endregion

        #region IByteArray

        /// <summary>
        /// Gets the length of the array.
        /// </summary>
        /// <value>The length of the array.</value>
        public int Length
        {
            get { return _length; }
        }

        /// <summary>
        /// Gets the byte at the specified index within the array.
        /// </summary>
        /// <param name="idx">The index of the byte to retrieve from the array.</param>
        /// <returns>The byte at the specified index.</returns>
        public byte this[int idx]
        {
            get
            {
                if(idx >= _length) {
                    throw new IndexOutOfRangeException();
                }

                _fileStream.Seek(idx, SeekOrigin.Begin);
                return (byte)_fileStream.ReadByte();
            }
            set
            {
                if(idx >= _length) {
                    throw new IndexOutOfRangeException();
                }

                _fileStream.Seek(idx, SeekOrigin.Begin);
                _fileStream.WriteByte(value);
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Dispose of the FileByteArray.
        /// </summary>
        public void Dispose()
        {
            _fileStream.Close();
        }

        #endregion
    }
}
