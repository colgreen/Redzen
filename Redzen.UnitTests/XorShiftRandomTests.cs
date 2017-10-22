using System;
using MathNet.Numerics.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;

namespace Redzen.UnitTests
{
    [TestClass]
    public class XorShiftRandomTests
    {
        #region Test Methods [Integer Tests]

        [TestMethod]
        [TestCategory("XorShiftRandom")]
        public void Next()
        {
            int sampleCount = 10000000;
            XorShiftRandom rng = new XorShiftRandom();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = rng.Next();
            }

            UniformDistributionTest(sampleArr, 0.0, int.MaxValue);
        }

        [TestMethod]
        [TestCategory("XorShiftRandom")]
        public void NextUpper()
        {
            int sampleCount = 10000000;
            XorShiftRandom rng = new XorShiftRandom();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = rng.Next(1234567);
            }

            UniformDistributionTest(sampleArr, 0.0, 1234567);
        }

        [TestMethod]
        [TestCategory("XorShiftRandom")]
        public void NextLowerUpper()
        {
            int sampleCount = 10000000;
            XorShiftRandom rng = new XorShiftRandom();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = rng.Next(1000000, 1234567);
            }

            UniformDistributionTest(sampleArr, 1000000, 1234567);
        }

        [TestMethod]
        [TestCategory("XorShiftRandom")]
        public void NextLowerUpper_LongRange_Bounds()
        {
            int sampleCount = 10000000;
            XorShiftRandom rng = new XorShiftRandom();
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
        [TestCategory("XorShiftRandom")]
        public void NextLowerUpper_LongRange_Distribution()
        {
            int sampleCount = 10000000;
            XorShiftRandom rng = new XorShiftRandom();
            
            int maxValHalf = int.MaxValue / 2;
            int lowerBound = -(maxValHalf + 10000);
            int upperBound = (maxValHalf + 10000);

            // N.B. double precision can represent every Int32 value exactly.
            double[] sampleArr = new double[sampleCount];
            for(int i=0; i<sampleCount; i++) {
                sampleArr[i] = rng.Next(lowerBound, upperBound);
            }

            UniformDistributionTest(sampleArr, lowerBound, upperBound);
        }

        [TestMethod]
        [TestCategory("XorShiftRandom")]
        public void NextUInt()
        {
            int sampleCount = 10000000;
            XorShiftRandom rng = new XorShiftRandom();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = rng.NextUInt();
            }

            UniformDistributionTest(sampleArr, 0.0, uint.MaxValue);
        }

        [TestMethod]
        [TestCategory("XorShiftRandom")]
        public void NextInt()
        {
            int sampleCount = 10000000;
            XorShiftRandom rng = new XorShiftRandom();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = rng.NextInt();
            }

            UniformDistributionTest(sampleArr, 0.0, int.MaxValue + 1.0);
        }

        #endregion

        #region Test Methods [Floating Point Tests]

        [TestMethod]
        [TestCategory("XorShiftRandom")]
        public void NextDouble()
        {
            int sampleCount = 10000000;
            XorShiftRandom rng = new XorShiftRandom();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = rng.NextDouble();
            }

            UniformDistributionTest(sampleArr, 0.0, 1.0);
        }

        [TestMethod]
        [TestCategory("XorShiftRandom")]
        public void NextDoubleNonZero()
        {
            int sampleCount = 10000000;
            XorShiftRandom rng = new XorShiftRandom();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++)
            {
                sampleArr[i] = rng.NextDoubleNonZero();
                if(0.0 == sampleArr[i]) Assert.Fail();
            }

            UniformDistributionTest(sampleArr, 0.0, 1.0);
        }

        [TestMethod]
        [TestCategory("XorShiftRandom")]
        public void NextFloat()
        {
            int sampleCount = 10000000;
            XorShiftRandom rng = new XorShiftRandom();
            double[] sampleArr = new double[sampleCount];

            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = rng.NextFloat();
            }

            UniformDistributionTest(sampleArr, 0.0, 1.0);
        }

        #endregion

        #region Text Methods [Bytes / Bools]

        [TestMethod]
        [TestCategory("XorShiftRandom")]
        public void NextBool()
        {
            int sampleCount = 10000000;
            XorShiftRandom rng = new XorShiftRandom();
            
            int trueCount = 0, falseCount = 0;
            double maxExpectedCountErr = sampleCount / 25.0;

            for(int i=0; i<sampleCount; i++) {
                if(rng.NextBool()) trueCount++; else falseCount++; 
            }

            double countErr = Math.Abs(trueCount - falseCount);
            if(countErr > maxExpectedCountErr) Assert.Fail();
        }

        [TestMethod]
        [TestCategory("XorShiftRandom")]
        public void NextByte()
        {
            int sampleCount = 10000000;
            XorShiftRandom rng = new XorShiftRandom();
            byte[] sampleArr = new byte[sampleCount];
            for(int i=0; i<sampleCount; i++){
                sampleArr[i] = rng.NextByte();
                
            }
            NextByteInner(sampleArr);
        }

        [TestMethod]
        [TestCategory("XorShiftRandom")]
        public void NextBytes()
        {
            int sampleCount = 10000000;
            XorShiftRandom rng = new XorShiftRandom();
            byte[] sampleArr = new byte[sampleCount];
            rng.NextBytes(sampleArr);
            NextByteInner(sampleArr);
        }

        #if UNSAFE

        [TestMethod]
        [TestCategory("XorShiftRandom")]
        public void NextBytes8()
        {
            int sampleCount = 10000000;
            XorShiftRandom rng = new XorShiftRandom();
            byte[] sampleArr = new byte[sampleCount];
            rng.NextBytes8(sampleArr);
            NextByteInner(sampleArr);
        }

        #endif

        #endregion

        #region Private Static Methods

        private static void UniformDistributionTest(double[] sampleArr, double lowerBound, double upperBound)
        {
            Array.Sort(sampleArr);
            RunningStatistics runningStats = new RunningStatistics(sampleArr);

            // Skewness should be pretty close to zero (evenly distributed samples)
            if(Math.Abs(runningStats.Skewness) > 0.01) Assert.Fail();
            
            // Mean test.
            double range = upperBound - lowerBound;
            double expectedMean = lowerBound + (range / 2.0);
            double meanErr = expectedMean - runningStats.Mean;
            double maxExpectedErr = range / 1000.0;

            if(Math.Abs(meanErr) > maxExpectedErr) Assert.Fail();

            // Test a range of centile/quantile values.
            double tauStep = (upperBound - lowerBound) / 10.0;

            for(double tau=0; tau <= 1.0; tau += 0.1)
            {
                double quantile = SortedArrayStatistics.Quantile(sampleArr, tau);
                double expectedQuantile = lowerBound + (tau * range);
                double quantileError = expectedQuantile - quantile;
                if(Math.Abs(quantileError) > maxExpectedErr) Assert.Fail();
            }
        }

        private static void NextByteInner(byte[] sampleArr)
        {
            int[] countArr = new int[256];
            int sampleCount = sampleArr.Length;
            for(int i=0; i<sampleCount; i++) {
                countArr[sampleArr[i]]++;
            }

            double expectedCount = sampleCount / 256;
            double maxExpectedCountErr = sampleCount / 10000;
            for(int i=0; i<256; i++)
            {
                double countErr = Math.Abs(countArr[i] - expectedCount);
                if(countErr > maxExpectedCountErr) Assert.Fail();
            }
        }

        #endregion
    }
}
