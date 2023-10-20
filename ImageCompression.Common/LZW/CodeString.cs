using System;
using System.Collections.Generic;
using System.Text;

namespace ImageCompression.Common.LZW
{
    public struct CodeString
    {

        public ushort? Code { get; set; }
        public char? Str { get; set; }

        public CodeString(ushort? code, char? str)
        {
            Code = code;
            Str = str;
        }

        public static bool Equals(CodeString lhs, CodeString rhs)
        {
            if (lhs.Code != rhs.Code) return false;
            if (lhs.Str != rhs.Str) return false;
            return true;
        }

        public bool IsNullable() => Code == null && Str == null;
    }
}
