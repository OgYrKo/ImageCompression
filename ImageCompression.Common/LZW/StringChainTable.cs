using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageCompression.Common.LZW
{
    public class StringChainTable: ChainTable<ushort, CodeString[]>
    {
        public StringChainTable(int chainCount = 4096) : base(chainCount) { }

        protected override void AddDefaultValuesToTable()
        {
            for (ushort i = 0; i <= byte.MaxValue; i++)
            {
                char ch = (char)i;
                Table.Add(i, new CodeString[] { new CodeString(null, ch) }); 
            }
        }

        public bool TryAddNewChain(CodeString value)
        {
            if (IsOverflow()) return false;
            TryUpdatePowerOfTwo();
            ushort code = NextCode++;
            Table.Add(code, new CodeString[] { value });
            return true;
        }

        public bool TryAddNewChain(CodeString[] previous, CodeString[] current)
        {
            if (IsOverflow()) return false;
            TryUpdatePowerOfTwo();
            ushort code = NextCode++;
            Table.Add(code, previous.Concat(current).ToArray());
            return true;
        }

        public bool Contains(ushort value) => Table.ContainsKey(value);
        public CodeString[] this[ushort key]
        {
            get => Table[key];//Rec(key);
        }

       public string GetString(ushort key)
        {
            return Rec(Table[key]);
        }

        private string Rec(CodeString[] codeStrings)
        {
            string resultStr = "";
            foreach (CodeString codeString in codeStrings)
            {
                if (codeString.Code == null)
                    resultStr+= $"{codeString.Str}";
                else
                    resultStr+= Rec(Table[(ushort)codeString.Code]) + codeString.Str;
            }
            return resultStr;
        }
    }
}
