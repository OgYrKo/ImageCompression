using System;
using System.Collections.Generic;

namespace ImageCompression.Common.LZW
{
    public class ChainTable<Key,Value>
    {
        Dictionary<Key, Value> Table;
        public readonly int MaxChainCount;
        private int CurrentChainLimit { get; set; }
        public byte CurrentChainLimitPower { get; private set; }

        ushort NextCode;

        public ChainTable() : this(4096) { }

        public ChainTable(int chainCount)
        {
            Table = new Dictionary<Key, Value>();
            MaxChainCount = chainCount;
        }

        public void SetTableByDefault()
        {
            Table.Clear();
            for (ushort i = 0; i <= byte.MaxValue; i++)
            {
                char ch= (char)i;
                Add(ch.ToString(), i);
            }
            NextCode = 258;

            CurrentChainLimitPower = 9;
            CurrentChainLimit = 512;
        }

        private void Add(string ch, ushort num)
        {
            if (typeof(Key) == typeof(string))
            {
                Table.Add((Key)(object)ch, (Value)(object)num); ;
            }
            else if (typeof(Value) == typeof(string))
            {
                Table.Add((Key)(object)num, (Value)(object)ch.ToString());
            }
            else throw new TypeLoadException("Один из параметризированых типов должен быть типом string");
        }

        public bool IsOverflow() => Table.Count == MaxChainCount;
        public bool TryAddNewChain(string chain)
        {
            if (IsOverflow()) return false;
            CheckPowerOfTwo();
            ushort value = NextCode++;
            Add(chain, value);
            return true;
        }

        private void CheckPowerOfTwo()
        {
            if (CurrentChainLimit == Table.Count)
            {
                CurrentChainLimit *= 2;
                CurrentChainLimitPower++;
            }
        }

        public bool Contains(Key value) => Table.ContainsKey(value);
        public Value this[Key key]
        {
            get => Table[key];
            set => Table[key] = value;
        }
    }
}
