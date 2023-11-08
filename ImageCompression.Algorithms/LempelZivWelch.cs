﻿using ImageCompression.Common;
using ImageCompression.Common.LZW;
using ImageCompression.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;

namespace ImageCompression.Algorithms
{
    /// <summary>
    /// LZW algorithm - цепочки байт по растрам
    /// </summary>
    public class LempelZivWelch : IAlgorithm
    {
        public const ushort ClearCode = 256;
        public const ushort CodeEndOfInformation = 257;
        const byte BITS_IN_BYTE = 8;

        private void TableInit(IChainTable chainTable)
        {
            chainTable.Reset();
        }

        public byte[] Compress(byte[] byteData)
        {
            List<byte> result = new List<byte>();
            NumericChainTable chainTable = new NumericChainTable();
            TableInit(chainTable);

            byte freeBitsCountInLastByte = WriteValue(ref result, ClearCode, 0, chainTable.CurrentChainLimitPower);

            CodeString current = null;
            int i = 0;
            for (; i < byteData.Length; i++)
            {
                byte C = byteData[i];
                if (chainTable.Contains(current, C))
                {
                    current = new CodeString(current, C);
                }
                else
                {
                    freeBitsCountInLastByte = WriteValue(ref result, chainTable[current], freeBitsCountInLastByte, chainTable.CurrentChainLimitPower);
                    if (!chainTable.TryAddNewChain(current, C))
                    {
                        freeBitsCountInLastByte = WriteValue(ref result, ClearCode, freeBitsCountInLastByte, chainTable.CurrentChainLimitPower);
                        TableInit(chainTable);
                    }
                    current = new CodeString(null, C);
                }
                if (current != null && current.Previous != null)
                    current.Code = chainTable[current.Previous];
            }
            
            freeBitsCountInLastByte = WriteValue(ref result, chainTable[current], freeBitsCountInLastByte, chainTable.CurrentChainLimitPower);
            WriteValue(ref result, CodeEndOfInformation, freeBitsCountInLastByte, chainTable.CurrentChainLimitPower);
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
            List<byte> shortBuffer = new List<byte>
            {
                buffer[currentIndex]
            };
            //вычесть прочитаные биты
            int readBitsCount = BITS_IN_BYTE - readBitsCountInLastByte;

            //если прочитано больше чем count, то выйти
            while (readBitsCount < bitsCount)
            {
                //получить следующий элемент
                currentIndex++;
                shortBuffer.Insert(0, buffer[currentIndex]);
                //добавить 8 бит
                readBitsCount += BITS_IN_BYTE;
            }

            BitArray bits = new BitArray(shortBuffer.ToArray());
            BitArray valueBits = new BitArray(bitsCount);
            for (int i = 0; i < bitsCount; i++)
            {
                valueBits[i] = bits[bits.Count - 1 - i - readBitsCountInLastByte];
            }

            readBitsCountInLastByte = BITS_IN_BYTE - (bits.Count - bitsCount - readBitsCountInLastByte);
            if (readBitsCountInLastByte == 0) currentIndex++;
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
            StringChainTable chainTable = new StringChainTable();
            List<byte> result = new List<byte>();
            int currentIndex = 0;
            int readBitsCountInLastByte = 0;
            ushort old_code = 0;


            ushort code = ReadValue(data, ref currentIndex, ref readBitsCountInLastByte, chainTable.CurrentChainLimitPower);
            while (code != CodeEndOfInformation)
            {
                if (code == ClearCode)
                {
                    TableInit(chainTable);
                    code = ReadValue(data, ref currentIndex, ref readBitsCountInLastByte, chainTable.CurrentChainLimitPower);
                    if (code == CodeEndOfInformation)
                    {
                        break;
                    }

                    result.AddRange(chainTable.GetBytes(code));
                }
                else
                {
                    if (chainTable.Contains(code))
                    {
                        
                        result.AddRange(chainTable.GetBytes(code));
                        chainTable.TryAddNewChain(chainTable[old_code], chainTable.GetFirstByte(code));
                    }
                    else
                    {
                        byte[] bytes = chainTable.GetBytes(old_code);
                        result.AddRange(bytes);
                        result.Add(bytes[0]);
                        chainTable.TryAddNewChain(chainTable[old_code], bytes[0]);
                    }
                }
                old_code = code;
                code = ReadValue(data, ref currentIndex, ref readBitsCountInLastByte, chainTable.CurrentChainLimitPower);
            }

            return result.ToArray();//Converter.ToBytes(string.Join("", result));

        }

    }
}
