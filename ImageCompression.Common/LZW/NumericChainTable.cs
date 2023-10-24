using System;
using System.Collections.Generic;
using System.Text;

namespace ImageCompression.Common.LZW
{
    public class NumericChainTable : ChainTable<CodeString,ushort>
    {
        
        public NumericChainTable(int chainCount = 4096) : base(chainCount) { }

        protected override void AddDefaultValuesToTable()
        {
            for (ushort i = 0; i <= byte.MaxValue; i++)
            {
                char ch = (char)i;
                Table.Add(new CodeString(null, ch),i);
            }
        }

        public bool TryAddNewChain(CodeString key)
        {
            if (IsOverflow()) return false;
            TryUpdatePowerOfTwo();
            ushort code = NextCode++;
            Table.Add(key,code);
            return true;
        }

        public bool TryAddNewChain(CodeString key,char c)
        {
            return TryAddNewChain(new CodeString(Table[key], c));
        }

        public bool Contains(CodeString key) => Table.ContainsKey(key);
        //public bool Contains(CodeString key, char newChar) => Table.ContainsKey(new CodeString(key.IsNullable()?null:(ushort?)Table[key],newChar));
        public bool Contains(CodeString key, char newChar) => Table.ContainsKey(new CodeString(key is null ? null:(ushort?)Table[key],newChar));
        public ushort this[CodeString key]
        {
            get => Table[key];
        }
    }
}
