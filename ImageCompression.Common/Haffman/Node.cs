using System;
using System.Collections.Generic;
using System.Text;

namespace ImageCompression.Common.Haffman
{
    public class Node
    {
        /// <summary>
        /// if array has 2 elements - it's a node, else it's a leaf (byte)
        /// </summary>
        public object[] Value { get; set; }
        public long Weight { get; set; }
    }
}
