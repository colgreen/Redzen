using System.Numerics;
using BenchmarkDotNet.Attributes;

#pragma warning disable CA1822 // Mark members as static

namespace Redzen.Benchmarks
{
    public class MathUtilsBenchmark
    {
        const int __loops = 10_000_000;

        [Benchmark]
        public void CeilingToPowerOfTwo_Int32()
        {
            for (int i = 0; i < __loops; i++)
            {
                MathUtils.CeilingToPowerOfTwo(i);
            }
        }

        [Benchmark]
        public void CeilingToPowerOfTwo_Int64()
        {
            for (long i = 0; i < __loops; i++)
            {
                MathUtils.CeilingToPowerOfTwo(i);
            }
        }

        [Benchmark]
        public void Log2Ceiling_UInt32()
        {
            for (uint i = 0; i < __loops; i++)
            {
                MathUtils.Log2Ceiling(i);
            }
        }

        [Benchmark]
        public void Log2Ceiling_UInt64()
        {
            for (ulong i = 0; i < __loops; i++)
            {
                MathUtils.Log2Ceiling(i);
            }
        }

        // Alternative implementations that avoid a conditional branch.
        // At time of writing these execute mush slower than the version with the conditional branch.

        private static int Log2Ceiling2(uint x)
        {
            int lzcnt = BitOperations.LeadingZeroCount(x | 1);

            // Clear high bit of x.
            x &= ~(1u << (31 ^ lzcnt));

            // Add one to final result if any of the remaining bits are set.
            // Popcount method.
            return (31 ^ lzcnt) + BitOperations.PopCount((uint)BitOperations.PopCount((uint)BitOperations.PopCount((uint)BitOperations.PopCount(x))));
        }

        private static int Log2Ceiling3(uint x)
        {
            int lzcnt = BitOperations.LeadingZeroCount(x | 1u);

            // Clear high bit of x.
            x &= ~(1u << (31 ^ lzcnt));

            // Add one to final result if any of the remaining bits are set.
            // Trailing zero shift method.

            // Of the remaining bits, find position of lowest set bit (if any).
            int tzcnt = BitOperations.TrailingZeroCount(x);

            // Shift lowest set bit to position zero, and clear all other bits.
            // Add to 31 - lzcnt.
            return (31 ^ lzcnt) + (int)((x >> tzcnt) & 0x1u);
        }

        private static int Log2Ceiling2(ulong x)
        {
            int lzcnt = BitOperations.LeadingZeroCount(x | 1ul);

            // Clear high bit of x.
            x &= ~(1ul << (63 ^ lzcnt));

            // Add one to final result if any of the remaining bits are set.
            // Popcount method.
            return (63 ^ lzcnt) + BitOperations.PopCount((uint)BitOperations.PopCount((uint)BitOperations.PopCount((uint)BitOperations.PopCount(x))));
        }

        private static int Log2Ceiling3(ulong x)
        {
            int lzcnt = BitOperations.LeadingZeroCount(x | 1ul);

            // Clear high bit of x.
            x &= ~(1ul << (63 ^ lzcnt));

            // Add one to final result if any of the remaining bits are set.
            // Trailing zero shift method.

            // Of the remaining bits, find position of lowest set bit (if any).
            int tzcnt = BitOperations.TrailingZeroCount(x);

            // Shift lowest set bit to position zero, and clear all other bits.
            // Add to 31 - lzcnt.
            return (63 ^ lzcnt) + (int)((x >> tzcnt) & 0x1ul);
        }
    }
}
