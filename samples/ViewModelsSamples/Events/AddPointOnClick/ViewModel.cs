using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Events.AddPointOnClick;

public partial class ViewModel
{
    public ObservableCollection<ObservablePoint> Points { get; set; }

    public ISeries[] SeriesCollection { get; set; }

    public ViewModel()
    {
        Points = [
            new(0, 5),
            new(3, 8),
            new(7, 9)
        ];

        SeriesCollection = [
            new LineSeries<ObservablePoint>
            {
                Values = Points,
                Fill = null,
                DataPadding = new LiveChartsCore.Drawing.LvcPoint(5, 5)
            }
        ];
    }

    [RelayCommand]
    public void PointerDown(PointerCommandArgs args)
    {
        var chart = (ICartesianChartView)args.Chart;

        // scales the UI coordinates to the corresponding data in the chart.
        var scaledPoint = chart.ScalePixelsToData(args.PointerPosition);

        // finally add the new point to the data in our chart.
        Points.Add(new ObservablePoint(scaledPoint.X, scaledPoint.Y));
    }
}
