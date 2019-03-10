using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SudokuSharp.Tests")]

namespace SudokuSharp
{
    internal class Program
    {
        private static readonly Dictionary<string, string> Puzzles = new Dictionary<string, string>
        {
            ["Easy"] = @"
                003|020|600
                900|305|001
                001|806|400
                008|102|900
                700|000|008
                006|708|200
                002|609|500
                800|203|009
                005|010|300",
            ["Medium"] = @"
                043|080|250
                600|000|000
                000|001|094
                900|004|070
                000|608|000
                010|200|003
                820|500|000
                000|000|005
                034|090|710",
            ["Hard"] = @"
                000|700|000
                100|000|000
                000|430|200
                000|000|006
                000|509|000
                000|000|418
                000|081|000
                002|000|050
                040|000|300",
            ["Hardest"] = @"
                800|000|000
                003|600|000
                070|090|200
                050|007|000
                000|045|700
                000|100|030
                001|000|068
                008|500|010
                090|000|400"
        };

        public static void Main(string[] args)
        {
            // attempt to resize window to fit all puzzles
            try { Console.SetWindowSize(Console.WindowWidth, Math.Min(Puzzles.Count * 15 + 5, Console.LargestWindowHeight)); }
            catch { /* can't resize window */ }

            foreach ((string key, string value) in Puzzles)
            {
                Cell[] start = Puzzle.ParsePuzzle(value);
                Cell[] cells = new Cell[81];
                Array.Copy(start, cells, 81);

                Solver.Result result = Solver.Solve(cells);

                Console.WriteLine(Puzzle.FormatResults(key, result, start, cells));
            }
        }
    }
}
