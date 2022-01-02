using FluentAssertions;
using Xunit;

namespace Redzen.Structures.Tests
{
    public class CircularBufferTests
    {
        #region Public Test Methods

        [Fact]
        public void LengthTests()
        {
            int size = 10;
            var buff = new CircularBuffer<int>(size);
            buff.Length.Should().Be(0);

            for(int i=0; i < 10; i++)
            {
                buff.Enqueue(i);
                buff.Length.Should().Be(i+1);
            }

            for(int i=0; i < 10_000; i++)
            {
                buff.Enqueue(i);
                buff.Length.Should().Be(10);
            }
        }

        [Fact]
        public void EnqueueDequeue()
        {
            const int size = 10;
            var buff = new CircularBuffer<int>(size);

            for(int i=0; i < size; i++)
            {
                buff.Enqueue(i);
                buff.Length.Should().Be(i+1);
            }

            for(int i=size; i < 10_000; i++)
            {
                int dequeued = buff.Dequeue();
                dequeued.Should().Be(i-size);
                buff.Length.Should().Be(size-1);

                buff.Enqueue(i);
                buff.Length.Should().Be(size);
            }
        }

        [Fact]
        public void Pop()
        {
            const int size = 10;
            var buff = new CircularBuffer<int>(size);

            for(int i=0; i < size; i++)
            {
                buff.Enqueue(i);
                buff.Length.Should().Be(i+1);
            }

            int popped;
            for(int i=size; i < 10_000; i++)
            {
                popped = buff.Pop();
                popped.Should().Be(i-1);
                buff.Length.Should().Be(size-1);

                buff.Enqueue(i);
                buff.Length.Should().Be(size);
            }

            popped = buff.Pop();
            Assert.Equal(9_999, popped);

            for(int i=8; i >= 0; i--)
            {
                popped = buff.Pop();
                popped.Should().Be(i);
                buff.Length.Should().Be(i);
            }
        }

        #endregion
    }
}
