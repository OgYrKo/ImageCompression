using System;
using System.Diagnostics;
using System.Collections.Generic;
using ImageCompression.Interfaces;
using System.Threading;

namespace ImageCompression.Algorithms
{
    /// <summary>
    /// RLE algorithm - lossless data compression
    /// </summary>
    public class RunLengthEncoding : IAlgorithm
    {
        private const byte MaxByteValue = 255;
        private const byte FlagSize = 128;
        private const byte MaxCount = 127;

        public byte[] Compress(byte[] ImageBytes)
        {
            if (ImageBytes == null || ImageBytes.Length == 0)
            {
                throw new ArgumentException("Недопустимые параметры.");
            }
            if (ImageBytes.Length == 1) return new byte[] { 0, ImageBytes[0] };

            List<byte> compressedData = new List<byte>();

            bool isRepeated = false;
            byte count = 0;
            int counterIndex = -1;
            for (int i = 2; i <= ImageBytes.Length; i++)
            {
                byte previous = ImageBytes[i - 2];
                byte current = ImageBytes[i - 1];

                if (current == previous)
                {
                    counterIndex = -1;
                    isRepeated = true;
                    if (count == MaxCount)
                    {
                        AddSimilarSequance(ref compressedData, count, previous);

                        isRepeated = false;
                        if (i == ImageBytes.Length)
                        {
                            compressedData.Add(0);
                            compressedData.Add(current);
                        }
                        count = 0;
                    }
                    else
                    {
                        count++;
                        if (i == ImageBytes.Length)
                        {
                            compressedData.Add(TransformNumberToRepeatedCounter(count));
                            compressedData.Add(current);
                        }
                    }
                }
                else
                {
                    if (isRepeated)
                    {
                        compressedData.Add(TransformNumberToRepeatedCounter(count));
                        compressedData.Add(previous);
                        count = 0;
                        if (i == ImageBytes.Length)
                        {
                            compressedData.Add(0);
                            compressedData.Add(current);
                        }
                    }
                    else
                    {
                        if (counterIndex == -1)
                        {
                            counterIndex = compressedData.Count;
                            compressedData.Add(0);
                        }
                        else
                        {
                            compressedData[counterIndex]++;
                        }
                        compressedData.Add(previous);
                        if (i == ImageBytes.Length)
                        {
                            compressedData[counterIndex]++;
                            compressedData.Add(current);
                        }
                    }
                    isRepeated = false;
                }
            }


            return compressedData.ToArray();
        }

        private void AddNotSimilarSequance(ref List<byte> result, List<byte> addedBytes)
        {
            result.Add((byte)(addedBytes.Count - 1));
            for (int i = 0; i < addedBytes.Count; i++)
            {
                result.Add(addedBytes[i]);
            }
        }

        private void AddSimilarSequance(ref List<byte> result, byte count, byte value)
        {
            result.Add((byte)(FlagSize + count));
            result.Add(value);
        }
        private byte TransformNumberToRepeatedCounter(byte count) => (byte)(FlagSize + count);
        private byte TransformRepeatedCountToNumber(byte imageByte) => (byte)(imageByte - FlagSize);
        private bool IsRepeatedCounter(byte num) => num > FlagSize;
        public byte[] Decompress(byte[] compressedImage)
        {
            if (compressedImage == null || compressedImage.Length == 0 || compressedImage.Length == 1)
            {
                throw new ArgumentException("Недопустимые параметры.");
            }

            List<byte> decompressedData = new List<byte>();
            for (int i = 0; i < compressedImage.Length; i++)
            {

                if (IsRepeatedCounter(compressedImage[i]))
                {
                    byte count = TransformRepeatedCountToNumber(compressedImage[i]);
                    DecompressRepeatedSequence(ref decompressedData, compressedImage, ref i, count + 1);
                }
                else
                {
                    DecompressNotRepeatedSequence(ref decompressedData, compressedImage, ref i, compressedImage[i] + 1);
                }
            }

            return decompressedData.ToArray();
        }

        /// <summary>
        /// Превращает сжатое значение в последовательность повторяющихся элементов
        /// </summary>
        /// <param name="result">Список хранящий декодированные элементы</param>
        /// <param name="compressedArray">Массив с сжатыми значениями</param>
        /// <param name="currentIndex">Индекс на мамент считывания количества элементов в последовательности</param>
        /// <param name="count">Количество элементов последовательности</param>
        private void DecompressRepeatedSequence(ref List<byte> result, in byte[] compressedArray,ref int currentIndex, int count) 
            => DecompressSequence(ref result, compressedArray, ref currentIndex, count, true);

        /// <summary>
        /// Превращает сжатое значение в последовательность неповторяющихся элементов
        /// </summary>
        /// <param name="result">Список хранящий декодированные элементы</param>
        /// <param name="compressedArray">Массив с сжатыми значениями</param>
        /// <param name="currentIndex">Индекс на мамент считывания количества элементов в последовательности</param>
        /// <param name="count">Количество элементов последовательности</param>
        private void DecompressNotRepeatedSequence(ref List<byte> result, in byte[] compressedArray, ref int currentIndex, int count) 
            => DecompressSequence(ref result, compressedArray, ref currentIndex, count);

        /// <summary>
        /// Превращает сжатое значение в последовательность
        /// </summary>
        /// <param name="result">Список хранящий декодированные элементы</param>
        /// <param name="compressedArray">Массив с сжатыми значениями</param>
        /// <param name="currentIndex">Индекс на мамент считывания количества элементов в последовательности</param>
        /// <param name="count">Количество элементов последовательности</param>
        /// <param name="isRepeated">Является ли последовательность повторяющей</param>
        private void DecompressSequence(ref List<byte> result, in byte[] compressedArray, ref int currentIndex, int count, bool isRepeated = false)
        {
            if(isRepeated) currentIndex += 1;
            int offset = isRepeated ? 0 : 1;
            for (byte i = 0; i < count; i++)
            {
                currentIndex += offset;
                result.Add(compressedArray[currentIndex]);
            }
        }


    }
}
