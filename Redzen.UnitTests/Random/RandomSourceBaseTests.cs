using Xunit;

namespace Redzen.UnitTests.Random
{
    public class RandomSourceBaseTests
    {
        // Constants.
        const double INCR_DOUBLE = 1.0 / (1UL << 53);
        const float INCR_FLOAT = 1f / (1U << 24);

        [Fact]
        public void Next_MinVal()
        {
            var rng = new ConstantRandomSource(0UL);
            int x = rng.Next();
            Assert.Equal(0, x);
        }

        [Fact]
        public void Next_MaxVal()
        {
            var rng = new ConstantRandomSource(0x7fff_fffeUL);
            int x = rng.Next();
            Assert.Equal(int.MaxValue-1, x);
        }

        [Fact]
        public void NextMax_MinVal()
        {
            var rng = new ConstantRandomSource(0UL);
            int x = rng.Next(1_234_567);
            Assert.Equal(0, x);
        }

        [Fact]
        public void NextMax_MaxVal()
        {
            const int max = 1_234_567;
            var rng = new ConstantRandomSource(((ulong)(max-1)) << 43);
            int x = rng.Next(max);
            Assert.Equal(max-1, x);   
        }

        [Fact]
        public void NextMinMax_MinVal()
        {
            var rng = new ConstantRandomSource(0UL);
            int x = rng.Next(123, 1_234_567);
            Assert.Equal(123, x);
        }

        [Fact]
        public void NextMinMax_MaxVal()
        {
            const int min = 123;
            const int max = 1_234_567;

            var rng = new ConstantRandomSource(((ulong)((max - min) - 1)) << 43);
            int x = rng.Next(min, max);
            Assert.Equal(max-1, x);
        }

        [Fact]
        public void NextMinMax_LongRange_MinVal()
        {
            const int maxValHalf = int.MaxValue / 2;
            const int lowerBound = -maxValHalf;
            const int upperBound = maxValHalf + 123;

            var rng = new ConstantRandomSource(0UL);
            int x = rng.Next(lowerBound, upperBound);
            Assert.Equal(lowerBound, x);
        }

        [Fact]
        public void NextMinMax_LongRange_MaxVal()
        {
            const int maxValHalf = int.MaxValue / 2;
            const int lowerBound = -maxValHalf;
            const int upperBound = maxValHalf + 123;

            var rng = new ConstantRandomSource(0x8000_0078UL << 32);
            int x = rng.Next(lowerBound, upperBound);
            Assert.Equal(upperBound-1, x);
        }

        [Fact]
        public void NextDouble_MinVal()
        {
            var rng = new ConstantRandomSource(0UL);
            double x = rng.NextDouble();
            Assert.Equal(0.0, x);
        }

        [Fact]
        public void NextDouble_MaxVal()
        {
            var rng = new ConstantRandomSource(ulong.MaxValue);
            double x = rng.NextDouble();
            Assert.Equal(1.0 - INCR_DOUBLE, x);
        }

        [Fact]
        public void NextInt_MinVal()
        {
            var rng = new ConstantRandomSource(0UL);
            int x = rng.Next();
            Assert.Equal(0, x);
        }

        [Fact]
        public void NextInt_MaxVal()
        {
            var rng = new ConstantRandomSource(ulong.MaxValue);
            int x = rng.NextInt();
            Assert.Equal(int.MaxValue, x);
        }

        [Fact]
        public void NextUInt_MinVal()
        {
            var rng = new ConstantRandomSource(0UL);
            uint x = rng.NextUInt();
            Assert.Equal(0u, x);
        }

        [Fact]
        public void NextUInt_MaxVal()
        {
            var rng = new ConstantRandomSource(ulong.MaxValue);
            uint x = rng.NextUInt();
            Assert.Equal(uint.MaxValue, x);
        }

        [Fact]
        public void NextULong_MinVal()
        {
            var rng = new ConstantRandomSource(0UL);
            ulong x = rng.NextULong();
            Assert.Equal(0UL, x);
        }

        [Fact]
        public void NextULong_MaxVal()
        {
            var rng = new ConstantRandomSource(ulong.MaxValue);
            ulong x = rng.NextULong();
            Assert.Equal(ulong.MaxValue, x);
        }

        [Fact]
        public void NextBool_False()
        {
            var rng = new ConstantRandomSource(0UL);
            bool x = rng.NextBool();
            Assert.False(x);
        }

        [Fact]
        public void NextBool_True()
        {
            var rng = new ConstantRandomSource(0x8000_0000_0000_0000UL);
            bool x = rng.NextBool();
            Assert.True(x);
        }

        [Fact]
        public void NextByte_MinVal()
        {
            var rng = new ConstantRandomSource(0UL);
            byte x = rng.NextByte();
            Assert.Equal(0, x);
        }

        [Fact]
        public void NextByte_MaxVal()
        {
            var rng = new ConstantRandomSource(0xFFUL << 56);
            byte x = rng.NextByte();
            Assert.Equal(255, x);
        }

        [Fact]
        public void NextFloat_MinVal()
        {
            var rng = new ConstantRandomSource(0UL);
            float x = rng.NextFloat();
            Assert.Equal(0f, x);
        }

        [Fact]
        public void NextFloat_MaxVal()
        {
            var rng = new ConstantRandomSource(ulong.MaxValue);
            float x = rng.NextFloat();
            Assert.Equal(1.0 - INCR_FLOAT, x);
        }

        [Fact]
        public void NextFloatNonZero_Min()
        {
            var rng = new ConstantRandomSource(0UL);
            double x = rng.NextFloatNonZero();
            Assert.Equal(INCR_FLOAT, x);   
        }

        [Fact]
        public void NextFloatNonZero_Max()
        {
            var rng = new ConstantRandomSource(ulong.MaxValue);
            double x = rng.NextFloatNonZero();
            Assert.Equal(1f, x);   
        }

        [Fact]
        public void NextDoubleNonZero_Min()
        {
            var rng = new ConstantRandomSource(0UL);
            double x = rng.NextDoubleNonZero();
            Assert.Equal(INCR_DOUBLE, x);
        }

        [Fact]
        public void NextDoubleNonZero_Max()
        {
            var rng = new ConstantRandomSource(ulong.MaxValue);
            double x = rng.NextDoubleNonZero();
            Assert.Equal(1.0, x);
        }

        [Fact]
        public void NextDoubleHighRes_Min()
        {
            var rng = new ConstantRandomSource(0UL);
            double x = rng.NextDoubleHighRes();
            Assert.Equal(0.0, x);
        }

        [Fact]
        public void NextDoubleHighRes_Max()
        {
            var rng = new ConstantRandomSource(ulong.MaxValue);
            double x = rng.NextDoubleHighRes();
            Assert.Equal(1.0, x);
        }
    }
}
