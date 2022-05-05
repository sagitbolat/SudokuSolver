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
        public MainWindow() {
            InitializeComponent();

            Button[,] buttonArray = new Button[9,9];
            Grid grid = PlayGrid;

            Color buttonBGColor = new Color();
            buttonBGColor.R = 221;
            buttonBGColor.G = 221;
            buttonBGColor.B = 221;
            buttonBGColor.A = 225;
            Brush buttonBGBrush = new SolidColorBrush(buttonBGColor);

            Color buttonFGColor = new Color();
            buttonFGColor.R = 34;
            buttonFGColor.G = 40;
            buttonFGColor.B = 49;
            buttonFGColor.A = 225;
            Brush buttonFGBrush = new SolidColorBrush(buttonFGColor);
            //FontFamily buttonFont = new FontFamily(new Uri("pack://application:,,,"), "FuturaBook.ttf");

            //iterate over the whole grid and add in the button components
            for (int x = 0; x < 9; x++) {
                for (int y = 0; y < 9; y++) {
                    //create button
                    Button button = new Button();

                    //set button asthetics
                    button.Background = buttonBGBrush;
                    button.Foreground = buttonFGBrush;
                    button.Content = "j";
                    button.FontSize = 19;
                    //button.FontFamily = buttonFont;

                    //add button to array
                    buttonArray[x, y] = button;
                    
                    // add the button to the grid control
                    Grid.SetRow(button, y);
                    Grid.SetColumn(button, x);
                    grid.Children.Add(button);
                }
            }

            //generate sudoku solution
            SudokuGenerator generator = new SudokuGenerator();
            int[,] numGrid = generator.Generate();

            //fill in the button text to match the sudoku grid
            for (int i = 0; i < 81; i++) {
                int y = i / 9;
                int x = i % 9;
                int value = numGrid[x, y];
                buttonArray[x, y].Content = value > 0 ? value.ToString() : " ";
            }
        }
    }
}
