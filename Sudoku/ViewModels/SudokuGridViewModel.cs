using Sudoku.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Sudoku.ViewModels
{
    public class SudokuGridViewModel : INotifyPropertyChanged
    {
        private SudokuGrid Grid;
        public ObservableCollection<ObservableCollection<ElementViewModel>> Elements { get; set; }
        public ObservableCollection<string> AllowedElementsColors { get; set; }

        private string _Status;
        public string Status
        {
            get { return _Status; }
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        private string _ValidNumberVisibility;
        public string ValidNumberVisibility
        {
            get { return _ValidNumberVisibility; }
            set
            {
                if(_ValidNumberVisibility != value)
                {
                    _ValidNumberVisibility = value;
                    OnPropertyChanged(nameof(ValidNumberVisibility));
                }
            }
        }

        private bool IsValidPuzzle = true;
        private bool IsPuzzleLoaded = false;
        private int SelectedX;
        private int SelectedY;

        public SudokuGridViewModel()
        {
            Elements = new ObservableCollection<ObservableCollection<ElementViewModel>>();
            AllowedElementsColors = new ObservableCollection<string>();

            Grid = new SudokuGrid();
            Reset();
        }

        public void Reset()
        {
            OpenPuzzle(null);
        }

        public void Solve()
        {
            Grid.Solve();
            var allElementViewModels = Elements.SelectMany(e => e);
            foreach (var elementViewModel in allElementViewModels)
            {
                var element = Grid.GetElement(elementViewModel.X, elementViewModel.Y);
                elementViewModel.Value = element.Value == 'X' ? "" : element.Value + "";
            }
        }

        public void OpenPuzzle(string data)
        {
            IsValidPuzzle = Grid.OpenPuzzle(data);

            IsPuzzleLoaded = false;

            Status = IsValidPuzzle ? "In progress." : "Invalid puzzle!";
            ValidNumberVisibility = "Hidden";

            Initialize();

            Status = Grid.IsSolved() ? "Solved!" : Status;

            IsPuzzleLoaded = true;
            if (IsValidPuzzle)
            {
                SelectedX = -1;
                SelectedY = -1;
                UpdateElementColors();
            }
        }

        private void Initialize()
        {
            for (int y = 0; y < 9; y++)
            {
                if (Elements.Count < 9)
                {
                    Elements.Add(new ObservableCollection<ElementViewModel>());
                    Elements[y].CollectionChanged += Elements_CollectionChanged;
                    AllowedElementsColors.Add("White");
                }

                AllowedElementsColors[y] = "White";
                for (int x = 0; x < 9; x++)
                {
                    SudokuElement element = Grid.GetElement(x, y);
                    string value = element.Value.ToString().Equals("X") ? "" : element.Value.ToString();
                    string color = "White";

                    bool locked = !IsValidPuzzle;

                    if (value.Length > 0)
                    {
                        locked = true;
                        color = element.Valid ? "LightGray" : "Salmon";
                    }

                    if (Elements[y].Count < 9)
                    {
                        Elements[y].Add(new ElementViewModel(value, x, y, color));
                        Elements[y][x].PropertyChanged += ElementViewModel_PropertyChanged;
                    }

                    ElementViewModel elementViewModel = Elements[y][x];
                    elementViewModel.Value = value;
                    elementViewModel.Color = color;
                    elementViewModel.Locked = locked;
                }
            }


        }

        private void Elements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ElementViewModel element = ((ObservableCollection<ElementViewModel>)sender)[e.NewStartingIndex];
            int x = element.X;
            int y = element.Y;
            string text = element.Value;

            SetElement(x, y, text);
        }

        private void ElementViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ElementViewModel element = (ElementViewModel)sender;
            int x = element.X;
            int y = element.Y;
            string text = element.Value;
            
            if(!element.Locked)
                SetElement(x, y, text);

            if (IsValidPuzzle)
            {
                if (IsPuzzleLoaded)
                {
                    UpdateElementColors();
                    UpdateAllowedNumbers(x, y);

                    if (Grid.IsSolved())
                        Status = "Solved!";
                    else
                        Status = "In progress.";
                }
            }
        }

        public bool IsElementValid(int x, int y, ElementViewModel element)
        {
            return Grid.IsElementValid(x, y, element.AsChar());
        }

        public void SelectElement(int x, int y)
        {
            if (IsValidPuzzle)
            {
                ValidNumberVisibility = "Visible";

                SelectedX = x;
                SelectedY = y;

                UpdateElementColors();
                UpdateAllowedNumbers(x, y);
            }
        }

        public void UpdateElementColors()
        {
            for(int y = 0; y < Elements.Count; y++)
            {
                for(int x = 0; x < Elements[y].Count; x++)
                {
                    ElementViewModel element = Elements[y][x];
                    bool isAllowed = IsElementValid(x, y, element);

                    string color;
                    if ((x == SelectedX || y == SelectedY) && isAllowed)
                    {
                        color = "Yellow";
                    }
                    else
                    {
                        if (!element.Locked)
                        {
                            color = isAllowed ? "White" : "Salmon";
                        }
                        else
                        {
                            color = "LightGray";
                        }
                    }
                    element.Color = color;
                }
            }
        }

        public void UpdateAllowedNumbers(int x, int y)
        {
            for (int i = 0; i < 9; i++)
            {
                char c = (i + 1).ToString()[0];
                char[] column = Grid.GetColumn(x);
                char[] row = Grid.GetRow(y);
                char[] region = Grid.GetRegion(x, y);

                bool valid = !column.Contains(c) &&
                                !row.Contains(c) &&
                                !region.Contains(c);

                char value = Elements[y][x].AsChar();

                string color;
                if (valid)
                {
                    color = "White";
                }
                else if(value.Equals(c))
                {
                    color = "Salmon";
                }
                else
                {
                    color = "Gray";
                }
                AllowedElementsColors[i] = color;
            }
        }

        public void SetElement(int x, int y, string s)
        {
            char c = s.Length > 0 ? s[0] : 'X';
            
            Grid.SetElement(x, y, c);
        }

        public override string ToString()
        {
            return Grid.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
