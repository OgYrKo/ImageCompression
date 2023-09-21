using System;

namespace ImageCompression.Interfaces
{
    public interface IAlgorithm
    {
        byte[] Compress(byte[] data);
        byte[] Decompress(byte[] data);
    }
}
