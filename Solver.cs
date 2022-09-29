using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;



namespace SudokuSolver {
    class Solver {

        public int iterationDelay;

        Color textColor;
        Button[,] buttons;
        TextBlock iterationText;

        
        public Solver(Color textColor, int iterationDelay, Button[,] buttons, TextBlock iterationText) {
            this.textColor = textColor;
            this.iterationDelay = iterationDelay;
            this.buttons = buttons;
            this.iterationText = iterationText;
        }
        private void UpdateGridAndUI(int[,] grid, int num, int x, int y, ref int iterations) {
            grid[x, y] = num;
            buttons[x, y].Dispatcher.Invoke(() => {
                buttons[x, y].Content = num > 0 ? num.ToString() : " ";
                buttons[x, y].Foreground = new SolidColorBrush(textColor);
            });
            iterations++;
            Thread.Sleep(iterationDelay);
            
        }
        private void UpdateIterationsGUI(ref int iterations) {
            iterations++;
            int iter = iterations;
            iterationText.Dispatcher.Invoke(() => {
                iterationText.Text = "Iterations: " + iter.ToString();
            });
        }

        public bool Solve(int[,] grid, ref int iterations) {
            if (grid == null || grid.Length != 81) return false;

            for (int y = 0; y < 9; y++) {
                for (int x = 0; x < 9; x++) {
                    if (grid[x, y] == 0) {
                        //shuffle numbers array
                        for (int n = 1; n < 10; n++) {
                            if (SudokuGenerator.isGridValid(grid, x, y, n)) {
                                UpdateGridAndUI(grid, n, x, y, ref iterations);
                                UpdateIterationsGUI(ref iterations);
                                if (Solve(grid, ref iterations)) return true;
                                else UpdateGridAndUI(grid, 0, x, y, ref iterations);
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
