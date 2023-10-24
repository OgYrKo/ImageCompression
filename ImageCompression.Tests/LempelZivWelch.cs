using ImageCompression.Algorithms;
using ImageCompression.Common;
using ImageCompression.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using static ImageCompression.Tests.CheckArrays;
using System.IO;
using LZW = ImageCompression.Algorithms.LempelZivWelch;

namespace ImageCompression.Tests
{
    [TestClass]
    public class LempelZivWelch
    {
        LZW Algorithm = new LZW();

        [TestMethod]
        public void TestCompressionHeader()
        {
            byte[] input = new byte[] { 66,77,70,25,73,0,0,0,0,0,//66,77 - 258, 77,70 - 259, 70,25 - 260, 25,73 - 261, 73,0 - 262, 0,0 - 263, (263) (0,0),0 - 264, (263) (0,0),54 - 265
                                        54,0,0,0,40,0,0,0,151,6,//54,0 - 266, (264) (0,0,0),40 - 267, 40,0 - 268, (264) (0,0,0),151 - 269, 151,6 - 270, 6,0 - 271
                                        0,0,178,3,0,0,1,0,24,0, //(263) (0,0),178 - 272, 178,3 - 273, 3,0 - 274, (263) (0,0),1 - 275, 1,0 - 276, 0,24 - 277, 24,0 - 278,
                                        0,0,0,0,16,25,73,0,0,0, // (264) (0,0,0), 0 - 279, (263) (0,0),16 - 280, 16,25 - 281, (261) (25,73),0 - 282,
                                        0,0,0,0,0,0,0,0,0,0, // (279) (0,0,0,0),0 - 283,(282) (0,0,0,0,0),0 - 284, (283) (0,0,0,0,0,0),0 - 285
                                        0,0,0,0,};
            ushort[] expected = new ushort[] { 256, 66, 77, 70, 25, 73, 0, 263, 263, 54, 264, 40, 264, 151, 6, 263, 178, 3, 263, 1, 0, 24, 264, 263, 16, 261,279,283,284,263,257 };
            byte[] compressed = Algorithm.Compress(input);
            List<ushort> actual = new List<ushort>();
            int currentIndex = 0;
            int readBeatsInLast = 0;


            byte bitsInNum = 9;
            int allBits = compressed.Length * 8;
            int bitsRead = 0;
            while (bitsRead + bitsInNum <= allBits)
            {
                actual.Add(LZW.ReadValue(compressed, ref currentIndex, ref readBeatsInLast, bitsInNum));
                bitsRead += bitsInNum;
            }
            CollectionAssert.AreEqual(expected, actual, $"Compression failed.\nExpected: {string.Join(" ", expected)}\nActual: {string.Join(" ", actual)}\n");
        }

        [TestMethod]
        public void TestDeompressionHeader()
        {
            byte[] expected = new byte[] { 66,77,70,25,73,0,0,0,0,0,//66,77 - 258, 77,70 - 259, 70,25 - 260, 25,73 - 261, 73,0 - 262, 0,0 - 263, (263) (0,0),0 - 264, (263) (0,0),54 - 265
                                        54,0,0,0,40,0,0,0,151,6,//54,0 - 266, (264) (0,0,0),40 - 267, 40,0 - 268, (264) (0,0,0),151 - 269, 151,6 - 270, 6,0 - 271
                                        0,0,178,3,0,0,1,0,24,0, //(263) (0,0),178 - 272, 178,3 - 273, 3,0 - 274, (263) (0,0),1 - 275, 1,0 - 276, 0,24 - 277, 24,0 - 278,
                                        0,0,0,0,16,25,73,0,0,0, // (264) (0,0,0), 0 - 279, (263) (0,0),16 - 280, 16,25 - 281, (261) (25,73),0 - 282,
                                        0,0,0,0,0,0,0,0,0,0, // (279) (0,0,0,0),0 - 283,(282) (0,0,0,0,0),0 - 284, (283) (0,0,0,0,0,0),0 - 285
                                        0,0,0,0,};
            ushort[] inputX10 = new ushort[] { 256, 66, 77, 70, 25, 73, 0, 263, 263, 54, 264, 40,
                                               264, 151, 6, 263, 178, 3, 263, 1, 0, 24, 264, 263,
                                               16, 261, 279, 283, 284, 263, 257 };
            List<byte> input = new List<byte>();
            byte freeBeatsInLast = 0;
            byte bitsInNum = 9;
            foreach (var value in inputX10)
            {
                freeBeatsInLast = LZW.WriteValue(ref input, value, freeBeatsInLast,bitsInNum);
            }
            byte[]actual = Algorithm.Decompress(input.ToArray());
            CollectionAssert.AreEqual(expected, actual, $"Compression failed.\nExpected: {string.Join(" ", expected)}\nActual: {string.Join(" ", actual)}\n");
        }

        [TestMethod]
        public void TestCompressionOne()
        {
            byte[] input = new byte[] { 1 };
            byte[] expected = new byte[] { 0b_10000000, 0b_0_0000000, 0b_01_100000, 0b_001_00000 };//256,1,257
            //Check(input, expected, Algorithm);
            CheckCompression(input, expected, Algorithm);
        }

