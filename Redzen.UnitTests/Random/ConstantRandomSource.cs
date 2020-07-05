using System;
using Redzen.Random;

namespace Redzen.UnitTests.Random
{
    /// <summary>
    /// A subclass of RandomSourceBase that provides an implementation of NextULongInner() that returns
    /// a constant value specified at construction time.
    /// 
    /// NextULongInner() is used by all RandomSourceBase subclasses as the source of entropy for most (but not all) 
    /// random number generation methods, and therefore overriding this method allows us to supply specific values
    /// (or bit patterns if you prefer) into those other methods, and therefore allowing us to test those methods
    /// with given input values that represent interesting test cases.
    /// </summary>
    public class ConstantRandomSource : RandomSourceBase
    {
        readonly ulong _x;

        public ConstantRandomSource(ulong x)
        {
            _x = x;
        }

        public override void NextBytes(Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        protected override ulong NextULongInner()
        {
            return _x;
        }
    }
}
