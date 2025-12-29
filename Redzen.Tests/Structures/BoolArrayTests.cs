using FluentAssertions;
using Xunit;

namespace Redzen.Structures;

public class BoolArrayTests
{
    [Fact]
    public void InitFalse()
    {
        const int len = 123;

        var arr = new BoolArray(len);
        for(int i=0; i < len; i++)
            arr[i].Should().BeFalse();

        arr = new BoolArray(len, false);
        for(int i=0; i < len; i++)
            arr[i].Should().BeFalse();
    }

    [Fact]
    public void InitTrue()
    {
        const int len = 123;

        var arr = new BoolArray(len, true);

        for(int i=0; i < len; i++)
            arr[i].Should().BeTrue();
    }

    [Fact]
    public void SingleBitFlipsOn()
    {
        for(int len=0; len < 258; len++)
            TestSingleBitFlipsOn(123);
    }

    [Fact]
    public void SingleBitFlipsOff()
    {
        for(int len=0; len < 258; len++)
            TestSingleBitFlipsOff(123);
    }

    [Fact]
    public void BoundsTests()
    {
        for(int len=1; len < 258; len++)
        {
            var arr = new BoolArray(len);

            ((Action)(() => { bool b = arr[-1]; })).Should().Throw<IndexOutOfRangeException>();
            ((Action)(() => { bool b = arr[arr.Length]; })).Should().Throw<IndexOutOfRangeException>();
            arr[^1].Should().BeFalse();
        }
    }

    private static void TestSingleBitFlipsOn(int len)
    {
        for(int i=0; i < len; i++)
        {
            var arr = new BoolArray(len);
            arr[i] = true;

            // Test all leading bits.
            for(int j=0; j < i; j++)
                arr[j].Should().BeFalse();

            // Test flipped bit.
            arr[i].Should().BeTrue();

            // Test all following bits.
            for(int j = i+1; j < len; j++)
                arr[j].Should().BeFalse();
        }
    }

    private static void TestSingleBitFlipsOff(int len)
    {
        for(int i=0; i < len; i++)
        {
            var arr = new BoolArray(len, true);
            arr[i] = false;

            // Test all leading bits.
            for(int j=0; j < i; j++)
                arr[j].Should().BeTrue();

            // Test flipped bit.
            arr[i].Should().BeFalse();

            // Test all following bits.
            for(int j = i+1; j < len; j++)
                arr[j].Should().BeTrue();
        }
    }
}
