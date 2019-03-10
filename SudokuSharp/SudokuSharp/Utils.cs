namespace SudokuSharp
{
    public static class Utils
    {
        public static int CountOnes(int value)
        {
            unchecked
            {
                int p = value - ((value >> 1) & 0x55555555);
                p = (p & 0x33333333) + ((p >> 2) & 0x33333333);
                return ((p + (p >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
            }
        }
    }
}