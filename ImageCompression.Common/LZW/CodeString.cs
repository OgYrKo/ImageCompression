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
            //int key = 0;

            //if (Code != null)
            //{
            //    // Если Code не является null, учитываем его в хэше.
            //    key = (Code.Value << 8);
            //}

            //if (Str != null)
            //{
            //    // Если Str не является null, учитываем его в хэше.
            //    key ^= Str.Value;
            //}


            //// Применяем заданную хэш-функцию.
            //return ((key >> 12) ^ key) & 8191;

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