        [TestMethod]
        public void TestCompressionWithTwoNumbers()
        {
            byte[] input = new byte[] { 1, 2 };
            byte[] expected = new byte[] { 0b_10000000, 0b_0_0000000, 0b_01_000000, 0b_010_10000, 0b_0001_0000 };//256,1,2,257
            CheckCompression(input, expected, Algorithm);
        }

        [TestMethod]
        public void TestCompressionWithTwoRepeat()
        {
            byte[] input = new byte[] { 1, 2, 1, 2, 1, 2, 1 };//1,2 - 258, 2,1 - 259, 1,2,1 - 260, 2,1,2 - 261 
            byte[] expected = new byte[] { 0b_10000000, 0b_0_0000000, 0b_01_000000, 0b_010_10000, 0b_0010_1000, 0b_00100_100, 0b_000001_00 };////256,1,2,258,260,267

            CheckCompression(input, expected, Algorithm);
        }

        [TestMethod]
        public void TestDeompressionOne()
        {
            byte[] input = new byte[] { 1 };
            byte[] expected = new byte[] { 0b_10000000, 0b_0_0000000, 0b_01_100000, 0b_001_00000 };//256,1,257
            CheckDecompression(input, expected, Algorithm);
            //CheckCompression(input, expected, Algorithm);
        }

        [TestMethod]
        public void TestWithTwoNumbers()
        {
            byte[] input = new byte[] { 1, 2 };
            byte[] expected = new byte[] { 0b_10000000, 0b_0_0000000, 0b_01_000000, 0b_010_10000, 0b_0001_0000 };//256,1,2,257
            Check(input, expected, Algorithm);
            //CheckCompression(input, expected, Algorithm);
        }

        [TestMethod]
        public void TestWithOneRepeat()
        {
            byte[] input = new byte[] { 1, 2, 1, 2 };
            byte[] expected = new byte[] { 0b_10000000, 0b_0_0000000, 0b_01_000000, 0b_010_10000, 0b_0010_1000, 0b_00001_000 };////256,1,2,258,257
            Check(input, expected, Algorithm);
            //CheckCompression(input, expected, Algorithm);
        }



        [TestMethod]
        public void TestWithTwoRepeat()
        {
            byte[] input = new byte[] { 1, 2, 1, 2, 1, 2, 1 };//1,2 - 258, 2,1 - 259, 1,2,1 - 260, 2,1,2 - 261 
            byte[] expected = new byte[] { 0b_10000000, 0b_0_0000000, 0b_01_000000, 0b_010_10000, 0b_0010_1000, 0b_00100_100, 0b_000001_00 };////256,1,2,258,260,267

            Check(input, expected, Algorithm);
            //CheckDecompression(input, expected, Algorithm);
        }

        [TestMethod]
        public void TestWriteBit()
        {

            byte[] expected = new byte[] { 0b_0000_0000, 0b_0001_0000, 0b_0000_1000, 0b_0000_0000, 0b_0100_1000 }, actual;
            List<byte> result = new List<byte>();
            byte freeBits = LZW.WriteValue(ref result, 1, 0, 12);
            freeBits = LZW.WriteValue(ref result, 8, freeBits, 12);//freeBits - 4
            freeBits = LZW.WriteValue(ref result, 1, freeBits, 10);//0
            LZW.WriteValue(ref result, 8, freeBits, 6);//6

            actual = result.ToArray();
            CollectionAssert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void TestReadBit()
        {
            byte[] input = new byte[] { 0b_0000_0000, 0b_0001_0000, 0b_0000_1000, 0b_0000_0000, 0b_0100_1000 };
            ushort[] expected = new ushort[] { 1, 8, 1, 8 };
            List<ushort> result = new List<ushort>();
            int curent = 0;
            int readBitsCount = 0;
            result.Add(LZW.ReadValue(input, ref curent, ref readBitsCount, 12));
            result.Add(LZW.ReadValue(input, ref curent, ref readBitsCount, 12));
            result.Add(LZW.ReadValue(input, ref curent, ref readBitsCount, 10));
            result.Add(LZW.ReadValue(input, ref curent, ref readBitsCount, 6));
            CollectionAssert.AreEqual(expected, result, $"Compression failed.\nExpected: {string.Join(" ", expected)}\nActual: {string.Join(" ", result)}\n");
        }

        [TestMethod]
        public void TestReadBit2()
        {
            byte[] input = new byte[] { 0b_1000_0000, 0b_0000_0000 };
            ushort[] expected = new ushort[] { 256 };
            List<ushort> result = new List<ushort>();
            int curent = 0;
            int readBitsCount = 0;
            result.Add(LZW.ReadValue(input, ref curent, ref readBitsCount, 9));
            CollectionAssert.AreEqual(expected, result, $"Compression failed.\nExpected: {string.Join(" ", expected)}\nActual: {string.Join(" ", result)}\n");
        }
    }
}
