using System;
using System.Collections.Generic;
using System.Text;

namespace ImageCompression.Common.LZW
{
    public class CodeString
    {

        public ushort? Code { get; set; }
        public char? Str { get; set; }

        public CodeString(ushort? code, char? str)
        {
            Code = code;
            Str = str;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CodeString)) return false;
            CodeString other = (CodeString)obj;
            return Code == other.Code && Str == other.Str;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Code.GetHashCode();
                hash = hash * 23 + Str.GetHashCode();
                return hash;
            }
        }

        public bool IsNullable() => Code == null && Str == null;
    }
}
