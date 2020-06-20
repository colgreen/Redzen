using System;
using Redzen.Structures;
using Xunit;

namespace Redzen.UnitTests.Structures
{
    public class BoolArrayTests
    {
        #region Public Test Methods

        [Fact]
        public void InitFalse()
        {
            const int len = 123;

            var arr = new BoolArray(len);
            for(int i=0; i < len; i++) {
                Assert.False(arr[i]);
            }

            arr = new BoolArray(len, false);
            for(int i=0; i < len; i++) {
                Assert.False(arr[i]);
            }
        }

        [Fact]
        public void InitTrue()
        {
            const int len = 123;

            var arr = new BoolArray(len, true);

            for(int i=0; i < len; i++) {
                Assert.True(arr[i]);
            }
        }

        [Fact]
        public void SingleBitFlipsOn()
        {
            for(int len=0; len < 258; len++) {
                TestSingleBitFlipsOn(123);
            }   
        }

        [Fact]
        public void SingleBitFlipsOff()
        {
            for(int len=0; len < 258; len++) {
                TestSingleBitFlipsOff(123);
            }
        }

        [Fact]
        public void BoundsTests()
        {
            for(int len=1; len < 258; len++)
            {
                var arr = new BoolArray(len);

                Assert.Throws<IndexOutOfRangeException>(() => { bool b = arr[-1]; });
                Assert.Throws<IndexOutOfRangeException>(() => { bool b = arr[arr.Length]; });
                Assert.False(arr[^1]);
            }
        }

        #endregion

        #region Private Static Methods

        private static void TestSingleBitFlipsOn(int len)
        {
            for(int i=0; i < len; i++)
            {
                var arr = new BoolArray(len);
                arr[i] = true;

                // Test all leading bits.
                for(int j=0; j < i; j++)
                {
                    Assert.False(arr[j]);
                }

                // Test flipped bit.
                Assert.True(arr[i]);

                // Test all following bits.
                for(int j = i+1; j < len; j++)
                {
                    Assert.False(arr[j]);
                }
            }
        }

        private static void TestSingleBitFlipsOff(int len)
        {
            for(int i=0; i < len; i++)
            {
                var arr = new BoolArray(len, true);
                arr[i] = false;

                // Test all leading bits.
                for(int j=0; j < i; j++)
                {
                    Assert.True(arr[j]);
                }

                // Test flipped bit.
                Assert.False(arr[i]);

                // Test all following bits.
                for(int j = i+1; j < len; j++)
                {
                    Assert.True(arr[j]);
                }
            }
        }

        #endregion
    }
}
