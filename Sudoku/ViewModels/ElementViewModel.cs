using Sudoku.Models;
using System.ComponentModel;

namespace Sudoku.ViewModels
{
    public class ElementViewModel : INotifyPropertyChanged
    {
        private string _color;
        public string Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged(nameof(Color));
                }
            }
        }

        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        private bool _locked;
        public bool Locked
        {
            get => _locked;
            set
            {
                if (_locked != value)
                {
                    _locked = value;
                    OnPropertyChanged(nameof(Locked));
                }
            }
        }

        public int X { get; }
        public int Y { get; }
        public int Region { get; }

        public ElementViewModel(char value, int x, int y, int region, string color)
        {
            Color = color;
            Value = value.ToString();
            X = x;
            Y = y;
            Region = region;
        }

        public char AsChar()
        {
            return Value.Length > 0 ? Value[0] : SudokuGrid.EmptyValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
