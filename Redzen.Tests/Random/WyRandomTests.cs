namespace Redzen.Random;

public class WyRandomTests : RandomSourceTests
{
    protected override IRandomSource CreateRandomSource()
    {
        return new WyRandom(1);
    }
}
