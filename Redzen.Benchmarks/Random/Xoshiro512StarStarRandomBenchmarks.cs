using BenchmarkDotNet.Attributes;

namespace Redzen.Random;

public class Xoshiro512StarStarRandomBenchmarks
{
    const int __loops = 10_000_000;
    readonly Xoshiro512StarStarRandom _rng = new();
    readonly byte[] _buff = new byte[1_000_000];

    #region Benchmark Methods [System.Random Equivalents]

    [Benchmark]
    public void Next10M()
    {
        for(int i=0; i < __loops; i++)
            _rng.Next();
    }

    [Benchmark]
    public void NextUpperBound10M()
    {
        for(int i=0; i < __loops; i++)
            _rng.Next(123);
    }

    [Benchmark]
    public void NextLowerUpperBound10M()
    {
        for(int i=0; i < __loops; i++)
            _rng.Next(1000, 10_000);
    }

    [Benchmark]
    public void NextDouble10M()
    {
        for(int i=0; i < __loops; i++)
            _rng.NextDouble();
    }

    [Benchmark]
    public void NextDoubleHighRes10M()
    {
        for(int i=0; i < __loops; i++)
            _rng.NextDoubleHighRes();
    }

    [Benchmark]
    public void NextBytes100M()
    {
        for(int i=0; i < 100; i++)
            _rng.NextBytes(_buff);
    }

    #endregion

    #region Benchmark Methods [Methods not present on System.Random]

    [Benchmark]
    public void NextFloat10M()
    {
        for(int i=0; i < __loops; i++)
            _rng.NextFloat();
    }

    [Benchmark]
    public void NextUInt10M()
    {
        for(int i=0; i < __loops; i++)
            _rng.NextUInt();
    }

    [Benchmark]
    public void NextULong10M()
    {
        for(int i=0; i < __loops; i++)
            _rng.NextULong();
    }

    [Benchmark]
    public void NextInt()
    {
        for(int i=0; i < __loops; i++)
            _rng.NextInt();
    }

    [Benchmark]
    public void NextDoubleNonZero10M()
    {
        for(int i=0; i < __loops; i++)
            _rng.NextDoubleNonZero();
    }

    [Benchmark]
    public void NextUnitInterval_Double_10M()
    {
        for(int i = 0; i < __loops; i++)
            _rng.NextUnitInterval<double>();
    }

    [Benchmark]
    public void NextUnitInterval_Float_10M()
    {
        for(int i = 0; i < __loops; i++)
            _rng.NextUnitInterval<float>();
    }

    [Benchmark]
    public void NextBool()
    {
        for(int i=0; i < __loops; i++)
            _rng.NextBool();
    }

    [Benchmark]
    public void NextByte()
    {
        for(int i=0; i < __loops; i++)
            _rng.NextByte();
    }

    #endregion
}
