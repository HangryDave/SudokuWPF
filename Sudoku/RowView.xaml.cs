using Sudoku.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Sudoku
{
    public partial class Row : UserControl
    {
        public static readonly DependencyProperty RowIndexProperty = DependencyProperty.Register("RowIndex", typeof(int), typeof(Row), new PropertyMetadata(-1, OnRowIndexChanged));

        public SudokuGridViewModel ViewModel
        {
            get { return ((SudokuGridViewModel)DataContext); }
        }

        public static void OnRowIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((Row)sender).RowIndex = int.Parse(e.NewValue.ToString());
        }

        public int RowIndex
        {
            get { return int.Parse(GetValue(RowIndexProperty).ToString()); }
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
                TextBox textBox = (TextBox)FindName("element_" + i);

                Binding valueBinding = new Binding();
                valueBinding.Path = new PropertyPath($"Elements[{RowIndex}][{i}].Value");
                valueBinding.Mode = BindingMode.TwoWay;
                valueBinding.Source = ViewModel;
                valueBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding((TextBox)FindName("element_" + i), TextBox.TextProperty, valueBinding);

                Binding backgroundBinding = new Binding();
                backgroundBinding.Path = new PropertyPath($"Elements[{RowIndex}][{i}].Color");
                backgroundBinding.Mode = BindingMode.OneWay;
                backgroundBinding.Source = ViewModel;
                backgroundBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding((TextBox)FindName("element_" + i), TextBox.BackgroundProperty, backgroundBinding);

                Binding readOnlyBinding = new Binding();
                readOnlyBinding.Path = new PropertyPath($"Elements[{RowIndex}][{i}].Locked");
                readOnlyBinding.Mode = BindingMode.OneWay;
                readOnlyBinding.Source = ViewModel;
                readOnlyBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding((TextBox)FindName("element_" + i), TextBox.IsReadOnlyProperty, readOnlyBinding);
            }
        }

        private void TextBox_ValidateCharacter(object sender, TextCompositionEventArgs e)
        {
            char c = e.Text[0];
            e.Handled = !Char.IsDigit(c) || c.Equals('0');
        }

        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox element = (TextBox)sender;

            int y = RowIndex;
            int x = int.Parse(element.Name.ToCharArray()[8].ToString());

            ViewModel.SelectElement(x, y);
        }

        private void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if(e.Command == ApplicationCommands.Paste)
                e.Handled = true;
        }
    }
}
