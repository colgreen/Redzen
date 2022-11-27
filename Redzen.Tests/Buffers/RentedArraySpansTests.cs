using FluentAssertions;
using Xunit;

namespace Redzen.Buffers;

public class RentedArraySpansTests
{
    [Fact]
    public void SpanLengthsShouldMatchSpecifiedLengths()
    {
        int[] lengths = new int[] { 0, 1, 10, 23};

        var rented = new RentedArraySpans<int>(lengths);

        rented.Should().NotBeNull();

        for(int i=0; i< lengths.Length; i++) 
        { 
            rented.GetSpan(i).Length.Should().Be(lengths[i]);
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public void RequestedInvalidSegmentIndexShouldThrow(int segmentIdx)
    {
        int[] lengths = new int[] { 0, 1, 10, 23 };

        var rented = new RentedArraySpans<int>(lengths);

        Action getSpanAction = () => rented.GetSpan(segmentIdx);

        getSpanAction.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void RentingNegativeLengthShouldThrow()
    {
        int[] lengths = new int[] { 0, 10, -1 };

        Func<RentedArraySpans<int>> cstr = () => new RentedArraySpans<int>(lengths);

        cstr.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SegmentSpansShouldNotOverlap()
    {
        int[] lengths = new int[] { 2, 8, 10, 23};

        var rented = new RentedArraySpans<int>(lengths);

        var span0 = rented.GetSpan(0);
        var span1 = rented.GetSpan(1);
        var span2 = rented.GetSpan(2);
        var span3 = rented.GetSpan(3);

        MemoryExtensions.Overlaps(span0, span1).Should().BeFalse();
        MemoryExtensions.Overlaps(span0, span2).Should().BeFalse();
        MemoryExtensions.Overlaps(span0, span3).Should().BeFalse();

        MemoryExtensions.Overlaps(span1, span2).Should().BeFalse();
        MemoryExtensions.Overlaps(span1, span3).Should().BeFalse();

        MemoryExtensions.Overlaps(span2, span3).Should().BeFalse();
    }
}
