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

        #region <--- Compression --->
        /// <summary>
        /// Сжатие данных 
        /// </summary>
        /// <param name="byteArray">Массив байт для сжатия</param>
        /// <returns>Сжатый массив байт</returns>
        /// <exception cref="ArgumentException"></exception>
        public byte[] Compress(byte[] byteArray)
        {
            CheckNull(byteArray);
            int inputLength = byteArray.Length;
            if (ArraysCount <= 0 || inputLength % ArraysCount != 0||ArraysCount>inputLength)
            {
                throw new ArgumentException("Недопустимое количество массивов для разделения.");
            }

            int subArrayLength = inputLength / ArraysCount;
            List<byte>[] result = new List<byte>[ArraysCount];
            
            Parallel.For(0, ArraysCount, i =>
            {
                
                byte[] subArray = new byte[subArrayLength];
                for (int j = 0; j < subArrayLength; j++)
                {
                    subArray[j] = byteArray[j * ArraysCount + i];
                }
                result[i] = CompressBytes(subArray);
            });


            byte[] finish = CombineCompressionArrays(result);
            return finish;
        }

        /// <summary>
        /// Сжимает переданный массив байтов по алгоритму RLE
        /// </summary>
        /// <param name="byteArray">Массив байтов для сжатия</param>
        /// <returns>Список сжатых байтов по алгоритму RLE</returns>
        private List<byte> CompressBytes(byte[] byteArray)
        {
            CheckNull(byteArray);
            List<byte> countedSequance = CountSequances(byteArray);
            return JoinSingleSequance(countedSequance);
        }

        /// <summary>
        /// Подсчитывает количество элементов в каждой последовательности массива
        /// </summary>
        /// <param name="byteArray">Массив байтов для сжатия</param>
        /// <returns>Список байтов вида: количесто, элемент</returns>
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
                    sequence.Clear();
                }
                previous = current;
            }
            sequence.Add(previous);
            SaveSequance(ref result, sequence);

            
            return result;
        }

        /// <summary>
        /// Сжатие последовательности в массиве
        /// </summary>
        /// <param name="result">Сжатая последовательность байт вида: количество, элемент</param>
        /// <param name="sequance">Последовательность байт в массиве</param>
        private void SaveSequance(ref List<byte> result, List<byte> sequance)
        {
            int count = sequance.Count;
            byte value = sequance[0];
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
            if (count == 0) return;

            if (count == 1)
                result.Add(TransformNumberToNotRepeatedCounter((byte)count));
            else
                result.Add(TransformNumberToRepeatedCounter((byte)count));

            result.Add(value);
        }

        /// <summary>
        /// Соеденяет еденичные элементы в одну последовательность
        /// </summary>
        /// <param name="byteArray">Список байтов вида: количесто, элемент</param>
        /// <returns>Список байтов по алгоритму RLE</returns>
        private List<byte> JoinSingleSequance(List<byte> byteArray)
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
                    AddRepeatedSequance(ref result, byteArray[i], byteArray[i + 1], ref seqIndex);
                }
            }
            return result;
        }

        /// <summary>
        /// Добавляет элемент в последовательность без повторений элемента
        /// </summary>
        /// <param name="result">Список в который сохраняются данные</param>
        /// <param name="count">Количество повторяющихся элементов</param>
        /// <param name="counterIndex">Счетчик</param>
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
        /// Добавляет элемент в последовательность с повторением элемента
        /// </summary>
        /// <param name="result">Список в который сохраняются данные</param>
        /// <param name="count">Количество повторяющихся элементов</param>
        /// <param name="value">Значение повторяющихся элементов</param>
        /// <param name="counterIndex">Счетчик</param>
        private void AddRepeatedSequance(ref List<byte> result, byte count, byte value, ref int counterIndex)
        {
            counterIndex = -1;
            result.Add(count);
            result.Add(value);
        }

        /// <summary>
        /// Соединение разделенных массивов для записи в файл
        /// </summary>
        /// <param name="bytes">Список сжатых массивов по RLE</param>
        /// <returns>Сжатый массив байтов для файла</returns>
        private byte[] CombineCompressionArrays(List<byte>[] bytes)
        {
            List<byte>[] results = new List<byte>[bytes.Length];

            Parallel.For(0, bytes.Length, arrayIndex =>
            {
                results[arrayIndex] = new List<byte>(bytes[arrayIndex].Count);
                int times = bytes[arrayIndex].Count / byte.MaxValue;
                byte count = (byte)(bytes[arrayIndex].Count % byte.MaxValue);

                for (int j = 0; j < times; j++)
                {
                    SetFileSequanceForOneDimension(ref results[arrayIndex], ref bytes[arrayIndex], byte.MaxValue, j);
                }
                if (count != 0) 
                {
                    SetFileSequanceForOneDimension(ref results[arrayIndex], ref bytes[arrayIndex], count, times);
                }
                results[arrayIndex].Add(0);
            });

            int totalBytes = results.Sum(arr => arr.Count);
            byte[] result = new byte[totalBytes+1];
            result[0] = (byte)bytes.Length;
            for (int i = 0, index = 1; i < results.Length; i++)
            {
                for (int j = 0; j < results[i].Count; j++, index++)
                {
                    result[index] = results[i][j];
                }
            }
            return result;
        }

        /// <summary>
        /// Создает последовательность для записи в файл для одного из разделенных массивов данных
        /// </summary>
        /// <param name="result">Список, в который записывается результат</param>
        /// <param name="bytes">Список сжатых байтов</param>
        /// <param name="count">Количество элементов для записи</param>
        /// <param name="offset">Смещение от начала списка сжатых байт</param>
        private void SetFileSequanceForOneDimension(ref List<byte> result, ref List<byte> bytes, byte count, int offset)
        {
            result.Add(count);
            for (int i = 0; i < count; i++)
            {
                result.Add(bytes[offset * byte.MaxValue + i]);
            }
        }
        private byte TransformNumberToRepeatedCounter(byte count) => (byte)(FlagSize + count - 1);
        private byte TransformNumberToNotRepeatedCounter(byte count) => (byte)(count - 1);
        #endregion <--- Compression --->

        #region <--- Decompression --->
        /// <summary>
        /// Преобразовывает сжатые данные в данные без сжатия
        /// </summary>
        /// <param name="byteArray">Массив сжатых данных из файла</param>
        /// <returns>Массив байтов без сжатия</returns>
        public byte[] Decompress(byte[] byteArray)
        {
            CheckNull(byteArray);
            byte[][] arrays = SplitCompressionArray(byteArray);
            byte[][] result = new byte[arrays.Length][];
            Parallel.For(0, arrays.Length, i =>
            {
                result[i] = DecompressBytes(arrays[i]);
            });
            return CombineDecompressionArrays(result);
        }

        /// <summary>
        /// Делит сжатый массив данных из файла на необходимое количество массивов (использующееся при сжатии)
        /// </summary>
        /// <param name="bytes">Массив сжатых данных из файла</param>
        /// <returns>Необходимое количесвто сжатых массивов за алгоритмом RLE</returns>
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

        /// <summary>
        /// Преобразовывает сжатые байты в обычную последовательность
        /// </summary>
        /// <param name="compressedImage">Массив сжатых байтов</param>
        /// <returns>Массив байтов без сжатия</returns>
        private byte[] DecompressBytes(byte[] compressedImage)
        {
            List<byte> decompressedData = new List<byte>();
            for (int i = 0; i < compressedImage.Length; i++)
            {
                if (IsRepeatedCounter(compressedImage[i]))
                {
                    DecompressRepeatedSequence(ref decompressedData, compressedImage, ref i, TransformRepeatedCountToNumber(compressedImage[i]));
                }
                else
                {
                    DecompressNotRepeatedSequence(ref decompressedData, compressedImage, ref i, TransformNotRepeatedCountToNumber(compressedImage[i]));
                }
            }
            return decompressedData.ToArray();
        }

        /// <summary>
        /// Объеденяет поделеные массивы в один
        /// </summary>
        /// <param name="bytes">Массив массивов байт, на которые был поделен файл</param>
        /// <returns>Первоначальную последовательность байт</returns>
        private byte[] CombineDecompressionArrays(byte[][] bytes)
        {
            byte[] result = new byte[bytes.Length * bytes[0].Length];
            for (int i = 0; i < bytes[0].Length; i++)
            {
                for (int j = 0; j < bytes.Length; j++)
                {
                    result[i * bytes.Length + j] = bytes[j][i];
                }
            }
            return result;
        }

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
        private byte TransformRepeatedCountToNumber(byte imageByte) => (byte)(imageByte - FlagSize + 1);
        private byte TransformNotRepeatedCountToNumber(byte imageByte) => (byte)(imageByte + 1);
        private bool IsRepeatedCounter(byte num) => num > FlagSize;
        #endregion <--- Decompression --->

        private void CheckNull(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                throw new ArgumentException("Недопустимые параметры.");
            }
        }
    }
}
