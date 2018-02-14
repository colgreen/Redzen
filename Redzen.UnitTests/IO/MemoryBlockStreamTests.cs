using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.IO;
using Redzen.Random;

namespace Redzen.UnitTests.IO
{
    [TestClass]
    public class MemoryBlockStreamTests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("MemoryBlockStream")]
        public void MemoryBlockStream_FuzzerTest()
        {
            MemoryStream ms = new MemoryStream();
            MemoryBlockStream ms2 = new MemoryBlockStream();

            MemoryStreamFuzzer fuzzer = new MemoryStreamFuzzer(ms, ms2, 0);
            for(int i=0; i<1000; i++)
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

        [TestMethod]
        [TestCategory("MemoryBlockStream")]
        public void TestWriteZeroBytes()
        {
            byte[] buf = new byte[0];
            MemoryBlockStream ms = new MemoryBlockStream();
            ms.Write(buf, 0, 0);
            Assert.AreEqual(ms.Length, 0);

            XorShiftRandom rng = new XorShiftRandom(1234567);
            byte[] buf2 = new byte[100];
            rng.NextBytes(buf2);
            ms.Write(buf2, 0, buf2.Length);

            if(!Utils.AreEqual(ms.ToArray(), buf2)) Assert.Fail();

            ms.Write(buf, 0, 0);
            Assert.AreEqual(ms.Length, buf2.Length);
        }

        #endregion

        #region Private Methods

        private void CompareState(MemoryStream ms, MemoryBlockStream ms2)
        {
            // Compare byte content.
            byte[] buff1 = ms.ToArray();
            byte[] buff2 = ms2.ToArray();

            if(!Utils.AreEqual(buff1, buff2)) {
                throw new Exception("State mismatch");
            }

            // Compare read/write position.
            if(ms.Position != ms2.Position) {
                throw new Exception("Position mismatch");
            }
        }

        #endregion
    }
}
