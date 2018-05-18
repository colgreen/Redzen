using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;
using static Redzen.UnitTests.Random.RandomTestUtils;

namespace Redzen.UnitTests.Random
{
    public abstract class RandomSourceTests
    {
        #region Test Methods [Integer Tests]

        [TestMethod]
        [TestCategory("RandomSource")]
        public void Next()
        {
            int sampleCount = 10_000_000;
            var rng = CreateRandomSource();
            double[] sampleArr = CreateSampleArray(sampleCount, () => rng.Next());
            UniformDistributionTest(sampleArr, 0.0, int.MaxValue);
        }

        [TestMethod]
        [TestCategory("RandomSource")]
        public void NextMax()
        {
            int sampleCount = 10_000_000;
            var rng = CreateRandomSource();
            double[] sampleArr = CreateSampleArray(sampleCount, () => rng.Next(1234567));
            UniformDistributionTest(sampleArr, 0.0, 1_234_567);
        }

        [TestMethod]
        [TestCategory("RandomSource")]
        public void NextMax_ArgumentBounds()
        {
            var rng = CreateRandomSource();

            // Out of range.
            Assert.ThrowsException<ArgumentOutOfRangeException>(()=>rng.Next(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(()=>rng.Next(0));

            // Within range.
            int x = rng.Next(1);
            Assert.AreEqual(0, x);

            x = rng.Next(int.MaxValue);
            Assert.IsTrue(x >=0 && x < int.MaxValue);
        }

        [TestMethod]
        [TestCategory("RandomSource")]
        public void NextMinMax()
        {
            int sampleCount = 10_000_000;
            var rng = CreateRandomSource();
            double[] sampleArr = CreateSampleArray(sampleCount, () => rng.Next(1_000_000, 1_234_567));
            UniformDistributionTest(sampleArr, 1_000_000, 1_234_567);
        }

        [TestMethod]
        [TestCategory("RandomSource")]
        public void NextMinMax_LongRange()
        {
            int sampleCount = 10_000_000;
            var rng = CreateRandomSource();
            System.Random sysRng = new System.Random();

            int maxValHalf = int.MaxValue / 2;

            for (int i = 0; i < sampleCount; i++)
            {
                int lowerBound = -(maxValHalf + (sysRng.Next() / 2));
                int upperBound = (maxValHalf + (sysRng.Next() / 2));
                int sample = rng.Next(lowerBound, upperBound);

                if (sample < lowerBound || sample >= upperBound)
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        [TestCategory("RandomSource")]
        public void NextMinMax_LongRange_Distribution()
        {
            int sampleCount = 10_000_000;
            var rng = CreateRandomSource();

            int maxValHalf = int.MaxValue / 2;
            int lowerBound = -(maxValHalf + 10_000);
            int upperBound = (maxValHalf + 10_000);

            // N.B. double precision can represent every Int32 value exactly.
            double[] sampleArr = CreateSampleArray(sampleCount, () => rng.Next(lowerBound, upperBound));
            UniformDistributionTest(sampleArr, lowerBound, upperBound);
        }

        [TestMethod]
        [TestCategory("RandomSource")]
        public void NextUInt()
        {
            int sampleCount = 10_000_000;
            var rng = CreateRandomSource();
            double[] sampleArr = CreateSampleArray(sampleCount, () => rng.NextUInt());
            UniformDistributionTest(sampleArr, 0.0, uint.MaxValue);
        }

        [TestMethod]
        [TestCategory("RandomSource")]
        public void NextInt()
        {
            int sampleCount = 10_000_000;
            var rng = CreateRandomSource();
            double[] sampleArr = CreateSampleArray(sampleCount, () => rng.NextInt());
            UniformDistributionTest(sampleArr, 0.0, int.MaxValue + 1.0);
        }

        [TestMethod]
        [TestCategory("RandomSource")]
        public void NextULong()
        {
            int sampleCount = 10_000_000;
            var rng = CreateRandomSource();
            double[] sampleArr = CreateSampleArray(sampleCount, () => rng.NextULong());
            UniformDistributionTest(sampleArr, 0.0, ulong.MaxValue);
        }

        #endregion

        #region Test Methods [Floating Point Tests]

        [TestMethod]
        [TestCategory("RandomSource")]
        public void NextDouble()
        {
            int sampleCount = 10_000_000;
            var rng = CreateRandomSource();
            double[] sampleArr = CreateSampleArray(sampleCount, () => rng.NextDouble());
            UniformDistributionTest(sampleArr, 0.0, 1.0);
        }

        [TestMethod]
        [TestCategory("RandomSource")]
        public void NextDoubleHighRes()
        {
            int sampleCount = 10_000_000;
            var rng = CreateRandomSource();
            double[] sampleArr = CreateSampleArray(sampleCount, () => rng.NextDoubleHighRes());
            UniformDistributionTest(sampleArr, 0.0, 1.0);
        }

        [TestMethod]
        [TestCategory("RandomSource")]
        public void NextDoubleNonZero()
        {
            int sampleCount = 10_000_000;
            var rng = CreateRandomSource();
            double[] sampleArr = new double[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                sampleArr[i] = rng.NextDoubleNonZero();
                if (0.0 == sampleArr[i]) Assert.Fail();
            }

            UniformDistributionTest(sampleArr, 0.0, 1.0);
        }

        [TestMethod]
        [TestCategory("RandomSource")]
        public void NextFloat()
        {
            int sampleCount = 10_000_000;
            var rng = CreateRandomSource();

            double[] sampleArr = CreateSampleArray(sampleCount, () => rng.NextFloat());
            UniformDistributionTest(sampleArr, 0.0, 1.0);
        }

        #endregion

        #region Test Methods [Bytes / Bools]

        [TestMethod]
        [TestCategory("RandomSource")]
        public void NextBool()
        {
            int sampleCount = 10_000_000;
            var rng = CreateRandomSource();

            int trueCount = 0, falseCount = 0;
            double maxExpectedCountErr = sampleCount / 25.0;

            for (int i = 0; i < sampleCount; i++) {
                if (rng.NextBool()) trueCount++; else falseCount++;
            }

            double countErr = Math.Abs(trueCount - falseCount);
            if (countErr > maxExpectedCountErr) Assert.Fail();
        }

        [TestMethod]
        [TestCategory("RandomSource")]
        public void NextByte()
        {
            int sampleCount = 10_000_000;
            var rng = CreateRandomSource();
            byte[] sampleArr = new byte[sampleCount];
            for (int i = 0; i < sampleCount; i++) {
                sampleArr[i] = rng.NextByte();
            }
            NextByteInner(sampleArr);
        }

        [TestMethod]
        [TestCategory("RandomSource")]
        public void NextBytes()
        {
            int sampleCount = 10_000_000;
            var rng = CreateRandomSource();
            byte[] sampleArr = new byte[sampleCount];
            rng.NextBytes(sampleArr);
            NextByteInner(sampleArr);
        }

        [TestMethod]
        [TestCategory("RandomSource")]
        public void NextBytes_LengthNotMultipleOfFour()
        {
            // Note. We want to check that the last three bytes are being assigned random bytes, but the RNG
            // can generate zeroes, so this test is reliant on the RNG seed being fixed to ensure we have non-zero 
            // values in those elements each time the test is run.
            int sampleCount = 10_000_003;
            var rng = CreateRandomSource();
            byte[] sampleArr = new byte[sampleCount];
            rng.NextBytes(sampleArr);
            NextByteInner(sampleArr);

            Assert.IsTrue(sampleArr[sampleCount - 1] != 0);
            Assert.IsTrue(sampleArr[sampleCount - 2] != 0);
            Assert.IsTrue(sampleArr[sampleCount - 3] != 0);
        }

        #endregion

        protected abstract IRandomSource CreateRandomSource();
    }
}
