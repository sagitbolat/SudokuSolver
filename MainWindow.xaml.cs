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

        public MainWindow() {
            InitializeComponent();

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

        }

        private async void GenerateButtonPressed(object sender, RoutedEventArgs e) {
            if (executing) return;

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

        private void ButtonSetup() {
            Brush buttonBGBrush = new SolidColorBrush(bg);
            Brush buttonFGBrush = new SolidColorBrush(fg);
            //FontFamily buttonFont = new FontFamily(new Uri("pack://application:,,,"), "FuturaBook.ttf");

            //iterate over the whole grid and add in the button components
            for (int x = 0; x < 9; x++) {
                for (int y = 0; y < 9; y++) {
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
            //generate sudoku solution
            SudokuGenerator generator = new SudokuGenerator();

            if (GuaranteeCheckbox.IsChecked == true) {
                numGrid = await generator.Generate(true, buttonArray, iterationsText);
            } else {
                numGrid = await generator.Generate(false, buttonArray, iterationsText);
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
            Solver solver = new Solver();
            if (numGrid == null || numGrid.Length != 81) return;
            await Task.Run(() => solver.Solve(numGrid, buttonArray, iterationsText));
        }
    }
    
}
