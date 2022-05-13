using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.Legends;

public class ViewModel : INotifyPropertyChanged
{
    private LegendPosition _position;
    private AvailablePosition _selectedPosition;

    public ViewModel()
    {
        _selectedPosition = Positions[0];
    }

    public IEnumerable<ISeries> Series { get; set; } = new ObservableCollection<ISeries>
    {
        new ColumnSeries<double>
        {
            Name = "Peru",
            Values = new ObservableCollection<double> { 3, 7, 3,},
        },
        new ColumnSeries<double>
        {
            Name = "Egypt",
            Values = new ObservableCollection<double> { 5, 3, 2 },
        },
        new ColumnSeries<double>
        {
            Name = "Portugal",
            Values = new ObservableCollection<double> { 8, 2, 4 },
        }
    };

    public List<AvailablePosition> Positions => new()
    {
        new AvailablePosition("hidden", LegendPosition.Hidden),
        new AvailablePosition("top", LegendPosition.Top),
        new AvailablePosition("bottom", LegendPosition.Bottom),
        new AvailablePosition("right", LegendPosition.Right),
        new AvailablePosition("left", LegendPosition.Left)
    };

    public AvailablePosition SelectedPosition
    {
        get => _selectedPosition;
        set
        {
            _selectedPosition = value;
            OnPropertyChanged();
            Position = _selectedPosition.Position;
        }
    }

    public LegendPosition Position { get => _position; set { _position = value; OnPropertyChanged(); } }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
