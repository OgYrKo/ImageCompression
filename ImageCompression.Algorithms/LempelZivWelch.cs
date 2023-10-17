using ImageCompression.Common;
using ImageCompression.Common.LZW;
using ImageCompression.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ImageCompression.Algorithms
{
    /// <summary>
    /// LZW algorithm - цепочки байт по растрам
    /// </summary>
    public class LempelZivWelch : IAlgorithm
    {
        ChainTable Table;
        public const ushort ClearCode = 256;
        public const ushort CodeEndOfInformation = 257;
        const byte BITS_IN_BYTE = 8;

        public LempelZivWelch()
        {
            Table = new ChainTable();
        }
        private void TableInit()
        {
            Table.SetTableByDefault();
        }

        public byte[] Compress(byte[] byteData)
        {
            TableInit();
            string[] charData = Converter.ToStrings(byteData);

            List<byte> result = new List<byte>();
            byte freeBitsCountInLastByte = WriteValue(ref result, ClearCode, 0, Table.CurrentChainLimitPower);

            string CurStr = string.Empty;
            for (int i = 0; i < charData.Length; i++)
            {
                string C = charData[i];

                if (Table.Contains(CurStr + C))
                {
                    CurStr = CurStr + C;
                }
                else
                {
                    freeBitsCountInLastByte = WriteValue(ref result, Table[CurStr], freeBitsCountInLastByte, Table.CurrentChainLimitPower);
                    if (Table.TryAddNewChain(CurStr + C))
                        CurStr = C;
                    else
                    {
                        freeBitsCountInLastByte = WriteValue(ref result, ClearCode, freeBitsCountInLastByte, Table.CurrentChainLimitPower);
                        TableInit();
                    }
                }
            }
            freeBitsCountInLastByte = WriteValue(ref result, Table[CurStr], freeBitsCountInLastByte, Table.CurrentChainLimitPower);
            WriteValue(ref result, CodeEndOfInformation, freeBitsCountInLastByte, Table.CurrentChainLimitPower);
            return result.ToArray();
        }

        public List<ushort> CompressChain(byte[] byteData)
        {
            TableInit();
            char[] charData = Converter.ToChars(byteData);

            List<ushort> result = new List<ushort>
            {
                ClearCode
            };

            string CurStr = string.Empty;

            for (int i = 0; i < charData.Length; i++)
            {
                string C = new string(charData[i], 1);

                if (Table.Contains(CurStr + C))
                {
                    CurStr = CurStr + C;
                }
                else
                {
                    result.Add(Table[CurStr]);
                    if (Table.TryAddNewChain(CurStr + C))
                        CurStr = C;
                    else
                    {
                        result.Add(ClearCode);
                        TableInit();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <param name="freeBitsCountInLastByte"></param>
        /// <param name="bitsCount"></param>
        /// <returns>free bits count in last byte</returns>
        public static byte WriteValue(ref List<byte> buffer, ushort value, byte freeBitsCountInLastByte, byte bitsCount)
        {
            freeBitsCountInLastByte %= BITS_IN_BYTE;
            ChangePreviousByte(ref buffer, value, bitsCount, freeBitsCountInLastByte);
            int bitsCountLeft = bitsCount - freeBitsCountInLastByte;
            if (bitsCountLeft == 0) return 0;


            if (bitsCountLeft % BITS_IN_BYTE != 0)
            {
                for (int i = bitsCountLeft / BITS_IN_BYTE; i > 0; i--)
                {
                    int rightOffset = i * BITS_IN_BYTE;
                    buffer.Add(GetNewByte(value, -rightOffset));
                }
            }
            byte freeBitsCount = (byte)(BITS_IN_BYTE - bitsCountLeft);
            freeBitsCount %= BITS_IN_BYTE;
            buffer.Add(GetNewByte(value, freeBitsCount));

            return freeBitsCount;
        }

        public static ushort ReadValue(in byte[] buffer, ref int currentIndex, ref int readBitsCountInLastByte, byte bitsCount)
        {
            //получить текущий элемент
            List<byte> shortBuffer = new List<byte>() { buffer[currentIndex] };
            //вычесть прочитаные биты
            int readBitsCount = BITS_IN_BYTE - readBitsCountInLastByte;

            //если прочитано больше чем count, то выйти
            while (readBitsCount < bitsCount)
            {
                //получить следующий элемент
                currentIndex++;
                shortBuffer.Add(buffer[currentIndex]);
                //добавить 8 бит
                readBitsCount += BITS_IN_BYTE;
            }

            BitArray bits = new BitArray(shortBuffer.ToArray());
            BitArray valueBits = new BitArray(bitsCount);
            for (int i = 0; i < bitsCount; i++)
            {
                valueBits[i] = bits[readBitsCountInLastByte + i];
            }

            readBitsCountInLastByte = bits.Count - bitsCount - readBitsCountInLastByte;

            return Converter.ToUshort(valueBits);
        }

        private static void ChangePreviousByte(ref List<byte> buffer, ushort value, int bitsCount, int freeBitsCountInLastByte)
        {
            int offset = bitsCount - freeBitsCountInLastByte;
            if (buffer.Count != 0 && freeBitsCountInLastByte != 0)
            {
                byte lastByte = buffer[buffer.Count - 1];
                buffer[buffer.Count - 1] = (byte)(lastByte + (value >> offset));
            }
        }

        private static byte GetNewByte(ushort value, int leftOffset) => (byte)(value << leftOffset);






        public byte[] Decompress(byte[] data)
        {
            throw new NotImplementedException();
        }

        public string DecompressChain(byte[] data)
        {
            throw new NotImplementedException();
        }
    }



}
