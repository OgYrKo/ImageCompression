using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ImageCompression.Common
{
    public class BitArrayEqualityComparer : IEqualityComparer<BitArray>
    {
        public bool Equals(BitArray x, BitArray y)
        {
            if (x == null || y == null)
            {
                return x == y;
            }
            if (x.Length != y.Length)
            {
                return false;
            }
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(BitArray obj)
        {
            int hash = 17;
            for (int i = 0; i < obj.Length; i++)
            {
                hash = hash * 31 + (obj[i] ? 1 : 0);
            }
            return hash;
        }
    }

}
