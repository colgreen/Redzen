using System;
using System.Diagnostics;

namespace Redzen.Sorting
{
    /// <summary>
    /// For sorting an array of key values, and two additional arrays based on the array of keys.
    /// This class exists because Array.Sort() has overloads for one additional array only, and this 
    /// class will sort two additional arrays.
    /// 
    /// This class is a modification of ArraySortHelper in the core framework:
    ///    https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Collections/Generic/ArraySortHelper.cs
    /// </summary>
    public static class IntroSort<K, V, W> where K : IComparable<K>
    {
        #region Statics / Consts

        // This is the threshold where Introspective sort switches to Insertion sort.
        // Empirically, 16 seems to speed up most cases without slowing down others, at least for integers.
        // Large value types may benefit from a smaller number.
        const int __introsortSizeThreshold = 17;

        #endregion

        #region Public Static Methods

        public static void Sort(K[] keys, V[] varr, W[] warr)
        {
            Debug.Assert(keys != null);
            Debug.Assert(varr != null);
            Debug.Assert(warr != null);

            IntrospectiveSort(keys, varr, warr, 0, keys.Length);
        }

        public static void Sort(K[] keys, V[] varr, W[] warr, int index, int length)
        {
            Debug.Assert(keys != null);
            Debug.Assert(varr != null);
            Debug.Assert(warr != null);

            IntrospectiveSort(keys, varr, warr, index, length);
        }

        #endregion

        #region Private Static Methods [Intro Sort]

        private static void IntrospectiveSort(K[] keys, V[] varr, W[] warr, int left, int length)
        {
            Debug.Assert(left >= 0);
            Debug.Assert(length >= 0);
            Debug.Assert(length <= keys.Length);
            Debug.Assert(length + left <= keys.Length);

            if (length < 2)
                return;

            IntroSortInner(keys, varr, warr, left, length + left - 1, 2 * FloorLog2(keys.Length));
        }

        private static void IntroSortInner(K[] keys, V[] varr, W[] warr, int lo, int hi, int depthLimit)
        {
            Debug.Assert(lo >= 0);
            Debug.Assert(hi < keys.Length);

            while (hi > lo)
            {
                int partitionSize = hi - lo + 1;
                if (partitionSize < __introsortSizeThreshold)
                {
                    if (partitionSize == 1) {
                        return;
                    }
                    if (partitionSize == 2) 
                    {
                        SwapIfGreaterWithItems(keys, varr, warr, lo, hi);
                        return;
                    }
                    if (partitionSize == 3) 
                    {
                        SwapIfGreaterWithItems(keys, varr, warr, lo, hi - 1);
                        SwapIfGreaterWithItems(keys, varr, warr, lo, hi);
                        SwapIfGreaterWithItems(keys, varr, warr, hi - 1, hi);
                        return;
                    }

                    InsertionSort(keys, varr, warr, lo, hi);
                    return;
                }

                if (depthLimit == 0)
                {
                    Heapsort(keys, varr, warr, lo, hi);
                    return;
                }
                depthLimit--;

                int p = PickPivotAndPartition(keys, varr, warr, lo, hi);
                // Note we've already partitioned around the pivot and do not have to move the pivot again.
                IntroSortInner(keys, varr, warr, p + 1, hi, depthLimit);
                hi = p - 1;
            }
        }

        private static int PickPivotAndPartition(K[] keys, V[] varr, W[] warr, int lo, int hi)
        {   
            Debug.Assert(lo >= 0);
            Debug.Assert(hi > lo);
            Debug.Assert(hi < keys.Length);

            // Compute median-of-three.  But also partition them, since we've done the comparison.
            int middle = lo + ((hi - lo) / 2);

            // Sort lo, mid and hi appropriately, then pick mid as the pivot.
            SwapIfGreaterWithItems(keys, varr, warr, lo, middle);  // swap the low with the mid point
            SwapIfGreaterWithItems(keys, varr, warr, lo, hi);      // swap the low with the high
            SwapIfGreaterWithItems(keys, varr, warr, middle, hi);  // swap the middle with the high

            K pivot = keys[middle];
            Swap(keys, varr, warr, middle, hi - 1);
            int left = lo, right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

            while (left < right)
            {
                if (pivot == null)
                {
                    while (left < (hi - 1) && keys[++left] == null) ;
                    while (right > lo && keys[--right] != null) ;
                }
                else
                {
                    while (pivot.CompareTo(keys[++left]) > 0) ;
                    while (pivot.CompareTo(keys[--right]) < 0) ;
                }

                if (left >= right)
                    break;

                Swap(keys, varr, warr, left, right);
            }

            // Put pivot in the right location.
            Swap(keys, varr, warr, left, (hi - 1));

            Debug.Assert(left >= lo && left <= hi);
            return left;
        }

