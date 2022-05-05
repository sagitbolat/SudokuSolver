using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver {
    class SudokuGenerator {

        //Generates a grid of integers;
        public int[,] Generate() {
            //a 9x9 array of integers that represents our sudoku grid
            int[,] puzzleGrid = new int[9, 9];


            //Generate a complete solution using backtracking that fills up the grid
            Random random = new Random();

            GenerateCompleteSolution(puzzleGrid, random);
            //TODO: Remove numbers from the grid, while making sure the solution is still unique.

            //DEBUG: print grid to console
            for (int i = 0; i < puzzleGrid.GetLength(0); i++) {
                for (int j = 0; j < puzzleGrid.GetLength(1); j++) {
                    Console.Write(puzzleGrid[i, j] + "\t");
                }
                Console.WriteLine();
            }

            return puzzleGrid;
        }


        //Generates a sudoku grid where all the numbers are filled out. 
        //This is also the final solution that the user will arrive at.
        private bool GenerateCompleteSolution(int[,] grid, Random random) {
            //possible number values in a sudoku puzzle
            int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int x = 0;
            int y = 0;

            for (int i = 0; i < 81; i++) {
                y = i / 9;
                x = i % 9;

                if (grid[x, y] == 0) {
                    //shuffle numbers array
                    numbers = numbers.OrderBy(a => random.Next()).ToArray();
                    foreach (int num in numbers) {
                        if (isGridValid(grid, x, y, num)) {
                            grid[x, y] = num;
                            if (!FindEmptySquare(grid)) {
                                return true;
                            } else if (GenerateCompleteSolution(grid, random)) {
                                return true;
                            }
                        }
                    }
                    
                    break;
                }
            }
            grid[x, y] = 0;
            return false;
        }

        //checks if num can go into the x and y coordinates of the grid by following sudoku rules
        private bool isGridValid(int[,] grid, int x, int y, int num) {
            //if num is not between 1- 9 => return false
            //if (num < 1 || num > 9) return false;

            //check row
            bool rowValid = isRowValid();
            bool colValid = isColValid();
            bool boxValid = isBoxValid();

            if (rowValid && colValid && boxValid) {
                return true;
            }
            return false;

            //Helper methods
            bool isRowValid() {
                for (int i = 0; i < 9; i++) {
                    if (x == i) continue;
                    else if (grid[i, y] == num) return false;
                }
                return true;
            }
            bool isColValid() {
                for (int i = 0; i < 9; i++) {
                    if (y == i) continue;
                    else if (grid[x, i] == num) return false;
                }
                return true;
            }
            bool isBoxValid() {
                int[] box1Range = { 0, 3 };
                int[] box2Range = { 3, 6 };
                int[] box3Range = { 6, 9 };
                int[][] boxRanges = { box1Range, box2Range, box3Range };

                int verticalBox = y / 3;
                int horizontalBox = x / 3;

                for (int boxX = boxRanges[horizontalBox][0]; boxX < boxRanges[horizontalBox][1]; boxX++) {
                    for (int boxY = boxRanges[verticalBox][0]; boxY < boxRanges[verticalBox][1]; boxY++) {
                        if (x == boxX && y == boxY) continue;
                        if (grid[boxX, boxY] == num) {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        //returns true if there is an empty square in the grid
        private bool FindEmptySquare(int[,] grid) {
            for (int i = 0; i < 81; i++) {
                int x = i % 9;
                int y = i / 9;
                if (grid[x, y] == 0) return true;
            }
            return false;
        }
    }
}
