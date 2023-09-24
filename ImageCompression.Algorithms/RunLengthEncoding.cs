using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ImageCompression.Interfaces;

namespace ImageCompression.Algorithms
{
    /// <summary>
    /// RLE algorithm - lossless data compression
    /// </summary>
    public class RunLengthEncoding : IAlgorithm
    {
        private const byte FlagSize = 128;
        private const byte MaxCount = 128;
        private readonly int ArraysCount;

        public RunLengthEncoding(int arraysCount)
        {
            ArraysCount = arraysCount;
        }

        public byte[] Compress(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                throw new ArgumentException("Недопустимые параметры.");
            }

            int inputLength = byteArray.Length;

            if (ArraysCount <= 0 || inputLength % ArraysCount != 0)
            {
                throw new ArgumentException("Недопустимое количество массивов для разделения.");
            }

            int subArrayLength = inputLength / ArraysCount;
            byte[][] result = new byte[ArraysCount][];
            Parallel.For(0, ArraysCount, i =>
            {
                byte[] subArray = new byte[subArrayLength];
                for (int j = 0; j < subArrayLength; j++)
                {
                    subArray[j] = byteArray[j * ArraysCount + i];
                }
                result[i] = CompressBytes(subArray);
            });
            return CombineCompressionArrays(result);
        }

        private byte[] CombineCompressionArrays(byte[][] bytes)
        {
            List<byte>[] results = new List<byte>[bytes.Length];

            Parallel.For(0, bytes.Length, arrayIndex =>
            {
                results[arrayIndex] = new List<byte>(bytes[arrayIndex].Length);
                int times = bytes[arrayIndex].Length / byte.MaxValue;
                int count = bytes[arrayIndex].Length % byte.MaxValue;

                for (int j = 0; j < times; j++)
                {
                    results[arrayIndex].Add(byte.MaxValue);
                    for(int i = 0; i < byte.MaxValue; i++)
                    {
                        results[arrayIndex].Add(bytes[arrayIndex][j * byte.MaxValue+i]);
                    }
                }
                if (count != 0)
                {
                    results[arrayIndex].Add((byte)count);
                    for (int i = 0; i < count; i++)
                    {
                        results[arrayIndex].Add(bytes[arrayIndex][times * byte.MaxValue + i]);
                    }
                }
                results[arrayIndex].Add(0);
            });

            int totalBytes = results.Sum(arr => arr.Count);
            IEnumerable<byte> result = new List<byte>(totalBytes)
            {
                (byte)bytes.Length
            };
            
            for (int i = 0; i < bytes.Length; i++)
            {
                result = result.Concat(results[i]);
            }
            
            return result.ToArray();
        }

        private byte[] CombineDecompressionArrays(byte[][] bytes)
        {
            byte[] result = new byte[bytes.Length * bytes[0].Length];
            for (int i = 0; i < bytes[0].Length; i++)
            {
                for(int j=0; j < bytes.Length; j++)
                {
                    result[i * bytes.Length + j] = bytes[j][i];
                }
            }
            return result;
        }

        private byte[][] SplitCompressionArray(byte[] bytes)
        {
            int arraysCount = bytes[0];

            byte[][] result = new byte[arraysCount][];
            for (int arrayIndex = 0, i = 1; arrayIndex < arraysCount; arrayIndex++, i++)
            {
                List<byte> listResult = new List<byte>();
                while (bytes[i] != 0)
                {
                    int count = bytes[i];
                    i++;
                    for (int j = 0; j < count; j++, i++)
                    {
                        listResult.Add(bytes[i]);
                    }
                }
                result[arrayIndex] = listResult.ToArray();
            }

            return result;
        }
        private byte[] CompressBytes(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                throw new ArgumentException("Недопустимые параметры.");
            }

