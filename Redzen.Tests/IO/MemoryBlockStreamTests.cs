using FluentAssertions;
using Redzen.Random;
using Xunit;

namespace Redzen.IO;

public class MemoryBlockStreamTests
{
    [Fact]
    public void MemoryBlockStreamFuzzer()
    {
        MemoryStream ms = new();
        MemoryBlockStream ms2 = new();

        MemoryStreamFuzzer fuzzer = new(ms, ms2, 0);
        for(int i=0; i < 1000; i++)
        {
            fuzzer.PerformMultipleOps(100);
            CompareState(ms, ms2);

            if(ms.Length > 3e9)
            {
                ms.Dispose();
                ms2.Dispose();

                ms = new MemoryStream();
                ms2 = new MemoryBlockStream();
            }
        }

        ms.Dispose();
        ms2.Dispose();
    }

    [Fact]
    public void WriteZeroBytes()
    {
        byte[] buf = Array.Empty<byte>();
        using MemoryBlockStream ms = new();
        ms.Write(buf, 0, 0);
        Assert.Equal(0, ms.Length);

        IRandomSource rng = RandomDefaults.CreateRandomSource(1234567);
        byte[] buf2 = new byte[100];
        rng.NextBytes(buf2);
        ms.Write(buf2);

        buf2.Should().BeEquivalentTo(ms.ToArray());

        ms.Write(buf, 0, 0);
        buf2.Length.Should().Be((int)ms.Length);
    }

    private static void CompareState(MemoryStream ms, MemoryBlockStream ms2)
    {
        // Compare byte content.
        byte[] buff1 = ms.ToArray();
        byte[] buff2 = ms2.ToArray();

        // Compare read/write position.
        buff2.Should().BeEquivalentTo(buff1);
        ms2.Position.Should().Be(ms.Position);
    }
}
