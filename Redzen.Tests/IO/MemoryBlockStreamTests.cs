using System;
using System.IO;
using Redzen.Random;
using Xunit;

namespace Redzen.IO.Tests
{
    public class MemoryBlockStreamTests
    {
        #region Test Methods

        [Fact]
        public void MemoryBlockStreamFuzzer()
        {
            MemoryStream ms = new MemoryStream();
            MemoryBlockStream ms2 = new MemoryBlockStream();

            MemoryStreamFuzzer fuzzer = new MemoryStreamFuzzer(ms, ms2, 0);
            for(int i=0; i < 1000; i++)
            {
                fuzzer.PerformMultipleOps(100);
                CompareState(ms, ms2);

                if(ms.Length > 3e9)
                {
                    ms = new MemoryStream();
                    ms2 = new MemoryBlockStream();
                }
            }
        }

        [Fact]
        public void WriteZeroBytes()
        {
            byte[] buf = Array.Empty<byte>();
            MemoryBlockStream ms = new MemoryBlockStream();
            ms.Write(buf, 0, 0);
            Assert.Equal(0, ms.Length);

            IRandomSource rng = RandomDefaults.CreateRandomSource(1234567);
            byte[] buf2 = new byte[100];
            rng.NextBytes(buf2);
            ms.Write(buf2);

            Assert.Equal(ms.ToArray(), buf2);

            ms.Write(buf, 0, 0);
            Assert.Equal(ms.Length, buf2.Length);
        }

        #endregion

        #region Private Methods

        private static void CompareState(MemoryStream ms, MemoryBlockStream ms2)
        {
            // Compare byte content.
            byte[] buff1 = ms.ToArray();
            byte[] buff2 = ms2.ToArray();

            // Compare read/write position.
            Assert.Equal(buff1, buff2);
            Assert.Equal(ms.Position, ms2.Position);
        }

        #endregion
    }
}
