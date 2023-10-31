using System;
using System.Collections.Generic;
using System.Text;

namespace ImageCompression.Common.LZW
{
    public class NumericChainTable : ChainTable<CodeString,ushort>
    {
        public NumericChainTable(int chainCount = 4096) : base(chainCount,2) { }

        protected override void AddDefaultValuesToTable()
        {
            checked
            {
                for (ushort i = 0; i <= byte.MaxValue; i++)
                {
                    Table.Add(new CodeString(null, i, (byte)i), i);
                }
            }
        }

        public override bool TryAddNewChain(CodeString previous, byte c)
        {
            return TryAddNewChain(new CodeString(previous, Table[previous], c));
        }

        protected override void AddToTable(CodeString newChain,ushort code)
        {
            Table.Add(newChain, code);
        }

        public bool Contains(CodeString key, byte newChar)
        {
            CodeString newCode = key == null ? new CodeString(key, newChar) : new CodeString(key, Table[key], newChar);
            return Table.ContainsKey(newCode);
        }
    }
}
