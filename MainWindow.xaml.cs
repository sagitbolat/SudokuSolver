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

        Color fg;
        Color bg;
        Color accent;

        UpdateButtonCommand updateCommand;

        public MainWindow() {
            InitializeComponent();

            gridControl = PlayGrid;

            fg = Color.FromRgb(34, 40, 49);
            bg = Color.FromRgb(221, 221, 221);
            accent = Color.FromRgb(240, 84, 84);

            updateCommand = new UpdateButtonCommand(buttonArray, new SolidColorBrush(fg));

            //setup buttons
            ButtonSetup();

            //assign Events
            GenerateButton.Click += GenerateButtonPressed;

            SolveButton.Click += SolveButtonPressed;

        }

        private void GenerateButtonPressed(object sender, RoutedEventArgs e) {
            GenerateGrid();
        }

        private void SolveButtonPressed(object sender, RoutedEventArgs e) {
            SolveGrid();
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

        private void GenerateGrid() {
            //generate sudoku solution
            SudokuGenerator generator = new SudokuGenerator();

            if (GuaranteeCheckbox.IsChecked == true) {
                numGrid = generator.Generate(true, updateCommand);
            } else {
                numGrid = generator.Generate(false, updateCommand);
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
        private void SolveGrid() {
            Solver solver = new Solver(updateCommand);
            if (numGrid == null || numGrid.Length != 81) return;
            solver.Solve(numGrid);

            // fill in the button text to match the sudoku grid
            for (int i = 0; i < 81; i++) {
                int y = i / 9;
                int x = i % 9;
                int value = numGrid[x, y];

                Button button = buttonArray[x, y];
                if (value.ToString() != button.Content.ToString()) {
                    buttonArray[x, y].Foreground = new SolidColorBrush(accent);
                }
                buttonArray[x, y].Content = value;// > 0 ? value.ToString() : " ";
            }
        }
    }

    public class UpdateButtonCommand {
        Button[,] buttons;
        SolidColorBrush fg;
        public UpdateButtonCommand(Button[,] buttons, SolidColorBrush fg) {
            this.buttons = buttons;
            this.fg = fg;
        }
        public void Execute(int[,] grid) {
            //fill in the button text to match the sudoku grid
            for (int i = 0; i < 81; i++) {
                int y = i / 9;
                int x = i % 9;
                int value = grid[x, y];
                buttons[x, y].Content = value > 0 ? value.ToString() : " ";
                //buttons[x, y].Foreground = fg;
            }
        }
    }
}
