using System.Collections.Generic;

namespace Redzen
{
    /// <summary>
    /// Array static utility methods.
    /// </summary>
    public static class ArrayUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Returns true if the two arrays are equal.
        /// </summary>
        /// <param name="x">First array.</param>
        /// <param name="y">Second array.</param>
        public static bool Equals<T>(T[] x, T[] y)
        {
            // x and y are equal if they are the same reference, or both are null.
            if(x == y) {
                return true;
            }

            // Test if one is null and the other not null.
            // Note. We already tested for both being null (above).
            if(null == x || null == y) {
                return false;
            }

            if(x.Length != y.Length) {
                return false;
            }

            var comp = Comparer<T>.Default;

            for(int i=0; i < x.Length; i++)
            {
                if(comp.Compare(x[i], y[i]) != 0){
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
