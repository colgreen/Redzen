using System;
using System.IO;
using System.Text;
using Redzen.Random;
using Xunit;

namespace Redzen.IO.Tests
{
    public class Base64EncodingOutputStreamFuzzerTests
    {
        #region Public Methods

        [Fact]
        public void Base64EncodeFuzzer()
        {
            // Get a source of entropy for the fuzzer.
            var rng = RandomDefaults.CreateRandomSource(0);

            // Alloc a byte array for storing the bytes to encode.
            Span<byte> buf = stackalloc byte[1024];

            for(int i=0; i < 10_000; i++)
            {
                // Fill buf with random bytes.
                rng.NextBytes(buf);

                // Select a random length of bytes in buf.
                int count = rng.Next(buf.Length - 1) + 1;
                var subspan = buf.Slice(0, count);

                // Base64 encode using the framework's Convert class.
                string base64Expected = Convert.ToBase64String(subspan, Base64FormattingOptions.None);

                // Base64 encode using our stream encoder.
                string base64Actual = Encode(subspan);

                // Compare.
                Assert.Equal(base64Expected, base64Actual);
            }
        }

        /// <summary>
        /// Writes fragments of data into the stream for encoding instead of writing one large byte array.
        /// </summary>
        [Fact]
        public void Base64EncodeFuzzer_WriteFragments()
        {
            // Get a source of entropy for the fuzzer.
            var rng = RandomDefaults.CreateRandomSource(0);

            // Alloc a byte array for storing the bytes to encode.
            var buf = new byte[1024];

            for(int i=0; i < 10_000; i++)
            {
                // Fill buf with random bytes.
                rng.NextBytes(buf);

                // Select a random length of bytes in buf.
                int count = rng.Next(buf.Length - 1) + 1;

                // Base64 encode using the framework's Convert class.
                string base64Expected = Convert.ToBase64String(buf.AsSpan(0, count), Base64FormattingOptions.None);

                // Base64 encode using our stream encoder.
                string base64Actual = Encode_WriteFragments(buf, count, rng);

                // Compare.
                Assert.Equal(base64Expected, base64Actual);
            }
        }

        #endregion

        #region Private Methods

        private static string Encode(Span<byte> span)
        {
            using(MemoryStream ms = new(span.Length))
            {
                using(Base64EncodingOutputStream base64Strm = new(ms, Encoding.UTF8))
                {
                    base64Strm.Write(span);
                }

                ms.Position = 0;

                using StreamReader sr = new(ms, Encoding.UTF8);
                string base64Str = sr.ReadToEnd();
                return base64Str;
            }
        }

        private static string Encode_WriteFragments(byte[] buf, int count, IRandomSource rng)
        {
            using(MemoryStream ms = new(buf.Length))
            {
                using(Base64EncodingOutputStream base64Strm = new(ms, Encoding.UTF8))
                {
                    int idx = 0;
                    int remain = count;

                    while(remain > 0)
                    {
                        int len = Math.Min(rng.Next(256), remain);
                        base64Strm.Write(buf, idx, len);
                        idx += len;
                        remain -= len;
                    }
                }

                ms.Position = 0;

                using StreamReader sr = new(ms, Encoding.UTF8);
                string base64Str = sr.ReadToEnd();
                return base64Str;
            }
        }

        #endregion
    }
}
