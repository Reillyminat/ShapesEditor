using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows.Media;

namespace ShapesEditor
{
    public class ShapeVM : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public PointCollection Points { get; set; }
        public SolidColorBrush Fill
        {
            get { return _fill; }
            set
            {
                _fill = value;
                OnPropertyChanged(nameof(Fill));
            }
        }
        public int CanvasLeftOffset
        {
            get { return _canvasLeftOffset; }
            set
            {
                _canvasLeftOffset = value;
                OnPropertyChanged(nameof(CanvasLeftOffset));
            }
        }
        public int CanvasTopOffset
        {
            get { return _canvasTopOffset; }
            set
            {
                _canvasTopOffset = value;
                OnPropertyChanged(nameof(CanvasTopOffset));
            }
        }
        public int SelectedColorIndex
        {
            get { return _selectedColorIndex; }
            set
            {
                _selectedColorIndex = value;
                OnPropertyChanged(nameof(SelectedColorIndex));
            }
        }
        [JsonIgnore]
        public SolidColorBrush Stroke { get; set; } = Brushes.Black;
        [JsonIgnore]
        public int StrokeThickness
        {
            get { return _strokeThickness; }
            set
            {
                _strokeThickness = value;
                OnPropertyChanged(nameof(StrokeThickness));
            }
        }
        [JsonIgnore]
        public float Opacity
        {
            get { return _opacity; }
            set
            {
                _opacity = value;
                OnPropertyChanged(nameof(Opacity));
            }
        }

        private int _strokeThickness = 0;
        private SolidColorBrush _fill;
        private float _opacity = 1;
        private int _canvasLeftOffset;
        private int _canvasTopOffset;
        private int _selectedColorIndex;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
