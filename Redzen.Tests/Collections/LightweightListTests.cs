using System;
using System.Linq;
using Xunit;

namespace Redzen.Collections
{
    public class LightweightListTests
    {
        [Fact]
        public void DefaultConstructEmptyList()
        {
            var list = new LightweightList<int>();
            Assert.Equal(0, list.Capacity);
            Assert.Equal(0, list.Count);
            Assert.Equal(0, list.AsSpan().Length);

            Assert.Throws<ArgumentOutOfRangeException>(() => list.AsSpan(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.AsSpan(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.AsSpan(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => list[0]);
        }

        [Fact]
        public void ConstructWithCapacity()
        {
            var list = new LightweightList<int>(10);
            Assert.Equal(10, list.Capacity);
            Assert.Equal(10, list.GetInternalArray().Length);

            Assert.Equal(0, list.Count);
            Assert.Equal(0, list.AsSpan().Length);

            Assert.Throws<ArgumentOutOfRangeException>(() => list.AsSpan(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.AsSpan(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => list.AsSpan(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => list[0]);
        }


        [Fact]
        public void Add()
        {
            var list = new LightweightList<int>();
            for(int i=0; i < 10; i++)
            {
                list.Add(i);
            }

            Assert.Equal(16, list.Capacity);
            Assert.Equal(16, list.GetInternalArray().Length);

            Assert.Equal(10, list.Count);
            Assert.Equal(10, list.AsSpan().Length);

            var span = list.AsSpan();
            var arr = list.GetInternalArray();

            for(int i=0; i < 10; i++)
            {
                Assert.Equal(i, list[i]);
                Assert.Equal(i, span[i]);
                Assert.Equal(i, arr[i]);
            }
        }

        [Fact]
        public void Insert_IntoEmptyList()
        {
            var list = new LightweightList<int>();
            for(int i=0; i < 10; i++)
                list.Insert(i, i);

            Assert.Equal(16, list.Capacity);
            Assert.Equal(16, list.GetInternalArray().Length);

            Assert.Equal(10, list.Count);
            Assert.Equal(10, list.AsSpan().Length);

            var span = list.AsSpan();
            var arr = list.GetInternalArray();
            for(int i = 0; i < 10; i++)
            {
                Assert.Equal(i, list[i]);
                Assert.Equal(i, span[i]);
                Assert.Equal(i, arr[i]);
            }
        }

        [Fact]
        public void Insert_IntoZeroIndex()
        {
            var list = new LightweightList<int>();
            for(int i=0; i < 10; i++)
                list.Insert(0, i);

            Assert.Equal(16, list.Capacity);
            Assert.Equal(16, list.GetInternalArray().Length);

            Assert.Equal(10, list.Count);
            Assert.Equal(10, list.AsSpan().Length);

            var span = list.AsSpan();
            var arr = list.GetInternalArray();
            for(int i=0; i < 10; i++)
            {
                Assert.Equal(9-i, list[i]);
                Assert.Equal(9-i, span[i]);
                Assert.Equal(9-i, arr[i]);
            }
        }


        [Fact]
        public void InsertRange_IntoEmptyList()
        {
            int[] insertArr = Enumerable.Range(0, 10).ToArray();

            var list = new LightweightList<int>();
            list.InsertRange(0, insertArr);

            Assert.Equal(10, list.Capacity);
            Assert.Equal(10, list.GetInternalArray().Length);

            Assert.Equal(10, list.Count);
            Assert.Equal(10, list.AsSpan().Length);

            var span = list.AsSpan();
            var arr = list.GetInternalArray();

            for(int i=0; i < 10; i++)
            {
                Assert.Equal(i, list[i]);
                Assert.Equal(i, span[i]);
                Assert.Equal(i, arr[i]);
            }
        }

        [Fact]
        public void InsertRange_AtEnd()
        {
            var list = new LightweightList<int>();

            list.InsertRange(0, Enumerable.Range(0, 10).ToArray());
            list.InsertRange(10, Enumerable.Range(10, 10).ToArray());

            Assert.Equal(20, list.Capacity);
            Assert.Equal(20, list.GetInternalArray().Length);

            Assert.Equal(20, list.Count);
            Assert.Equal(20, list.AsSpan().Length);

            var span = list.AsSpan();
            var arr = list.GetInternalArray();
            for(int i=0; i < 10; i++)
            {
                Assert.Equal(i, list[i]);
                Assert.Equal(i, span[i]);
                Assert.Equal(i, arr[i]);
            }

            for(int i=10; i < 20; i++)
            {
                Assert.Equal(i, list[i]);
                Assert.Equal(i, span[i]);
                Assert.Equal(i, arr[i]);
            }
        }

        [Fact]
        public void InsertRange_AtStart()
        {
            var list = new LightweightList<int>();

            list.InsertRange(0, Enumerable.Range(10, 10).ToArray());
            list.InsertRange(0, Enumerable.Range(0, 10).ToArray());

            Assert.Equal(20, list.Capacity);
            Assert.Equal(20, list.GetInternalArray().Length);

            Assert.Equal(20, list.Count);
            Assert.Equal(20, list.AsSpan().Length);

            var span = list.AsSpan();
            var arr = list.GetInternalArray();
            for(int i=0; i < 10; i++)
            {
                Assert.Equal(i, list[i]);
                Assert.Equal(i, span[i]);
                Assert.Equal(i, arr[i]);
            }

            for(int i=10; i < 20; i++)
            {
                Assert.Equal(i, list[i]);
                Assert.Equal(i, span[i]);
                Assert.Equal(i, arr[i]);
            }
        }

        [Fact]
        public void InsertRange_InMiddle()
        {
            var list = new LightweightList<int>();

            list.InsertRange(0, Enumerable.Range(0, 10).ToArray());
            list.InsertRange(4, Enumerable.Range(40, 10).ToArray());

            Assert.Equal(20, list.Capacity);
            Assert.Equal(20, list.GetInternalArray().Length);

            Assert.Equal(20, list.Count);
            Assert.Equal(20, list.AsSpan().Length);

            var span = list.AsSpan();
            var arr = list.GetInternalArray();
            for(int i=0; i < 4; i++)
            {
                Assert.Equal(i, list[i]);
                Assert.Equal(i, span[i]);
                Assert.Equal(i, arr[i]);
            }

            for(int i=4, j=40; i < 14; i++, j++)
            {
                Assert.Equal(j, list[i]);
                Assert.Equal(j, span[i]);
                Assert.Equal(j, arr[i]);
            }

            for(int i=4, j=40; i < 14; i++, j++)
            {
                Assert.Equal(j, list[i]);
                Assert.Equal(j, span[i]);
                Assert.Equal(j, arr[i]);
            }

            for(int i = 14, j = 4; i < 20; i++, j++)
            {
                Assert.Equal(j, list[i]);
                Assert.Equal(j, span[i]);
                Assert.Equal(j, arr[i]);
            }
        }

        [Fact]
        public void Clear()
        {
            var list = new LightweightList<int>();
            for(int i=0; i < 10; i++)
                list.Add(i);

            list.Clear();

            Assert.Equal(16, list.Capacity);
            Assert.Equal(16, list.GetInternalArray().Length);

            Assert.Equal(0, list.Count);
            Assert.Equal(0, list.AsSpan().Length);
        }

        [Fact]
        public void RemoveAt()
        {
            var list = new LightweightList<int>();
            for(int i=0; i < 10; i++)
                list.Add(i);

            list.RemoveAt(9);

            var span = list.AsSpan();
            var arr = list.GetInternalArray();

            Assert.Equal(16, list.Capacity);
            Assert.Equal(16, list.GetInternalArray().Length);

            Assert.Equal(9, list.Count);
            Assert.Equal(9, span.Length);

            for(int i = 0; i < 9; i++)
            {
                Assert.Equal(i, list[i]);
                Assert.Equal(i, span[i]);
                Assert.Equal(i, arr[i]);
            }

            list.RemoveAt(0);
            span = list.AsSpan();
            Assert.Equal(8, list.Count);
            Assert.Equal(8, span.Length);

            for(int i=0, j=1; i < 8; i++, j++)
            {
                Assert.Equal(j, list[i]);
                Assert.Equal(j, span[i]);
                Assert.Equal(j, arr[i]);
            }
        }

        [Fact]
        public void RemoveRange()
        {
            var list = new LightweightList<int>();
            for(int i=0; i < 10; i++)
                list.Add(i);

            list.RemoveRange(0, 3);
            var span = list.AsSpan();
            var arr = list.GetInternalArray();
            Assert.Equal(7, list.Count);
            Assert.Equal(7, span.Length);

            for(int i=0, j=3; i < 7; i++, j++)
            {
                Assert.Equal(j, list[i]);
                Assert.Equal(j, span[i]);
                Assert.Equal(j, arr[i]);
            }

            list.RemoveRange(5, 2);
            span = list.AsSpan();
            arr = list.GetInternalArray();
            Assert.Equal(5, list.Count);
            Assert.Equal(5, span.Length);

            for(int i=0, j=3; i < 5; i++, j++)
            {
                Assert.Equal(j, list[i]);
                Assert.Equal(j, span[i]);
                Assert.Equal(j, arr[i]);
            }

            list.RemoveRange(2, 2);
            span = list.AsSpan();
            arr = list.GetInternalArray();
            Assert.Equal(3, list.Count);
            Assert.Equal(3, span.Length);

            Assert.Equal(3, list[0]);
            Assert.Equal(3, span[0]);
            Assert.Equal(3, arr[0]);

            Assert.Equal(4, list[1]);
            Assert.Equal(4, span[1]);
            Assert.Equal(4, arr[1]);

            Assert.Equal(7, list[2]);
            Assert.Equal(7, span[2]);
            Assert.Equal(7, arr[2]);
        }

        [Fact]
        public void SetInternalArray()
        {
            var list = new LightweightList<int>();
            list.SetInternalArray(Enumerable.Range(0, 10).ToArray());

            var span = list.AsSpan();
            var arr = list.GetInternalArray();
            Assert.Equal(10, list.Capacity);
            Assert.Equal(10, arr.Length);

            Assert.Equal(10, list.Count);
            Assert.Equal(10, span.Length);

            for(int i = 0; i < 10; i++)
            {
                Assert.Equal(i, list[i]);
                Assert.Equal(i, span[i]);
                Assert.Equal(i, arr[i]);
            }
        }

        [Fact]
        public void SetInternalArray_WithCount()
        {
            var list = new LightweightList<int>();
            list.SetInternalArray(Enumerable.Range(0, 10).ToArray(), 7);

            var span = list.AsSpan();
            var arr = list.GetInternalArray();
            Assert.Equal(10, list.Capacity);
            Assert.Equal(10, arr.Length);

            Assert.Equal(7, list.Count);
            Assert.Equal(7, span.Length);

            for(int i = 0; i < 7; i++)
            {
                Assert.Equal(i, list[i]);
                Assert.Equal(i, span[i]);
                Assert.Equal(i, arr[i]);
            }
        }

        [Fact]
        public void ForEachEnumeration()
        {
            var list = new LightweightList<int>();
            list.AddRange(Enumerable.Range(0, 10).ToArray());

            int count = 0;
            foreach(int val in list)
                Assert.Equal(count++, val);

            Assert.Equal(10, count);
        }

        [Fact]
        public void GetEnumerator()
        {
            var list = new LightweightList<int>();
            list.AddRange(Enumerable.Range(0, 10).ToArray());

            var enumerator = list.GetEnumerator();
            Assert.Equal(0, enumerator.Current);

            int count = 0;
            for(; enumerator.MoveNext(); count++)
                Assert.Equal(count, enumerator.Current);

            Assert.Equal(10, count);
            Assert.Equal(0, enumerator.Current);
        }

        [Fact]
        public void GetEnumerator_RefType()
        {
            var list = new LightweightList<string>();
            list.AddRange(Enumerable.Range(0, 10).Select(x => x.ToString()).ToArray());

            var enumerator = list.GetEnumerator();
            Assert.Null(enumerator.Current);

            int count = 0;
            for(; enumerator.MoveNext(); count++)
                Assert.Equal(count.ToString(), enumerator.Current);

            Assert.Equal(10, count);
            Assert.Null(enumerator.Current);
        }
    }
}
