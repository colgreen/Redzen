
namespace Redzen
{
    /// <summary>
    /// General purpose helper methods.
    /// </summary>
    public static class VariableUtils
    {
        /// <summary>
        /// Swap two variables.
        /// </summary>
        /// <typeparam name="T">Variable type.</typeparam>
        /// <param name="a">First variable.</param>
        /// <param name="b">Second variable.</param>
        public static void Swap<T>(ref T a, ref T b)
        {
            var tmp = a;
            a = b;
            b = tmp;
        }
    }
}
