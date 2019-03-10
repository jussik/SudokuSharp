using System;

namespace SudokuSharp
{
    internal static class NakedSolver
    {
        private struct PossPair
        {
            public readonly int Index;
            public readonly int Possible;
            public PossPair(int index, int possible)
            {
                Index = index;
                Possible = possible;
            }

            public void Deconstruct(out int index, out int possible)
            {
                index = Index;
                possible = Possible;
            }
        }
        public static bool Solve(Span<Cell> cells, Solver.Indexer indexer)
        {
            Span<PossPair> poss = stackalloc PossPair[9];
            bool changed = false;
            for (int major = 0; major < 9; major++)
            {
                int count = 0;
                for (int minor = 0; minor < 9; minor++)
                {
                    int i = indexer(major, minor);
                    Cell cell = cells[i];
                    if (cell.Value == Cell.Unknown) {
                        poss[count] = new PossPair(i, cell.Possible);
                        count += 1;
                    }
                }
                if (count < 3) {
                    break;
                }

                for (int a = 0; a < count -1; a++)
                {
                    for (int b = a + 1; b < count; b++)
                    {
                        int aPoss = poss[a].Possible;
                        // pair with the same 2-value possibles
                        if (aPoss == poss[b].Possible && Utils.CountOnes(aPoss) == 2) {
                            for (int o = 0; o < count; o++)
                            {
                                (int i, int oPoss) = poss[o];
                                if (oPoss != 0
                                    && o != a && o != b
                                    && (oPoss & aPoss) != 0) {
                                    ref Cell cell = ref cells[i];
                                    cell.Possible &= ~aPoss;
                                    cell.CheckPossible();
                                    changed = true;
                                }
                            }
                            // already found the pair, check next outer cell
                            break;
                        }
                    }
                }
            }
            return changed;
        }
    }
}