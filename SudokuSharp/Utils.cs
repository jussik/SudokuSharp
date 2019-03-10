using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace SudokuSharp
{
    public static class Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountOnes(int value)
        {
            unchecked
            {
                if (Popcnt.IsSupported)
                    return (int)Popcnt.PopCount((uint)value);

                int p = value - ((value >> 1) & 0x55555555);
                p = (p & 0x33333333) + ((p >> 2) & 0x33333333);
                return ((p + (p >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
            }
        }
    }
}