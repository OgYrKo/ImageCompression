using System.Collections.Generic;

namespace ImageCompression.Common.LZW
{
    public class ChainTable
    {
        Dictionary<string, ushort> Table;
        public readonly int MaxChainCount;
        private int CurrentChainLimit { get; set; }
        public byte CurrentChainLimitPower { get; private set; }

        ushort NextCode;

        public ChainTable() : this(4096) { }

        public ChainTable(int chainCount)
        {
            Table = new Dictionary<string, ushort>();
            MaxChainCount = chainCount;
        }

        public void SetTableByDefault()
        {
            Table.Clear();
            for (ushort i = 0; i <= byte.MaxValue; i++)
            {
                char ch= (char)i;
                Table.Add(ch.ToString(), i);
            }
            NextCode = 258;

            CurrentChainLimitPower = 9;
            CurrentChainLimit = 512;
        }

        public bool IsOverflow() => Table.Count == MaxChainCount;
        public bool TryAddNewChain(string chain)
        {
            if (IsOverflow()) return false;
            CheckPowerOfTwo();
            ushort value = NextCode++;
            Table.Add(chain, value);
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

        public bool Contains(string value) => Table.ContainsKey(value);
        public ushort this[string key]
        {
            get => Table[key];
            set => Table[key] = value;
        }
    }
}
