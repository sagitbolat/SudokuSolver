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
        int iterations;

        public Solver() {
            iterations = 0;
        }
        private void UpdateGridAndUI(int[,] grid, int num, int x, int y, Button[,] buttons) {
            grid[x, y] = num;
            buttons[x, y].Dispatcher.Invoke(() => {
                buttons[x, y].Content = num > 0 ? num.ToString() : " ";
                Color accent = Color.FromRgb(240, 84, 84);
                buttons[x, y].Foreground = new SolidColorBrush(accent);
            });
            iterations++;
            Thread.Sleep(10);
            
        }
        private void UpdateIterationsGUI(TextBlock iterationText) {
            iterations++;
            iterationText.Dispatcher.Invoke(() => {
                iterationText.Text = "Iterations: " + iterations.ToString();
            });
        }

        public bool Solve(int[,] grid, Button[,] buttons, TextBlock iterationText) {
            if (grid == null || grid.Length != 81) return false;

            for (int y = 0; y < 9; y++) {
                for (int x = 0; x < 9; x++) {
                    if (grid[x, y] == 0) {
                        //shuffle numbers array
                        for (int n = 1; n < 10; n++) {
                            if (SudokuGenerator.isGridValid(grid, x, y, n)) {
                                UpdateGridAndUI(grid, n, x, y, buttons);
                                UpdateIterationsGUI(iterationText);
                                if (Solve(grid, buttons, iterationText)) return true;
                                else UpdateGridAndUI(grid, 0, x, y, buttons);
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
