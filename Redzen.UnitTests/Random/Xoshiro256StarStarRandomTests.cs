using Redzen.Random;

namespace Redzen.UnitTests.Random
{
    public class Xoshiro256StarStarRandomTests : RandomSourceTests
    {
        protected override IRandomSource CreateRandomSource()
        {
            return new Xoshiro256StarStarRandom(1);
        }
    }
}
