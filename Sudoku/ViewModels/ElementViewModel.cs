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
        private string _Color;
        public string Color
        {
            get { return _Color; }
            set
            {
                if (_Color != value)
                {
                    _Color = value;
                    OnPropertyChanged(nameof(Color));
                }
            }
        }

        private string _Value;
        public string Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        private bool _Locked;
        public bool Locked
        {
            get { return _Locked; }
            set
            {
                if (_Locked != value)
                {
                    _Locked = value;
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
            return Value.Length > 0 ? Value[0] : 'X';
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
