# Redzen
A general purpose C# code library.

Available in nuget form at [nuget.org/packages/Redzen/](https://www.nuget.org/packages/Redzen/)

Much of the code in Redzen began life in [SharpNEAT](../../../../colgreen/sharpneat) and other projects, and has found a new home in the Redzen library to allow for reuse of
general purpose code.

This project endeavours to provide code that is high quality, well tested, clean, performant, and memory efficient. 


## Redzen top level namespaces

* Collections
* IO
* Linq
* Numerics
* Random
* Sorting
* Structures

## Redzen project map

* **Collections**
  * *IntStack*
    * A stack of int32 values. A simpler alternative to Stack\<int\> that provides additional Poke() and TryPoke() methods. Optimised for stacks of Int32.
  * *LightweightList*
    * A simpler alternative to List<T> that provides an AsSpan() method for efficient access to the internal array element.
  * *LightweightStack*
    * A simpler alternative to Stack\<T\> that provides additional Poke() and TryPoke() methods.
    
* **IO**
  * *Base64EncodingOutputStream*
    * Base64 stream encoder.
  * *FileByteArray*
    * Presents a 'byte array' backed by a file on disk.
  * *MemoryBlockStream*
    * A memory backed stream that stores byte data in blocks, this gives improved performance over System.IO.MemoryStream in some circumstances.
  * *NonClosingStreamWrapper*
    * Wraps a stream and prevents calls to Close() and Dispose() from being made on it.
  * *StreamHelper*
    * General purpose helper methods for working with streams.
    
* **Linq**
  * *EnumerableUtils*
    * Utility methods related to LINQ and IEnumerable.
    
* **Numerics**
  * **Distributions**
    * **Double**    
      * *BoxMullerGaussian*
        * Static methods for taking samples from Gaussian distributions using the Box-Muller transform.
      * *BoxMullerGaussianSampler*
        * A Gaussian distribution sampler based on the Box-Muller transform.
      * *BoxMullerGaussianStatelessSampler*
        * A stateless Gaussian distribution sampler based on the Box-Muller transform.
      * *UniformDistribution*
        * Static methods for taking samples from uniform distributions.
      * *UniformDistributionSampler*
        * A uniform distribution sampler.
      * *UniformDistributionStatelessSampler*
        * A stateless uniform distribution sampler.
      * *ZigguratGaussian*
        * Static methods for taking samples from Gaussian distributions using the Ziggurat algorithm.
      * *ZigguratGaussianSampler*
        * A Gaussian distribution sampler based on the Ziggurat algorithm.
      * *ZigguratGaussianStatelessSampler*
        * A stateless Gaussian distribution sampler based on the Ziggurat algorithm.
    * **Float**    
      * *BoxMullerGaussian*
        * Static methods for taking samples from Gaussian distributions using the Box-Muller transform.
      * *BoxMullerGaussianSampler*
        * A Gaussian distribution sampler based on the Box-Muller transform.
      * *BoxMullerGaussianStatelessSampler*
        * A stateless Gaussian distribution sampler based on the Box-Muller transform.
      * *UniformDistribution*
        * Static methods for taking samples from uniform distributions.
      * *UniformDistributionSampler*
        * A uniform distribution sampler.
      * *UniformDistributionStatelessSampler*
        * A stateless uniform distribution sampler.
      * *ZigguratGaussian*
        * Static methods for taking samples from Gaussian distributions using the Ziggurat algorithm.
      * *ZigguratGaussianSampler*
        * A Gaussian distribution sampler based on the Ziggurat algorithm.
      * *ZigguratGaussianStatelessSampler*
        * A stateless Gaussian distribution sampler based on the Ziggurat algorithm.

* **Random**
  * *DefaultRandomSeedSource*
    * A default source of seed values for use by pseudo-random number generators (PRNGs).
  * *RandomDefaults*
    * Provides a means of creating default implementations of IRandomSource, and also a standard way of generating seed values for PRNGs generally.
  * *Splitmix64Rng*
    * Splitmix64 Pseudo Random Number Generator (PRNG).
  * *WyRandom*
    * wyrand pseudo-random number generator. Uses the wyrand PRNG defined at https://github.com/wangyi-fudan/wyhash.
  * *XorShiftRandom*
    * xor-shift pseudo random number generator (PRNG) devised by George Marsaglia.
  * *Xoshiro256PlusPlusRandom*
    * xoshiro256+ (xor, shift, rotate) pseudo-random number generator (PRNG).
  * *Xoshiro256PlusRandom*
    * xoshiro256++ (xor, shift, rotate) pseudo-random number generator (PRNG).
  * *Xoshiro256StarStarRandom*
    * xoshiro256** (xor, shift, rotate) pseudo-random number generator (PRNG).
  * *Xoshiro512StarStarRandom*
    * xoshiro512** (xor, shift, rotate) pseudo-random number generator (PRNG).

* **Sorting**
  * *IntroSort\<K, V, W\>*
    * For sorting an array of key values, and two additional arrays based on the array of keys.
  * *SortUtils*
    * Helper methods related to sorting.
  * *TimSort\<T\>*
    * A timsort implementation.
  * *TimSort\<K,V\>*
    * A timsort implementation. This version accepts a secondary values array, the elements of which are repositioned in-line with their associated key values.
  * *TimSort\<K,V,W\>*
    * A timsort implementation. This version accepts two secondary values arrays, the elements of which are repositioned in-line with their associated key values.

* **Structures**
  * **Compact**
    * *CompactIntegerList*
      * A compact list of sequential integer values.
    * *FixedPointDecimal*
      * A fixed point decimal data type that uses Int32 for its underlying state, i.e. 4 bytes versus the native decimal's 16 bytes.
  * *BoolArray*
    * A leaner faster alternative to System.Collections.BitArray.
  * *CircularBuffer\<T\>*
    * A generic circular buffer of items of type T. 
  * *CircularBufferWithStats*
    * A circular buffer of double precision floating point values, that maintains a sum of the contained values, and therefore also the arithmetic mean.
  * *Int32Sequence*
    * Conveniently encapsulates a single Int32, which is incremented to produce new IDs.
  * *KeyedCircularBuffer\<K,V\>*
    *  A generic circular buffer of KeyValuePairs. The values are retrievable by their key.

* **Miscellany / Root Namespace**
* *FloatUtils*
  * Static utility methods for `IBinaryFloatingPointIeee754<T>` types.
* *MathSpan*
  * Math utility methods for working with spans.
* *MathUtils*
  * Math utility methods.
* *PrimeUtils*
  * Utility methods related to prime numbers.
* *SearchUtils*
  * Helper methods related to binary search.
* *SpanUtils*
  * Span static utility methods.
* *VariableUtils*
    * General purpose helper methods.
  
