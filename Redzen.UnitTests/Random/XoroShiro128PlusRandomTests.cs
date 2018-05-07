using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;
using static Redzen.UnitTests.Random.RandomTestUtils;

namespace Redzen.UnitTests.Random
{
    [TestClass]
    public class XoroShiro128PlusRandomTests
    {
        #region Test Methods [Integer Tests]

        [TestMethod]
        [TestCategory("XoroShiro128PlusRandom")]
        public void Next()
        {
            int sampleCount = 10_000_000;
            var rng = new Xoroshiro128PlusRandom();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = rng.Next();
            }

            UniformDistributionTest(sampleArr, 0.0, int.MaxValue);
        }

        [TestMethod]
        [TestCategory("XoroShiro128PlusRandom")]
        public void NextUpper()
        {
            int sampleCount = 10_000_000;
            var rng = new Xoroshiro128PlusRandom();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = rng.Next(1234567);
            }

            UniformDistributionTest(sampleArr, 0.0, 1_234_567);
        }

        [TestMethod]
        [TestCategory("XoroShiro128PlusRandom")]
        public void NextLowerUpper()
        {
            int sampleCount = 10_000_000;
            var rng = new Xoroshiro128PlusRandom();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = rng.Next(1_000_000, 1_234_567);
            }

            UniformDistributionTest(sampleArr, 1_000_000, 1_234_567);
        }

        [TestMethod]
        [TestCategory("XoroShiro128PlusRandom")]
        public void NextLowerUpper_LongRange_Bounds()
        {
            int sampleCount = 10_000_000;
            var rng = new Xoroshiro128PlusRandom();
            System.Random sysRng = new System.Random();

            int maxValHalf = int.MaxValue / 2;
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++)
            {
                int lowerBound = -(maxValHalf + (sysRng.Next()/2));
                int upperBound = (maxValHalf + (sysRng.Next()/2));
                int sample = rng.Next(lowerBound, upperBound);

                if(sample < lowerBound || sample >= upperBound) {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        [TestCategory("XoroShiro128PlusRandom")]
        public void NextLowerUpper_LongRange_Distribution()
        {
            int sampleCount = 10_000_000;
            var rng = new Xoroshiro128PlusRandom();
            
            int maxValHalf = int.MaxValue / 2;
            int lowerBound = -(maxValHalf + 10_000);
            int upperBound = (maxValHalf + 10_000);

            // N.B. double precision can represent every Int32 value exactly.
            double[] sampleArr = new double[sampleCount];
            for(int i=0; i<sampleCount; i++) {
                sampleArr[i] = rng.Next(lowerBound, upperBound);
            }

            UniformDistributionTest(sampleArr, lowerBound, upperBound);
        }

        [TestMethod]
        [TestCategory("XoroShiro128PlusRandom")]
        public void NextUInt()
        {
            int sampleCount = 10_000_000;
            var rng = new Xoroshiro128PlusRandom();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = rng.NextUInt();
            }

            UniformDistributionTest(sampleArr, 0.0, uint.MaxValue);
        }

        [TestMethod]
        [TestCategory("XoroShiro128PlusRandom")]
        public void NextInt()
        {
            int sampleCount = 10_000_000;
            var rng = new Xoroshiro128PlusRandom();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = rng.NextInt();
            }

            UniformDistributionTest(sampleArr, 0.0, int.MaxValue + 1.0);
        }

        #endregion

        #region Test Methods [Floating Point Tests]

        [TestMethod]
        [TestCategory("XoroShiro128PlusRandom")]
        public void NextDouble()
        {
            int sampleCount = 10_000_000;
            var rng = new Xoroshiro128PlusRandom();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = rng.NextDouble();
            }

            UniformDistributionTest(sampleArr, 0.0, 1.0);
        }

        [TestMethod]
        [TestCategory("XoroShiro128PlusRandom")]
        public void NextDoubleNonZero()
        {
            int sampleCount = 10_000_000;
            var rng = new Xoroshiro128PlusRandom();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++)
            {
                sampleArr[i] = rng.NextDoubleNonZero();
                if(0.0 == sampleArr[i]) Assert.Fail();
            }

            UniformDistributionTest(sampleArr, 0.0, 1.0);
        }

        [TestMethod]
        [TestCategory("XoroShiro128PlusRandom")]
        public void NextFloat()
        {
            int sampleCount = 10_000_000;
            var rng = new Xoroshiro128PlusRandom();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = rng.NextFloat();
            }

            UniformDistributionTest(sampleArr, 0.0, 1.0);
        }

        #endregion

        #region Text Methods [Bytes / Bools]

        [TestMethod]
        [TestCategory("XoroShiro128PlusRandom")]
        public void NextBool()
        {
            int sampleCount = 10_000_000;
            var rng = new Xoroshiro128PlusRandom();
            
            int trueCount = 0, falseCount = 0;
            double maxExpectedCountErr = sampleCount / 25.0;

            for(int i=0; i<sampleCount; i++) {
                if(rng.NextBool()) trueCount++; else falseCount++; 
            }

            double countErr = Math.Abs(trueCount - falseCount);
            if(countErr > maxExpectedCountErr) Assert.Fail();
        }

        [TestMethod]
        [TestCategory("XoroShiro128PlusRandom")]
        public void NextByte()
        {
            int sampleCount = 10_000_000;
            var rng = new Xoroshiro128PlusRandom();
            byte[] sampleArr = new byte[sampleCount];
            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = rng.NextByte();
                
            }
            NextByteInner(sampleArr);
        }

        [TestMethod]
        [TestCategory("XoroShiro128PlusRandom")]
        public void NextBytes()
        {
            int sampleCount = 10_000_000;
            var rng = new Xoroshiro128PlusRandom();
            byte[] sampleArr = new byte[sampleCount];
            rng.NextBytes(sampleArr);
            NextByteInner(sampleArr);
        }

        [TestMethod]
        [TestCategory("XoroShiro128PlusRandom")]
        public void NextBytes_LengthNotMultipleOfFour()
        {
            // Note. We want to check that the last three bytes are being assigned random bytes, but the RNG
            // can generate zeroes, so this test is reliant on the RNG seed being fixed to ensure we have non-zero 
            // values in those elements each time the test is run.
            int sampleCount = 10_000_003;
            var rng = new Xoroshiro128PlusRandom(0);
            byte[] sampleArr = new byte[sampleCount];
            rng.NextBytes(sampleArr);
            NextByteInner(sampleArr);

            Assert.IsTrue(sampleArr[sampleCount-1] != 0);
            Assert.IsTrue(sampleArr[sampleCount-2] != 0);
            Assert.IsTrue(sampleArr[sampleCount-3] != 0);
        }

        #endregion
    }
}
