using FluentAssertions;
using Xunit;

namespace Redzen.Buffers;

public class RentedArraySpanTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(23)]
    [InlineData(100)]
    [InlineData(101)]
    public void RentedArrayShouldHaveCorrectLength(int length)
    {
        var rented = new RentedArraySpan<int>(length);

        rented.Should().NotBeNull();
        var span = rented.AsSpan();
        span.Length.Should().Be(length);
    }

    [Fact]
    public void RentingNegativeLengthShouldThrow()
    {
        Func<RentedArraySpan<int>> cstr = () => new RentedArraySpan<int>(-1);

        cstr.Should().Throw<ArgumentOutOfRangeException>();
    }
}
