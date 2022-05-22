/*
 * Copyright (c) 2009, 2013, Oracle and/or its affiliates. All rights reserved.
 * Copyright 2009 Google Inc.  All Rights Reserved.
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
 *
 * This code is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License version 2 only, as
 * published by the Free Software Foundation.  Oracle designates this
 * particular file as subject to the "Classpath" exception as provided
 * by Oracle in the LICENSE file that accompanied this code.
 *
 * This code is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
 * version 2 for more details (a copy is included in the LICENSE file that
 * accompanied this code).
 *
 * You should have received a copy of the GNU General Public License version
 * 2 along with this work; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 * Please contact Oracle, 500 Oracle Parkway, Redwood Shores, CA 94065 USA
 * or visit www.oracle.com if you need additional information or have any
 * questions.
 */

/*
 * A stable, adaptive, iterative mergesort that requires far fewer than
 * n log(n) comparisons when running on partially sorted arrays, while
 * offering performance comparable to a traditional mergesort when run
 * on random arrays.  Like all proper mergesorts, this sort is stable and
 * runs O(n log n) time (worst case).  In the worst case, this sort requires
 * temporary storage space for n/2 object references; in the best case,
 * it requires only a small constant amount of space.
 *
 * This implementation was adapted from Tim Peters' list sort for
 * Python, which is described in detail here:
 *
 *   http://svn.python.org/projects/python/trunk/Objects/listsort.txt
 *
 * Tim's C code may be found here:
 *
 *   http://svn.python.org/projects/python/trunk/Objects/listobject.c
 *
 * The underlying techniques are described in this paper (and may have
 * even earlier origins):
 *
 *  "Optimistic Sorting and Information Theoretic Complexity"
 *  Peter McIlroy
 *  SODA (Fourth Annual ACM-SIAM Symposium on Discrete Algorithms),
 *  pp 467-474, Austin, Texas, 25-27 January 1993.
 *
 * While the API to this class consists solely of static methods, it is
 * (privately) instantiable; a TimSort instance holds the state of an ongoing
 * sort, assuming the input array is large enough to warrant the full-blown
 * TimSort. Small arrays are sorted in place, using a binary insertion sort.
 *
 * @author Josh Bloch
 */

/*
 * The below C# code is a port of the Java source code, with fixes applied
 * from:
 *
 *   "OpenJDK’s java.utils.Collection.sort() is broken: The good, the bad and
 *   the worst case?" Stijn de Gouw1, Jurriaan Rot, Frank S. de Boer,
 *   Richard Bubel, Reiner Hähnle.
 *
 *   http://envisage-project.eu/wp-content/uploads/2015/02/sorting.pdf
 *
 * That paper proposes multiple possible fixes for a possible
 * index-out-of-range exception. The below C# code uses the recommended fix
 * from the paper whereas the Java source applied one of the alternative
 * fixes; possibly because it was a safer, more conservative approach for
 * such a widely used implementation, i.e. the default sort algorithm for
 * java collections.
 *
 * Colin Green, April 2018.
 */

// Note. Currently this will sort spans of non-null elements only
// (i.e. when handling spans of reference types).
using System.Diagnostics;
using System.Numerics;

namespace Redzen.Sorting;

#pragma warning disable SA1649 // File name should match first type name

