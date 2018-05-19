using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;

namespace Redzen.UnitTests.Random
{
    [TestClass]
    public class Xoshiro512StarStarRandomTests : RandomSourceTests
    {
        protected override IRandomSource CreateRandomSource()
        {
            return new Xoshiro512StarStarRandom(1);
        }
    }
}
