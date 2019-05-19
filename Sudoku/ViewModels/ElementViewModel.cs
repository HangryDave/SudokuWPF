using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ElementViewModel(string value, int x, int y, string color)
        {
            Color = color;
            Value = value;
            X = x;
            Y = y;
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
