using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redzen.Random;

namespace Redzen.UnitTests.Random
{
    [TestClass]
    public class Xoshiro256PlusRandomTests : RandomSourceTests
    {
        protected override IRandomSource CreateRandomSource()
        {
            return new Xoshiro256PlusRandom();
        }
    }
}
