using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver {
    class SudokuGenerator {

        //Generates a grid of integers;
        public int[,] Generate() {
            int[,] puzzleGrid = new int[9, 9];

            //Generate a complete solution using backtracking that fills up the grid
            puzzleGrid = GenerateCompleteSolution();
            //Remove numbers from the grid, while making sure the solution is still unique.

            return puzzleGrid;
        }

        private int[,] GenerateCompleteSolution() {
            int[,] grid = new int[9, 9];

            for (int i = 0; i < 81; i++) {
                int row = i / 9;
                int col = i % 9;
                if (grid[row,col] == 0) {

                }
            }

            return grid;
        }
    }
}
