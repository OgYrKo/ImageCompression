using ImageCompression.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCompression.Tests
{
    static internal class CheckArrays
    {
        static public void Check(byte[] uncompressedExpected, byte[] compressedExpected, IAlgorithm algorithm)
        {
            CheckCompression(uncompressedExpected, compressedExpected, algorithm);
            CheckDecompression(uncompressedExpected, compressedExpected, algorithm);
        }

        static public void CheckCompression(byte[] uncompressedExpected, byte[] compressedExpected, IAlgorithm algorithm)
        {
            byte[] compressedActual = algorithm.Compress(uncompressedExpected);
            CollectionAssert.AreEqual(compressedExpected, compressedActual, "Compression failed");
        }

        static public void CheckDecompression(byte[] uncompressedExpected, byte[] compressedExpected, IAlgorithm algorithm)
        {
            byte[] uncompressedActual = algorithm.Decompress(compressedExpected);
            CollectionAssert.AreEqual(uncompressedExpected, uncompressedActual, "Decompression failed");
        }
    }
}
