using System;
using System.Collections.Generic;

namespace ImageCompression.Common.LZW
{
    
    abstract public class ChainTable<Key,Value>: IChainTable
    {
        public readonly int MaxChainCount;//максимальное число цепочек в таблице
        public byte CurrentChainLimitPower { get; private set; }//необходимое количество бит для записи кода

        protected Dictionary<Key, Value> Table;
        protected int CurrentChainLimit { get; set; }//текущее предельное количество цепочек в таблице (связано с CurrentChainLimitPower)

        protected ushort NextCode;//следующий выделяемый код
        protected readonly int LimitDifference;

        protected ChainTable(int chainCount, int limitDiff)
        {
            Table = new Dictionary<Key, Value>();
            MaxChainCount = chainCount;
            LimitDifference = limitDiff;
            SetLimitsByDefault();
        }

        /// <summary>
        /// Сброс таблицs
        /// </summary>
        public void Reset()
        {
            Table.Clear();
            AddDefaultValuesToTable();
            NextCode = 258;
            SetLimitsByDefault();
        }

        /// <summary>
        /// Заполнение таблицы значениями по умолчанию
        /// </summary>
        protected abstract void AddDefaultValuesToTable();
        private void SetLimitsByDefault()
        {
            CurrentChainLimitPower = 9;// 12;// 9;
            CurrentChainLimit = 512;// 4096;// 512;
        }
        public virtual bool IsOverflow() => Table.Count == MaxChainCount- LimitDifference;
        public bool Contains(Key key) => Table.ContainsKey(key);
        public Value this[Key key]
        {
            get => Table[key];
        }

        protected bool TryAddNewChain(CodeString newChain)
        {
            if (IsOverflow()) 
                return false;
            TryUpdatePowerOfTwo();
            AddToTable(newChain, NextCode++);
            return true;
        }

        public abstract bool TryAddNewChain(CodeString previous, byte c);
        protected abstract void AddToTable(CodeString newChain, ushort code);

        /// <summary>
        /// Обновление лимитов
        /// </summary>
        protected virtual void TryUpdatePowerOfTwo()
        {
            if (CurrentChainLimit-LimitDifference == Table.Count)
            {
                CurrentChainLimit *= 2;
                CurrentChainLimitPower++;
            }
        }
    }
}
