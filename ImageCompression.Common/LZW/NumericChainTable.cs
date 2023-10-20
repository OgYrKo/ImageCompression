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
        public bool Contains(CodeString key, char newChar) => Table.ContainsKey(new CodeString(key.IsNullable()?null:(ushort?)Table[key],newChar));
        public ushort this[CodeString key]
        {
            get => Table[key];
        }
    }

    //public class NumericChainTable<Key, Value>
    //{
    //    Dictionary<Key, Value> Table;
    //    public readonly int MaxChainCount;
    //    private int CurrentChainLimit { get; set; }
    //    public byte CurrentChainLimitPower { get; private set; }

    //    ushort NextCode;

    //    public NumericChainTable() : this(4096) { }

    //    public NumericChainTable(int chainCount)
    //    {
    //        Table = new Dictionary<Key, Value>();
    //        MaxChainCount = chainCount;
    //        SetLimitsByDefault();
    //    }

    //    public void SetTableByDefault()
    //    {
    //        Table.Clear();
    //        for (ushort i = 0; i <= byte.MaxValue; i++)
    //        {
    //            char ch = (char)i;
    //            Add(ch.ToString(), i);
    //        }
    //        NextCode = 258;

    //        SetLimitsByDefault();
    //    }

    //    private void SetLimitsByDefault()
    //    {
    //        CurrentChainLimitPower = 9;
    //        CurrentChainLimit = 512;
    //    }

    //    private void Add(string ch, ushort num)
    //    {
    //        if (typeof(Key) == typeof(string))
    //        {
    //            Table.Add((Key)(object)ch, (Value)(object)num); ;
    //        }
    //        else if (typeof(Value) == typeof(string))
    //        {
    //            Table.Add((Key)(object)num, (Value)(object)ch.ToString());
    //        }
    //        else throw new TypeLoadException("Один из параметризированых типов должен быть типом string");
    //    }

    //    public bool IsOverflow() => Table.Count == MaxChainCount;
    //    public bool TryAddNewChain(ushort previousCode, char newChar)
    //    {
    //        if (IsOverflow()) return false;
    //        CheckPowerOfTwo();
    //        ushort value = NextCode++;
    //        Add(GetCodeString(previousCode, newChar), value);
    //        return true;
    //    }

    //    private string GetCodeString(ushort code, char newChar)
    //    {

    //    }

    //    private void CheckPowerOfTwo()
    //    {
    //        if (CurrentChainLimit == Table.Count)
    //        {
    //            CurrentChainLimit *= 2;
    //            CurrentChainLimitPower++;
    //        }
    //    }

    //    public bool Contains(Key value) => Table.ContainsKey(value);
    //    public Value this[Key key]
    //    {
    //        get => Table[key];
    //        set => Table[key] = value;
    //    }
    //}
}
