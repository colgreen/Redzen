using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Structures;

namespace Redzen.UnitTests.Structures
{
    [TestClass]
    public class BoolArrayTests
    {
        #region Public Test Methods

        [TestMethod]
        [TestCategory("BoolArray")]
        public void Test_InitFalse()
        {
            const int len = 123;

            var arr = new BoolArray(123);
            for(int i=0; i < len; i++) {
                Assert.IsFalse(arr[i]);
            }

            arr = new BoolArray(123, false);
            for(int i =0; i < len; i++) {
                Assert.IsFalse(arr[i]);
            }
        }

        [TestMethod]
        [TestCategory("BoolArray")]
        public void Test_InitTrue()
        {
            const int len = 123;

            var arr = new BoolArray(123, true);

            for(int i=0; i < len; i++) {
                Assert.IsTrue(arr[i]);
            }
        }

        [TestMethod]
        [TestCategory("BoolArray")]
        public void Test_SingleBitFlipsOn()
        {
            for(int len=0; len < 258; len++) {
                TestSingleBitFlipsOn(123);
            }   
        }

        [TestMethod]
        [TestCategory("BoolArray")]
        public void Test_SingleBitFlipsOff()
        {
            for(int len=0; len < 258; len++) {
                TestSingleBitFlipsOff(123);
            }
        }

        [TestMethod]
        [TestCategory("BoolArray")]
        public void Test_Bounds()
        {
            for(int len=1; len < 258; len++)
            {
                var arr = new BoolArray(len);

                Assert.ThrowsException<IndexOutOfRangeException>(() => { bool b = arr[-1]; });
                Assert.ThrowsException<IndexOutOfRangeException>(() => { bool b = arr[arr.Length]; });
                Assert.IsFalse(arr[arr.Length-1]);
            }
        }

        #endregion

        #region Private Static Methods

        private static void TestSingleBitFlipsOn(int len)
        {
            for(int i=0; i<len; i++)
            {
                var arr = new BoolArray(len);
                arr[i] = true;

                // Test all leading bits.
                for(int j=0; j<i; j++)
                {
                    Assert.IsFalse(arr[j]);
                }

                // Test flipped bit.
                Assert.IsTrue(arr[i]);

                // Test all following bits.
                for(int j=i+1; j<len; j++)
                {
                    Assert.IsFalse(arr[j]);
                }
            }
        }

        private static void TestSingleBitFlipsOff(int len)
        {
            for(int i=0; i<len; i++)
            {
                var arr = new BoolArray(len, true);
                arr[i] = false;

                // Test all leading bits.
                for(int j=0; j<i; j++)
                {
                    Assert.IsTrue(arr[j]);
                }

                // Test flipped bit.
                Assert.IsFalse(arr[i]);

                // Test all following bits.
                for(int j=i+1; j<len; j++)
                {
                    Assert.IsTrue(arr[j]);
                }
            }
        }

        #endregion
    }
}
