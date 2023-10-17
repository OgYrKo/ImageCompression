using ImageCompression.Algorithms;
using ImageCompression.Common;
using ImageCompression.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using LZW = ImageCompression.Algorithms.LempelZivWelch;

namespace ImageCompression.Tests
{
    [TestClass]
    public class LempelZivWelch
    {
        LZW Algorithm = new LZW();


        [TestMethod]
        public void TestCommonLZW()
        {

            string str = "/WED/WE/WEE/WEB/WET";
            string[] compressedExpected = new string[] 
            {
                "256","/","W","E","D","258","E","262","263","259","B","262"
            };
            CheckString(str,compressedExpected);
        }

        [TestMethod]
        public void Compressed()
        {
            byte[] uncompressedExpected = new byte[] { 45, 55, 55, 151, 55, 55, 55 };
            List<ushort> compressedExpected = new List<ushort>() { 256, 45, 55, 55, 151, 259 };
            List<ushort> compressedActual = Algorithm.CompressChain(uncompressedExpected);
            CollectionAssert.AreEqual(compressedExpected, compressedActual, "Compression failed");
        }

        [TestMethod]
        public void DecompressedTrap()
        {
            string str = "JOEYNJOEYNJOEY";
        }

        [TestMethod]
        public void TestWriteBit()
        {
           
            byte[] expected = new byte[] { 0b_0000_0000, 0b_0001_0000, 0b_0000_1000, 0b_0000_0000, 0b_0100_1000 }, actual;
            List<byte>result=new List<byte>();
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
            List<ushort> result = new List<ushort>();
            int curent = 0;
            int readBitsCount = 0;
            result.Add(LZW.ReadValue(input,ref curent, ref readBitsCount, 12));
            result.Add(LZW.ReadValue(input,ref curent, ref readBitsCount, 12));
            result.Add(LZW.ReadValue(input,ref curent, ref readBitsCount, 10));
            result.Add(LZW.ReadValue(input,ref curent, ref readBitsCount, 6));
        }



        private void CheckString(string inputStr, string[] compressedExpected)
        {
            byte[] uncompressedExpected = Converter.ToBytes(inputStr);
            byte[] compressed = Algorithm.Compress(uncompressedExpected);
            string[] compressedActual = Converter.ToStringsMixed(Algorithm.CompressChain(uncompressedExpected).ToArray());
            byte[] decompressed = Algorithm.Decompress(compressed);
            string decompressedActual = Algorithm.DecompressChain(compressed);
            CollectionAssert.AreEqual(compressedExpected, compressedActual, "Compression failed");
            CollectionAssert.AreEqual(compressedExpected, compressedActual, "Decompression failed");
        }



        
    }
}
