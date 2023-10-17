using ImageCompression.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RLE = ImageCompression.Algorithms.RunLengthEncoding;
using static ImageCompression.Tests.CheckArrays;

namespace ImageCompression.Tests
{
    [TestClass]
    public class RunLengthEncoding
    {
        IAlgorithm Algorithm = new RLE(1);
        IAlgorithm AlgorithmRGB = new RLE(3);

        [TestMethod]
        public void OneByte()
        {
            byte[] uncompressedExpected = new byte[] { 1 };
            byte[] compressedExpected = new byte[] { 1, 2, 0, 1, 0 };
            Check(uncompressedExpected, compressedExpected, Algorithm);
        }

        [TestMethod]
        public void TwoSimilarBytes()
        {
            byte[] uncompressedExpected = new byte[] { 1, 1 };
            byte[] compressedExpected = new byte[] { 1,2,129, 1,0 };
            Check(uncompressedExpected, compressedExpected, Algorithm);
        }

        [TestMethod]
        public void TwoNotSimilarBytes()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2 };
            byte[] compressedExpected = new byte[] {1,3, 1, 1, 2,0 };
            Check(uncompressedExpected, compressedExpected, Algorithm);
        }
        [TestMethod]
        public void ThreeNotSimilarBytes()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 3 };
            byte[] compressedExpected = new byte[] {1,4, 2,1, 2, 3,0 };
            Check(uncompressedExpected, compressedExpected, Algorithm);
        }
        [TestMethod]
        public void RGBByOneBytes()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 3 };
            byte[] compressedExpected = new byte[] { 3, 2, 0,1,0,2,0,2,0,2,0,3,0 };
            Check(uncompressedExpected, compressedExpected, AlgorithmRGB);
        }

        [TestMethod]
        public void RGBByThreeBytes()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 3,4,2,5,6,2,7 };
            byte[] compressedExpected = new byte[] { 3, 4, 
                                                     2, 1, 4,6,
                                                     0, 2,
                                                     130,2,
                                                     0, 4,
                                                     2,3,5,7,0};
            Check(uncompressedExpected, compressedExpected, AlgorithmRGB);
        }

        [TestMethod]
        public void ThreeSimilarBytes()
        {
            byte[] uncompressedExpected = new byte[] { 2, 2, 2 };
            byte[] compressedExpected = new byte[] { 1,2, 130, 2,0 };
            Check(uncompressedExpected, compressedExpected, Algorithm);
        }
        [TestMethod]
        public void FourNotSimilarBytes()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 3, 4 };
            byte[] compressedExpected = new byte[] {1,5,3, 1, 2, 3, 4,0 };
            Check(uncompressedExpected, compressedExpected, Algorithm);
        }

        [TestMethod]
        public void FourSimilarBytes()
        {
            byte[] uncompressedExpected = new byte[] { 2, 2, 2, 2 };
            byte[] compressedExpected = new byte[] {1,2, 131, 2,0 };
            Check(uncompressedExpected, compressedExpected, Algorithm);
        }

        [TestMethod]
        public void FourBytesTwoSimilar()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 2, 3 };
            byte[] compressedExpected = new byte[] {1,6, 0, 1, 129, 2,0, 3,0 };
            Check(uncompressedExpected, compressedExpected, Algorithm);
        }

        [TestMethod]
        public void MixBytesFinischWithSimilar()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 2, 2, 2, 3, 4, 5, 1, 1, 1, 1 };
            byte[] compressedExpected = new byte[] {1,10, 0, 1, 131, 2,2, 3, 4, 5, 131, 1,0 };
            Check(uncompressedExpected, compressedExpected, Algorithm);
        }

        [TestMethod]
        public void MixBytesFinischWithNotSimilar()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 2, 2, 2, 3, 4, 5, 1, 1, 1, 1, 2, 3 };
            byte[] compressedExpected = new byte[] {1,13, 0, 1, 131, 2, 2, 3, 4, 5, 131, 1,1, 2, 3,0 };
            Check(uncompressedExpected, compressedExpected, Algorithm);
        }

        [TestMethod]
        public void MixBytesWithThreeSimilarSequence()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 2, 2, 2, 3, 4, 5, 1, 1, 1, 1, 2, 3, 4, 4, 4, 4 };
            byte[] compressedExpected = new byte[] {1,15, 0, 1, 131, 2,2, 3, 4, 5, 131, 1, 1, 2, 3, 131, 4,0 };
            Check(uncompressedExpected, compressedExpected, Algorithm);
        }

        [TestMethod]
        public void MixBytesWithThreeSimilarSequence2()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 2, 2, 2, 1, 1, 1, 1, 2, 3, 4, 4, 4, 4 };
            byte[] compressedExpected = new byte[] {1,11, 0, 1, 131, 2, 131, 1, 1, 2, 3, 131, 4,0 };
            Check(uncompressedExpected, compressedExpected, Algorithm);
        }

        [TestMethod]
        public void DecompressionWith255()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 2, 2, 2, 255, 255, 255, 255, 1, 255, 1, 2, 3, 4, 4, 4, 4 };
            byte[] compressedExpected = new byte[] {1,14, 0, 1, 131, 2, 131, 255, 4, 1, 255, 1, 2, 3, 131, 4,0 };
            CheckDecompression(uncompressedExpected, compressedExpected,Algorithm);
        }

        [TestMethod]
        public void BorderValues()
        {
            byte[] uncompressedExpected = new byte[] { 255,252,252,255,251,251,255,250,247,255,248,245,254,243,240,250,243,237};
            byte[] compressedExpected = new byte[] {1,21, 0,255,129, 252,0,255,129,251,11,255,250,247,255,248,245,254,243,240,250,243,237,0};
            Check(uncompressedExpected, compressedExpected, Algorithm);
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
            byte[] compressedExpected = new byte[] {1,4, 255, 1, 0, 1,0 };
            Check(uncompressedExpected, compressedExpected, Algorithm);
        }

        [TestMethod]
        public void More128ByteNotSimilar()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //10
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //20
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //30
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //40
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //50
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //60
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //70
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //80
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //90
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //100
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //110
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //120
                                                       1, 2, 1, 2, 1, 2, 1, 2,       //128
                                                       1, 2,                         //130
                                                       1, 1, 1, 1, 1, 1, 1, 1, 1 };  //9
            byte[] compressedExpected = new byte[] {1,134, 127,
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //10
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //20
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //30
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //40
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //50
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //60
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //70
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //80
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //90
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //100
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //110
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //120
                                                       1, 2, 1, 2, 1, 2, 1, 2,       //128
                                                       1, 1, 2, 136, 1,0 };
            Check(uncompressedExpected, compressedExpected, Algorithm);
        }

        [TestMethod]
        public void More128ByteNotSimilarRGB()
        {
            byte[] uncompressedExpected = new byte[] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //10
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //20
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //30
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //40
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //50
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //60
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //70
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //80
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //90
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //100
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //110
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //120
                                                       1, 2, 1, 2, 1, 2, 1, 2,       //128

                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //10
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //20
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //30
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //40
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //50
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //60
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //70
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //80
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //90
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //100
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //110
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //120
                                                       1, 2, 1, 2, 1, 2, 1, 2,       //128

                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //10
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //20
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //30
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //40
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //50
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //60
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //70
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //80
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //90
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //100
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //110
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //120
                                                       1, 2, 1, 2, 1, 2, 1, 2,       //128

                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //10
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //20
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //30
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //40
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //50
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //60
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //70
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //80
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //90
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //100
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //110
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //120
                                                       1, 2, 1, 2, 1, 2, 1, 2,       //128

                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //10
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //20
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //30
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //40
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //50
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //60
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //70
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //80
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //90
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //100
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //110
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //120
                                                       1, 2, 1, 2, 1, 2, 1, 2,       //128

                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //10
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //20
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //30
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //40
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //50
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //60
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //70
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //80
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //90
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //100
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //110
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1 };  //119
            byte[] compressedExpected = new byte[] {3,255, 127,
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //10
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //20
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //30
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //40
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //50
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //60
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //70
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //80
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //90
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //100
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //110
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //120
                                                       1, 2, 1, 2, 1, 2, 1, 2,       //128
                                                       124,
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //10
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //20
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //30
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //40
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //50
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //60
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //70
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //80
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //90
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //100
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //110
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //120
                                                       1, 2, 1, 2, 1,                //125
                                                       0,
                                                       255, 127,
                                                       2, 1, 2, 1, 2, 1, 2, 1, 2,    //9
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //19
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //29
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //39
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //49
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //59
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //69
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //79
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //89
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //99
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //109
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //119
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1,    //128
                                                       124,
                                                       2, 1, 2, 1, 2, 1, 2, 1, 2,    //9
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //19
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //29
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //39
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //49
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //59
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //69
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //79
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //89
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //99
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //109
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //119
                                                       1, 2, 1, 2, 1, 2,    //125
                                                       0,
                                                       255, 127,
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //10
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //20
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //30
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //40
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //50
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //60
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //70
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //80
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //90
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //100
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //110
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //120
                                                       1, 2, 1, 2, 1, 2, 1, 2,       //128
                                                       124,
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //10
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //20
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //30
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //40
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //50
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //60
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //70
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //80
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //90
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //100
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //110
                                                       1, 2, 1, 2, 1, 2, 1, 2, 1, 2, //120
                                                       1, 2, 1, 2, 1,                //125
                                                       0};
            Check(uncompressedExpected, compressedExpected, AlgorithmRGB);
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
            byte[] compressedExpected = new byte[] {1,6, 255, 1, 255, 1,141,1,0 };
            Check(uncompressedExpected, compressedExpected, Algorithm);
        }
    }
}
