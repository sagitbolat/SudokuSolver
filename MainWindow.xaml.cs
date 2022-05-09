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
        Button[,] buttonArray = new Button[9,9];
        Grid gridControl;
        int[,] numGrid;
        TextBlock iterationsText;
        bool executing = false;
        

        Color fg;
        Color bg;
        Color accent;
        int iterationDelay = 10;
        int difficulty = 0;

        public MainWindow() {
            InitializeComponent();

            gridControl = PlayGrid;

            fg = Color.FromRgb(34, 40, 49);
            bg = Color.FromRgb(221, 221, 221);
            accent = Color.FromRgb(240, 84, 84);

            iterationsText = IterationsTextBlock;
            difficulty = DifficultySelect.SelectedIndex;

            //setup buttons
            ButtonSetup();

            //assign Events
            GenerateButton.Click += GenerateButtonPressed;

            SolveButton.Click += SolveButtonPressed;

            ClearButton.Click += ClearButtonPressed;

            DifficultySelect.SelectionChanged += DifficultyChanged;

            SpeedSelect.SelectionChanged += SpeedChanged;

        }

        private async void GenerateButtonPressed(object sender, RoutedEventArgs e) {
            if (executing) return;

            await Task.Run(() => ClearGridAsync());
            executing = true;
            await GenerateGrid();
            executing = false;
        }

        private async void SolveButtonPressed(object sender, RoutedEventArgs e) {
            if (executing) return;

            executing = true;
            await SolveGrid();
            executing = false;
        }

        private async void ClearButtonPressed(object sender, RoutedEventArgs e) {
            if (executing) return;

            await Task.Run(() => ClearGridAsync());
        }

        private void DifficultyChanged(object sender, RoutedEventArgs e) {
            difficulty = DifficultySelect.SelectedIndex;
        }

        private void SpeedChanged(object sender, RoutedEventArgs e) {
            iterationDelay = 10;
            switch(SpeedSelect.SelectedIndex) {
                case 0:
                    iterationDelay = 10;
                    break;
                case 1:
                    iterationDelay = 50;
                    break;
                case 2:
                    iterationDelay = 100;
                    break;
            }
        }

        private void ClearGridAsync() {
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

                    //set button asthetics
                    button.Background = buttonBGBrush;
                    button.Foreground = buttonFGBrush;
                    button.BorderBrush = buttonFGBrush;
                    button.Content = " ";
                    button.FontSize = 19;
                    //button.FontFamily = buttonFont;

                    //add button to array
                    buttonArray[x, y] = button;

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
            int minCellFilled = 17;
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
            SudokuGenerator generator = new SudokuGenerator(singleSolution, fg, iterationDelay, buttonArray, IterationsTextBlock, 0, minCellFilled);
            numGrid = await generator.Generate();


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
            Solver solver = new Solver(accent, iterationDelay, buttonArray, IterationsTextBlock);
            if (numGrid == null || numGrid.Length != 81) return;
            await Task.Run(() => solver.Solve(numGrid));
        }
    }
    
}
