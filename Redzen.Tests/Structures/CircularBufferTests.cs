using Redzen.Structures;
using Xunit;

namespace Redzen.UnitTests.Structures
{
    public class CircularBufferTests
    {
        #region Public Test Methods

        [Fact]
        public void LengthTests()
        {
            int size = 10;
            var buff = new CircularBuffer<int>(size);
            Assert.Equal(0, buff.Length);

            for(int i=0; i < 10; i++)
            {
                buff.Enqueue(i);
                Assert.Equal(i+1, buff.Length);
            }

            for(int i=0; i < 10_000; i++)
            {
                buff.Enqueue(i);
                Assert.Equal(10, buff.Length);
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
                Assert.Equal(i+1, buff.Length);
            }

            for(int i=size; i < 10_000; i++)
            {
                int dequeued = buff.Dequeue();
                Assert.Equal(i-size, dequeued);
                Assert.Equal(size-1, buff.Length);

                buff.Enqueue(i);
                Assert.Equal(size, buff.Length);
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
                Assert.Equal(i+1, buff.Length);
            }

            int popped;
            for(int i=size; i < 10_000; i++)
            {
                popped = buff.Pop();
                Assert.Equal(i-1, popped);
                Assert.Equal(size-1, buff.Length);

                buff.Enqueue(i);
                Assert.Equal(size, buff.Length);
            }

            popped = buff.Pop();
            Assert.Equal(9_999, popped);

            for(int i=8; i >= 0; i--)
            {
                popped = buff.Pop();
                Assert.Equal(i, popped);
                Assert.Equal(i, buff.Length);
            }
        }

        #endregion
    }
}
