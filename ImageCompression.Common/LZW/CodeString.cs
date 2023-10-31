using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageCompression.Common.LZW
{
    public class CodeString
    {
        public CodeString Previous { get; set; }
        public ushort? Code { get; set; }
        public byte Str { get; set; }

        public CodeString(CodeString previous,ushort? code, byte c)
        {
            Previous = previous;
            Code = code;
            Str = c;
        }

        public CodeString(CodeString previous, byte c)
        {
            Previous = previous;
            Code = null;
            Str = c;
        }

        public override bool Equals(object obj)
        {
            if(Previous != null&&Code==null)
                throw new NullReferenceException("Код предыдущего элемента не может быть пустым");
            if (!(obj is CodeString)) 
                return false;
            CodeString other = (CodeString)obj;
            if(other.Previous != null&&other.Code==null)
                throw new NullReferenceException("Код предыдущего элемента не может быть пустым");
            
            bool previousEqual = (Previous != null && other.Previous != null && Previous.Code == other.Previous.Code&&Previous.Str==other.Previous.Str) 
                || (Previous == null && other.Previous == null);
            return previousEqual && Str == other.Str;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                if(Previous != null )  hash = hash * 23 + Previous.Code.GetHashCode();
                hash = hash * 23 + Str.GetHashCode();
                return hash;
            }
        }

        public byte[] ToBytes()
        {
            List<byte> byteList = new List<byte>();
            CodeString current = this;

            while (current != null)
            {
                byteList.Add(current.Str);
                current = current.Previous;
            }

            byte[] byteArray = byteList.ToArray();
            Array.Reverse(byteArray);

            return byteArray;
        }

        public byte GetFirstByte()
        {
            CodeString current = this;
            while (current.Previous != null)
            {
                current = current.Previous;
            }
            return current.Str;
        }
    }
}
