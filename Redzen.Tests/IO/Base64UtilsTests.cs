using FluentAssertions;
using Redzen.Random;
using Xunit;

namespace Redzen.IO;

#pragma warning disable CA1861 // Avoid constant arrays as arguments

public class Base64UtilsTests
{
    [Fact]
    public void ToBase64String_ShouldEncodeEmptySpan()
    {
        // Act.
        string base64 = Base64Utils.ToBase64String(Array.Empty<int>());

        // Assert.
        // "AA" is the encoding for a single zero byte, which here indicates how many IDs are encoded (zero).
        base64.Should().Be("AA==");
    }

    [Fact]
    public void FromBase64String_ShouldDecodeEmptyArray()
    {
        // Act.
        // "AA" is the encoding for a single zero byte, which here indicates how many IDs are encoded (zero).
        int[] ids = Base64Utils.FromBase64String("AA==");

        // Assert.
        ids.Should().NotBeNull().And.BeEmpty();
    }

    [Theory]
    [InlineData(new int[] { 123 })]
    [InlineData(new int[] { 5, 6 })]
    [InlineData(new int[] { 7, 8, 9 })]
    [InlineData(new int[] { 10, 11, 12, 13, 14 })]
    [InlineData(new int[] { 100, int.MaxValue, 101 })]
    [InlineData(new int[] { int.MaxValue, 102, 103 })]
    [InlineData(new int[] { 100, int.MinValue, 101 })]
    [InlineData(new int[] { int.MinValue, 102, 103 })]
    public void EncodeDecodeRoundtripShouldProduceInputArray(int[] ids)
    {
        // Act.
        string base64 = Base64Utils.ToBase64String(ids);
        int[] decodedIds = Base64Utils.FromBase64String(base64);

        // Assert.
        decodedIds.Should().NotBeNull().And.Equal(ids);
    }

    [Theory]
    [InlineData(127)]
    [InlineData(128)]
    [InlineData(129)]
    [InlineData(1023)]
    [InlineData(1024)]
    [InlineData(1025)]
    [InlineData(1026)]
    [InlineData(16383)]
    [InlineData(16384)]
    [InlineData(16385)]
    public void EncodeDecodeRoundtripShouldProduceInputArray_ForLongArray(int length)
    {
        // Arrange.
        var rng = RandomDefaults.CreateRandomSource(123);
        var ids = new int[length];

        for(int i = 0; i<length; i++)
            ids[i] = rng.Next();

        // Act.
        string base64 = Base64Utils.ToBase64String(ids);
        int[] decodedIds = Base64Utils.FromBase64String(base64);

        // Assert.
        decodedIds.Should().NotBeNull().And.Equal(ids);
    }
}
