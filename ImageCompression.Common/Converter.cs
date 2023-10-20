using System;
using System.Collections;
using ImageCompression.Common.LZW;

namespace ImageCompression.Common
{
    public static class Converter
    {
        public static string[] ToStrings<Numeric>(Numeric[] data)
        {
            string[] chars = new string[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                chars[i] = data[i].ToString();
            }
            return chars;
        }

        public static char[] ToChars(byte[] data)
        {
            char[] chars = new char[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                chars[i] = (char)data[i];
            }
            return chars;
        }

        public static string[] ToStringsMixed(ushort[] data)
        {
            string[] chars = new string[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] <= byte.MaxValue)
                    chars[i] = ((char)data[i]).ToString();
                else
                    chars[i] = data[i].ToString();
            }
            return chars;
        }

        public static byte[] ToBytes(string str)
        {
            char[] chars = str.ToCharArray();
            byte[] bytes = new byte[chars.Length];
            for(int i = 0;i<chars.Length;i++)
            {
                bytes[i] = (byte)chars[i];
            }
            return bytes;
        }

        public static string ToString(byte byteData)
        {
            return byteData.ToString();
        }

        public static byte[] ToBytes<Numeric>(Numeric[] bits)
        {
            const byte BitCount = 8;
            string stringBits = bits.ToString();
            byte[] byteArray = new byte[stringBits.Length / BitCount];

            // Проходимся по строке с битами и конвертируем их в байты
            for (int i = 0; i < byteArray.Length; i++)
            {
                // Извлекаем 8 символов (битов) из строки
                string eightBits = stringBits.Substring(i * BitCount, BitCount);

                // Преобразуем в байт и добавляем в массив
                byteArray[i] = Convert.ToByte(eightBits, 2);
            }
            return byteArray;
        }

        

        public static ushort ToUshort(BitArray bitArray)
        {
            if (bitArray == null)
            {
                throw new ArgumentNullException("bitArray");
            }

            ushort result = 0;

            for (int i = bitArray.Length - 1; i >= 0; i--)
            {
                if (bitArray[i])
                {
                    result += (ushort)Math.Pow(2, bitArray.Length - 1 - i);
                }
            }

            return result;
        }
    }
}
