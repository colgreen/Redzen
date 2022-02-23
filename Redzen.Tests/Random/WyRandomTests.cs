namespace Redzen.Random.Tests;

public class WyRandomTests : RandomSourceTests
{
    protected override IRandomSource CreateRandomSource()
    {
        return new WyRandom(1);
    }
}
