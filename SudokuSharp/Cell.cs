using System;
using System.Runtime.CompilerServices;

namespace SudokuSharp
{
    public struct Cell
    {
        public const int Unknown = -1;
        public int Value;
        public int Possible;

        public Cell(int value)
        {
            if (value != Unknown && (value < 0 || value > 8))
                throw new ArgumentOutOfRangeException(nameof(value));
            Value = value;
            Possible = value != Unknown
                ? 1 << value
                : (1 << 9) - 1;
        }

        public bool Set(int value)
        {
            if (Value != Unknown)
                return false;
            Value = value;
            return true;
        }

        public bool TryRemovePossible(int value)
        {
            int mask = 1 << value;
            if ((Possible & mask) == 0u || Value != Unknown)
                return false; // already known

            Possible &= ~mask;
            CheckPossible();
            return true;
        }

        public bool CheckPossible()
        {
            // count ones (number of possibilities left)
            int ones = Utils.CountOnes(Possible);

            if (ones != 1 || Value != Unknown)
                return false;

            Value = GetOnlyPossibleValue();
            return true;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPossible(int value)
        {
            return (Possible & (1 << value)) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetOnlyPossibleValue()
        {
            switch (Possible)
            {
                case 0x001: return 0;
                case 0x002: return 1;
                case 0x004: return 2;
                case 0x008: return 3;
                case 0x010: return 4;
                case 0x020: return 5;
                case 0x040: return 6;
                case 0x080: return 7;
                case 0x100: return 8;
                default: return Unknown;
            }
        }
    }
}