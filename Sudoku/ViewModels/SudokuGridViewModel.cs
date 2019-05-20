using Sudoku.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Sudoku.ViewModels
{
    public class SudokuGridViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ObservableCollection<ElementViewModel>> Elements { get; }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        private string _validNumberVisibility;
        public string ValidNumberVisibility
        {
            get => _validNumberVisibility;
            set
            {
                if(_validNumberVisibility != value)
                {
                    _validNumberVisibility = value;
                    OnPropertyChanged(nameof(ValidNumberVisibility));
                }
            }
        }

        private const string ValidColor = "White";
        private const string InvalidColor = "Salmon";
        private const string SelectedColor = "Yellow";
        private const string HighlightedColor = "Gray";
        private const string LockedColor = "LightGray";
        
        public ObservableCollection<string> AllowedElementsColors { get; }

        private bool _isValidPuzzle = true;
        private bool _isPuzzleLoaded = false;
        private int _selectedX = -1;
        private int _selectedY = -1;
        private int _selectedRegion = -1;
        
        private readonly SudokuGrid _grid;

        public SudokuGridViewModel()
        {
            Elements = new ObservableCollection<ObservableCollection<ElementViewModel>>();
            AllowedElementsColors = new ObservableCollection<string>();
            _grid = new SudokuGrid();
            
            for (int y = 0; y < 9; y++)
            {
                AllowedElementsColors.Add(ValidColor);
                
                Elements.Add(new ObservableCollection<ElementViewModel>());
                Elements[y].CollectionChanged += Elements_CollectionChanged;
                for (int x = 0; x < 9; x++)
                {
                    var region = SudokuGrid.GetRegionNumber(x, y);
                    Elements[y].Add(new ElementViewModel(SudokuGrid.EmptyValue, x, y, region, ValidColor));
                    Elements[y][x].PropertyChanged += ElementViewModel_PropertyChanged;
                }
            }

            Reset();
        }

        public void Reset()
        {
            OpenPuzzle(null);
        }

        public void Solve()
        {
            var success = _grid.Solve();
            if (!success)
                Status = "Invalid puzzle.";
            
            var allElementViewModels = Elements.SelectMany(e => e);
            foreach (var elementViewModel in allElementViewModels)
            {
                var element = _grid.GetElement(elementViewModel.X, elementViewModel.Y);
                elementViewModel.Value = element.Value == 'X' ? "" : element.Value + "";
            }
        }

        public void OpenPuzzle(string data)
        {
            _isValidPuzzle = _grid.OpenPuzzle(data);

            _isPuzzleLoaded = false;

            Status = _isValidPuzzle ? "In progress." : "Invalid puzzle!";
            ValidNumberVisibility = "Hidden";

            Initialize();

            Status = _grid.IsSolved() ? "Solved!" : Status;

            _isPuzzleLoaded = true;
            if (_isValidPuzzle)
            {
                _selectedX = -1;
                _selectedY = -1;
                _selectedRegion = -1;
                UpdateElementColors();
            }
        }

        private void Initialize()
        {
            for (int y = 0; y < 9; y++)
            {
                AllowedElementsColors[y] = ValidColor;
                for (int x = 0; x < 9; x++)
                {
                    SudokuElement element = _grid.GetElement(x, y);
                    string value = element.Value.Equals(SudokuGrid.EmptyValue) ? "" : element.Value.ToString();
                    string color = ValidColor;

                    bool locked = !_isValidPuzzle;

                    if (value.Length > 0)
                    {
                        locked = true;
                        color = element.IsValid ? LockedColor : InvalidColor;
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
            var element = (ElementViewModel)sender;
            if (e.PropertyName.Equals(nameof(element.Color)))
                return;

            if (e.PropertyName.Equals(nameof(element.Locked)))
                return;
            
            var x = element.X;
            var y = element.Y;
            var text = element.Value;
            
            if (!element.Locked)
                SetElement(x, y, text);

            if (!_isValidPuzzle)
                return;

            if (!_isPuzzleLoaded)
                return;
            
            UpdateElementColors();
            UpdateAllowedNumbers(x, y);

            Status = _grid.IsSolved() ? "Solved!" : "In progress.";
        }

        private bool IsElementValid(ElementViewModel element)
        {
            return _grid.IsValuePossible(element.AsChar(), element.X, element.Y);
        }

        public void SelectElement(int x, int y, int region)
        {
            if (_isValidPuzzle)
            {
                ValidNumberVisibility = "Visible";

                _selectedX = x;
                _selectedY = y;
                _selectedRegion = region;

                UpdateElementColors();
                UpdateAllowedNumbers(x, y);
            }
        }

        private void UpdateElementColors()
        {
            for(int y = 0; y < Elements.Count; y++)
            {
                for(int x = 0; x < Elements[y].Count; x++)
                {
                    var element = Elements[y][x];
                    
                    var isAllowed = IsElementValid(element);
                    string color;
                    if (element.Locked)
                    {
                        color = LockedColor;
                    }
                    else if (!isAllowed)
                    {
                        color = InvalidColor;
                    }
                    else if (x == _selectedX && y == _selectedY)
                    {
                        color = SelectedColor;
                    }
                    else if (x == _selectedX || y == _selectedY || element.Region == _selectedRegion)
                    {
                        color = HighlightedColor;
                    }
                    else
                    {
                        color = ValidColor;
                    }
                    element.Color = color;
                }
            }
        }

        private void UpdateAllowedNumbers(int x, int y)
        {
            for (int i = 0; i < 9; i++)
            {
                var num = (i + 1).ToString()[0];
                var column = _grid.GetColumn(x);
                var row = _grid.GetRow(y);
                var region = _grid.GetRegion(x, y);

                var valid = !column.Contains(num) &&
                            !row.Contains(num) &&
                            !region.Contains(num);

                var value = Elements[y][x].AsChar();

                string color;
                if (valid)
                {
                    color = ValidColor;
                }
                else if(value.Equals(num))
                {
                    color = InvalidColor;
                }
                else
                {
                    color = HighlightedColor;
                }
                AllowedElementsColors[i] = color;
            }
        }

        private void SetElement(int x, int y, string value)
        {
            var valueAsChar = value.Length > 0 ? value[0] : SudokuGrid.EmptyValue;
            _grid.SetElement(x, y, valueAsChar);
        }

        public override string ToString()
        {
            return _grid.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
