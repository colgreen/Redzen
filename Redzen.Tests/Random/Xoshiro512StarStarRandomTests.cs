namespace Redzen.Random;

public class Xoshiro512StarStarRandomTests : RandomSourceTests
{
    protected override IRandomSource CreateRandomSource()
    {
        return new Xoshiro512StarStarRandom(1);
    }
}
