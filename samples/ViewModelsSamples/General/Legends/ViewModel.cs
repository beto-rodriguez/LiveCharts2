using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewModelsSamples.General.Legends
{
    public class ViewModel : INotifyPropertyChanged
    {
        private LegendPosition position;
        private AvailablePositions selectedPosition;

        public IEnumerable<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
        {
            new ColumnSeries<double>
            {
                Values = new ObservableCollection<double> { 3, 7, 3,},
            },
            new ColumnSeries<double>
            {
                Values = new ObservableCollection<double> { 5, 3, 2 },
            },
            new ColumnSeries<double>
            {
                Values = new ObservableCollection<double> { 8, 2, 4 },
            }
        };

        public List<AvailablePositions> Positions => new List<AvailablePositions>
        {
            new AvailablePositions("hidden", LegendPosition.Hidden),
            new AvailablePositions("top", LegendPosition.Top),
            new AvailablePositions("bottom", LegendPosition.Bottom),
            new AvailablePositions("right", LegendPosition.Right),
            new AvailablePositions("left", LegendPosition.Left)
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

        public LegendPosition Position { get => position; set { position = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class AvailablePositions
    {
        public AvailablePositions(string name, LegendPosition position)
        {
            Name = name;
            Position = position;
        }

        public string Name { get; set; }
        public LegendPosition Position { get; set; }
    }
}
