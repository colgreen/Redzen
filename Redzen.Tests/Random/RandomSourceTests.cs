using FluentAssertions;
using Xunit;
using static Redzen.Random.RandomTestUtils;

namespace Redzen.Random;

public abstract class RandomSourceTests
{
    #region Test Methods [Integer Tests]

    [Fact]
    public void Next_UniformDistribution()
    {
        const int sampleCount = 10_000_000;
        var rng = CreateRandomSource();
        double[] sampleArr = CreateSampleArray(sampleCount, () => rng.Next());
        UniformDistributionTest(sampleArr, 0.0, int.MaxValue);
    }

    [Fact]
    public void NextMax_UniformDistribution()
    {
        const int sampleCount = 10_000_000;
        var rng = CreateRandomSource();
        double[] sampleArr = CreateSampleArray(sampleCount, () => rng.Next(1_234_567));
        UniformDistributionTest(sampleArr, 0.0, 1_234_567);
    }

    [Fact]
    public void NextMax_ArgumentBounds()
    {
        var rng = CreateRandomSource();

        // Out of range.
        rng.Invoking(x => x.Next(-1)).Should().Throw<ArgumentOutOfRangeException>();
        rng.Invoking(x => x.Next(0)).Should().Throw<ArgumentOutOfRangeException>();

        // Special case.
        int x = rng.Next(1);
        Assert.Equal(0, x);

        // int.MaxValue is within range.
        x = rng.Next(int.MaxValue);
        x.Should().BeInRange(0, int.MaxValue-1);
    }

    [Fact]
    public void NextMinMax_UniformDistribution()
    {
        const int sampleCount = 10_000_000;
        var rng = CreateRandomSource();
        double[] sampleArr = CreateSampleArray(sampleCount, () => rng.Next(1_000_000, 1_234_567));
        UniformDistributionTest(sampleArr, 1_000_000, 1_234_567);
    }

    [Fact]
    public void NextMinMax_LongRange()
    {
        const int sampleCount = 10_000_000;
        var rng = CreateRandomSource();
        System.Random sysRng = new();

        const int maxValHalf = int.MaxValue / 2;

        for(int i=0; i < sampleCount; i++)
        {
            // Generate lower and upper bounds, with a delta between them that is larger than int.MaxValue.
            int lowerBound = -(maxValHalf + (sysRng.Next() / 2));
            int upperBound = (maxValHalf + (sysRng.Next() / 2));

            // Sample a random value within the bounds, and verify it is within the bounds.
            int sample = rng.Next(lowerBound, upperBound);
            sample.Should().BeInRange(lowerBound, upperBound-1);
        }
    }

    [Fact]
    public void NextMinMax_LongRange_UniformDistribution()
    {
        const int sampleCount = 10_000_000;
        var rng = CreateRandomSource();

        int maxValHalf = int.MaxValue / 2;
        int lowerBound = -(maxValHalf + 10_000);
        int upperBound = (maxValHalf + 10_000);

        // Note. double precision can represent every Int32 value exactly.
        double[] sampleArr = CreateSampleArray(sampleCount, () => rng.Next(lowerBound, upperBound));
        UniformDistributionTest(sampleArr, lowerBound, upperBound);
    }

    [Fact]
    public void NextUInt_UniformDistribution()
    {
        const int sampleCount = 10_000_000;
        var rng = CreateRandomSource();
        double[] sampleArr = CreateSampleArray(sampleCount, () => rng.NextUInt());
        UniformDistributionTest(sampleArr, 0.0, uint.MaxValue + 1.0);
    }

    [Fact]
    public void NextInt_UniformDistribution()
    {
        const int sampleCount = 10_000_000;
        var rng = CreateRandomSource();
        double[] sampleArr = CreateSampleArray(sampleCount, () => rng.NextInt());
        UniformDistributionTest(sampleArr, 0.0, int.MaxValue + 1.0);
    }

    [Fact]
    public void NextULong_UniformDistribution()
    {
        const int sampleCount = 10_000_000;
        var rng = CreateRandomSource();
        double[] sampleArr = CreateSampleArray(sampleCount, () => rng.NextULong());
        UniformDistributionTest(sampleArr, 0.0, ulong.MaxValue + 1.0);
    }

