using Sudoku.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Sudoku.Models;

namespace Sudoku
{
    public partial class Row : UserControl
    {
        public static readonly DependencyProperty RowIndexProperty = DependencyProperty.Register("RowIndex", typeof(int), typeof(Row), new PropertyMetadata(-1, OnRowIndexChanged));

        private SudokuGridViewModel ViewModel => ((SudokuGridViewModel)DataContext);

        public static void OnRowIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((Row)sender).RowIndex = int.Parse(e.NewValue.ToString());
        }

        public int RowIndex
        {
            get => int.Parse(GetValue(RowIndexProperty).ToString());
            set
            {
                SetValue(RowIndexProperty, int.Parse(value.ToString()));
                InitializeElements();
            }
        }

        public Row()
        {
            InitializeComponent();
        }

        private void InitializeElements()
        {
            for (int i = 0; i < 9; i++)
            {
                var textBox = (TextBox)FindName("Element" + i);
                var element = $"Elements[{RowIndex}][{i}]";

                var valueBinding = new Binding
                {
                    Path = new PropertyPath($"{element}.Value"),
                    Mode = BindingMode.TwoWay,
                    Source = ViewModel,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                BindingOperations.SetBinding(textBox, TextBox.TextProperty, valueBinding);

                var backgroundBinding = new Binding
                {
                    Path = new PropertyPath($"{element}.Color"),
                    Mode = BindingMode.OneWay,
                    Source = ViewModel,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                BindingOperations.SetBinding(textBox, TextBox.BackgroundProperty, backgroundBinding);

                var readOnlyBinding = new Binding
                {
                    Path = new PropertyPath($"{element}.Locked"),
                    Mode = BindingMode.OneWay,
                    Source = ViewModel,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                BindingOperations.SetBinding(textBox, TextBox.IsReadOnlyProperty, readOnlyBinding);
            }
        }

        private void TextBox_ValidateCharacter(object sender, TextCompositionEventArgs e)
        {
            var value = e.Text[0];
            e.Handled = !char.IsDigit(value) || value.Equals('0');
        }

        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = (TextBox)sender;

            var x = int.Parse(element.Name.ToCharArray()[7].ToString());
            var y = RowIndex;
            var region = SudokuGrid.GetRegionNumber(x, y);

            ViewModel.SelectElement(x, y, region);
        }

        private void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if(e.Command == ApplicationCommands.Paste)
                e.Handled = true;
        }
    }
}
