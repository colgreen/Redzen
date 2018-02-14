using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Redzen.UnitTests
{
    [TestClass]
    public class MathUtilsTests
    {
        [TestMethod]
        [TestCategory("MathUtils")]
        public void CeilingPower2()
        {
            for(int i=0; i < 100000000; i++)
            {
                int result = MathUtils.CeilingPowerOfTwo(i);

                Assert.IsTrue(result >= i);
                Assert.IsTrue(i == 0 || MathUtils.IsPowerOfTwo(result));

                // Note. This could be zero, which is generally not considered to be a power of two, but is suitable for this test.
                int prevPowerOfTwo = result >> 1;
                Assert.IsTrue(i ==0 || prevPowerOfTwo < i);
            }
        }
    }
}