    #endregion

    #region Test Methods [Floating Point Tests]

    [Fact]
    public void NextDouble_UniformDistribution()
    {
        const int sampleCount = 10_000_000;
        var rng = CreateRandomSource();
        double[] sampleArr = CreateSampleArray(sampleCount, () => rng.NextDouble());
        UniformDistributionTest(sampleArr, 0.0, 1.0);
    }

    [Fact]
    public void NextDoubleHighRes_UniformDistribution()
    {
        const int sampleCount = 10_000_000;
        var rng = CreateRandomSource();
        double[] sampleArr = CreateSampleArray(sampleCount, () => rng.NextDoubleHighRes());
        UniformDistributionTest(sampleArr, 0.0, 1.0);
    }

    [Fact]
    public void NextDoubleNonZero_UniformDistribution()
    {
        const int sampleCount = 10_000_000;
        var rng = CreateRandomSource();
        double[] sampleArr = new double[sampleCount];

        for(int i=0; i < sampleCount; i++)
        {
            sampleArr[i] = rng.NextDoubleNonZero();
            sampleArr[i].Should().NotBe(0.0);
        }

        UniformDistributionTest(sampleArr, 0.0, 1.0);
    }

    [Fact]
    public void NextFloat_UniformDistribution()
    {
        const int sampleCount = 10_000_000;
        var rng = CreateRandomSource();

        double[] sampleArr = CreateSampleArray(sampleCount, () => rng.NextFloat());
        UniformDistributionTest(sampleArr, 0.0, 1.0);
    }

    [Fact]
    public void NextHalf_UniformDistribution()
    {
        const int sampleCount = 10_000_000;
        var rng = CreateRandomSource();

        double[] sampleArr = CreateSampleArray(sampleCount, () => (double)rng.NextHalf());
        UniformDistributionTest(sampleArr, 0.0, 1.0);
    }

    #endregion

    #region Test Methods [Bytes / Bools]

    [Fact]
    public void NextBool_UniformDistribution()
    {
        const int sampleCount = 10_000_000;
        var rng = CreateRandomSource();

        int trueCount = 0, falseCount = 0;
        double maxExpectedCountErr = sampleCount / 25.0;

        for(int i=0; i < sampleCount; i++)
        {
            if(rng.NextBool())
                trueCount++;
            else 
                falseCount++;
        }

        double countErr = Math.Abs(trueCount - falseCount);
        countErr.Should().BeLessThanOrEqualTo(maxExpectedCountErr);
    }

    [Fact]
    public void NextByte_UniformDistribution()
    {
        const int sampleCount = 10_000_000;
        var rng = CreateRandomSource();
        byte[] sampleArr = new byte[sampleCount];
        for(int i=0; i < sampleCount; i++)
            sampleArr[i] = rng.NextByte();

        UniformDistributionTest(sampleArr);
    }

    [Fact]
    public void NextBytes_UniformDistribution()
    {
        const int sampleCount = 10_000_000;
        var rng = CreateRandomSource();
        byte[] sampleArr = new byte[sampleCount];
        rng.NextBytes(sampleArr);
        UniformDistributionTest(sampleArr);
    }

    [Fact]
    public void NextBytes_LengthNotMultipleOfFour()
    {
        // Note. We want to check that the last three bytes are being assigned random bytes, but the RNG
        // can generate zeroes, so this test is reliant on the RNG seed being fixed to ensure we have non-zero
        // values in those elements each time the test is run.
        const int sampleCount = 10_000_003;
        var rng = CreateRandomSource();
        byte[] sampleArr = new byte[sampleCount];
        rng.NextBytes(sampleArr);
        UniformDistributionTest(sampleArr);

        sampleArr[^1].Should().NotBe(0);
        sampleArr[^2].Should().NotBe(0);
        sampleArr[^3].Should().NotBe(0);
    }

    #endregion

    protected abstract IRandomSource CreateRandomSource();
}
