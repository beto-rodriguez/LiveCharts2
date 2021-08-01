using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewModelsSamples.General.Tooltips
{
    public class ViewModel : INotifyPropertyChanged
    {
        private TooltipPosition position;
        private AvailablePositions selectedPosition;

        public IEnumerable<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
        {
            new ColumnSeries<double>
            {
                Values = new ObservableCollection<double> { 3, 7, 3, 1, 4, 5, 6 },
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double> { 2, 1, 3, 5, 3, 4, 6 },
                Fill = null
            }
        };

        public List<AvailablePositions> Positions => new List<AvailablePositions>
        {
            new AvailablePositions("hidden", TooltipPosition.Hidden),
            new AvailablePositions("top", TooltipPosition.Top),
            new AvailablePositions("bottom", TooltipPosition.Bottom),
            new AvailablePositions("right", TooltipPosition.Right),
            new AvailablePositions("left", TooltipPosition.Left),
            new AvailablePositions("center", TooltipPosition.Center),
        };

        public AvailablePositions SelectedPosition
        {
            get => selectedPosition;
            set
            {
                selectedPosition = value;
                OnPropertyChanged();

                // Workaroud for Avalonia, DisplayMemberPath is not supported
                // https://github.com/AvaloniaUI/Avalonia/issues/4718
                Position = selectedPosition.Position;
            }
        }

        public TooltipPosition Position { get => position; set { position = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class AvailablePositions
    {
        public AvailablePositions(string name, TooltipPosition position)
        {
            Name = name;
            Position = position;
        }

        public string Name { get; set; }
        public TooltipPosition Position { get; set; }
    }
}
