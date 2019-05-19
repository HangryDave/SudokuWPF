using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Models
{
    public class SudokuGrid
    {
        public const char EmptyValue = 'X';
        private static readonly char[] PossibleValues = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        private List<SudokuElement> Elements;

        public SudokuGrid()
        {
            Elements = Parse(null);
        }

        public bool OpenPuzzle(string data)
        {
            Elements = Parse(data);
            return IsValidPuzzle();
        }

        private List<SudokuElement> Parse(string grid)
        {
            var result = new List<SudokuElement>();
            var rows = new string[9];

            if (grid != null)
            {
                var fixedGrid = grid.Replace(" ", "");
                var reader = new StringReader(fixedGrid);

                int y = 0;
                var line = reader.ReadLine();
                while (line != null)
                {
                    if (line.Length > 0)
                    {
                        if (y != 0)
                            rows[y] = line + '\n';
                        else
                            rows[y] = line;

                        y++;
                    }

                    line = reader.ReadLine();
                }
                reader.Close();
            }
            else
            {
                // There is no predefined puzzle, so just create an empty set of rows.
                rows = new string[9].Select(s => new String(EmptyValue, 9)).ToArray();
            }

            for (int y = 0; y < 9; y++)
            {
                var row = rows[y];
                for (int x = 0; x < 9; x++)
                {
                    int partition = (x / 3) + (y / 3) * 3;
                    var element = new SudokuElement(x, y, partition, row[x]);
                    result.Add(element);
                }
            }

            return result;
        }

        public void Solve()
        {
            var firstEmptyElement = Elements.FirstOrDefault(e => e.Value == EmptyValue);
            var solveUnitRecursive = Solve(firstEmptyElement);
        }

        private bool Solve(SudokuElement element)
        {
            var legalValues = GetLegalValuesForElement(element);
            if (legalValues.Count() == 0) // There are no possible options.
                return false;

            foreach (var valueToTest in legalValues)
            {
                element.Value = valueToTest;

                var nextEmptyElement = Elements.FirstOrDefault(e => e.Value.Equals(EmptyValue));
                if (nextEmptyElement == null) // If there are no empty elements, then we have solved the puzzle!
                    return true;

                var solved = Solve(nextEmptyElement);
                if (solved) // If solved is true, we chose the correct option from the legal values.
                    return true;
            }
            // We made it through all of the possible options and failed to solve the puzzle. Reset the element.
            element.Value = EmptyValue;
            return false;
        }

        private List<char> GetLegalValuesForElement(SudokuElement element)
        {
            if (!element.Value.Equals(EmptyValue))
                return null;

            var partitionUnitValues = GetRegion(element.X, element.Y).Where(v => v != EmptyValue);
            var horizontalUnitValues = GetRow(element.Y).Where(v => v != EmptyValue);
            var verticalUnitValues = GetColumn(element.X).Where(v => v != EmptyValue);
            return PossibleValues.Except(partitionUnitValues).Except(horizontalUnitValues).Except(verticalUnitValues).ToList();
        }

        private bool IsValidPuzzle()
        {
            var success = true;
            foreach (var element in Elements)
            {
                var canSet = IsValuePossible(element.Value, element.X, element.Y);

                element.IsValid = canSet;

                if (!canSet)
                    success = false;
            }
            return success;
        }

        public bool IsValuePossible(char value, int x, int y)
        {
            if (value.Equals(EmptyValue))
                return true;
            
            var columnValues = GetColumn(x);
            var rowValues = GetRow(y);
            var regionValues = GetRegion(x, y);

            var validColumn = columnValues.Where(e => e.Equals(value)).Count() <= 1;
            var validRow = rowValues.Where(e => e.Equals(value)).Count() <= 1;
            var validRegion = regionValues.Where(e => e.Equals(value)).Count() <= 1;
            var validValue = char.IsDigit(value) && !char.IsLetter(value) || value.Equals(EmptyValue);

            return validColumn && validRow && validRegion && validValue;
        }

        public bool IsSolved()
        {
            foreach (var element in Elements)
            {
                if (element.Value.Equals(EmptyValue))
                    return false;

                if (!IsValuePossible(element.Value, element.X, element.Y))
                    return false;
            }

            return true;
        }

        public SudokuElement GetElement(int x, int y)
        {
            return Elements.FirstOrDefault(e => e.X == x && e.Y == y);
        }

        public void SetElement(int x, int y, char value)
        {
            var element = GetElement(x, y);
            element.Value = value;
        }

        public char[] GetRegion(int x, int y)
        {
            var element = GetElement(x, y);
            return Elements.Where(e => e.Region == element.Region)
                           .Select(e => e.Value)
                           .ToArray();
        }

        public char[] GetColumn(int index)
        {
            return Elements.Where(e => e.X == index)
                           .Select(e => e.Value)
                           .ToArray();
        }

        public char[] GetRow(int index)
        {
            return Elements.Where(e => e.Y == index)
                           .Select(e => e.Value)
                           .ToArray();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            for (int y = 0; y < Elements.Count(); y++)
            {
                var add = new string(GetRow(y));
                if (y != Elements.Count() - 1)
                    add += Environment.NewLine;

                builder.Append(add);
            }

            return builder.ToString();
        }
    }
}
