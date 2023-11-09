using System;
using System.Collections;
using System.Collections.Generic;

namespace ImageCompression.Common
{
    public class BitWriter
    {
        private byte currentByte;
        private int bitPosition;
        private List<byte> bytes;

        public BitWriter()
        {
            currentByte = 0;
            bitPosition = 0;
            bytes = new List<byte>();
        }

        public void WriteBit(bool bit)
        {
            if (bitPosition == 8)
            {
                bytes.Add(currentByte);
                currentByte = 0;
                bitPosition = 0;
            }

            if (bit)
            {
                currentByte |= (byte)(1 << bitPosition);
            }

            bitPosition++;
        }

        public void WriteBitArray(BitArray bitArray)
        {
            foreach (bool bit in bitArray)
            {
                WriteBit(bit);
            }
        }
        public void WriteByte(byte value, int bitsToWriteCount)
        {
            if (bitsToWriteCount < 0 || bitsToWriteCount > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(bitsToWriteCount), "Bits to write count must be between 0 and 8.");
            }

            if (bitPosition == 0 && bitsToWriteCount == 8)
            {
                bytes.Add(value);
            }
            else
            {
                for (int i = 0; i < bitsToWriteCount; i++)
                {
                    WriteBit((value & (1 << i)) != 0);
                }
            }
        }

        public byte[] GetBytes()
        {
            byte[] result;

            if (bitPosition > 0)
            {
                result = new byte[bytes.Count + 1];
                bytes.CopyTo(result);
                result[bytes.Count] = currentByte;
            }
            else
            {
                result = bytes.ToArray();
            }

            return result;
        }
    }
}
