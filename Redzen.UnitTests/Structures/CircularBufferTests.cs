using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Structures;

namespace Redzen.UnitTests.Structures
{
    [TestClass]
    public class CircularBufferTests
    {
        #region Public Test Methods

        [TestMethod]
        [TestCategory("CircularBuffer")]
        public void Test_Length()
        {
            int size = 10;
            var buff = new CircularBuffer<int>(size);
            Assert.AreEqual(0, buff.Length);

            for(int i=0; i < 10; i++)
            {
                buff.Enqueue(i);
                Assert.AreEqual(i+1, buff.Length);
            }

            for(int i=0; i < 10_000; i++)
            {
                buff.Enqueue(i);
                Assert.AreEqual(10, buff.Length);
            }
        }

        [TestMethod]
        [TestCategory("CircularBuffer")]
        public void Test_EnqueueDequeue()
        {
            const int size = 10;
            var buff = new CircularBuffer<int>(10);

            for(int i=0; i < size; i++)
            {
                buff.Enqueue(i);
                Assert.AreEqual(i+1, buff.Length);
            }

            for(int i=size; i < 10_000; i++)
            {
                int dequeued = buff.Dequeue();
                Assert.AreEqual(i-size, dequeued);
                Assert.AreEqual(size-1, buff.Length);

                buff.Enqueue(i);
                Assert.AreEqual(size, buff.Length);
            }
        }

        [TestMethod]
        [TestCategory("CircularBuffer")]
        public void Test_Pop()
        {
            const int size = 10;
            var buff = new CircularBuffer<int>(size);

            for(int i=0; i < size; i++)
            {
                buff.Enqueue(i);
                Assert.AreEqual(i+1, buff.Length);
            }

            int popped;
            for(int i=size; i < 10_000; i++)
            {
                popped = buff.Pop();
                Assert.AreEqual(i-1, popped);
                Assert.AreEqual(size-1, buff.Length);

                buff.Enqueue(i);
                Assert.AreEqual(size, buff.Length);
            }

            popped = buff.Pop();
            Assert.AreEqual(9_999, popped);

            for(int i=8; i >= 0; i--)
            {
                popped = buff.Pop();
                Assert.AreEqual(i, popped);
                Assert.AreEqual(i, buff.Length);
            }
        }

        #endregion
    }
}
