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
                        AddSimilarSequance(ref compressedData, count,previous);
                        
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
                            compressedData.Add(TransformToRepeatedCounter(count));
                            compressedData.Add(current);
                        }
                    }
                }
                else
                {
                    if (isRepeated)
                    {
                        compressedData.Add(TransformToRepeatedCounter(count));
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
        private byte TransformToRepeatedCounter(byte count) => (byte)(FlagSize + count);
        private byte TransformRepeatedCountToNumber(byte imageByte) => (byte)(imageByte - FlagSize);
        private bool IsRepeatedCounter(byte num) => num > FlagSize;
        public byte[] Decompress(byte[] compressedImage)
        {
            if (compressedImage == null || compressedImage.Length == 0)
            {
                throw new ArgumentException("Недопустимые параметры.");
            }
            if (compressedImage.Length == 1) return compressedImage;

            List<byte> decompressedData = new List<byte>();
            for (int i = 0; i < compressedImage.Length; i++)
            {

                if (IsRepeatedCounter(compressedImage[i]))
                {
                    byte count = (byte)(TransformRepeatedCountToNumber(compressedImage[i]) + 1);
                    i++;
                    for (byte j = 0; j < count; j++)
                    {
                        decompressedData.Add(compressedImage[i]);
                    }
                }
                else
                {
                    byte count = (byte)(compressedImage[i] + 1);
                    for (byte j = 0; j < count; j++)
                    {
                        i++;
                        decompressedData.Add(compressedImage[i]);
                    }
                }
            }

            return decompressedData.ToArray();
        }



    }
}
