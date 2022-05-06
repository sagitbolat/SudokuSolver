using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver {
    class Solver {

        UpdateButtonCommand updateCommand;

        public Solver(UpdateButtonCommand uc) {
            updateCommand = uc;
        }

        private void UpdateGridAndUI(int[,] grid, int num, int x, int y) {
            grid[x, y] = num;
            updateCommand.Execute(grid);
        }

        public bool Solve(int[,] grid) {
            if (grid == null || grid.Length != 81) return false;

            for (int x = 0; x < 9; x++) {
                for (int y = 0; y < 9; y++) {
                    if (grid[x, y] == 0) {
                        //shuffle numbers array
                        for (int n = 1; n < 10; n++) {
                            if (SudokuGenerator.isGridValid(grid, x, y, n)) {
                                UpdateGridAndUI(grid, n, x, y);
                                if (Solve(grid)) return true;
                                else UpdateGridAndUI(grid, 0, x, y); ;
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
