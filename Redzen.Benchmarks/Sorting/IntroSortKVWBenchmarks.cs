﻿using BenchmarkDotNet.Attributes;
using Redzen.Random;

namespace Redzen.Sorting;

public class IntroSortKVWBenchmarks
{
    [Params(50_000)]
    public int ArrayLength;

    [Params(100)]
    public int ArrayCount;

    int[] _keysRandom;
    int[] _keysNaturalRandom;
    int[] _vals;
    int[] _vals2;
    int[][] _arrays;

    #region Public Methods

    [GlobalSetup]
    public void Setup()
    {
        // Alloc arrays.
        _keysRandom = new int[ArrayLength];
        _keysNaturalRandom = new int[ArrayLength];
        _vals = new int[ArrayLength];
        _vals2 = new int[ArrayLength];
        _arrays = new int[ArrayCount][];

        for(int i=0; i < _arrays.Length; i++)
            _arrays[i] = new int[ArrayLength];

        // Fill key arrays with random values.
        IRandomSource rng = RandomDefaults.CreateRandomSource(123);
        SortBenchmarkUtils.InitRandom(_keysRandom, rng);
        SortBenchmarkUtils.InitNatural(_keysNaturalRandom, rng);
    }

    [IterationSetup(Target = nameof(SortRandom))]
    public void IterationSetup_Random()
    {
        // Load a fresh copy of the random values into all test arrays prior to each benchmark iteration
        // (otherwise most iterations will be asked to sort data that is already sorted).
        InitArrays(_arrays, _keysRandom);
    }

    [IterationSetup(Target = nameof(SortNaturalRandom))]
    public void IterationSetup_NaturalRandom()
    {
        // Load a fresh copy of the random values into all test arrays prior to each benchmark iteration
        // (otherwise most iterations will be asked to sort data that is already sorted).
        InitArrays(_arrays, _keysNaturalRandom);
    }

    [Benchmark]
    public void SortRandom()
    {
        var valsSpan = _vals.AsSpan();
        var vals2Span = _vals2.AsSpan();

        for(int i=0; i < _arrays.Length; i++)
        {
            IntroSort.Sort<int,int,int>(_arrays[i], valsSpan, vals2Span);
        }
    }

    [Benchmark]
    public void SortNaturalRandom()
    {
        var valsSpan = _vals.AsSpan();
        var vals2Span = _vals2.AsSpan();

        for(int i=0; i < _arrays.Length; i++)
        {
            IntroSort.Sort<int,int,int>(_arrays[i], valsSpan, vals2Span);
        }
    }

    #endregion

    #region Private Static Methods

    private static void InitArrays(int[][] arrays, int[] sourceVals)
    {
        foreach(int[] arr in arrays)
            Array.Copy(sourceVals, arr, sourceVals.Length);
    }

    #endregion
}
