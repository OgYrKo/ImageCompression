using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCompression.Common.LZW
{
    public class StringChainTable : ChainTable<ushort, CodeString[]>
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

        //TODO need optimisation (try change branch)
        public string GetString(ushort key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Rec(Table[key], stringBuilder);
            return stringBuilder.ToString();
        }

        public byte[] GetBytes(ushort key)
        {
            //List<byte> resultBytes = new List<byte>();
            //Rec(Table[key], resultBytes);
            //return resultBytes.ToArray();
            return Rec(key).ToArray();
        }

        public byte GetFirstByte(ushort key)
        {
            CodeString[] codeStrings = Table[key];
            if (codeStrings[0].Code == null) return (byte)codeStrings[0].Str;
            else return GetFirstByte((ushort)codeStrings[0].Code);
        }

        private List<byte> Rec(ushort key)
        {
            CodeString[] codeStrings = Table[key];
            List<byte>[] results = new List<byte>[codeStrings.Length];
            Parallel.For(0, codeStrings.Length, i =>
            {
                results[i] = new List<byte>();
                if (codeStrings[i].Code != null) results[i] = Rec((ushort)codeStrings[i].Code);
                results[i].Add((byte)codeStrings[i].Str);
            });
            
            return results.SelectMany(array => array).ToList();
        }

        //private byte[] Rec(ushort key)
        //{
        //    List<byte> resultBytes=new List<byte>();
        //    Stack<CodeString[]> stack = new Stack<CodeString[]>();
        //    stack.Push(Table[key]);

        //    while (stack.Count > 0)
        //    {
        //        CodeString[] currentCodeStrings = stack.Pop();

        //        foreach (CodeString codeString in currentCodeStrings)
        //        {
        //            if (codeString.Code == null)
        //            {
        //                resultBytes.Add((byte)codeString.Str);
        //            }
        //            else
        //            {
        //                stack.Push(Table[(ushort)codeString.Code]);
        //                resultBytes.Add((byte)codeString.Str);
        //            }
        //        }
        //    }
        //    return resultBytes.ToArray();
        //}

        private void Rec(CodeString[] codeStrings, List<byte> resultBytes)
        {
            foreach (CodeString codeString in codeStrings)
            {

                if (codeString.Code == null)
                {
                    resultBytes.Add((byte)codeString.Str);
                }
                else
                {
                    Rec(Table[(ushort)codeString.Code], resultBytes);
                    resultBytes.Add((byte)codeString.Str);
                }
            }
        }

        private void Rec(CodeString[] codeStrings, StringBuilder stringBuilder)
        {
            foreach (CodeString codeString in codeStrings)
            {
                if (codeString.Code == null)
                {
                    stringBuilder.Append(codeString.Str);
                }
                else
                {
                    Rec(Table[(ushort)codeString.Code], stringBuilder);
                    stringBuilder.Append(codeString.Str);
                }
            }
        }
    }
}
