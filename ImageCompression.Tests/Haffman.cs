using ImageCompression.Common;
using ImageCompression.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HaffmanMethod = ImageCompression.Algorithms.Haffman;
using System;
using static ImageCompression.Tests.CheckArrays;

namespace ImageCompression.Tests
{
    [TestClass]
    public class Haffman
    {
        HaffmanMethod algorithm = new HaffmanMethod();
        //[TestMethod]
        //public void TestMethod1()
        //{
        //    string s = "ABCDEABCDEABCDEABCDEABCDEACDEACDEACDEADEADEDDD";
        //    byte[] data = Converter.ToBytes(s);
        //    //A - 10
        //    //B - 000
        //    //C - 001
        //    //D - 01
        //    //E - 11
        //    byte[] expected = new byte[] { 0b10_000_001,0b_01_11_10_00,0b_0_001_01_11, 0b10_000_001, 0b_01_11_10_00, 0b_0_001_01_11, 0b10_000_001, 0b_01_11_10_00, //ABCDE x 5 + A +2/3C
        //      0b_1_01_11_10_0,0b_01_01_11_10,0b_001_01_11_1,//ACDE x 3 +1/2 A
        //      0b_0_01_11_10_0,0b_1_11_01_01_0,0b_1_0000000//ADE x 2 + D x 3
        //    };
        //    CheckCompression(data,expected, algorithm);
        //}
        [TestMethod]
        public void TestMethod1()
        {
            string s = "ABCDEABCDEABCDEABCDEABCDEACDEACDEACDEADEADEDDD";
            byte[] data = Converter.ToBytes(s);
            byte[]compressed = algorithm.Compress(data);
            byte[] decompressed = algorithm.Decompress(compressed);

            CollectionAssert.AreEqual(data, decompressed, $"Failed.\nInput: {string.Join(" ", data)}\nActual: {string.Join(" ", decompressed)}\n");
        }
    }
}
