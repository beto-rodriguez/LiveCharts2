using System.Collections.Generic;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Events.Tutorial;

public partial class ViewModel
{
    private readonly HashSet<ChartPoint> _activePoints = [];

    public FindingStrategy Strategy { get; } = FindingStrategy.Automatic;

    public ISeries[] SeriesCollection { get; set; } = [
        new ColumnSeries<int>([1, 5, 4, 3]),
        new ColumnSeries<int>([3, 2, 6, 2])
    ];

    [RelayCommand]
    public void OnPressed(PointerCommandArgs args)
    {
        var foundPoints = args.Chart.GetPointsAt(args.PointerPosition);

        foreach (var point in foundPoints)
        {
            var geometry = (Geometry)point.Context.Visual!;

            if (!_activePoints.Contains(point))
            {
                geometry.Fill = new SolidColorPaint { Color = SKColors.Yellow };
                _activePoints.Add(point);
            }
            else
            {
                // clear the fill to the default value
                geometry.Fill = null;
                _activePoints.Remove(point);
            }

            Trace.WriteLine($"found {point.Context.DataSource}");
        }
    }
}
