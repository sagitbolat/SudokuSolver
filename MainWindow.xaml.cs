using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace SudokuSolver {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        Button selectedGridCell = null;
        Grid gridControl;
        Button[,] buttonArray = new Button[9,9];
        TextBlock[,] cornerTextArray = new TextBlock[9, 9];
        bool[,] mutable = new bool[9, 9];
        int[,] numGrid;
        int[,] userSolutionGrid;
        
        
        TextBlock iterationsText;
        int iterations = 0;
        bool executing = false;
        bool gridClear = true;
        int minCellFilled = 17;
        int iterationDelay = 10;

        Solver solver;
        SudokuGenerator generator;

        Color fg;
        Color bg;
        Color accent;
        double borderThickness = 2;

        public MainWindow() {
            InitializeComponent();

            for (int i = 0; i < 81; i++) {
                mutable[i % 9, i / 9] = true;
            }
            gridControl = PlayGrid;

            fg = Color.FromRgb(34, 40, 49);
            bg = Color.FromRgb(221, 221, 221);
            accent = Color.FromRgb(240, 84, 84);

            iterationsText = IterationsTextBlock;

            //setup buttons
            ButtonSetup();

            //assign Events
            GenerateButton.Click += GenerateButtonPressed;

            SolveButton.Click += SolveButtonPressed;

            ClearButton.Click += ClearButtonPressed;

            DifficultySelect.SelectionChanged += DifficultyChanged;

            SpeedSelect.ValueChanged += SpeedChanged;

            MainGrid.KeyDown += KeyPressed;


            //construct algorithm objects
            generator = new SudokuGenerator((bool)GuaranteeCheckbox.IsChecked, fg, accent, iterationDelay, buttonArray, IterationsTextBlock, 0, minCellFilled);
            solver = new Solver(accent, iterationDelay, buttonArray, IterationsTextBlock);


        }

        private async void GenerateButtonPressed(object sender, RoutedEventArgs e) {
            if (executing) return;

            if (!gridClear)
                await Task.Run(() => ClearGridAsync());
            executing = true;
            await GenerateGrid();
            executing = false;
            gridClear = false;
        }

        private async void SolveButtonPressed(object sender, RoutedEventArgs e) {
            if (executing) return;
            if (gridClear) return;

            executing = true;
            await SolveGrid();
            executing = false;
        }

        private async void ClearButtonPressed(object sender, RoutedEventArgs e) {
            if (executing) return;
            if (gridClear == true) return;

            await Task.Run(() => ClearGridAsync());
            for (int i = 0; i < 81; i++) {
                mutable[i % 9, i / 9] = true;
            }
            gridClear = true;
        }

        private void DifficultyChanged(object sender, RoutedEventArgs e) {
            int difficulty = DifficultySelect.SelectedIndex;
            minCellFilled = 17;
            switch (difficulty) {
                case 0:
                    minCellFilled = 17;
                    break;
                case 1:
                    minCellFilled = 25;
                    break;
                case 2:
                    minCellFilled = 45;
                    break;
            }
        }

        private void SpeedChanged(object sender, RoutedEventArgs e) {
            iterationDelay = 10;
            iterationDelay = (int)SpeedSelect.Value;
            if (solver != null) solver.iterationDelay = iterationDelay;
            if (generator != null) generator.iterationDelay = iterationDelay;
        }

        private void GridButtonPressed(object sender, RoutedEventArgs e) {
            ChangeSelectedButton((Button)sender);
        }

        private void KeyPressed(object sender, KeyEventArgs e) {

            //if no selected grid, return
            if (selectedGridCell == null) return;

            //get button coords
            string coord = selectedGridCell.Name;
            int x = coord[1] - '0';
            int y = coord[2] - '0';


            //move selection if arrow key is pressed
            if (e.Key == Key.S || e.Key == Key.Down) {
                ChangeSelectedButton(buttonArray[x, y == 8 ? 0 : y + 1]);
                return;
            } else if (e.Key == Key.W || e.Key == Key.Up) {
                ChangeSelectedButton(buttonArray[x, y == 0 ? 8 : y - 1]);
                return;
            } else if (e.Key == Key.A || e.Key == Key.Left) {
                ChangeSelectedButton(buttonArray[x == 0 ? 8 : x - 1, y]);
                return;
            } else if (e.Key == Key.D || e.Key == Key.Right ) {
                ChangeSelectedButton(buttonArray[x == 8 ? 0 : x + 1, y]);
                return;
            }

            //if cell not mutable, return
            if (!mutable[x, y]) return;

            //if backspace or 0, set content to empty and return
            if(e.Key == Key.D0 || e.Key == Key.Delete) {
                selectedGridCell.Content = " ";
                return;
            }

            //init num
            int num = 0;

            //set num to the number pressed
            switch(e.Key) {
                case Key.D1:
                    num = 1;
                    break;
                case Key.D2:
                    num = 2;
                    break;
                case Key.D3:
                    num = 3;
                    break;
                case Key.D4:
                    num = 4;
                    break;
                case Key.D5:
                    num = 5;
                    break;
                case Key.D6:
                    num = 6;
                    break;
                case Key.D7:
                    num = 7;
                    break;
                case Key.D8:
                    num = 8;
                    break;
                case Key.D9:
                    num = 9;
                    break;
            }

            //change grid number
            selectedGridCell.Foreground = new SolidColorBrush(accent);
            selectedGridCell.Content = num > 0 && num < 10 ? num.ToString() : selectedGridCell.Content;

            //mark grid as not clear
            gridClear = false;

            //add number to grid user solution grid
            if (userSolutionGrid == null) return;
            userSolutionGrid[x, y] = num;
        }

        private void ChangeSelectedButton(Button newSelection) {
            //unhighlight old selection
            if (selectedGridCell != null)
                selectedGridCell.BorderBrush = new SolidColorBrush(fg);
            //set new selection
            selectedGridCell = newSelection;
            //highlight new selection
            selectedGridCell.BorderBrush = new SolidColorBrush(accent);
        }

        private void ClearGridAsync() {
            iterations = 0;
            for (int y = 0; y < 9; y++) {
                for (int x = 0; x < 9; x++) {
                    buttonArray[x, y].Dispatcher.Invoke(() => {
                        buttonArray[x, y].Content = " ";
                        buttonArray[x, y].Foreground = new SolidColorBrush(fg);
                    });
                    Thread.Sleep(iterationDelay);
                }
            }
        }

        private void ButtonSetup() {
            Brush buttonBGBrush = new SolidColorBrush(bg);
            Brush buttonFGBrush = new SolidColorBrush(fg);
            //FontFamily buttonFont = new FontFamily(new Uri("pack://application:,,,"), "FuturaBook.ttf");

            //iterate over the whole grid and add in the button components
            for (int y = 0; y < 9; y++) {
                for (int x = 0; x < 9; x++) {
                    //create button
                    Button button = new Button();
                    TextBlock cornerText = new TextBlock();

                    //set button asthetics
                    button.Background = buttonBGBrush;
                    button.Foreground = buttonFGBrush;
                    button.BorderBrush = buttonFGBrush;
                    button.BorderThickness = new Thickness(borderThickness);
                    button.Content = " ";
                    button.FontSize = 19;
                    //button.FontFamily = buttonFont;

                    //add button to array
                    buttonArray[x, y] = button;
                    cornerTextArray[x, y] = cornerText;
                    //set button name to be coordinates
                    button.Name = "C" + x.ToString() + y.ToString();

                    //subscribe the event to the button press
                    button.Click += GridButtonPressed;

                    // add the button to the grid control
                    Grid.SetRow(button, y);
                    Grid.SetColumn(button, x);
                    gridControl.Children.Add(button);
                }
            }
        }

        private async Task GenerateGrid() {
            
            bool singleSolution = (bool)GuaranteeCheckbox.IsChecked;
            //generate sudoku solution
            if (generator == null) 
                generator = new SudokuGenerator(singleSolution, fg, accent, iterationDelay, buttonArray, IterationsTextBlock, 0, minCellFilled);
            else {
                generator.guaranteeSingleSolution = singleSolution;
                generator.iterationDelay = iterationDelay;
                generator.minFilledCells = minCellFilled;
            }
            numGrid = await generator.Generate(iterations);
            userSolutionGrid = new int[9, 9];
            //copy generated grid to user solution grid, and toggle mutable cells
            for (int i = 0; i < 81; i++) {
                userSolutionGrid[i % 9, i / 9] = numGrid[i % 9, i / 9];
                if (numGrid[i % 9, i / 9] != 0) mutable[i % 9, i / 9] = false;
                else mutable[i % 9, i / 9] = true;
            }

            //fill in the button text to match the sudoku grid
            for (int i = 0; i < 81; i++) {
                int y = i / 9;
                int x = i % 9;
                int value = numGrid[x, y];
                buttonArray[x, y].Content = value > 0 ? value.ToString() : " ";
                buttonArray[x, y].Foreground = new SolidColorBrush(fg);
            }
        }
        private async Task SolveGrid() {
            if (solver == null)
                solver = new Solver(accent, iterationDelay, buttonArray, IterationsTextBlock);
            else {
                solver.iterationDelay = iterationDelay;
            }
            if (numGrid == null || numGrid.Length != 81) return;
            await Task.Run(() => solver.Solve(numGrid, ref iterations));

            for (int i = 0; i < 81; i++) {
                mutable[i % 9, i / 9] = false;
            }
        }
    }
    
}
