using Redzen.Random;

namespace Redzen.UnitTests.Random
{
    public class Xoshiro256PlusRandomTests : RandomSourceTests
    {
        protected override IRandomSource CreateRandomSource()
        {
            return new Xoshiro256PlusRandom(1);
        }
    }
}
