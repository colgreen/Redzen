using System.Diagnostics;
using FluentAssertions;
using Xunit;

namespace Redzen.Sorting;

/// <summary>
/// Test to cover Java JDK bug 8072909.
/// https://bugs.java.com/view_bug.do?bug_id=8072909
/// </summary>
public class TimSortStackSizeTest2
{
    #region Public Test Methods

    [Fact]
    public void StackSizeExceptionTest2()
    {
        int[] arr = new TimSortStackSize2(67_108_864).CreateArray();
        TimSort.Sort<int>(arr);

        // While we're here, check the sort actually worked.
        SortUtils.IsSortedAscending<int>(arr).Should().BeTrue();
    }

    [Fact(Skip = "Disabled by default. Allocates 4GB of RAM.")]
    public void StackSizeExceptionTest2B()
    {
        int[] arr = new TimSortStackSize2(1_073_741_824).CreateArray();
        TimSort.Sort<int>(arr);

        // While we're here, check the sort actually worked.
        SortUtils.IsSortedAscending<int>(arr).Should().BeTrue();
    }

    #endregion

    #region Inner Class

    sealed class TimSortStackSize2
    {
        const int MIN_MERGE = 32;
        readonly int _minRun;
        readonly int _length;
        readonly List<long> _runs = new();

        public TimSortStackSize2(int len)
        {
            _length = len;
            _minRun = MinRunLength(len);
            FillRunsJDKWorstCase();
        }

        public int[] CreateArray()
        {
            int[] a = new int[_length];
            int endRun = -1;

            foreach(long len in _runs)
            {
                a[endRun += (int)len] = 1;
            }
            a[_length - 1] = 0;
            return a;
        }

        /**
         * Fills <code>runs</code> with a sequence of run lengths of the form<br>
         * Y_n     x_{n,1}   x_{n,2}   ... x_{n,l_n} <br>
         * Y_{n-1} x_{n-1,1} x_{n-1,2} ... x_{n-1,l_{n-1}} <br>
         * ... <br>
         * Y_1     x_{1,1}   x_{1,2}   ... x_{1,l_1}<br>
         * The Y_i's are chosen to satisfy the invariant throughout execution,
         * but the x_{i,j}'s are merged (by <code>TimSort.mergeCollapse</code>)
         * into an X_i that violates the invariant.
         * X is the sum of all run lengths that will be added to <code>runs</code>.
         */
        private void FillRunsJDKWorstCase()
        {
            long runningTotal = 0;
            long Y = _minRun + 4;
            long X = _minRun;

            while(runningTotal + Y + X <= _length)
            {
                runningTotal += X + Y;
                GenerateJDKWrongElem(X);
                _runs.Insert(0, Y);

                // X_{i+1} = Y_i + x_{i,1} + 1, since runs.get(1) = x_{i,1}
                X = Y + _runs[1] + 1;

                // Y_{i+1} = X_{i+1} + Y_i + 1
                Y += X + 1;
            }

            if(runningTotal + X <= _length)
            {
                runningTotal += X;
                GenerateJDKWrongElem(X);
            }

            _runs.Add(_length - runningTotal);
        }

        /**
         * Adds a sequence x_1, ..., x_n of run lengths to <code>runs</code> such that:<br>
         * 1. X = x_1 + ... + x_n <br>
         * 2. x_j >= minRun for all j <br>
         * 3. x_1 + ... + x_{j-2}  <  x_j  <  x_1 + ... + x_{j-1} for all j <br>
         * These conditions guarantee that TimSort merges all x_j's one by one
         * (resulting in X) using only merges on the second-to-last element.
         * @param X  The sum of the sequence that should be added to runs.
         */
        private void GenerateJDKWrongElem(long X)
        {
            for(long newTotal; X >= 2 * _minRun + 1; X = newTotal)
            {
                // Default strategy.
                newTotal = X / 2 + 1;

                // Specialized strategies.
                if(3 * _minRun + 3 <= X && X <= 4*_minRun+1)
                {
                    // add x_1=MIN+1, x_2=MIN, x_3=X-newTotal  to runs.
                    newTotal = 2 * _minRun + 1;
                }
                else if(5 * _minRun + 5 <= X && X <= 6 * _minRun + 5)
                {
                    // add x_1=MIN+1, x_2=MIN, x_3=MIN+2, x_4=X-newTotal  to runs.
                    newTotal = 3 * _minRun + 3;
                }
                else if(8 * _minRun + 9 <= X && X <= 10 * _minRun + 9)
                {
                    // add x_1=MIN+1, x_2=MIN, x_3=MIN+2, x_4=2MIN+2, x_5=X-newTotal  to runs.
                    newTotal = 5 * _minRun + 5;
                }
                else if(13 * _minRun + 15 <= X && X <= 16 * _minRun + 17)
                {
                    // add x_1=MIN+1, x_2=MIN, x_3=MIN+2, x_4=2MIN+2, x_5=3MIN+4, x_6=X-newTotal  to runs.
                    newTotal = 8 * _minRun + 9;
                }
                _runs.Insert(0, X - newTotal);
            }
            _runs.Insert(0, X);
        }

        private static int MinRunLength(int n)
        {
            Debug.Assert(n >= 0);

            int r = 0;  // Becomes 1 if any 1 bits are shifted off.
            while(n >= MIN_MERGE)
            {
                r |= (n & 1);
                n >>= 1;
            }
            return n + r;
        }
    }

    #endregion
}
