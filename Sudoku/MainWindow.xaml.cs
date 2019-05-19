using Microsoft.Win32;
using Sudoku.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sudoku
{
    public partial class MainWindow : Window
    {
        private SudokuGridViewModel ViewModel;

        public MainWindow()
        {
            ViewModel = new SudokuGridViewModel();
            DataContext = ViewModel;
            InitializeComponent();
        }

        private void WriteItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, ViewModel.ToString());
            }
        }

        private void OpenItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text file (*.txt)|*.txt";
            if(openFileDialog.ShowDialog() == true)
            {
                MessageBoxResult confirm = MessageBox.Show("Are you sure that you want to load a new puzzle?\nUnsaved changes will be overwritten.", "Open Puzzle", MessageBoxButton.YesNo);
                if (confirm == MessageBoxResult.Yes)
                {
                    ViewModel.OpenPuzzle(File.ReadAllText(openFileDialog.FileName));
                }
            }
        }

        private void ResetPuzzle_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Reset();
        }

        private void SolvePuzzle_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Solve();
        }
    }
}
