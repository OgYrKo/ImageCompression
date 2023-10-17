using System;
using System.Collections.Generic;
using System.Text;

namespace ImageCompression.Common.LZW
{
    public struct CustomBits
    {
        public const ushort MaxValue = 4095;

        public ushort Value { get; private set; } // Используем ushort (16 битов) для хранения 12 битов данных

        public CustomBits(ushort value)
        {
            // Убедимся, что значение не превышает 12 битов
            if (value > 0xFFF)
            {
                throw new ArgumentOutOfRangeException("Значение должно быть в диапазоне от 0x000 до 0xFFF.");
            }

            this.Value = value;
        }

        //public override string ToString()
        //{
        //    return  Convert.ToString(Value, 2).PadLeft(12, '0');
        //}
    }
}