        private static void SwapIfGreaterWithItems(K[] keys, V[] varr, W[] warr, int a, int b)
        {
            if (a != b && null != keys[a] && keys[a].CompareTo(keys[b]) > 0)
            {
                K key = keys[a];
                keys[a] = keys[b];
                keys[b] = key;

                V v = varr[a];
                varr[a] = varr[b];
                varr[b] = v;

                W w = warr[a];
                warr[a] = warr[b];
                warr[b] = w;
            }   
        }

        private static void Swap(K[] keys, V[] varr, W[] warr, int i, int j)
        {
            if (i != j)
            {
                K key = keys[i];
                keys[i] = keys[j];
                keys[j] = key;

                V v = varr[i];
                varr[i] = varr[j];
                varr[j] = v;

                W w = warr[i];
                warr[i] = warr[j];
                warr[j] = w;
            }
        }

        #endregion

        #region Private Static Methods [Insertion Sort]

        private static void InsertionSort(K[] keys, V[] varr, W[] warr, int lo, int hi)
        {
            Debug.Assert(lo >= 0);
            Debug.Assert(hi >= lo);
            Debug.Assert(hi <= keys.Length);

            int i, j;

            for (i = lo; i < hi; i++)
            {
                j = i;
                K t = keys[i + 1];
                V v = varr[i + 1];
                W w = warr[i + 1];
                while (j >= lo && (t == null || t.CompareTo(keys[j]) < 0))
                {
                    keys[j + 1] = keys[j];
                    varr[j + 1] = varr[j];
                    warr[j + 1] = warr[j];
                    j--;
                }
                keys[j + 1] = t;
                varr[j + 1] = v;
                warr[j + 1] = w;
            }
        }

        #endregion

        #region Private Static Methods [Heap Sort]

        private static void Heapsort(K[] keys, V[] varr, W[] warr, int lo, int hi)
        {
            Debug.Assert(lo >= 0);
            Debug.Assert(hi > lo);
            Debug.Assert(hi < keys.Length);

            int n = hi - lo + 1;
            for (int i = n / 2; i >= 1; i -= 1)
            {
                DownHeap(keys, varr, warr, i, n, lo);
            }
            for (int i = n; i > 1; i -= 1)
            {
                Swap(keys, varr, warr, lo, lo + i - 1);
                DownHeap(keys, varr, warr, 1, i - 1, lo);
            }
        }

        private static void DownHeap(K[] keys, V[] varr, W[] warr, int i, int n, int lo)
        {
            Debug.Assert(keys != null);
            Debug.Assert(lo >= 0);
            Debug.Assert(lo < keys.Length);

            K d = keys[lo + i - 1];
            V dv = varr[lo + i - 1];
            W dw = warr[lo + i - 1];
            int child;
            while (i <= n / 2)
            {
                child = 2 * i;
                if (child < n && (keys[lo + child - 1] == null || keys[lo + child - 1].CompareTo(keys[lo + child]) < 0))
                {
                    child++;
                }
                if (keys[lo + child - 1] == null || keys[lo + child - 1].CompareTo(d) < 0)
                    break;
                keys[lo + i - 1] = keys[lo + child - 1];
                varr[lo + i - 1] = varr[lo + child - 1];
                warr[lo + i - 1] = warr[lo + child - 1];
                i = child;
            }
            keys[lo + i - 1] = d;
            varr[lo + i - 1] = dv;
            warr[lo + i - 1] = dw;
        }

        #endregion

        #region Private Static Methods [Misc]

        private static int FloorLog2(int n)
        {
            int result = 0;
            while (n >= 1)
            {
                result++;
                n /= 2;
            }
            return result;
        }

        #endregion
    }
}
