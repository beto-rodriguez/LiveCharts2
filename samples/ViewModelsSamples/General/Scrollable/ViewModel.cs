using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;

namespace ViewModelsSamples.General.Scrollable;

public partial class ViewModel : ObservableObject
{
    private bool _isDown = false;

    public ObservablePoint[] Values { get; } = Fetch();

    [ObservableProperty]
    public partial double? MinX { get; set; }

    [ObservableProperty]
    public partial double? MaxX { get; set; }

    [ObservableProperty]
    public partial double? MinXThumb { get; set; } = 0;

    [ObservableProperty]
    public partial double? MaxXThumb { get; set; } = 100;

    [RelayCommand]
    public void ChartUpdated(ChartCommandArgs args)
    {
        var cartesianChart = (CartesianChartEngine)args.Chart.CoreChart;
        var x = cartesianChart.XAxes.First();

        // when the main chart is updated, we need to update the scroll bar thumb limits
        // this will sync the scroll bar with the main chart when the user is zooming or panning

        MinXThumb = x.MinLimit;
        MaxXThumb = x.MaxLimit;
    }

    [RelayCommand]
    public void PointerDown(PointerCommandArgs args) =>
        _isDown = true;

    [RelayCommand]
    public void PointerMove(PointerCommandArgs args)
    {
        if (!_isDown) return;

        var chart = (ICartesianChartView)args.Chart;
        var positionInData = chart.ScalePixelsToData(args.PointerPosition);

        if (MaxXThumb is null || MinXThumb is null)
        {
            // if the limits are not defined yet, we skip the update
            return;
        }

        var currentRange = MaxXThumb - MinXThumb;

        var min = positionInData.X - currentRange / 2;
        var max = positionInData.X + currentRange / 2;

        // optional, use the data bounds as limits for the thumb
        if (min < Values[0].X)
        {
            min = Values[0].X;
            max = min + currentRange;
        }
        if (max > Values[^1].X)
        {
            max = Values[^1].X;
            min = max - currentRange;
        }

        // update the scroll bar thumb when the user is dragging the chart
        MinXThumb = min;
        MaxXThumb = max;

        // update the chart visible range
        MinX = min;
        MaxX = max;
    }

    [RelayCommand]
    public void PointerUp(PointerCommandArgs args) =>
        _isDown = false;

    private static ObservablePoint[] Fetch()
    {
        var trend = 1000;
        var r = new Random();

        var values = new ObservablePoint[500];

        for (var i = 0; i < 500; i++)
            values[i] = new ObservablePoint(i, trend += r.Next(-20, 20));

        return values;
    }
}
