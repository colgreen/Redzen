using System;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace Redzen.Collections
{
    public class LightweightListTests
    {
        [Fact]
        public void DefaultConstructEmptyList()
        {
            var list = new LightweightList<int>();
            list.Capacity.Should().Be(0);
            list.Count.Should().Be(0);
            list.AsSpan().Length.Should().Be(0);

            list.Invoking(x => x.AsSpan(-1)).Should().Throw<ArgumentOutOfRangeException>();
            list.Invoking(x => x.AsSpan(0)).Should().Throw<ArgumentOutOfRangeException>();
            list.Invoking(x => x.AsSpan(1)).Should().Throw<ArgumentOutOfRangeException>();
            list.Invoking(x => x[0]).Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void ConstructWithCapacity()
        {
            var list = new LightweightList<int>(10);
            list.Capacity.Should().Be(10);
            list.GetInternalArray().Length.Should().Be(10);

            list.Count.Should().Be(0);
            list.AsSpan().Length.Should().Be(0);

            list.Invoking(x => x.AsSpan(-1)).Should().Throw<ArgumentOutOfRangeException>();
            list.Invoking(x => x.AsSpan(0)).Should().Throw<ArgumentOutOfRangeException>();
            list.Invoking(x => x.AsSpan(1)).Should().Throw<ArgumentOutOfRangeException>();
            list.Invoking(x => x[0]).Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Add()
        {
            var list = new LightweightList<int>();
            for(int i=0; i < 10; i++)
                list.Add(i);

            list.Capacity.Should().Be(16);
            list.GetInternalArray().Length.Should().Be(16);

            list.Count.Should().Be(10);
            list.AsSpan().Length.Should().Be(10);

            var span = list.AsSpan();
            var arr = list.GetInternalArray();

            for(int i=0; i < 10; i++)
            {
                list[i].Should().Be(i);
                span[i].Should().Be(i);
                arr[i].Should().Be(i);
            }
        }

        [Fact]
        public void Insert_IntoEmptyList()
        {
            var list = new LightweightList<int>();
            for(int i=0; i < 10; i++)
                list.Insert(i, i);

            list.Capacity.Should().Be(16);
            list.GetInternalArray().Length.Should().Be(16);

            list.Count.Should().Be(10);
            list.AsSpan().Length.Should().Be(10);

            var span = list.AsSpan();
            var arr = list.GetInternalArray();

            for(int i=0; i < 10; i++)
            {
                list[i].Should().Be(i);
                span[i].Should().Be(i);
                arr[i].Should().Be(i);
            }
        }

        [Fact]
        public void Insert_IntoZeroIndex()
        {
            var list = new LightweightList<int>();
            for(int i=0; i < 10; i++)
                list.Insert(0, i);

            list.Capacity.Should().Be(16);
            list.GetInternalArray().Length.Should().Be(16);

            list.Count.Should().Be(10);
            list.AsSpan().Length.Should().Be(10);

            var span = list.AsSpan();
            var arr = list.GetInternalArray();

            for(int i=0; i < 10; i++)
            {
                list[i].Should().Be(9-i);
                span[i].Should().Be(9-i);
                arr[i].Should().Be(9-i);
            }
        }


        [Fact]
        public void InsertRange_IntoEmptyList()
        {
            int[] insertArr = Enumerable.Range(0, 10).ToArray();

            var list = new LightweightList<int>();
            list.InsertRange(0, insertArr);

            list.Capacity.Should().Be(10);
            list.GetInternalArray().Length.Should().Be(10);

            list.Count.Should().Be(10);
            list.AsSpan().Length.Should().Be(10);

            var span = list.AsSpan();
            var arr = list.GetInternalArray();

            for(int i=0; i < 10; i++)
            {
                list[i].Should().Be(i);
                span[i].Should().Be(i);
                arr[i].Should().Be(i);
            }
        }

        [Fact]
        public void InsertRange_AtEnd()
        {
            var list = new LightweightList<int>();

            list.InsertRange(0, Enumerable.Range(0, 10).ToArray());
            list.InsertRange(10, Enumerable.Range(10, 10).ToArray());

            list.Capacity.Should().Be(20);
            list.GetInternalArray().Length.Should().Be(20);

            list.Count.Should().Be(20);
            list.AsSpan().Length.Should().Be(20);

            var span = list.AsSpan();
            var arr = list.GetInternalArray();

            for(int i=0; i < 10; i++)
            {
                list[i].Should().Be(i);
                span[i].Should().Be(i);
                arr[i].Should().Be(i);
            }

            for(int i=10; i < 20; i++)
            {
                list[i].Should().Be(i);
                span[i].Should().Be(i);
                arr[i].Should().Be(i);
            }
        }

        [Fact]
        public void InsertRange_AtStart()
        {
            var list = new LightweightList<int>();

            list.InsertRange(0, Enumerable.Range(10, 10).ToArray());
            list.InsertRange(0, Enumerable.Range(0, 10).ToArray());

            list.Capacity.Should().Be(20);
            list.GetInternalArray().Length.Should().Be(20);

            list.Count.Should().Be(20);
            list.AsSpan().Length.Should().Be(20);

            var span = list.AsSpan();
            var arr = list.GetInternalArray();

            for(int i=0; i < 10; i++)
            {
                list[i].Should().Be(i);
                span[i].Should().Be(i);
                arr[i].Should().Be(i);
            }

            for(int i=10; i < 20; i++)
            {
                list[i].Should().Be(i);
                span[i].Should().Be(i);
                arr[i].Should().Be(i);
            }
        }

        [Fact]
        public void InsertRange_InMiddle()
        {
            var list = new LightweightList<int>();

            list.InsertRange(0, Enumerable.Range(0, 10).ToArray());
            list.InsertRange(4, Enumerable.Range(40, 10).ToArray());

            list.Capacity.Should().Be(20);
            list.GetInternalArray().Length.Should().Be(20);

            list.Count.Should().Be(20);
            list.AsSpan().Length.Should().Be(20);

            var span = list.AsSpan();
            var arr = list.GetInternalArray();

            for(int i=0; i < 4; i++)
            {
                list[i].Should().Be(i);
                span[i].Should().Be(i);
                arr[i].Should().Be(i);
            }

            for(int i=4, j=40; i < 14; i++, j++)
            {
                list[i].Should().Be(j);
                span[i].Should().Be(j);
                arr[i].Should().Be(j);
            }

            for(int i=4, j=40; i < 14; i++, j++)
            {
                list[i].Should().Be(j);
                span[i].Should().Be(j);
                arr[i].Should().Be(j);
            }

            for(int i=14, j=4; i < 20; i++, j++)
            {
                list[i].Should().Be(j);
                span[i].Should().Be(j);
                arr[i].Should().Be(j);
            }
        }

        [Fact]
        public void Clear()
        {
            var list = new LightweightList<int>();
            for(int i=0; i < 10; i++)
                list.Add(i);

            list.Clear();

            list.Capacity.Should().Be(16);
            list.GetInternalArray().Length.Should().Be(16);

            list.Count.Should().Be(0);
            list.AsSpan().Length.Should().Be(0);
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

            list.Capacity.Should().Be(16);
            list.GetInternalArray().Length.Should().Be(16);

            list.Count.Should().Be(9);
            span.Length.Should().Be(9);

            for(int i=0; i < 9; i++)
            {
                list[i].Should().Be(i);
                span[i].Should().Be(i);
                arr[i].Should().Be(i);
            }

            list.RemoveAt(0);
            span = list.AsSpan();
            list.Count.Should().Be(8);
            span.Length.Should().Be(8);

            for(int i=0, j=1; i < 8; i++, j++)
            {
                list[i].Should().Be(j);
                span[i].Should().Be(j);
                arr[i].Should().Be(j);
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
            list.Count.Should().Be(7);
            span.Length.Should().Be(7);

            for(int i=0, j=3; i < 7; i++, j++)
            {
                list[i].Should().Be(j);
                span[i].Should().Be(j);
                arr[i].Should().Be(j);
            }

            list.RemoveRange(5, 2);
            span = list.AsSpan();
            arr = list.GetInternalArray();
            list.Count.Should().Be(5);
            span.Length.Should().Be(5);

            for(int i=0, j=3; i < 5; i++, j++)
            {
                list[i].Should().Be(j);
                span[i].Should().Be(j);
                arr[i].Should().Be(j);
            }

            list.RemoveRange(2, 2);
            span = list.AsSpan();
            arr = list.GetInternalArray();
            list.Count.Should().Be(3);
            span.Length.Should().Be(3);

            list[0].Should().Be(3);
            span[0].Should().Be(3);
            arr[0].Should().Be(3);

            list[1].Should().Be(4);
            span[1].Should().Be(4);
            arr[1].Should().Be(4);

            list[2].Should().Be(7);
            span[2].Should().Be(7);
            arr[2].Should().Be(7);
        }

        [Fact]
        public void SetInternalArray()
        {
            var list = new LightweightList<int>();
            list.SetInternalArray(Enumerable.Range(0, 10).ToArray());

            var span = list.AsSpan();
            var arr = list.GetInternalArray();

            list.Capacity.Should().Be(10);
            arr.Length.Should().Be(10);

            list.Count.Should().Be(10);
            span.Length.Should().Be(10);

            for(int i = 0; i < 10; i++)
            {
                list[i].Should().Be(i);
                span[i].Should().Be(i);
                arr[i].Should().Be(i);
            }
        }

        [Fact]
        public void SetInternalArray_WithCount()
        {
            var list = new LightweightList<int>();
            list.SetInternalArray(Enumerable.Range(0, 10).ToArray(), 7);

            var span = list.AsSpan();
            var arr = list.GetInternalArray();

            list.Capacity.Should().Be(10);
            arr.Length.Should().Be(10);

            list.Count.Should().Be(7);
            span.Length.Should().Be(7);

            for(int i = 0; i < 7; i++)
            {
                list[i].Should().Be(i);
                span[i].Should().Be(i);
                arr[i].Should().Be(i);
            }
        }

        [Fact]
        public void ForEachEnumeration()
        {
            var list = new LightweightList<int>();
            list.AddRange(Enumerable.Range(0, 10).ToArray());

            int count = 0;
            foreach(int val in list)
                val.Should().Be(count++);

            count.Should().Be(10);
        }

        [Fact]
        public void GetEnumerator()
        {
            var list = new LightweightList<int>();
            list.AddRange(Enumerable.Range(0, 10).ToArray());

            var enumerator = list.GetEnumerator();
            enumerator.Current.Should().Be(0);

            int count = 0;
            for(; enumerator.MoveNext(); count++)
                enumerator.Current.Should().Be(count);

            count.Should().Be(10);
            enumerator.Current.Should().Be(0);
        }

        [Fact]
        public void GetEnumerator_RefType()
        {
            var list = new LightweightList<string>();
            list.AddRange(Enumerable.Range(0, 10).Select(x => x.ToString()).ToArray());

            var enumerator = list.GetEnumerator();
            enumerator.Current.Should().BeNull();

            int count = 0;
            for(; enumerator.MoveNext(); count++)
                enumerator.Current.Should().Be(count.ToString());

            count.Should().Be(10);
            enumerator.Current.Should().BeNull();
        }
    }
}
