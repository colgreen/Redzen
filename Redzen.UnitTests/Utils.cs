
namespace Redzen.UnitTests
{
    public static class Utils
    {
        public static bool AreEqual(byte[] x, byte[] y)
        {
            if(x.Length != y.Length) {
                return false;
            }

            for(int i=0; i<x.Length; i++)
            {
                if(x[i] != y[i]) {
                    return false;
                }
            }

            return true;
        }
    }
}
