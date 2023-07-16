using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace ViewModelsSamples.Events.AddPointOnClick;

public partial class ViewModel : ObservableObject
{
    public ISeries[] SeriesCollection { get; set; } =
        new ISeries[]
        {
            new LineSeries<ObservablePoint>
            {
                Values = new ObservableCollection<ObservablePoint>
                {
                    new(0, 5),
                    new(3, 8),
                    new(7, 9)
                },
                Fill = null,
                DataPadding = new LiveChartsCore.Drawing.LvcPoint(5, 5)
            }
        };

    [RelayCommand]
    public void PointerDown(PointerCommandArgs args)
    {
        var chart = (ICartesianChartView<SkiaSharpDrawingContext>)args.Chart;
        var values = (ObservableCollection<ObservablePoint>)SeriesCollection[0].Values!;

        // scales the UI coordinates to the corresponding data in the chart.
        var scaledPoint = chart.ScalePixelsToData(args.PointerPosition);

        // finally add the new point to the data in our chart.
        values.Add(new ObservablePoint(scaledPoint.X, scaledPoint.Y));
    }
}
