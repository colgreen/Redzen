using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;

namespace Redzen.UnitTests.Random
{
    [TestClass]
    public class XorShiftRandomTests : RandomSourceTests
    {
        protected override IRandomSource CreateRandomSource()
        {
            return new XorShiftRandom(1);
        }
    }
}
