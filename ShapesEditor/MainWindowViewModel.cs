using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ShapesEditor
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Point _objectStartLocation;
        private Point _mouseStartLocation;
        private bool _shapeCaptured = false;
        private bool _isPanning = false;
        private bool _isAllowedPanning = false;
        private readonly ISessionManager _sessionManager;
        private ICommand _shapeClick;
        private double _panelX = 100;
        private double _panelY = 100;
        private Brush _selectedBackground = Brushes.White;
        private ShapeVM _selectedShape;
        private readonly decimal _defaultScaleFactor = 1;
        private decimal _scaleFactor = 1;
        private double _borderWidth = 110;
        private double _borderHeight = 110;
        private double _borderLeftOffset = 0;
        private double _borderTopOffset = 0;
        private bool _borderVisible = false;
        private static readonly double DragThreshold = 5;
        private bool _isMouseLeftButtonDown = false;
        private double _scrollVertical;
        private double _scrollHorizontal;

        public MainWindowViewModel(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public ObservableCollection<ShapeVM> Shapes { get; set; } = new ObservableCollection<ShapeVM>();

        public ObservableCollection<ShapeVM> SelectedShapes { get; set; } = new ObservableCollection<ShapeVM>();

        public decimal ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                if (value.Equals(_scaleFactor)) return;
                _scaleFactor = value;
                OnPropertyChanged(nameof(ScaleFactor));
            }
        }

        public Brush SelectedBackground
        {
            get { return _selectedBackground; }
            set
            {
                if (value.Equals(_selectedBackground)) return;
                _selectedBackground = value;
                OnPropertyChanged(nameof(SelectedBackground));
            }
        }

        public ShapeVM SelectedShapeFromList
        {
            get { return _selectedShape; }
            set
            {
                _selectedShape = value;
                OnPropertyChanged(nameof(SelectedShapeFromList));
            }
        }

        public ObservableCollection<string> BackgroundsNames { get; set; } = new ObservableCollection<string> { "White", "Black", "Green", "Yellow", "Red", "Blue", "Cyan" };

        public ICommand PreviewMouseMove => new RelayCommand(_ =>
        {
            if (_isMouseLeftButtonDown)
            {
                if (_isPanning)
                {
                    ScrollHorizontal += 1;
                    ScrollVertical += 1;
                }
                else
                {
                    var dragDelta = _mouseStartLocation - new Point(PanelX, PanelY);
                    if (Math.Abs(dragDelta.Length) > DragThreshold)
                    {
                        UpdateDragSelectionRect(_mouseStartLocation.X, _mouseStartLocation.Y, PanelX, PanelY);
                        BorderVisible = true;
                        OnPropertyChanged(nameof(BorderVisible));
                    }
                }
            }
            if (_shapeCaptured)
            {
                foreach (var shape in SelectedShapes)
                {
                    shape.CanvasLeftOffset = (int)(PanelX - _objectStartLocation.X);
                    shape.CanvasTopOffset = (int)(PanelY - _objectStartLocation.Y);
                }
                OnPropertyChanged(nameof(Shapes));
            }
        });

        public ICommand ShapeClick
        {
            get
            {
                return _shapeClick ?? (_shapeClick = new RelayCommand(SelectedShape =>
                {
                    ShapeVM shape = default;
                    switch (SelectedShape)
                    {
                        case "Rectangle":
                            shape = new ShapeVM() { Name = "Rectangle", Points = new PointCollection { new Point { X = 5, Y = 150 }, new Point { X = 5, Y = 0 }, new Point { X = 150, Y = 0 }, new Point { X = 150, Y = 150 } }, Fill = Brushes.Blue, SelectedColorIndex = 2 };
                            break;
                        case "Star":
                            shape = new ShapeVM() { Name = "Star", Points = new PointCollection { new Point { X = 110, Y = 50 }, new Point { X = 60, Y = 20 }, new Point { X = 10, Y = 50 }, new Point { X = 20, Y = 110 }, new Point { X = 100, Y = 110 } }, Fill = Brushes.Yellow, SelectedColorIndex = 3 };
                            break;
                        case "Triangle":
                            shape = new ShapeVM() { Name = "Triangle", Points = new PointCollection { new Point { X = 50, Y = 50 }, new Point { X = 0, Y = 30 }, new Point { X = 150, Y = 0 } }, Fill = Brushes.Red, SelectedColorIndex = 4 };
                            break;
                    }
                    Shapes.Add(shape);
                }));
            }
        }

        public ICommand ListElementClick => new RelayCommand(_ =>
        {
            var t = 0;
        });

        public ICommand SaveCommand => new RelayCommand(_ =>
        {
            _sessionManager.SaveSession(Shapes, SelectedBackground);
        });

        public ICommand LoadCommand => new RelayCommand(_ =>
        {
            var session = _sessionManager.RestoreLastSession();
            Shapes = session.Item1;
            SelectedBackground = session.Item2;
            OnPropertyChanged(nameof(Shapes));
        });

        public ICommand PreviewMouseLeftButtonUp => new RelayCommand(_ =>
        {
            BorderVisible = false;
            _isMouseLeftButtonDown = false;

            if (IsAllowedPanning)
                _isPanning = false;

            foreach (var shape in SelectedShapes)
            {
                shape.StrokeThickness = 0;
                shape.Opacity = 1;
            }
            SelectedShapes.Clear();
            OnPropertyChanged(nameof(BorderVisible));
        });
        public ICommand PreviewMouseLeftButtonDown => new RelayCommand(param =>
        {
            if (IsAllowedPanning)
                _isPanning = true;
            _mouseStartLocation.X = PanelX;
            _mouseStartLocation.Y = PanelY;
            _isMouseLeftButtonDown = true;
        });
        public ICommand PreviewMouseWheel => new RelayCommand(param =>
        {
            ShapeVM shape = param as ShapeVM;
            Shapes.Remove(shape);
        });
        public ICommand MouseLeave => new RelayCommand(param =>
        {
            _isMouseLeftButtonDown = false;
            BorderVisible = false;
            OnPropertyChanged(nameof(BorderVisible));
        });

        public ICommand PreviewMouseLeftButtonDownOnShape => new RelayCommand(param =>
        {
            _isMouseLeftButtonDown = false;
            var shape = param as ShapeVM;
            if (SelectedShapes.Count == 0 || SelectedShapes.Contains(shape))
            {
                _objectStartLocation.X = PanelX - shape.CanvasLeftOffset;
                _objectStartLocation.Y = PanelY - shape.CanvasTopOffset;
                _shapeCaptured = true;
            }
        });

        public ICommand PreviewMouseLeftButtonUpOnShape => new RelayCommand(param =>
        {
            if (_shapeCaptured)
            {
                var shape = param as ShapeVM;
                SelectedShapes.Add(shape);
                _shapeCaptured = false;
                SelectedShapes[SelectedShapes.Count - 1].StrokeThickness = 2;
                SelectedShapes[SelectedShapes.Count - 1].Opacity = 0.75f;
            }
        });

        public ICommand ResetZoomCommand => new RelayCommand(_ => ScaleFactor = _defaultScaleFactor);

        public bool IsAllowedPanning
        {
            get { return _isAllowedPanning; }
            set
            {
                if (value.Equals(_isAllowedPanning)) return;
                _isAllowedPanning = value;
                OnPropertyChanged(nameof(IsAllowedPanning));
            }
        }
        public double PanelX
        {
            get { return _panelX; }
            set
            {
                if (value.Equals(_panelX)) return;
                _panelX = value;
                OnPropertyChanged(nameof(PanelX));
            }
        }
        public double PanelY
        {
            get { return _panelY; }
            set
            {
                if (value.Equals(_panelY)) return;
                _panelY = value;
                OnPropertyChanged(nameof(PanelY));
            }
        }
        public double ScrollVertical
        {
            get { return _scrollVertical; }
            set
            {
                if (value.Equals(_scrollVertical)) return;
                _scrollVertical = value;
                OnPropertyChanged(nameof(ScrollVertical));
            }
        }
        public double ScrollHorizontal
        {
            get { return _scrollHorizontal; }
            set
            {
                if (value.Equals(_scrollHorizontal)) return;
                _scrollHorizontal = value;
                OnPropertyChanged(nameof(ScrollHorizontal));
            }
        }
        public double BorderWidth
        {
            get { return _borderWidth; }
            set
            {
                if (value.Equals(_borderWidth)) return;
                _borderWidth = value;
                OnPropertyChanged(nameof(BorderWidth));
            }
        }

        public double BorderHeight
        {
            get { return _borderHeight; }
            set
            {
                if (value.Equals(_borderHeight)) return;
                _borderHeight = value;
                OnPropertyChanged(nameof(BorderHeight));
            }
        }

        public double BorderLeftOffset
        {
            get { return _borderLeftOffset; }
            set
            {
                if (value.Equals(_borderLeftOffset)) return;
                _borderLeftOffset = value;
                OnPropertyChanged(nameof(BorderLeftOffset));
            }
        }

        public double BorderTopOffset
        {
            get { return _borderTopOffset; }
            set
            {
                if (value.Equals(_borderTopOffset)) return;
                _borderTopOffset = value;
                OnPropertyChanged(nameof(BorderTopOffset));
            }
        }

        public bool BorderVisible
        {
            get { return _borderVisible; }
            set
            {
                if (value.Equals(_borderVisible)) return;
                _borderVisible = value;
                OnPropertyChanged(nameof(BorderVisible));
            }
        }
        private void UpdateDragSelectionRect(double x1, double y1, double x2, double y2)
        {
            double x, y, width, height;

            if (x2 < x1)
            {
                x = x2;
                width = x1 - x2;
            }
            else
            {
                x = x1;
                width = x2 - x1;
            }

            if (y2 < y1)
            {
                y = y2;
                height = y1 - y2;
            }
            else
            {
                y = y1;
                height = y2 - y1;
            }

            BorderLeftOffset = x;
            BorderTopOffset = y;
            BorderWidth = width;
            BorderHeight = height;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
