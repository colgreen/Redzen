using Redzen.Random;

namespace Redzen.UnitTests.Random
{
    public class XorShiftRandomTests : RandomSourceTests
    {
        protected override IRandomSource CreateRandomSource()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return new XorShiftRandom(1);
#pragma warning restore CS0618
        }
    }
}
