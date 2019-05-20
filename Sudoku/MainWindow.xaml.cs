using Microsoft.Win32;
using Sudoku.ViewModels;
using System.IO;
using System.Windows;

namespace Sudoku
{
    public partial class MainWindow : Window
    {
        private readonly SudokuGridViewModel _viewModel;

        public MainWindow()
        {
            _viewModel = new SudokuGridViewModel();
            DataContext = _viewModel;
            InitializeComponent();
        }

        private void WriteItem_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog { Filter = "Text file (*.txt)|*.txt" };
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, _viewModel.ToString());
            }
        }

        private void OpenItem_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = "Text file (*.txt)|*.txt" };
            if (openFileDialog.ShowDialog() != true) // Because I'll forget, ShowDialog() is a nullable, so you have to compare it. If it's null or false, return.
                return;
            
            var confirm = MessageBox.Show("Are you sure that you want to load a new puzzle?\nUnsaved changes will be overwritten.", "Open Puzzle", MessageBoxButton.YesNo);
            if (confirm == MessageBoxResult.Yes)
                _viewModel.OpenPuzzle(File.ReadAllText(openFileDialog.FileName));
        }

        private void ResetPuzzle_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Reset();
        }

        private void SolvePuzzle_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Solve();
        }
    }
}
