using System;
using System.Collections.Generic;

namespace ImageCompression.Common.LZW
{
    
    abstract public class ChainTable<Key,Value>
    {
        public readonly int MaxChainCount;//максимальное число цепочек в таблице
        public byte CurrentChainLimitPower { get; private set; }//необходимое количество бит для записи кода

        protected Dictionary<Key, Value> Table;
        protected int CurrentChainLimit { get; set; }//текущее предельное количество цепочек в таблице (связано с CurrentChainLimitPower)

        protected ushort NextCode;//следующий выделяемый код

        protected ChainTable(int chainCount)
        {
            Table = new Dictionary<Key, Value>();
            MaxChainCount = chainCount;
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
            CurrentChainLimitPower = 12;// 9;
            CurrentChainLimit = 4096;// 512;
        }

        public bool IsOverflow() => Table.Count == MaxChainCount;

        /// <summary>
        /// Обновление лимитов
        /// </summary>
        protected void TryUpdatePowerOfTwo()
        {
            //if (CurrentChainLimit == Table.Count)
            //{
            //    CurrentChainLimit *= 2;
            //    CurrentChainLimitPower++;
            //}
        }
    }
}
