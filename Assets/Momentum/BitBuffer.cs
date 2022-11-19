
using System;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;

namespace Mirage.Momentum
{

    // a buffer that supports bit compression
    class BitBuffer
    {
        // 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00011111

        const int BITCOUNT = 64;
        const int USEDMASK = BITCOUNT - 1;
        const int INDEXSHIFT = 6;
        const ulong MAXVALUE = ulong.MaxValue;

        ulong[] _data;
        int _offsetInBits;

        public BitBuffer(int size)
        {
            Assert.IsTrue(size >= 0, "size must be greater than 0");

            _data = new ulong[(size + sizeof(ulong) -1 ) / sizeof(ulong)];
            _offsetInBits = 0;
        }

        // blocksize = 2-32
        // 32/8 = 4 chunks

        // some random uint value:
        // -> 00000000 00000000 00011100 00000110
        //                         13th bit - 12th index

        public void WriteUInt32VarLength(uint value, int blockSize)
        {
            var blocks = (Maths.BitScanReverse(value) + blockSize) / blockSize;

            // write data
            WriteInternal(1UL << (blocks - 1), blocks);
            WriteInternal(value, blocks * blockSize);
        }

        // 
        // 1UL << (2-1)
        // 10 <-

        public uint ReadUInt32VarLength(int blockSize)
        {
            var blocks = 1;

            while (ReadBoolean() == false)
            {
                ++blocks;
            }

            return ReadUInt32(blocks * blockSize);
        }

        public bool ReadBoolean()
        {
            return ReadInternal(1) == 1;
        }
        public void WriteBoolean(bool value)
        {
            WriteInternal(value ? 1UL : 0UL, 1);
        }

        public uint ReadUInt32(int bits)
        {
            return (uint)ReadInternal(bits);
        }

        public unsafe void Write(float value)
        {
            WriteInternal(*(uint*)&value, 32);
        }

        public unsafe void Write(double value)
        {
            WriteInternal(*(ulong*)&value, 64);
        }

        public void WriteCompressedFloat(float value, int min, int max, int accuracy)
        {
            var q = (int)((value * accuracy) + 0.5f);
            q -= min * accuracy;
            int maxquantized = max * accuracy - min * accuracy;
            q = Maths.Clamp(q, 0, maxquantized);
            Write(q, Maths.BitsRequiredForNumber(maxquantized));
        }

        public float ReadCompressedFloat(int min, int max, int accuracy)
        {
            int q = (int)ReadInternal( Maths.BitsRequiredForNumber(max * accuracy - min * accuracy));
            q += min * accuracy;
            return q * (1.0f / accuracy);
        }

        public void Write(uint value, int bits = 32)
        {
            Assert.IsTrue(bits >= 0 && bits <= 32);
            WriteInternal(value, bits);
        }

        public void Write(int value, int bits = 32)
        {
            Assert.IsTrue(bits >= 0 && bits <= 32);
            WriteInternal((ulong)value, bits);
        }

        ulong ReadInternal(int bits)
        {
            Assert.IsTrue(bits >= 0 && bits <= 64);

            if (bits == 0)
            {
                return 0;
            }

            int p = _offsetInBits >> INDEXSHIFT;
            int bitsUsed = _offsetInBits & USEDMASK;
            ulong first = _data[p] >> bitsUsed;
            int remainingBits = bits - (BITCOUNT - bitsUsed);

            ulong value;

            if (remainingBits == 0)
            {
                value = (first & (MAXVALUE >> (BITCOUNT - bits)));
            }
            else
            {
                ulong second = _data[p + 1] & (MAXVALUE >> (BITCOUNT - remainingBits));
                value = (first | (second << (bits - remainingBits)));
            }

            _offsetInBits += bits;
            return value;
        }


        void WriteInternal(ulong value, int bits)
        {
            Assert.IsTrue(bits >= 0 && bits <= 64);

            if (bits == 0)
            {
                return;
            }

            value &= (MAXVALUE >> (BITCOUNT - bits));

            // our current index
            var p = _offsetInBits >> INDEXSHIFT;

            // how many bits are currently _used_ in this index
            var bitsUsed = _offsetInBits & USEDMASK;
            var bitsFree = BITCOUNT - bitsUsed;
            var bitsLeft = bitsFree - bits;

            if (bitsLeft >= 0)
            {
                ulong mask = (MAXVALUE >> bitsFree) | (MAXVALUE << (BITCOUNT - bitsLeft));
                _data[p] = (_data[p] & mask) | (value << bitsUsed);
            }
            else
            {
                _data[p] = ((_data[p] & (MAXVALUE >> bitsFree)) | (value << bitsUsed));
                _data[p + 1] = ((_data[p + 1] & (MAXVALUE << (bits - bitsFree))) | (value >> bitsFree));
            }

            _offsetInBits += bits;
        }

        internal ReadOnlyMemory<byte> ToReadOnlyMemory()
        {
            return MemoryMarshal.Cast<ulong, byte>(_data.AsSpan(0, (_offsetInBits + 7) / 8)).ToArray();
        }
    }
}