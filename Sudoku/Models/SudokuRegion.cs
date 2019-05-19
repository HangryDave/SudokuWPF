using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Models
{
    public class SudokuRegion
    {
        public int StartX { get; set; }
        public int StartY { get; set;  }
        public int EndX { get; set; }
        public int EndY { get; set; }

        public SudokuElement[] Elements { get; set; }

        public SudokuRegion(SudokuElement[][] elements, int startX, int startY, int endX, int endY)
        {
            StartX = startX;
            StartY = startY;

            EndX = endX;
            EndY = endY;

            UpdateElements(elements);
        }

        public bool ContainsPoint(int x, int y)
        {
            return StartX <= x && x <= EndX && StartY <= y && y <= EndY;
        }

        public void UpdateElements(SudokuElement[][] allElements)
        {
            Elements = new SudokuElement[9];

            int i = 0;
            for(int y = StartY; y <= EndY; y++)
            {
                for(int x = StartX; x <= EndX; x++)
                {
                    SudokuElement element = allElements[y][x];
                    Elements[i] = element;
                    i++;
                }
            }
        }
    }
}
