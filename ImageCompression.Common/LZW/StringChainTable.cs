using System;
using System.Collections.Generic;
using System.Text;

namespace ImageCompression.Common.LZW
{
    public class StringChainTable: ChainTable<ushort, CodeString>
    {
        public StringChainTable(int chainCount = 4096) : base(chainCount) { }

        protected override void AddDefaultValuesToTable()
        {
            for (ushort i = 0; i <= byte.MaxValue; i++)
            {
                char ch = (char)i;
                Table.Add(i, new CodeString(null,ch)); 
            }
        }

        public bool TryAddNewChain(CodeString value)
        {
            if (IsOverflow()) return false;
            TryUpdatePowerOfTwo();
            ushort code = NextCode++;
            Table.Add(code, value);
            return true;
        }

        public bool Contains(ushort value) => Table.ContainsKey(value);
        public string this[ushort key]
        {
            get => Rec(key);
        }

        private string Rec(ushort code)
        {
            return Rec(Table[code]);
        }

        private string Rec(CodeString codeString)
        {
            if (codeString.Code == null)
                return $"{codeString.Str}";
            return Rec(Table[(ushort)codeString.Code]) + codeString.Str;
        }
    }
}
