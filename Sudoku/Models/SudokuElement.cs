using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Models
{
    public class SudokuElement
    {
        public int X { get; }
        public int Y { get; }
        public int Region { get; }
        public char Value { get; set; }
        public bool Valid { get; set; }

        public SudokuElement(int x, int y, int region, char value)
        {
            X = x;
            Y = y;
            Region = region;
            Value = value;
            Valid = true;
        }
    }
}
