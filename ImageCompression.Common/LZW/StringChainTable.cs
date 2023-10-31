using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCompression.Common.LZW
{
    public class StringChainTable : ChainTable<ushort, CodeString>
    {
        public StringChainTable(int chainCount = 4096) : base(chainCount,3) { }

        protected override void AddDefaultValuesToTable()
        {
            checked
            {
                for (ushort i = 0; i <= byte.MaxValue; i++)
                {
                    Table.Add(i, new CodeString(null, i, (byte)i));
                }
            }
        }
        public override bool TryAddNewChain(CodeString previous, byte c)
        {
            return TryAddNewChain(new CodeString(previous, c));
        }
        protected override void AddToTable(CodeString newChain, ushort code)
        {
            Table.Add(code, newChain);
        }

        public byte[] GetBytes(ushort code)
        {
            return Table[code].ToBytes();
        }

        public byte GetFirstByte(ushort code)
        {
            return Table[code].GetFirstByte();
        }
    }
}
