using ImageCompression.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RLE = ImageCompression.Algorithms.RunLengthEncoding;

namespace ImageCompression.Tests
{
    [TestClass]
    public class RunLengthEncoding
    {
        IAlgorithm Algorithm = new RLE();

        [TestMethod]
        public void OneByte()
        {
            byte[] uncompressedExpected = new byte[] { 1 };
            byte[] compressedExpected = new byte[] {0, 1 };
            CheckArrays(uncompressedExpected, compressedExpected);
        }

        [TestMethod]
        public void TwoSimilarBytes()
        {
            byte[] uncompressedExpected = new byte[] { 1, 1 };
            byte[] compressedExpected = new byte[] { 129, 1 };
            CheckArrays(uncompressedExpected, compressedExpected);
        }

        [TestMethod]
        public void TwoNotSimilarBytes()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2 };
            byte[] compressedExpected = new byte[] { 1, 1, 2 };
            CheckArrays(uncompressedExpected, compressedExpected);
        }
        [TestMethod]
        public void ThreeNotSimilarBytes()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 3 };
            byte[] compressedExpected = new byte[] { 2,1, 2, 3 };
            CheckArrays(uncompressedExpected, compressedExpected);
        }

        [TestMethod]
        public void ThreeSimilarBytes()
        {
            byte[] uncompressedExpected = new byte[] { 2, 2, 2 };
            byte[] compressedExpected = new byte[] { 130, 2 };
            CheckArrays(uncompressedExpected, compressedExpected);
        }
        [TestMethod]
        public void FourNotSimilarBytes()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 3, 4 };
            byte[] compressedExpected = new byte[] {3, 1, 2, 3, 4 };
            CheckArrays(uncompressedExpected, compressedExpected);
        }

        [TestMethod]
        public void FourSimilarBytes()
        {
            byte[] uncompressedExpected = new byte[] { 2, 2, 2, 2 };
            byte[] compressedExpected = new byte[] { 131, 2 };
            CheckArrays(uncompressedExpected, compressedExpected);
        }

        [TestMethod]
        public void FourBytesTwoSimilar()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 2, 3 };
            byte[] compressedExpected = new byte[] { 0, 1, 129, 2,0, 3 };
            CheckArrays(uncompressedExpected, compressedExpected);
        }

        [TestMethod]
        public void MixBytesFinischWithSimilar()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 2, 2, 2, 3, 4, 5, 1, 1, 1, 1 };
            byte[] compressedExpected = new byte[] {0, 1, 131, 2,2, 3, 4, 5, 131, 1 };
            CheckArrays(uncompressedExpected, compressedExpected);
        }

        [TestMethod]
        public void MixBytesFinischWithNotSimilar()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 2, 2, 2, 3, 4, 5, 1, 1, 1, 1, 2, 3 };
            byte[] compressedExpected = new byte[] {0, 1, 131, 2, 2, 3, 4, 5, 131, 1,1, 2, 3 };
            CheckArrays(uncompressedExpected, compressedExpected);
        }

        [TestMethod]
        public void MixBytesWithThreeSimilarSequence()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 2, 2, 2, 3, 4, 5, 1, 1, 1, 1, 2, 3, 4, 4, 4, 4 };
            byte[] compressedExpected = new byte[] {0, 1, 131, 2,2, 3, 4, 5, 131, 1, 1, 2, 3, 131, 4 };
            CheckArrays(uncompressedExpected, compressedExpected);
        }

        [TestMethod]
        public void MixBytesWithThreeSimilarSequence2()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 2, 2, 2, 1, 1, 1, 1, 2, 3, 4, 4, 4, 4 };
            byte[] compressedExpected = new byte[] { 0, 1, 131, 2, 131, 1, 1, 2, 3, 131, 4 };
            CheckArrays(uncompressedExpected, compressedExpected);
        }

        [TestMethod]
        public void DecompressionWith255()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 2, 2, 2, 255, 255, 255, 255, 1, 255, 1, 2, 3, 4, 4, 4, 4 };
            byte[] compressedExpected = new byte[] { 0, 1, 131, 2, 131, 255, 4, 1, 255, 1, 2, 3, 131, 4 };
            CheckDecompression(uncompressedExpected, compressedExpected);
        }

        [TestMethod]
        public void More128ByteSimilar()
        {
            byte[] uncompressedExpected = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //10
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //20
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //30
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //40
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //50
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //60
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //70
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //80
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //90
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //100
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //110
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //120
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1 };
            byte[] compressedExpected = new byte[] { 255, 1, 0, 1 };
            CheckArrays(uncompressedExpected, compressedExpected);
        }

        [TestMethod]
        public void More256ByteSimilar()
        {
            byte[] uncompressedExpected = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //10
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //20
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //30
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //40
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //50
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //60
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //70
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //80
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //90
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //100
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //110
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //120
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //130
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //140
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //150
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //160
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //170
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //180
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //190
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //200
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //210
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //220
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //230
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //240
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //250
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1, //260
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            byte[] compressedExpected = new byte[] { 255, 1, 255, 1,141,1 };
            CheckArrays(uncompressedExpected, compressedExpected);
        }

        public void CheckArrays(byte[] uncompressedExpected, byte[] compressedExpected)
        {
            CheckCompression(uncompressedExpected, compressedExpected);
            CheckDecompression(uncompressedExpected, compressedExpected);
        }

        public void CheckCompression(byte[] uncompressedExpected, byte[] compressedExpected)
        {
            byte[] compressedActual = Algorithm.Compress(uncompressedExpected);
            CollectionAssert.AreEquivalent(compressedExpected, compressedActual, "Compression failed");
        }

        public void CheckDecompression(byte[] uncompressedExpected, byte[] compressedExpected)
        {
            byte[] uncompressedActual = Algorithm.Decompress(compressedExpected);
            CollectionAssert.AreEquivalent(uncompressedExpected, uncompressedActual, "Decompression failed");
        }
    }
}
