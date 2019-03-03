using System.Runtime.CompilerServices;

namespace SudokuSharp
{
    internal static class Solver
    {
        public enum Result
        {
            Success, Incomplete, Invalid
        }

        public delegate int Indexer(int major, int minor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RowsIndex(int major, int minor) => major * 9 + minor;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ColsIndex(int major, int minor) => minor * 9 + major;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BoxRowsIndex(int major, int minor) => (major % 3) * 3
            + (major / 3) * 27
            + (minor % 3)
            + (minor / 3) * 9;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BoxColsIndex(int major, int minor) => (major % 3) * 27
            + (major / 3) * 3
            + (minor % 3) * 9
            + (minor / 3);

        public static Result Solve(Cell[] cells, int maxGuessLevels = 10)
        {
            while (true)
            {
                bool changed = false;
                changed |= SimpleSolver.Solve(cells);
                changed |= HiddenSolver.Solve(cells);
                changed |= LockedSolver.Solve(cells);

                if (!changed)
                    return Result.Incomplete;

                Result result = Verifier.Verify(cells);
                if (result != Result.Incomplete)
                    return result;
            }
        }
    }
}