using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;

namespace Redzen.UnitTests.Random
{
    [TestClass]
    public class Xoshiro256StarStarRandomTests : RandomSourceTests
    {
        protected override IRandomSource CreateRandomSource()
        {
            return new Xoshiro256StarStarRandom();
        }
    }
}
