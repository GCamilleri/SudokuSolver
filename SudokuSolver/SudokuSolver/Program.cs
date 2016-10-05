using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Timers;
using System.Xml.Schema;

namespace SudokuSolver
{
    class Solver
    {

        private static int[,] _puzzle = new int[9,9];
        private static int _steps;
        private static int _puzzleCount = 0;

        private static Stopwatch _globalStopwatch = new Stopwatch();

        private static String _filename = "top10.txt";

        //Show solving progress
        private static bool _vis = false;

        //Pause to show initial puzzle
        private static bool _show = false;

        private void Execute()
        {   
            Stopwatch puzzleTimer = new Stopwatch();
            puzzleTimer.Start();
            _globalStopwatch.Start();
            if (Solve())
            {
                puzzleTimer.Stop();
                _globalStopwatch.Stop();
                PrintSoln();
            }
            if (_show) Console.ReadLine();
            writeMetrics(puzzleTimer);
        }

        private bool Solve() //Backtracking Alg
        {
            //Print for visualisation
            if (_vis)
            {
                PrintSoln();
                Console.SetCursorPosition(0, 0);
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
                    if (_puzzle[i,j] == 0)
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
            outputBuilder.Append("|-------------------|\n\n" + _steps);
            outputBuilder.Append("                     \n\n\n");

            Console.Write(outputBuilder);

        }

        private void writeMetrics(Stopwatch puzzleStopwatch)
        {
            StringBuilder writeBuilder = new StringBuilder();

            writeBuilder.Append(_puzzleCount + ": " + _steps + " steps  --  " + puzzleStopwatch.ElapsedMilliseconds/1000.0 + "s");

            File.AppendAllText(@"D:/Dev_Projects/SudokuSolver/SudokuSolver/"+_filename+" Compute Metrics.txt", writeBuilder + Environment.NewLine);
        }

        private int[,] parsePuzzle(String line)
        {
            int [,] grid = new int[9,9];
            int row = 0;
            int col = 0;

            for (int i = 0; i < line.Length; i++)
            {

                if (line[i] == '.') grid[row, col] = 0;
                else grid[row, col] = line[i] - '0';

                col ++;

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
                using (StreamReader sr = new StreamReader(@"D:/Dev_Projects/SudokuSolver/SudokuSolver/" + _filename))
                {
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

                        if (_show)
                        {
                            Console.SetCursorPosition(0, 0);
                            solver.PrintSoln();
                            Console.ReadLine();
                        }

                        Console.SetCursorPosition(0, 0);
                        solver.Execute();
                    }

                    File.AppendAllText(@"D:/Dev_Projects/SudokuSolver/SudokuSolver/" + _filename + " Compute Metrics.txt", "Total Elapsed Time: " + _globalStopwatch.ElapsedMilliseconds/1000.0 + "s.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: Problem with input. -- " + e.Message);
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