            List<byte> countedSequance = CountSequances(byteArray);
            return JoinSingleSequance(countedSequance);
        }
        public byte[] Decompress(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                throw new ArgumentException("Недопустимые параметры.");
            }

            byte[][] arrays = SplitCompressionArray(byteArray);
            byte[][] result = new byte[arrays.Length][];
            Parallel.For(0, arrays.Length, i =>
            {
                result[i] = DecompressBytes(arrays[i]);
            });

            return CombineDecompressionArrays(result);
        }
        private byte[] DecompressBytes(byte[] compressedImage)
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
        private List<byte> CountSequances(byte[] byteArray)
        {
            List<byte> result = new List<byte>();
            List<byte> sequence = new List<byte>();
            byte previous = byteArray[0];
            byte current;
            for (int i = 1; i < byteArray.Length; i++)
            {
                current = byteArray[i];
                sequence.Add(previous);
                if (previous != current)
                {
                    SaveSequance(ref result, sequence);
                    sequence = new List<byte>();
                }
                previous = current;
            }
            sequence.Add(previous);
            SaveSequance(ref result, sequence);

            return result;
        }
        private bool IsRpeatedSequance(List<byte> result)
        {
            if (result.Count == 1) return false;
            return result[0] == result[1];
        }
        private void SaveSequance(ref List<byte> result, List<byte> sequance)
        {
            if (IsRpeatedSequance(sequance)) SaveRepeatedSequance(ref result, sequance.Count, sequance[0]);
            else SaveNotRepeatedSequance(ref result, sequance);
        }
        private byte[] JoinSingleSequance(List<byte> byteArray)
        {
            List<byte> result = new List<byte>();

            int seqIndex = -1;
            for (int i = 0; i < byteArray.Count; i += 2)
            {
                if (byteArray[i] == 0)
                {
                    AddNotRepeatedSequance(ref result, byteArray[i + 1], ref seqIndex);
                }
                else
                {
                    seqIndex = -1;
                    result.Add(byteArray[i]);
                    result.Add(byteArray[i + 1]);
                }
            }


            return result.ToArray();
        }
        private void AddNotRepeatedSequance(ref List<byte> result, byte value, ref int counterIndex)
        {
            if (counterIndex != -1 && result[counterIndex] != FlagSize - 1)
            {
                result[counterIndex]++;
            }
            else
            {
                counterIndex = result.Count;
                result.Add(0);
            }
            result.Add(value);
        }
        /// <summary>
        /// Сжатие последовательности повторяющихся элементов
        /// </summary>
        /// <param name="result">Список для сжатых последовательностей</param>
        /// <param name="count">Количество повторяющихся элементов</param>
        /// <param name="value">Повторяющийся элемент</param>
        private void SaveRepeatedSequance(ref List<byte> result, int count, byte value)
        {
            if (count > MaxCount)
            {
                int times = count / MaxCount;
                count = count % MaxCount;
                for (int i = 0; i < times; i++)
                {
                    result.Add(TransformNumberToRepeatedCounter(MaxCount));
                    result.Add(value);
                }
            }
            if (count == 0)
                return;
            if (count == 1)
                result.Add((byte)(count - 1));
            else
                result.Add(TransformNumberToRepeatedCounter((byte)count));


            result.Add(value);
        }
        /// <summary>
        /// Сжатие последовательности неповторяющихся элементов
        /// </summary>
        /// <param name="result">Список для сжатых последовательностей</param>
        /// <param name="array">Массив неповторяющихся элементов</param>
        private void SaveNotRepeatedSequance(ref List<byte> result, List<byte> array)
        {
            result.Add(0);
            result.Add(array[0]);
        }
        private byte TransformNumberToRepeatedCounter(byte count) => (byte)(FlagSize + count - 1);
        private byte TransformRepeatedCountToNumber(byte imageByte) => (byte)(imageByte - FlagSize);
        private bool IsRepeatedCounter(byte num) => num > FlagSize;
        /// <summary>
        /// Превращает сжатое значение в последовательность повторяющихся элементов
        /// </summary>
        /// <param name="result">Список хранящий декодированные элементы</param>
        /// <param name="compressedArray">Массив с сжатыми значениями</param>
        /// <param name="currentIndex">Индекс на мамент считывания количества элементов в последовательности</param>
        /// <param name="count">Количество элементов последовательности</param>
        private void DecompressRepeatedSequence(ref List<byte> result, in byte[] compressedArray, ref int currentIndex, int count)
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
            if (isRepeated) currentIndex += 1;
            int offset = isRepeated ? 0 : 1;
            for (byte i = 0; i < count; i++)
            {
                currentIndex += offset;
                result.Add(compressedArray[currentIndex]);
            }
        }
    }
}
