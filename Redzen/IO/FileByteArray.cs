/* ****************************************************************************
 * This file is part of the Redzen code library.
 *
 * Copyright 2015 Colin D. Green (colin.green1@gmail.com)
 *
 * This software is issued under the MIT License.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
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
