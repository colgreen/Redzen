using System;
using System.Diagnostics;
using System.IO;
using Redzen.Numerics.Distributions.Double;
using Redzen.Random;
using Redzen.Tests;

namespace Redzen.IO.Tests
{
    public class MemoryStreamFuzzer
    {
        readonly MemoryStream _strmA;
        readonly MemoryBlockStream _strmB;
        readonly IRandomSource _rng;
        readonly DiscreteDistribution _opDistribution = new DiscreteDistribution(new double[] 
        { 
            0.688,  // Write
            0.05,   // Write byte
            0.05,   // Change read/write head position.
            0.05,   // SetLength
            0.05,   // Seek
            0.002,  // Trim
            0.01,   // Read byte
            0.1,    // Read
        });

        #region Constructors

        public MemoryStreamFuzzer(MemoryStream strmA, MemoryBlockStream strmB) 
            : this(strmA, strmB, 0)
        {}

        public MemoryStreamFuzzer(MemoryStream strmA, MemoryBlockStream strmB, int seed)
        {
            _strmA = strmA;
            _strmB = strmB;
            _rng = RandomDefaults.CreateRandomSource((ulong)seed);
            _opDistribution = new DiscreteDistribution(
                new double[] 
                { 
                    0.688,  // Write
                    0.05,   // Write byte
                    0.05,   // Change read/write head position.
                    0.05,   // SetLength
                    0.05,   // Seek
                    0.002,  // Trim
                    0.01,   // Read byte
                    0.1,    // Read
                });
        }

        #endregion

        #region Public Methods

        public void PerformMultipleOps(int count)
        {
            for(int i=0; i < count; i++) {
                PerformMutationOp();
            }
        }

        #endregion

        #region Private Methods

        private void PerformMutationOp()
        {
            int outcome = DiscreteDistribution.Sample(_rng, _opDistribution);
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
                    if(a!=b) {
                        throw new Exception("ReadByte mismatch");
                    }
                    Debug.WriteLine("ReadByte");
                    break;
                }
                case 7: // Read
                {
                    int len = _rng.Next(20000);

                    byte[] abuf = new byte[len];
                    byte[] bbuf = new byte[len];

                    int alen = _strmA.Read(abuf, 0, len);
                    int blen = _strmB.Read(bbuf, 0, len);

                    if(alen!=blen) {
                        throw new Exception("Read mismatch");
                    }

                    if(!Utils.AreEqual(abuf, bbuf)) {
                        throw new Exception("Read mismatch");
                    }
                    Debug.WriteLine("Read");
                    break;
                }
            }
        }

        private void PerformMutationOp_Write()
        {
            int buffLen = _rng.Next(1000) + 1;
            int offset = _rng.Next(buffLen);
            int count = _rng.Next(buffLen - offset);
            
            byte[] tmp = new byte[count];
            _rng.NextBytes(tmp);

            byte[] buff = new byte[buffLen];
            Array.Copy(tmp, 0, buff, offset, count);

            _strmA.Write(buff, offset, count);
            _strmB.Write(buff, offset, count);

            Debug.WriteLine(string.Format("Write offset={0}, count={1}", offset, count));
        }

        private void PerformMutationOp_Position()
        {
            int oldPos = (int)_strmA.Position;
            int newPos = (int)(_rng.NextDouble() * _strmA.Length); 
            _strmA.Position = newPos;
            _strmB.Position = newPos;

            Debug.WriteLine(string.Format("Position = {0} (was {1})", newPos, oldPos));
        }

        private void PerformMutationOp_SetLength()
        {
            int oldLen = (int)_strmA.Length;
            int newLen = (int)(_rng.NextDouble() * 1.02 * oldLen); 

            _strmA.SetLength(newLen);
            _strmB.SetLength(newLen);

            Debug.WriteLine(string.Format("SetLength = {0} (was {1})", newLen, oldLen));
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
                Debug.WriteLine(string.Format("Seek({0}, SeekOrigin.Begin) (pos was {1})", offset, currPos));
            }
            else if(dice >= 0.33 || dice < 0.66)
            {
                // Current.
                int offset = (int)(_rng.NextDouble() * (currLen - currPos));
                _strmA.Seek(offset, SeekOrigin.Current);
                _strmB.Seek(offset, SeekOrigin.Current);
                Debug.WriteLine(string.Format("Seek({0}, SeekOrigin.Current) (pos was {1})", offset, currPos));
            }
            else
            {
                // End.
                int offset = -(int)(_rng.NextDouble() * currLen);
                _strmA.Seek(offset, SeekOrigin.End);
                _strmB.Seek(offset, SeekOrigin.End);
                Debug.WriteLine(string.Format("Seek({0}, SeekOrigin.End) (pos was {1})", offset, currPos));
            }
        }

        #endregion
    }
}
