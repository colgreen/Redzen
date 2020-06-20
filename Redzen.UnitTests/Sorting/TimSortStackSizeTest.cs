using System;
using System.Collections.Generic;
using Redzen.Sorting;
using Xunit;

namespace Redzen.UnitTests.Sorting
{
    /// <summary>
    /// Test to cover Java JDK bug 8011944.
    /// https://bugs.java.com/view_bug.do?bug_id=8011944
    /// </summary>
    public class TimSortStackSizeTest
    {
        #region Consts

        const int MIN = 16;

        const int BOUND1 = 2 * MIN + 1;
        const int BOUND2 = BOUND1 + MIN + 2;
        const int BOUND3 = BOUND1 + 1 + BOUND2;
        const int BOUND4 = BOUND2 + 1 + BOUND3;
        const int BOUND5 = BOUND3 + 1 + BOUND4;

        #endregion

        #region Public Test Methods

        [Fact]
        public void StackSizeExceptionTest()
        {
            // Generate an array crafted to invoke the bug in the Java TimSort before it was fixed.
            // Before the fix an index-out-of-range exception would be thrown.
            int[] arr = GenData();
            TimSort<int>.Sort(arr);

            // While we're here, check the sort actually worked.
            Assert.True(SortUtils.IsSortedAscending(arr));
        }

        #endregion

        #region Private Static Methods

        private static int[] GenData() 
        {
            List<int> chunks = new List<int>();
            chunks.Insert(0, MIN);

            int B = MIN + 4;
            int A = B + MIN + 1;

            for (int i = 0; i < 8; i++) {
                int eps = Build(A, B, chunks);
                B = B + A + 1;
                A = B + eps + 1;
            }
            chunks.Insert(0,B);
            chunks.Insert(0,A);
            int total = 0;
            foreach (int len in chunks) {
                total += len;
            }
            int pow = MIN;
            while (pow < total) {
                pow += pow;
            }
            chunks.Add(pow - total);

            Console.WriteLine(" Total: " + total);

            int[] array = new int[pow];
            int off = 0;
            int pos = 0;
            foreach (int len in chunks) 
            {
                for (int i = 0; i < len; i++) 
                {
                    array[pos++] = (i == 0 ? 0 : 1);
                }
                off++;
            }
            return array;
        }

        private static int Build(int size, int B, List<int> chunks) 
        {
            chunks.Insert(0, B);
            if (size < BOUND1) {
                chunks.Insert(0, size);
                return size;
            }

            int asize = (size + 2) / 2;
            if (size >= BOUND2 && asize < BOUND1) {
                asize = BOUND1;
            } else if (size >= BOUND3 && asize < BOUND2) {
                asize = BOUND2;
            } else if (size >= BOUND4 && asize < BOUND3) {
                asize = BOUND3;
            } else if (size >= BOUND5 && asize < BOUND4) {
                asize = BOUND4;
            }
            if (size - asize >= B) {
                throw new ArgumentException(" " + size + " , " + asize + " , " + B);
            }
            return Build(asize, size - asize, chunks);
        }

        #endregion
    }
}
