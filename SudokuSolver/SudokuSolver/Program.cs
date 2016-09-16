using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Solver
    {

        private static int[,] _puzzle = new int[9,9];
        private static int _steps = 0;

        private void Execute()
        {
            if (Solve()) printSoln();
        }

        private bool Solve() //Backtracking Alg
        {
            _steps++;
            Cell blank = findNextBlank();

            int row = blank.row;
            int col = blank.col;

            if (row == -1) return true;

            for (int i = 1; i <= 9; i++)
            {
                if (isValid(row, col, i))
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


        private bool isValid(int row, int col, int n)
        {
            if (!rowContains(row, n) &&
                !colContains(col, n) &&
                !blockContains(row - row % 3, col - col % 3, n))
            {
                return true;
            }

            return false;
        }


        private bool rowContains(int row, int num)
        {
            for (int i = 0; i < _puzzle.GetLength(0); i++)
            {
                if (_puzzle[row, i] == num) return true;
            }
            return false;
        }

        private bool colContains(int col, int num)
        {
            for (int i = 0; i < _puzzle.GetLength(0); i++)
            {
                if (_puzzle[i, col] == num) return true;
            }
            return false;
        }

        private bool blockContains(int blockStartRow, int boxStartCol, int num)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (_puzzle[i + blockStartRow, j + boxStartCol] == num) return true;
                }
            }

            return false;
        }

        private Cell findNextBlank()
        {
            Cell blank = new Cell();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (_puzzle[i,j] == 0)
                    {
                        blank.row = i;
                        blank.col = j;
                        return blank;
                    }
                }
            }

            return new Cell(-1, -1);
        }

        private void printSoln()
        {
            StringBuilder outputBuilder = new StringBuilder();

            outputBuilder.Append("|-------------------|\n");
            for (int i = 0; i < _puzzle.GetLength(0); i++)
            {
                outputBuilder.Append("| ");
                for (int j = 0; j < _puzzle.GetLength(1); j++)
                {
                    outputBuilder.Append(_puzzle[i, j] + " ");
                }
                outputBuilder.Append("|\n");
            }
            outputBuilder.Append("|-------------------|\n\n" + _steps);

            Console.Write(outputBuilder);
            Console.Read();
        }

        static void Main(string[] args)
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
        public int row { get; set; }
        public int col { get; set; }

        public Cell(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

    }
}
