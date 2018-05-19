using System.Runtime.CompilerServices;

namespace Redzen
{

    public static class BitwiseUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateLeft(ulong x, int k)
        {
            // Note. RyuJIT will compile this to a single rotate CPU instruction (as of about .NET 4.6.1 and dotnet core 2.0).
            return (x << k) | (x >> (64 - k));
        }
    }
}
