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

        int iterations;
        Color textColor;
        Button[,] buttons;
        TextBlock iterationText;


        public Solver(Color textColor, int iterationDelay, Button[,] buttons, TextBlock iterationText, int iterations = 0) {
            iterations = 0;
            this.textColor = textColor;
            this.iterationDelay = iterationDelay;
            this.buttons = buttons;
            this.iterationText = iterationText;
            this.iterations = iterations;
        }
        private void UpdateGridAndUI(int[,] grid, int num, int x, int y) {
            grid[x, y] = num;
            buttons[x, y].Dispatcher.Invoke(() => {
                buttons[x, y].Content = num > 0 ? num.ToString() : " ";
                buttons[x, y].Foreground = new SolidColorBrush(textColor);
            });
            iterations++;
            Thread.Sleep(iterationDelay);
            
        }
        private void UpdateIterationsGUI() {
            iterations++;
            iterationText.Dispatcher.Invoke(() => {
                iterationText.Text = "Iterations: " + iterations.ToString();
            });
        }

        public bool Solve(int[,] grid) {
            if (grid == null || grid.Length != 81) return false;

            for (int y = 0; y < 9; y++) {
                for (int x = 0; x < 9; x++) {
                    if (grid[x, y] == 0) {
                        //shuffle numbers array
                        for (int n = 1; n < 10; n++) {
                            if (SudokuGenerator.isGridValid(grid, x, y, n)) {
                                UpdateGridAndUI(grid, n, x, y);
                                UpdateIterationsGUI();
                                if (Solve(grid)) return true;
                                else UpdateGridAndUI(grid, 0, x, y);
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
