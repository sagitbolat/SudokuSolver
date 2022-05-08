using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;



namespace SudokuSolver {
    class SudokuGenerator {

        int iterations;
        public SudokuGenerator() {
            iterations = 0;
        }

        //Generates a grid of integers;
        public async Task<int[,]> Generate(bool guaranteeSingleSolution, Button[,] buttons, TextBlock iterationText) {
            //a 9x9 array of integers that represents our sudoku grid
            int[,] puzzleGrid = new int[9, 9];

            //Generate a complete solution using backtracking that fills up the grid
            Random random = new Random();
            await Task.Run(() => GenerateCompleteSolution(puzzleGrid, random, buttons, iterationText));

            //Remove numbers from the grid, while making sure the solution is still unique.
            await Task.Run(() => RemoveNumbersFromGrid(puzzleGrid, random, guaranteeSingleSolution, buttons, iterationText));
            

            return puzzleGrid;
        }

        private void UpdateGridAndUI(int[,] grid, int num, int x, int y, Button[,] buttons) {
            grid[x, y] = num;
            buttons[x,y].Dispatcher.Invoke(() =>
            {
                buttons[x,y].Content = num > 0 ? num.ToString() : " ";
                Color fg = Color.FromRgb(34, 40, 49);
                buttons[x, y].Foreground = new SolidColorBrush(fg);
            });
            Thread.Sleep(10);

        }
        private void UpdateIterationsGUI(TextBlock iterationText) {
            iterations++;
            iterationText.Dispatcher.Invoke(() => {
                iterationText.Text = "Iterations: " + iterations.ToString();
            });
        }

        //Generates a sudoku grid where all the numbers are filled out. 
        //This is also the final solution that the user will arrive at.
        private bool GenerateCompleteSolution(int[,] grid, Random random, Button[,] buttons, TextBlock iterationText) {
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

                            //update grid
                            UpdateGridAndUI(grid, num, x, y, buttons);
                            UpdateIterationsGUI(iterationText);


                            if (!FindEmptySquare(grid)) {
                                return true;
                            } else if (GenerateCompleteSolution(grid, random, buttons, iterationText)) {
                                return true;
                            }
                        }
                    }
                    
                    break;
                }
            }
            UpdateGridAndUI(grid, 0, x, y, buttons);
            UpdateIterationsGUI(iterationText);


            return false;
        }


        //checks if num can go into the x and y coordinates of the grid by following sudoku rules
        public static bool isGridValid(int[,] grid, int x, int y, int num) {
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
        public static bool FindEmptySquare(int[,] grid) {
            for (int i = 0; i < 81; i++) {
                int x = i % 9;
                int y = i / 9;
                if (grid[x, y] == 0) return true;
            }
            return false;
        }


        //Removes numbers from a complete solution to generate a sudoku puzzle.
        //Ensures there is only one solution.
        private void RemoveNumbersFromGrid(int[,] grid, Random random, bool guaranteeSingleSolution, Button[,] buttons, TextBlock iterationText) {
            //count and find nonempty squares in grid.
            int nonEmptySqaresCount = 0;
            int[][] nonEmptySquares = GetNonEmptySquares(out nonEmptySqaresCount, grid);

            //Shuffle nonEmptySquares while perserving the pairings of coordinates
            nonEmptySquares = nonEmptySquares.OrderBy(a => random.Next()).ToArray();


            //limit the number of failed attempts to 3
            int rounds = 3;

            int currSquare = 0;
            while (rounds > 0 && nonEmptySqaresCount > 17) {
                //get next nonEmptySquare
                int x = nonEmptySquares[currSquare][0];
                int y = nonEmptySquares[currSquare][1];
                currSquare++;
                nonEmptySqaresCount--;

                //store the removed value and then empty the square
                int removed_value = grid[x, y];
                UpdateGridAndUI(grid, 0, x, y, buttons);
                UpdateIterationsGUI(iterationText);



                //copy the grid array;
                int[,] gridCopy = new int[9, 9];
                for (int i = 0; i < 81; i++) {
                    int _x = i % 9;
                    int _y = i / 9;
                    gridCopy[_x, _y] = grid[_x, _y];
                }

                //Solve the Grid
                Solver solver = new Solver();
                bool isSolutionUnique = true;
                if (guaranteeSingleSolution) isSolutionUnique = solver.Solve(gridCopy, buttons, iterationText);

                //if solution is not unique, put the last removed number back and try again
                if (!isSolutionUnique) {
                    UpdateGridAndUI(grid, removed_value, x, y, buttons);
                    nonEmptySqaresCount++;
                    currSquare--;
                    rounds--;
                }
            }
            return;
        }

        private int[][] GetNonEmptySquares(out int nonEmptyCount, int[,] grid) {
            //count how many nonempty squares there are;
            int numNonEmpty = 0;
            for (int i = 0; i < 81; i++) {
                int x = i % 9;
                int y = i / 9;
                if (grid[x, y] == 0) continue;
                numNonEmpty++;
            }
            nonEmptyCount = numNonEmpty;

            //initialize array of squares;
            int[][] nonEmptySquares = new int[numNonEmpty][];
            //add all nonempty squares to the array
            for (int i = 0; i < 81; i++) {
                int x = i % 9;
                int y = i / 9;
                if (grid[x, y] == 0) continue;
                int[] square = { x, y };
                nonEmptySquares[i] = square;
            }

            return nonEmptySquares;
        }
    }
}
