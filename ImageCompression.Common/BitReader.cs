using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ImageCompression.Common
{
    public class BitReader
    {
        private int currentIndex;
        private int bitPosition;
        private byte[] Bytes;

        public BitReader(byte[]bytes)
        {
            currentIndex = 0;
            bitPosition = 0;
            Bytes = bytes;
        }

        public bool IsEnd()
        {
            return currentIndex == Bytes.Length - 1 && bitPosition == 8;
        }

        public bool ReadBit()
        {
            if (bitPosition == 8)
            {
                currentIndex++;
                bitPosition = 0;
            }
            bool bit = (Bytes[currentIndex] & (1 << bitPosition)) != 0;
            bitPosition++;
            return bit;
        }

    }
}
