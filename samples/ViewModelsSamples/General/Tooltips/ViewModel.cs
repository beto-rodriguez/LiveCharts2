using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.Tooltips;

public class ViewModel : INotifyPropertyChanged
{
    private TooltipPosition _position;
    private AvailablePositions _selectedPosition;

    public ViewModel()
    {
        _selectedPosition = Positions[0];
    }

    public IEnumerable<ISeries> Series { get; set; }
        = new ObservableCollection<ISeries>
        {
            new ColumnSeries<double>
            {
                Values = new ObservableCollection<double> { 3, 7, 3, 1, 4, 5, 6 },
                Name = "Sales"
            },
            new LineSeries<double>
            {
                Values = new ObservableCollection<double> { 2, 1, 3, 5, 3, 4, 6 },
                Fill = null,
                Name = "Customers"
            }
        };

    public List<AvailablePositions> Positions => new()
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
        get => _selectedPosition;
        set
        {
            _selectedPosition = value;
            OnPropertyChanged();

            // Workaroud for Avalonia, DisplayMemberPath is not supported
            // https://github.com/AvaloniaUI/Avalonia/issues/4718
            Position = _selectedPosition.Position;
        }
    }

    public TooltipPosition Position { get => _position; set { _position = value; OnPropertyChanged(); } }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
