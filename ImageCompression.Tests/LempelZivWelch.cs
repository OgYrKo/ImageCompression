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
            byte[] expected = new byte[] { 0b_10000000, 0b_0_0000000, 0b_01_000000, 0b_010_10000, 0b_0010_1000,0b_00100_100,0b_000001_00 };////256,1,2,258,260,267
            
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
