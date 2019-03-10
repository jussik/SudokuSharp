using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

            List<(string key, Cell[] cells)> puzzles = Puzzles
                .Select(p => (key: p.Key, cells: Puzzle.ParsePuzzle(p.Value)))
                .ToList();

            Cell[] cellsToSolve = new Cell[81];
            foreach ((string key, Cell[] start) in puzzles)
            {
                Array.Copy(start, cellsToSolve, 81);
                Solver.Result result = Solver.Solve(cellsToSolve);
                Console.WriteLine(Puzzle.FormatResults(key, result, start, cellsToSolve));
            }

            const int iterations = 1000;
            Console.WriteLine($"Calculating durations (average of {iterations} iterations)...");
            // run some iterations for jitting using easier puzzles
            for (int i = 0; i < 500; i++)
            {
                Array.Copy(puzzles[2].cells, cellsToSolve, 81);
                Solver.Solve(cellsToSolve);
                Array.Copy(puzzles[1].cells, cellsToSolve, 81);
                Solver.Solve(cellsToSolve);
                Array.Copy(puzzles[0].cells, cellsToSolve, 81);
                Solver.Solve(cellsToSolve);
            }

            // timed runs
            foreach ((string key, string value) in Puzzles)
            {
                Cell[] start = Puzzle.ParsePuzzle(value);
                Stopwatch sw = Stopwatch.StartNew();
                for (int i = 0; i < iterations; i++)
                {
                    Array.Copy(start, cellsToSolve, 81);
                    Solver.Solve(cellsToSolve);
                }
                sw.Stop();
                Console.WriteLine($"{key}: {((double)sw.ElapsedTicks / Stopwatch.Frequency) * 1000.0 / iterations} ms");
            }
        }
    }
}
