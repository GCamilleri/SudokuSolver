using System;
using System.Text;

namespace SudokuSolver
{
    class Solver
    {

        private static int[,] _puzzle = new int[9,9];
        private static int _steps;

        //Show solving progress
        private static bool _vis = false;

        private void Execute()
        {
            if (Solve()) PrintSoln();
            Console.Read();
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

            Console.Write(outputBuilder);

        }

        private static void Main(string[] args)
        {
            //HARD FOR COMPUTERS
            int[,] numbers = new int[,]{
                {4,0,0,0,0,0,8,0,5},
                {0,3,0,0,0,0,0,0,0},
                {0,0,0,7,0,0,0,0,0},
                {0,2,0,0,0,0,0,6,0},
                {0,0,0,0,8,0,4,0,0},
                {0,0,0,0,1,0,0,0,0},
                {0,0,0,6,0,3,0,7,0},
                {5,0,0,2,0,0,0,0,0},
                {1,0,4,0,0,0,0,0,0}
            };

            //HARD FOR HUMANS
            //int[,] numbers = new int[,]{
            //    {8,0,0,0,0,0,0,0,0},
            //    {0,0,3,6,0,0,0,0,0},
            //    {0,7,0,0,9,0,2,0,0},
            //    {0,5,0,0,0,7,0,0,0},
            //    {0,0,0,0,4,5,7,0,0},
            //    {0,0,0,1,0,0,0,3,0},
            //    {0,0,1,0,0,0,0,6,8},
            //    {0,0,8,5,0,0,0,1,0},
            //    {0,9,0,0,0,0,4,0,0}
            //};

            _puzzle = numbers;

            Solver solver = new Solver();

            solver.Execute();

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
