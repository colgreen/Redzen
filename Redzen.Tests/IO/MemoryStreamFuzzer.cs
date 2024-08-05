using System.Diagnostics;
using Redzen.Numerics.Distributions;
using Redzen.Random;

namespace Redzen.IO;

#pragma warning disable CA1861 // Avoid constant arrays as arguments

public class MemoryStreamFuzzer
{
    readonly MemoryStream _strmA;
    readonly MemoryBlockStream _strmB;
    readonly IRandomSource _rng;
    readonly DiscreteDistribution<double> _opDistribution = new(
    [
        0.688,  // Write
        0.05,   // Write byte
        0.05,   // Change read/write head position.
        0.05,   // SetLength
        0.05,   // Seek
        0.002,  // Trim
        0.01,   // Read byte
        0.1,    // Read
    ]);

    #region Constructors

    public MemoryStreamFuzzer(MemoryStream strmA, MemoryBlockStream strmB)
        : this(strmA, strmB, 0)
    {
    }

    public MemoryStreamFuzzer(MemoryStream strmA, MemoryBlockStream strmB, int seed)
    {
        _strmA = strmA;
        _strmB = strmB;
        _rng = RandomDefaults.CreateRandomSource((ulong)seed);
        _opDistribution = new DiscreteDistribution<double>(
            [
                0.688,  // Write
                0.05,   // Write byte
                0.05,   // Change read/write head position.
                0.05,   // SetLength
                0.05,   // Seek
                0.002,  // Trim
                0.01,   // Read byte
                0.1,    // Read
            ]);
    }

    #endregion

    #region Public Methods

    public void PerformMultipleOps(int count)
    {
        for(int i=0; i < count; i++)
            PerformMutationOp();
    }

    #endregion

    #region Private Methods

    private void PerformMutationOp()
    {
        int outcome = _opDistribution.Sample(_rng);
        switch(outcome)
        {
            case 0: // Write.
                {
                    PerformMutationOp_Write();
                    break;
                }
            case 1: // Write byte.
                {
                    byte b = (byte)_rng.Next();
                    _strmA.WriteByte(b);
                    _strmB.WriteByte(b);
                    Debug.WriteLine("WriteByte");
                    break;
                }
            case 2: // Change read/write head position.
                {
                    PerformMutationOp_Position();
                    break;
                }
            case 3: // SetLength
                {
                    PerformMutationOp_SetLength();
                    break;
                }
            case 4: // Seek
                {
                    PerformMutationOp_Seek();
                    break;
                }
            case 5: // Trim
                {
                    _strmB.Trim();
                    Debug.WriteLine("Trim");
                    break;
                }
            case 6: // Read byte.
                {
                    int a = _strmA.ReadByte();
                    int b = _strmB.ReadByte();
                    if(a != b)
                        throw new InvalidOperationException("ReadByte mismatch");

                    Debug.WriteLine("ReadByte");
                    break;
                }
            case 7: // Read
                {
                    int len = _rng.Next(20_000);

                    byte[] abuf = new byte[len];
                    byte[] bbuf = new byte[len];

                    int alen = _strmA.Read(abuf);
                    int blen = _strmB.Read(bbuf);

                    if(alen != blen)
                        throw new InvalidOperationException("Read mismatch");

                    if(!SpanUtils.Equal<byte>(abuf, bbuf))
                        throw new InvalidOperationException("Read mismatch");

                    Debug.WriteLine("Read");
                    break;
                }
        }
    }

    private void PerformMutationOp_Write()
    {
        int len = _rng.Next(300);
        Span<byte> buf = stackalloc byte[len];
        _rng.NextBytes(buf);

        _strmA.Write(buf);
        _strmB.Write(buf);

        Debug.WriteLine($"Write count={len}");
    }

    private void PerformMutationOp_Position()
    {
        int oldPos = (int)_strmA.Position;
        int newPos = (int)(_rng.NextDouble() * _strmA.Length);
        _strmA.Position = newPos;
        _strmB.Position = newPos;

        Debug.WriteLine($"Position = {newPos} (was {oldPos})");
    }

    private void PerformMutationOp_SetLength()
    {
        int oldLen = (int)_strmA.Length;
        int newLen = (int)(_rng.NextDouble() * 1.02 * oldLen);

        _strmA.SetLength(newLen);
        _strmB.SetLength(newLen);

        Debug.WriteLine($"SetLength = {newLen} (was {oldLen})");
    }

    private void PerformMutationOp_Seek()
    {
        int currPos = (int)_strmA.Position;
        int currLen = (int)_strmA.Length;

        double dice = _rng.NextDouble();
        if(dice < 0.33)
        {
            // Begin.
            int offset = (int)(_rng.NextDouble() * currLen);
            _strmA.Seek(offset, SeekOrigin.Begin);
            _strmB.Seek(offset, SeekOrigin.Begin);
            Debug.WriteLine($"Seek({offset}, SeekOrigin.Begin) (pos was {currPos})");
        }
        else if(dice >= 0.33 || dice < 0.66)
        {
            // Current.
            int offset = (int)(_rng.NextDouble() * (currLen - currPos));
            _strmA.Seek(offset, SeekOrigin.Current);
            _strmB.Seek(offset, SeekOrigin.Current);
            Debug.WriteLine($"Seek({offset}, SeekOrigin.Current) (pos was {currPos})");
        }
        else
        {
            // End.
            int offset = -(int)(_rng.NextDouble() * currLen);
            _strmA.Seek(offset, SeekOrigin.End);
            _strmB.Seek(offset, SeekOrigin.End);
            Debug.WriteLine($"Seek({offset}, SeekOrigin.End) (pos was {currPos})");
        }
    }

    #endregion
}
