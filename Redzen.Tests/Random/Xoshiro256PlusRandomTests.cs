
namespace Redzen.Random.Tests
{
    public class Xoshiro256PlusRandomTests : RandomSourceTests
    {
        protected override IRandomSource CreateRandomSource()
        {
            return new Xoshiro256PlusRandom(1);
        }
    }
}
