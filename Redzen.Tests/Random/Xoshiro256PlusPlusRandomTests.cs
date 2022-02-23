namespace Redzen.Random.Tests;

public class Xoshiro256PlusPlusRandomTests : RandomSourceTests
{
    protected override IRandomSource CreateRandomSource()
    {
        return new Xoshiro256PlusPlusRandom(1);
    }
}