/// <summary>
/// A timsort implementation.
/// </summary>
/// <typeparam name="K">The sort array element type.</typeparam>
/// <typeparam name="V">The secondary values array element type.</typeparam>
public sealed class TimSort<K,V>
    where K : IComparable<K>
{
    #region Consts

    /// <summary>
    /// This is the minimum sized sequence that will be merged. Shorter
    /// sequences will be lengthened by calling binarySort. If the entire
    /// span is less than this length, no merges will be performed.
    ///
    /// This constant should be a power of two. It was 64 in Tim Peters' C
    /// implementation, but 32 was empirically determined to work better in
    /// this implementation. In the unlikely event that you set this constant
    /// to be a number that's not a power of two, you'll need to change the
    /// {minRunLength} computation.
    ///
    /// If you decrease this constant, you must change the stackLen
    /// computation in the TimSort constructor, or you risk an
    /// ArrayOutOfBounds exception. See timsort.txt for a discussion
    /// of the minimum stack length required as a function of the length
    /// of the span being sorted and the minimum merge sequence length.
    /// </summary>
    const int MIN_MERGE = 32;

    /// <summary>
    /// When we get into galloping mode, we stay there until both runs win less
    /// often than MIN_GALLOP consecutive times.
    /// </summary>
    const int MIN_GALLOP = 7;

    /// <summary>
    /// Maximum initial size of tmp array, which is used for merging.  The array
    /// can grow to accommodate demand.
    ///
    /// Unlike Tim's original C version, we do not allocate this much storage
    /// when sorting smaller arrays. This change was required for performance.
    /// </summary>
    const int INITIAL_TMP_STORAGE_LENGTH = 256;

    #endregion

    #region Instance Fields

    // This controls when we get *into* galloping mode.  It is initialized
    // to MIN_GALLOP. The mergeLo and mergeHi methods nudge it higher for
    // random data, and lower for highly structured data.
    int _minGallop = MIN_GALLOP;

    // Temp storage for merges. A workspace array may optionally be
    // provided in constructor, and if so will be used as long as it
    // is big enough.
    K[] _tmp;
    V[] _tmpv;

    // A stack of pending runs yet to be merged. Run i starts at
    // address base[i] and extends for len[i] elements.  It's always
    // true (so long as the indices are in bounds) that:
    //
    //     runBase[i] + runLen[i] == runBase[i + 1]
    //
    // so we could cut the storage for this, but it's a minor amount,
    // and keeping all the info explicit simplifies the code.
    int _stackSize = 0;  // Number of pending runs on stack
    readonly int[] _runBase;
    readonly int[] _runLen;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a TimSort instance to maintain the state of an ongoing sort.
    /// </summary>
    /// <param name="len">The length of the span to be sorted.</param>
    /// <param name="work">An optional workspace array.</param>
    /// <param name="workv">An optional workspace array for secondary values.</param>
    private TimSort(int len, ref K[]? work, ref V[]? workv)
    {
        // Allocate temp storage (which may be increased later if necessary).
        int tlen = (len < 2 * INITIAL_TMP_STORAGE_LENGTH) ?
            len >> 1 : INITIAL_TMP_STORAGE_LENGTH;

        if(work is null || work.Length < tlen
         || workv is null || workv.Length < tlen)
        {
            _tmp = work = new K[tlen];
            _tmpv = workv = new V[tlen];
        }
        else
        {
            _tmp = work;
            _tmpv = workv;
        }

        // Allocate runs-to-be-merged stack (which cannot be expanded). The
        // stack length requirements are described in timsort.txt. The C
        // version always uses the same stack length (85), but this was
        // measured to be too expensive when sorting "mid-sized" spans (e.g.,
        // 100 elements) in Java. Therefore, we use smaller (but sufficiently
        // large) stack lengths for smaller spans.  The "magic numbers" in the
        // computation below must be changed if MIN_MERGE is decreased. See
        // the MIN_MERGE declaration above for more information.
        //
        // The stackLen constants defined here are taken from:
        // http://envisage-project.eu/wp-content/uploads/2015/02/sorting.pdf
        //
        // The values are lower than those used in the Java source that this source
        // is a port of. The reason is that the above linked paper identifies a
        // bug and two proposed fixes, one is to fix the invariant enforced by
        // mergeCollapse, the other is to not apply that fix, but to increase stackLen
        // in line with the worst case scenarios without the invariant fix.
        //
        // The java source also seems to have an additional +1 safety margin
        // (or off-by-one error?), that is not applied here in the spirit of
        // achieving maximum possible performance.
        //
        // Note that the python timsort uses a fixed stackLen of 85.
        int stackLen = (len < 120 ? 4 :
                        len < 1542 ? 9 :
                        len < 119151 ? 18 : 39);

        _runBase = new int[stackLen];
        _runLen = new int[stackLen];
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Pushes the specified run onto the pending-run stack.
    /// </summary>
    /// <param name="runBase">Index of the first element in the run.</param>
    /// <param name="runLen">The number of elements in the run.</param>
    private void PushRun(int runBase, int runLen)
    {
        _runBase[_stackSize] = runBase;
        _runLen[_stackSize] = runLen;
        _stackSize++;
    }

    /// <summary>
    /// Examines the stack of runs waiting to be merged and merges adjacent runs
    /// until the stack invariants are re-established:
    ///
    ///     1. runLen[i - 3] > runLen[i - 2] + runLen[i - 1]
    ///     2. runLen[i - 2] > runLen[i - 1]
    ///
    /// This method is called each time a new run is pushed onto the stack,
    /// so the invariants are guaranteed to hold for i &lt; stackSize upon
    /// entry to the method.
    /// </summary>
    private void MergeCollapse(Span<K> s, Span<V> v)
    {
        // Note. Contains the fix from:
        // http://envisage-project.eu/proving-android-java-and-python-sorting-algorithm-is-broken-and-how-to-fix-it/
        while(_stackSize > 1)
        {
            int n = _stackSize - 2;
            if(     (n >= 1 && _runLen[n-1] <= _runLen[n] + _runLen[n+1])
                 || (n >= 2 && _runLen[n-2] <= _runLen[n] + _runLen[n-1]))
            {
                if(_runLen[n - 1] < _runLen[n + 1])
                    n--;
            }
            else if(_runLen[n] > _runLen[n + 1])
            {
                break; // Invariant is established.
            }
            MergeAt(s, v, n);
        }
    }

    /// <summary>
    /// Merges all runs on the stack until only one remains. This method is
    /// called once, to complete the sort.
    /// </summary>
    private void MergeForceCollapse(Span<K> s, Span<V> v)
    {
        while(_stackSize > 1)
        {
            int n = _stackSize - 2;
            if(n > 0 && _runLen[n - 1] < _runLen[n + 1])
                n--;

            MergeAt(s, v, n);
        }
    }

    /// <summary>
    /// Merges the two runs at stack indices i and i+1. Run i must be
    /// the penultimate or ante-penultimate run on the stack. In other words,
    /// i must be equal to stackSize-2 or stackSize-3.
    /// </summary>
    /// <param name="s">The span being sorted.</param>
    /// <param name="v">Secondary values span.</param>
    /// <param name="i">Stack index of the first of the two runs to merge.</param>
    private void MergeAt(Span<K> s, Span<V> v, int i)
    {
        Debug.Assert(_stackSize >= 2);
        Debug.Assert(i >= 0);
        Debug.Assert(i == _stackSize - 2 || i == _stackSize - 3);

        int base1 = _runBase[i];
        int len1 = _runLen[i];
        int base2 = _runBase[i + 1];
        int len2 = _runLen[i + 1];
        Debug.Assert(len1 > 0 && len2 > 0);
        Debug.Assert(base1 + len1 == base2);

        // Record the length of the combined runs; if i is the 3rd-last
        // run now, also slide over the last run (which isn't involved
        // in this merge).  The current run (i+1) goes away in any case.
        _runLen[i] = len1 + len2;
        if(i == _stackSize - 3)
        {
            _runBase[i + 1] = _runBase[i + 2];
            _runLen[i + 1] = _runLen[i + 2];
        }
        _stackSize--;

        // Find where the first element of run2 goes in run1. Prior elements
        // in run1 can be ignored (because they're already in place).
        int k = TimSortUtils<K>.GallopRight(s[base2], s, base1, len1, 0);
        Debug.Assert(k >= 0);
        base1 += k;
        len1 -= k;
        if(len1 == 0)
            return;

        // Find where the last element of run1 goes in run2. Subsequent elements.
        // in run2 can be ignored (because they're already in place).
        len2 = TimSortUtils<K>.GallopLeft(s[base1 + len1 - 1], s, base2, len2, len2 - 1);
        Debug.Assert(len2 >= 0);
        if(len2 == 0)
            return;

        // Merge remaining runs, using tmp array with min(len1, len2) elements.
        if(len1 <= len2)
            MergeLo(s, v, base1, len1, base2, len2);
        else
            MergeHi(s, v, base1, len1, base2, len2);
    }

    /// <summary>
    /// Merges two adjacent runs in place, in a stable fashion. The first
    /// element of the first run must be greater than the first element of the
    /// second run (a[base1] &gt; a[base2]), and the last element of the first run
    /// (a[base1 + len1-1]) must be greater than all elements of the second run.
    ///
    /// For performance, this method should be called only when len1 &lt;= len2;
    /// its twin, mergeHi should be called if len1 &gt;= len2. (Either method
    /// may be called if len1 == len2.)
    /// </summary>
    /// <param name="s">The span being sorted.</param>
    /// <param name="v">Secondary values span.</param>
    /// <param name="base1">Index of first element in first run to be merged.</param>
    /// <param name="len1">Length of first run to be merged (must be &gt; 0).</param>
    /// <param name="base2">Index of first element in second run to be merged (must be base1 + len1).</param>
    /// <param name="len2">Length of second run to be merged (must be &gt; 0).</param>
    private void MergeLo(Span<K> s, Span<V> v, int base1, int len1, int base2, int len2)
    {
        Debug.Assert(len1 > 0 && len2 > 0 && base1 + len1 == base2);

        // Copy first run into temp array.
        EnsureCapacity(len1, s.Length);
        Span<K> tmp = _tmp;
        Span<V> tmpv = _tmpv;

        int cursor1 = 0;        // Indexes into tmp array.
        int cursor2 = base2;    // Indexes int a.
        int dest = base1;       // Indexes int a.
        s.Slice(base1, len1).CopyTo(tmp.Slice(cursor1));
        v.Slice(base1, len1).CopyTo(tmpv.Slice(cursor1));

        // Move first element of second run and deal with degenerate cases.
        s[dest] = s[cursor2];
        v[dest++] = v[cursor2++];
        if(--len2 == 0)
        {
            tmp.Slice(cursor1, len1).CopyTo(s.Slice(dest));
            tmpv.Slice(cursor1, len1).CopyTo(v.Slice(dest));
            return;
        }

        if(len1 == 1)
        {
            s.Slice(cursor2, len2).CopyTo(s.Slice(dest));
            s[dest + len2] = tmp[cursor1]; // Last element of run 1 to end of merge.

            v.Slice(cursor2, len2).CopyTo(v.Slice(dest));
            v[dest + len2] = tmpv[cursor1];
            return;
        }

        int minGallop = this._minGallop;  // Use local variable for performance.
                                          // outer:
        while(true)
        {
            int count1 = 0; // Number of times in a row that first run won.
            int count2 = 0; // Number of times in a row that second run won.

            // Do the straightforward thing until (if ever) one run starts
            // winning consistently.
            do
            {
                Debug.Assert(len1 > 1 && len2 > 0);

                if(TimSortUtils<K>.LessThan(ref s[cursor2], ref tmp[cursor1]))
                {
                    s[dest] = s[cursor2];
                    v[dest++] = v[cursor2++];
                    count2++;
                    count1 = 0;
                    if(--len2 == 0)
                        goto outerExit;
                }
                else
                {
                    s[dest] = tmp[cursor1];
                    v[dest++] = tmpv[cursor1++];
                    count1++;
                    count2 = 0;
                    if(--len1 == 1)
                        goto outerExit;
                }
            }
            while((count1 | count2) < minGallop);

            // One run is winning so consistently that galloping may be a
            // huge win. So try that, and continue galloping until (if ever)
            // neither run appears to be winning consistently anymore.
            do
            {
                Debug.Assert(len1 > 1 && len2 > 0);

                count1 = TimSortUtils<K>.GallopRight(s[cursor2], tmp, cursor1, len1, 0);
                if(count1 != 0)
                {
                    tmp.Slice(cursor1, count1).CopyTo(s.Slice(dest));
                    tmpv.Slice(cursor1, count1).CopyTo(v.Slice(dest));
                    dest += count1;
                    cursor1 += count1;
                    len1 -= count1;
                    if(len1 <= 1) // len1 == 1 || len1 == 0
                        goto outerExit;
                }
                s[dest] = s[cursor2];
                v[dest++] = v[cursor2++];
                if(--len2 == 0)
                    goto outerExit;

                count2 = TimSortUtils<K>.GallopLeft(tmp[cursor1], s, cursor2, len2, 0);
                if(count2 != 0)
                {
                    s.Slice(cursor2, count2).CopyTo(s.Slice(dest));
                    v.Slice(cursor2, count2).CopyTo(v.Slice(dest));
                    dest += count2;
                    cursor2 += count2;
                    len2 -= count2;
                    if(len2 == 0)
                        goto outerExit;
                }
                s[dest] = tmp[cursor1];
                v[dest++] = tmpv[cursor1++];
                if(--len1 == 1)
                    goto outerExit;

                minGallop--;
            }
            while(count1 >= MIN_GALLOP | count2 >= MIN_GALLOP);

            if(minGallop < 0)
                minGallop = 0;

            // Note. Original source used +=1. The JDK version changed this to +=2 which in simple tests appears to be a better choice.
            minGallop += 2;  // Penalize for leaving gallop mode.
        }  // End of "outer" loop
    outerExit:

        _minGallop = minGallop < 1 ? 1 : minGallop;  // Write back to field.

        if(len1 == 1)
        {
            Debug.Assert(len2 > 0);
            s.Slice(cursor2, len2).CopyTo(s.Slice(dest));
            s[dest + len2] = tmp[cursor1];  // Last element of run 1 to end of merge.

            v.Slice(cursor2, len2).CopyTo(v.Slice(dest));
            v[dest + len2] = tmpv[cursor1];
        }
        else if(len1 == 0)
        {
            throw new ArgumentException(
                "Comparison method violates its general contract!");
        }
        else
        {
            Debug.Assert(len2 == 0);
            Debug.Assert(len1 > 1);
            tmp.Slice(cursor1, len1).CopyTo(s.Slice(dest));
            tmpv.Slice(cursor1, len1).CopyTo(v.Slice(dest));
        }
    }

    /// <summary>
    /// Like mergeLo, except that this method should be called only if
    /// len1 &gt;= len2; mergeLo should be called if len1 &lt;= len2.  (Either method
    /// may be called if len1 == len2.)
    /// </summary>
    /// <param name="s">The span being sorted.</param>
    /// <param name="v">Secondary values span.</param>
    /// <param name="base1">Index of first element in first run to be merged.</param>
    /// <param name="len1">Length of first run to be merged (must be &gt; 0).</param>
    /// <param name="base2">Index of first element in second run to be merged (must be base1 + len1).</param>
    /// <param name="len2">Length of second run to be merged (must be &gt; 0).</param>
    private void MergeHi(Span<K> s, Span<V> v, int base1, int len1, int base2, int len2)
    {
        Debug.Assert(len1 > 0 && len2 > 0 && base1 + len1 == base2);

        // Copy second run into temp array.
        EnsureCapacity(len2, s.Length);
        Span<K> tmp = _tmp;
        Span<V> tmpv = _tmpv;

        s.Slice(base2, len2).CopyTo(tmp);
        v.Slice(base2, len2).CopyTo(tmpv);

        int cursor1 = base1 + len1 - 1; // Indexes into a.
        int cursor2 = len2 - 1;         // Indexes into tmp array.
        int dest = base2 + len2 - 1;    // Indexes into a.

        // Move last element of first run and deal with degenerate cases.
        s[dest] = s[cursor1];
        v[dest--] = v[cursor1--];
        if(--len1 == 0)
        {
            tmp.Slice(0, len2).CopyTo(s.Slice(dest - (len2 - 1)));
            tmpv.Slice(0, len2).CopyTo(v.Slice(dest - (len2 - 1)));
            return;
        }

        if(len2 == 1)
        {
            dest -= len1;
            cursor1 -= len1;
            s.Slice(cursor1 + 1, len1).CopyTo(s.Slice(dest + 1));
            s[dest] = tmp[cursor2];

            v.Slice(cursor1 + 1, len1).CopyTo(v.Slice(dest + 1));
            v[dest] = tmpv[cursor2];
            return;
        }

        int minGallop = _minGallop;  // Use local variable for performance.

        while(true)
        {
            int count1 = 0; // Number of times in a row that first run won.
            int count2 = 0; // Number of times in a row that second run won.

            // Do the straightforward thing until (if ever) one run
            // appears to win consistently.
            do
            {
                Debug.Assert(len1 > 0 && len2 > 1);

                if(TimSortUtils<K>.LessThan(ref tmp[cursor2], ref s[cursor1]))
                {
                    s[dest] = s[cursor1];
                    v[dest--] = v[cursor1--];
                    count1++;
                    count2 = 0;
                    if(--len1 == 0)
                        goto outerExit;
                }
                else
                {
                    s[dest] = tmp[cursor2];
                    v[dest--] = tmpv[cursor2--];
                    count2++;
                    count1 = 0;
                    if(--len2 == 1)
                        goto outerExit;
                }
            }
            while((count1 | count2) < minGallop);

            // One run is winning so consistently that galloping may be a
            // huge win. So try that, and continue galloping until (if ever)
            // neither run appears to be winning consistently anymore.
            do
            {
                Debug.Assert(len1 > 0 && len2 > 1);

                count1 = len1 - TimSortUtils<K>.GallopRight(tmp[cursor2], s, base1, len1, len1 - 1);
                if(count1 != 0)
                {
                    dest -= count1;
                    cursor1 -= count1;
                    len1 -= count1;
                    s.Slice(cursor1 + 1, count1).CopyTo(s.Slice(dest + 1));
                    v.Slice(cursor1 + 1, count1).CopyTo(v.Slice(dest + 1));
                    if(len1 == 0)
                        goto outerExit;
                }
                s[dest] = tmp[cursor2];
                v[dest--] = tmpv[cursor2--];
                if(--len2 == 1)
                {
                    goto outerExit;
                }

                count2 = len2 - TimSortUtils<K>.GallopLeft(s[cursor1], tmp, 0, len2, len2 - 1);
                if(count2 != 0)
                {
                    dest -= count2;
                    cursor2 -= count2;
                    len2 -= count2;
                    tmp.Slice(cursor2 + 1, count2).CopyTo(s.Slice(dest + 1));
                    tmpv.Slice(cursor2 + 1, count2).CopyTo(v.Slice(dest + 1));
                    if(len2 <= 1) // len2 == 1 || len2 == 0
                        goto outerExit;
                }
                s[dest] = s[cursor1];
                v[dest--] = v[cursor1--];
                if(--len1 == 0)
                    goto outerExit;

                minGallop--;
            }
            while(count1 >= MIN_GALLOP | count2 >= MIN_GALLOP);

            if(minGallop < 0)
                minGallop = 0;

            // Note. Original source used +=1. The JDK version changed this to +=2 which in simple tests appears to be a better choice.
            minGallop += 2;  // Penalize for leaving gallop mode.
        }  // End of "outer" loop
    outerExit:

        _minGallop = minGallop < 1 ? 1 : minGallop;  // Write back to field.

        if(len2 == 1)
        {
            Debug.Assert(len1 > 0);
            dest -= len1;
            cursor1 -= len1;
            s.Slice(cursor1 + 1, len1).CopyTo(s.Slice(dest + 1));
            s[dest] = tmp[cursor2]; // Move first element of run2 to front of merge.

            v.Slice(cursor1 + 1, len1).CopyTo(v.Slice(dest + 1));
            v[dest] = tmpv[cursor2];
        }
        else if(len2 == 0)
        {
            throw new ArgumentException(
                "Comparison method violates its general contract!");
        }
        else
        {
            Debug.Assert(len1 == 0);
            Debug.Assert(len2 > 0);
            tmp.Slice(0, len2).CopyTo(s.Slice(dest - (len2 - 1)));
            tmpv.Slice(0, len2).CopyTo(v.Slice(dest - (len2 - 1)));
        }
    }

    /// <summary>
    /// Ensures that the external array tmp has at least the specified
    /// number of elements, increasing its size if necessary. The size
    /// increases exponentially to ensure amortized linear time complexity.
    /// </summary>
    /// <param name="minCapacity">The minimum required capacity of the tmp array.</param>
    /// <param name="spanLen">The length of the span to be sorted.</param>
    private void EnsureCapacity(int minCapacity, int spanLen)
    {
        if(_tmp.Length < minCapacity)
        {
            // Compute smallest power of 2 > minCapacity.
            int newSize = 1 << (32 - BitOperations.LeadingZeroCount((uint)minCapacity));

            if(newSize < 0)
            {   // Not bloody likely!
                newSize = minCapacity;
            }
            else
            {
                newSize = Math.Min(newSize, spanLen >> 1);
            }

            _tmp = new K[newSize];
            _tmpv = new V[newSize];
        }
    }

    #endregion

    #region Private Static Methods

    /// <summary>
    /// Insertion sort.
    ///
    /// If the initial part of the specified range is already sorted, this method can take advantage of it:
    /// the method assumes that the elements from index 0, inclusive, to {start}, exclusive are already
    /// sorted.
    /// </summary>
    /// <param name="s">The span in which a range is to be sorted.</param>
    /// <param name="v">The secondary values span.</param>
    /// <param name="start">The index of the first element in the range that is not already known to be sorted.</param>
    /// <remarks>
    /// The original timsort uses a binary insertion sort here. This has been replaced with a simple insertion
    /// sort as this was empirically observed to be much faster.
    /// </remarks>
    private static void InsertionSort(Span<K> s, Span<V> v, int start)
    {
        if(start > 0) start--;

        for(int i = start; i < s.Length - 1; i++)
        {
            K k = s[i + 1];
            V vt = v[i + 1];

            int j = i;
            while(j >= 0 && TimSortUtils<K>.LessThan(ref k, ref s[j]))
            {
                s[j + 1] = s[j];
                v[j + 1] = v[j];
                j--;
            }

            s[j + 1] = k;
            v[j + 1] = vt;
        }
    }

    /// <summary>
    /// Returns the length of the run beginning at the specified position in
    /// the specified span and reverses the run if it is descending (ensuring
    /// that the run will always be ascending when the method returns).
    ///
    /// A run is the longest ascending sequence with:
    ///
    ///    a[lo] &lt;= a[lo + 1] &lt;= a[lo + 2] &lt;= ...
    ///
    /// or the longest descending sequence with:
    ///
    ///    a[lo] &gt;  a[lo + 1] &gt;  a[lo + 2] &gt;  ...
    ///
    /// For its intended use in a stable mergesort, the strictness of the
    /// definition of "descending" is needed so that the call can safely
    /// reverse a descending sequence without violating stability.
    /// </summary>
    /// <param name="s">The span in which a run is to be counted and possibly reversed.</param>
    /// <param name="v">The secondary values span.</param>
    /// <returns>The length of the run beginning at the specified position in the specified span.</returns>
    private static int CountRunAndMakeAscending(Span<K> s, Span<V> v)
    {
        int runHi = 1;
        if(runHi == s.Length)
            return 1;

        // Find end of run, and reverse range if descending.
        if(TimSortUtils<K>.LessThan(ref s[runHi++], ref s[0]))
        {
            // Descending.
            while(runHi < s.Length && TimSortUtils<K>.LessThan(ref s[runHi], ref s[runHi - 1]))
                runHi++;

            s.Slice(0, runHi).Reverse();
            v.Slice(0, runHi).Reverse();
        }
        else
        {   // Ascending.
            while(runHi < s.Length && !TimSortUtils<K>.LessThan(ref s[runHi], ref s[runHi - 1]))
                runHi++;
        }

        return runHi;
    }

    #endregion

    #region Public Static Methods [Sort API]

    /// <summary>
    /// Sorts the given span.
    /// </summary>
    /// <param name="span">The span to be sorted.</param>
    /// <param name="vals">Secondary values span.</param>
    public static void Sort(Span<K> span, Span<V> vals)
    {
        K[]? work = null;
        V[]? workv = null;
        Sort(span, vals, ref work, ref workv);
    }

    /// <summary>
    /// Sorts the specified range within the given span, using the given workspace array
    /// for temp storage when possible.
    /// </summary>
    /// <param name="span">The span to be sorted.</param>
    /// <param name="vals">Secondary values span.</param>
    /// <param name="work">An optional workspace array.</param>
    /// <param name="workv">An optional workspace array for the secondary values.</param>
    public static void Sort(Span<K> span, Span<V> vals, ref K[]? work, ref V[]? workv)
    {
        // Require that the two work arrays are the same length (if provided).
        Debug.Assert((work is null && workv is null) || (work is not null && workv is not null && work.Length == workv.Length));

        if(span.Length < 2)
            return; // Arrays of size 0 and 1 are always sorted.

        // If span is small, do a "mini-TimSort" with no merges.
        int lo = 0;
        int hi = span.Length;

        if(span.Length < MIN_MERGE)
        {
            int initRunLen = CountRunAndMakeAscending(span[lo..hi], vals[lo..hi]);
            InsertionSort(span, vals, initRunLen);
            return;
        }

        // March over the span once, left to right, finding natural runs,
        // extending short natural runs to minRun elements, and merging runs
        // to maintain stack invariant.
        TimSort<K,V> ts = new(span.Length, ref work, ref workv);
        int minRun = TimSortUtils<K>.MinRunLength(span.Length, MIN_MERGE);
        int nRemaining = span.Length;
        do
        {
            // Identify next run.
            int runLen = CountRunAndMakeAscending(span[lo..hi], vals[lo..hi]);

            // If run is short, extend to min(minRun, nRemaining).
            if(runLen < minRun)
            {
                int force = nRemaining <= minRun ? nRemaining : minRun;
                InsertionSort(
                    span.Slice(lo, force),
                    vals.Slice(lo, force),
                    runLen);
                runLen = force;
            }

            // Push run onto pending-run stack, and maybe merge.
            ts.PushRun(lo, runLen);
            ts.MergeCollapse(span, vals);

            // Advance to find next run.
            lo += runLen;
            nRemaining -= runLen;
        }
        while(nRemaining != 0);

        // Merge all remaining runs to complete sort.
        Debug.Assert(lo == hi);
        ts.MergeForceCollapse(span, vals);
        Debug.Assert(ts._stackSize == 1);
    }

    #endregion
}
