// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using Redzen.Random;

namespace Redzen.Numerics.Distributions;

/// <summary>
/// Static factory methods for creating instances of ISampler{T} and IStatelessSampler{T} for sampling from uniform distributions.
/// </summary>
public static class UniformDistributionSamplerFactory
{
    /// <summary>
    /// Create a sampler for the uniform distribution with interval [0,1).
    /// </summary>
    /// <typeparam name="T">Data type of the samples.</typeparam>
    /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
    public static ISampler<T> CreateSampler<T>()
        where T : struct, IBinaryFloatingPointIeee754<T>
    {
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        return CreateSampler(T.One, false, rng);
    }

    /// <summary>
    /// Create a sampler for the uniform distribution with interval [0,1).
    /// </summary>
    /// <typeparam name="T">Data type of the samples.</typeparam>
    /// <param name="seed">Random source seed.</param>
    /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
    public static ISampler<T> CreateSampler<T>(ulong seed)
        where T : struct, IBinaryFloatingPointIeee754<T>
    {
        IRandomSource rng = RandomDefaults.CreateRandomSource(seed);
        return CreateSampler(T.One, false, rng);
    }

    /// <summary>
    /// Create a sampler for the uniform distribution with interval [0,1).
    /// </summary>
    /// <typeparam name="T">Data type of the samples.</typeparam>
    /// <param name="rng">Random source.</param>
    /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
    public static ISampler<T> CreateSampler<T>(IRandomSource rng)
        where T : struct, IBinaryFloatingPointIeee754<T>
    {
        return CreateSampler(T.One, false, rng);
    }

    /// <summary>
    /// Create a sampler for the uniform distribution with interval [0,max) or (-max,max).
    /// </summary>
    /// <typeparam name="T">Data type of the samples.</typeparam>
    /// <param name="max">Maximum value (exclusive).</param>
    /// <param name="signed">If true the distribution has interval (-max,max); otherwise [0,max).</param>
    /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
    public static ISampler<T> CreateSampler<T>(T max, bool signed)
        where T : struct, IBinaryFloatingPointIeee754<T>
    {
        IRandomSource rng = RandomDefaults.CreateRandomSource();
        return CreateSampler(max, signed, rng);
    }

    /// <summary>
    /// Create a sampler for the uniform distribution with interval [0,max) or (-max,max).
    /// </summary>
    /// <typeparam name="T">Data type of the samples.</typeparam>
    /// <param name="max">Maximum value (exclusive).</param>
    /// <param name="signed">If true the distribution has interval (-max,max); otherwise [0,max).</param>
    /// <param name="seed">Random source seed.</param>
    /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
    public static ISampler<T> CreateSampler<T>(T max, bool signed, ulong seed)
        where T : struct, IBinaryFloatingPointIeee754<T>
    {
        IRandomSource rng = RandomDefaults.CreateRandomSource(seed);
        return CreateSampler(max, signed, rng);
    }

    /// <summary>
    /// Create a sampler for the uniform distribution with interval [0,max) or (-max,max).
    /// </summary>
    /// <typeparam name="T">Data type of the samples.</typeparam>
    /// <param name="max">Maximum value (exclusive).</param>
    /// <param name="signed">If true the distribution has interval (-max,max); otherwise [0,max).</param>
    /// <param name="rng">Random source.</param>
    /// <returns>A new instance of <see cref="ISampler{T}"/>.</returns>
    public static ISampler<T> CreateSampler<T>(T max, bool signed, IRandomSource rng)
        where T : struct, IBinaryFloatingPointIeee754<T>
    {
        if(typeof(T) == typeof(double))
        {
            return (ISampler<T>)new Double.UniformDistributionSampler(double.CreateChecked(max), signed, rng);
        }
        else if(typeof(T) == typeof(float))
        {
            return (ISampler<T>)new Float.UniformDistributionSampler(float.CreateChecked(max), signed, rng);
        }
        else
        {
            throw new ArgumentException("Unsupported type argument");
        }
    }

    /// <summary>
    /// Create a stateless sampler for the uniform distribution with interval [0,1).
    /// </summary>
    /// <typeparam name="T">Data type of the samples.</typeparam>
    /// <returns>A new instance of <see cref="IStatelessSampler{T}"/>.</returns>
    public static IStatelessSampler<T> CreateStatelessSampler<T>()
        where T : struct, IBinaryFloatingPointIeee754<T>
    {
        return CreateStatelessSampler(T.One, false);
    }

    /// <summary>
    /// Create a stateless sampler for the uniform distribution with interval [0,max) or (-max,max).
    /// </summary>
    /// <typeparam name="T">Data type of the samples.</typeparam>
    /// <param name="max">Maximum value (exclusive).</param>
    /// <param name="signed">If true the distribution has interval (-max,max); otherwise [0,max).</param>
    /// <returns>A new instance of <see cref="IStatelessSampler{T}"/>.</returns>
    public static IStatelessSampler<T> CreateStatelessSampler<T>(T max, bool signed)
        where T : struct, IBinaryFloatingPointIeee754<T>
    {
        if(typeof(T) == typeof(double))
        {
            return (IStatelessSampler<T>)new Double.UniformDistributionStatelessSampler(double.CreateChecked(max), signed);
        }
        else if(typeof(T) == typeof(float))
        {
            return (IStatelessSampler<T>)new Float.UniformDistributionStatelessSampler(float.CreateChecked(max), signed);
        }
        else
        {
            throw new ArgumentException("Unsupported type argument");
        }
    }
}
