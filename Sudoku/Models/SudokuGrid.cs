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
        private SudokuRegion[] Regions;
        private List<SudokuElement> Elements;
        private const char EmptyValue = 'X';

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
            string[] rows = new string[9];

            if (grid != null)
            {
                string fixedGrid = grid.Replace(" ", "");
                StringReader reader = new StringReader(fixedGrid);

                int i = 0;
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line.Length > 0)
                    {
                        if (i != 0)
                            rows[i] = line + '\n';
                        else
                            rows[i] = line;

                        i++;
                    }

                    line = reader.ReadLine();
                }
                reader.Close();
            }
            else
            {
                rows = new string[9].Select(s => new String('X', 9)).ToArray();
            }

            for (int y = 0; y < 9; y++)
            {
                string row = rows[y];
                for (int x = 0; x < 9; x++)
                {
                    int partition = (x / 3) + (y / 3) * 3;
                    SudokuElement element = new SudokuElement(x, y, partition, row[x]);
                    result.Add(element);
                }
            }

            return result;
        }

        public void Solve()
        {
            var nextEmptyUnit = Elements.FirstOrDefault(e => e.Value == 'X');
            var solveUnitRecursive = SolveUnitRecursive(nextEmptyUnit);
        }

        private bool SolveUnitRecursive(SudokuElement element)
        {
            var legalValues = GetLegalValuesForElement(element);
            if (legalValues.Count() == 0)
                return false;

            foreach (var valueToTest in legalValues)
            {
                element.Value = valueToTest;

                var nextEmptyUnit = Elements.FirstOrDefault(u => u.Value.Equals(EmptyValue));
                if (nextEmptyUnit == null)
                    return true;

                var solved = SolveUnitRecursive(nextEmptyUnit);
                if (solved)
                    return true;
            }
            element.Value = EmptyValue;
            return false;
        }

        private char[] PossibleValues = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
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
            bool success = true;
            for (int i = 0; i < Elements.Count(); i++)
            {
                SudokuElement element = Elements.ElementAt(i);

                char c = element.Value;
                bool canSet = IsElementValid(element.X, element.Y, c);

                element.Valid = canSet;
                if (!canSet)
                    success = false;
            }
            return success;
        }

        public bool IsElementValid(int x, int y, char c)
        {
            char[] column = GetColumn(x);
            char[] row = GetRow(y);
            char[] region = GetRegion(x, y);

            bool canSet = (column.Where(e => e.Equals(c)).Count() <= 1 &&
                                  row.Where(e => e.Equals(c)).Count() <= 1 &&
                                  region.Where(e => e.Equals(c)).Count() <= 1 &&
                                  Char.IsDigit(c) && !Char.IsLetter(c)) || c.Equals('X');

            return canSet;
        }

        public bool IsSolved()
        {
            for (int i = 0; i < Elements.Count(); i++)
            {
                var element = Elements.ElementAt(i);
                char value = element.Value;
                if (!IsElementValid(element.X, element.Y, value) || value.Equals('X'))
                    return false;
            }

            return true;
        }

        public SudokuElement GetElement(int x, int y)
        {
            return Elements.FirstOrDefault(e => e.X == x && e.Y == y);
        }

        public void SetElement(int x, int y, char c)
        {
            var element = GetElement(x, y);
            element.Value = c;
        }

        public char[] GetRegion(int x, int y)
        {
            var element = GetElement(x, y);
            return Elements.Where(e => e.Region == element.Region).Select(e => e.Value).ToArray();
        }

        public char[] GetColumn(int index)
        {
            return Elements.Where(e => e.X == index).Select(e => e.Value).ToArray();
        }

        public char[] GetRow(int index)
        {
            return Elements.Where(e => e.Y == index).Select(e => e.Value).ToArray();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int y = 0; y < Elements.Count(); y++)
            {
                string add = new string(GetRow(y));
                if (y != Elements.Count() - 1)
                {
                    add += Environment.NewLine;
                }
                builder.Append(add);
            }

            return builder.ToString();
        }

        /*private SudokuRegion[] InitializeRegions()
        {
            SudokuRegion[] regions = new SudokuRegion[9];
            regions[0] = new SudokuRegion(Elements, 0, 0, 2, 2);
            regions[1] = new SudokuRegion(Elements, 3, 0, 5, 2);
            regions[2] = new SudokuRegion(Elements, 6, 0, 8, 2);

            regions[3] = new SudokuRegion(Elements, 0, 3, 2, 5);
            regions[4] = new SudokuRegion(Elements, 3, 3, 5, 5);
            regions[5] = new SudokuRegion(Elements, 6, 3, 8, 5);

            regions[6] = new SudokuRegion(Elements, 0, 6, 2, 8);
            regions[7] = new SudokuRegion(Elements, 3, 6, 5, 8);
            regions[8] = new SudokuRegion(Elements, 6, 6, 8, 8);

            return regions;
        }*/
    }
}
