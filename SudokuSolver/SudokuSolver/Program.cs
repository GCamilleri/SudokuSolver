using System;
using System.CodeDom;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Timers;
using System.Xml.Schema;

namespace SudokuSolver
{
    class Solver
    {

        private static int[,] _puzzle = new int[9, 9];
        private static int _steps;
        private static int _puzzleCount = 0;

        private static readonly Stopwatch _globalStopwatch = new Stopwatch();

        private static string _filepath = "";

        //Show solving progress
        private static bool _vis = false;

        //Pause to show initial puzzle
        private static bool _showPuzzle = false;


        private void Execute()
        {
            Stopwatch puzzleTimer = new Stopwatch();
            puzzleTimer.Start();
            _globalStopwatch.Start();
            if (Solve())
            {
                puzzleTimer.Stop();
                _globalStopwatch.Stop();
                if (_showPuzzle)
                {
                    PrintSoln();
                    Console.WriteLine("Puzzle No: " + _puzzleCount + " Done - Press ENTER to continue...");
                    Console.ReadLine();
                }
            }
            Console.WriteLine("\n");
            writeMetrics(puzzleTimer);
        }

        private bool Solve() //Backtracking Alg
        {
            //Print for visualisation
            if (_vis)
            {
                PrintSoln();
                Console.SetCursorPosition(0, 4);
            }

            _steps++;

            Cell blank = FindNextBlank();

            int row = blank.Row;
            int col = blank.Col;

            if (row == -1) return true;

            for (var i = 1; i <= 9; i++)
            {
                if (IsValid(row, col, i))
                {
                    _puzzle[row, col] = i;

                    if (Solve())
                    {
                        return true;
                    }
                    _puzzle[row, col] = 0;
                }
            }
            return false;
        }


        private bool IsValid(int row, int col, int n)
        {
            if (!RowContains(row, n) &&
                !ColContains(col, n) &&
                !BlockContains(row - row % 3, col - col % 3, n))
            {
                return true;
            }

            return false;
        }


        private bool RowContains(int row, int num)
        {
            for (var i = 0; i < _puzzle.GetLength(0); i++)
            {
                if (_puzzle[row, i] == num) return true;
            }
            return false;
        }

        private bool ColContains(int col, int num)
        {
            for (var i = 0; i < _puzzle.GetLength(0); i++)
            {
                if (_puzzle[i, col] == num) return true;
            }
            return false;
        }

        private bool BlockContains(int blockStartRow, int boxStartCol, int num)
        {
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    if (_puzzle[i + blockStartRow, j + boxStartCol] == num) return true;
                }
            }

            return false;
        }

        private Cell FindNextBlank()
        {
            Cell blank = new Cell();

            for (var i = 0; i < 9; i++)
            {
                for (var j = 0; j < 9; j++)
                {
                    if (_puzzle[i, j] == 0)
                    {
                        blank.Row = i;
                        blank.Col = j;
                        return blank;
                    }
                }
            }

            return new Cell(-1, -1);
        }

        private void PrintSoln()
        {
            StringBuilder outputBuilder = new StringBuilder();

            outputBuilder.Append("|-------------------|\n");
            for (var i = 0; i < _puzzle.GetLength(0); i++)
            {
                outputBuilder.Append("| ");
                for (var j = 0; j < _puzzle.GetLength(1); j++)
                {
                    outputBuilder.Append(_puzzle[i, j] + " ");
                }
                outputBuilder.Append("|\n");
            }
            outputBuilder.Append("|-------------------|\n\n" + _steps + " steps taken.");
            outputBuilder.Append("                     \n\n\n");

            Console.Write(outputBuilder);

        }

        private void writeMetrics(Stopwatch puzzleStopwatch)
        {
            StringBuilder writeBuilder = new StringBuilder();

            writeBuilder.Append(_puzzleCount + ": " + _steps + " steps  --  " + puzzleStopwatch.ElapsedMilliseconds / 1000.0 + "s");

            File.AppendAllText(@_filepath + " Compute Metrics.metrics", writeBuilder + Environment.NewLine);
        }

        private int[,] parsePuzzle(String line)
        {
            int[,] grid = new int[9, 9];
            int row = 0;
            int col = 0;

            for (int i = 0; i < line.Length; i++)
            {

                if (line[i] == '.') grid[row, col] = 0;
                else grid[row, col] = line[i] - '0';

                col++;

                if (col == 9)
                {
                    col = 0;
                    row++;
                }
            }

            return grid;
        }


        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Please enter the path to the .batch puzzle batch file: ");
                _filepath = Console.ReadLine();

                if (@_filepath != null)
                {
                    using (StreamReader sr = new StreamReader(@_filepath))
                    {

                        Console.WriteLine("Working on puzzle set: " + Path.GetFileName(_filepath) + "\n\n\n");
                        File.Delete(_filepath + " Compute Metrics.metrics");

                        while (!sr.EndOfStream)
                        {
                            String line = sr.ReadLine();

                            Solver solver = new Solver();

                            int[,] grid = solver.parsePuzzle(line);

                            //BLANK GRID
                            //int[,] grid = new int[9,9];

                            _puzzleCount++;
                            _steps = 0;
                            _puzzle = grid;

                            if (_showPuzzle)
                            {
                                Console.SetCursorPosition(0, 4);
                                solver.PrintSoln();

                                Console.WriteLine("Puzzle No: " + _puzzleCount + " - Press ENTER to continue...           ");
                                Console.ReadLine();
                            }

                            Console.SetCursorPosition(0, Console.CursorTop - 2);
                            Console.WriteLine("Puzzle No: " + _puzzleCount + " - Working...                 ");

                            Console.SetCursorPosition(0, 4);
                            solver.Execute();
                        }

                        Console.SetCursorPosition(0, Console.CursorTop - 2);
                        Console.WriteLine("BATCH COMPLETED.                    \nCompute metrics saved at: \"" + _filepath + "Compute Metrics.metrics\" \n\nPress ENTER to exit...");
                        Console.ReadLine();

                        File.AppendAllText(_filepath + " Compute Metrics.metrics",
                            "Total Elapsed Time: " + _globalStopwatch.ElapsedMilliseconds / 1000.0 + "s.");
                    }
                }
                else throw new NullReferenceException();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: Problem with input. -- " + e.Message);
                Console.ReadLine();
            }


        }
    }

    struct Cell
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }
}
