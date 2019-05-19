namespace Sudoku.Models
{
    public class SudokuElement
    {
        public int X { get; }
        public int Y { get; }
        public int Region { get; }
        public char Value { get; set; }
        public bool IsValid { get; set; }

        public SudokuElement(int x, int y, int region, char value)
        {
            X = x;
            Y = y;
            Region = region;
            Value = value;
            IsValid = true;
        }
    }
}
