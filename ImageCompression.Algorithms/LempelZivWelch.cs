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
            char[] charData = Converter.ToChars(byteData);

            List<byte> result = new List<byte>();
            byte freeBitsCountInLastByte = WriteValue(ref result, ClearCode, 0, Table.CurrentChainLimitPower);

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

        /// <summary>
        /// Добавляет число в массив байт
        /// </summary>
        /// <param name="buffer">место записи</param>
        /// <param name="value">число для записи</param>
        /// <param name="freeBitsCountInLastByte">количество неиспользованых бит в предыдущем байте</param>
        /// <param name="bitsCount">количество записываемых бит</param>
        /// <returns>free bits count in last byte</returns>
        /// <exception cref="ArgumentException">Количество свободных бит в байте не может превышать 8</exception>
        public static byte WriteValue(ref List<byte> buffer, ushort value, byte freeBitsCountInLastByte, byte bitsCount)
        {
            if (freeBitsCountInLastByte >= BITS_IN_BYTE) throw new ArgumentException("Количество свободных бит в байте не может превышать 8");

            ChangePreviousByte(ref buffer, value, bitsCount, freeBitsCountInLastByte);

            //количество бит для обработки
            int bitsCountLeft = bitsCount - freeBitsCountInLastByte;
            if (bitsCountLeft == 0) return 0;

            //добавляет целый байт, если количество оставшихся бит позволяет
            if (bitsCountLeft % BITS_IN_BYTE != 0)
            {
                for (int i = bitsCountLeft / BITS_IN_BYTE; i > 0; i--)
                {
                    int rightOffset = bitsCountLeft - i * BITS_IN_BYTE;
                    buffer.Add(GetNewByteWithRightOffset(value, rightOffset));
                }
            }

            //добавляет остатки бит в последний байт
            byte freeBitsCount = (byte)(BITS_IN_BYTE - bitsCountLeft);
            freeBitsCount %= BITS_IN_BYTE;
            buffer.Add(GetNewByteWithLeftOffset(value, freeBitsCount));

            return freeBitsCount;//количество незаписаных бит в последний байт
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
        private static byte GetNewByteWithLeftOffset(ushort value, int leftOffset) => (byte)(value << leftOffset);
        private static byte GetNewByteWithRightOffset(ushort value, int rightOffset) => (byte)(value >> rightOffset);






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
