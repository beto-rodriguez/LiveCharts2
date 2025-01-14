using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Events.Tutorial;

public partial class ViewModel
{
    private readonly HashSet<ChartPoint> _activePoints = [];

    public FindingStrategy Strategy { get; } = FindingStrategy.ExactMatch;

    public ISeries[] SeriesCollection { get; set; } = [
        new ColumnSeries<int>([1, 5, 4, 3])
        {
            Stroke = new SolidColorPaint { Color = SKColors.Transparent }
        },
        new ColumnSeries<int>([3, 2, 6, 2])
        {
            Stroke = new SolidColorPaint { Color = SKColors.Transparent }
        }
    ];

    [RelayCommand]
    public void OnPressed(PointerCommandArgs args)
    {
        var foundPoints = args.Chart.GetPointsAt(args.PointerPosition);

        foreach (var point in foundPoints)
        {
            var geometry = (DrawnGeometry)point.Context.Visual!;

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

    [RelayCommand]
    public void OnHoveredPointsChanged(HoverCommandArgs args)
    {
        // the NewPoints contains the new hovered points // mark
        foreach (var hovered in args.NewPoints ?? [])
        {
            // in this case, we will set a black stroke on the drawn gemetry. // mark
            var geometry = (DrawnGeometry)hovered.Context.Visual!;
            geometry.Stroke = new SolidColorPaint(SKColors.Black, 3);
        }

        // the OldPoints contains the points that are not hovered anymore // mark
        foreach (var hovered in args.OldPoints ?? [])
        {
            // now, we will clear the stroke. // mark
            var geometry = (DrawnGeometry)hovered.Context.Visual!;
            geometry.Stroke = null;
        }

        Trace.WriteLine(
            $"hovered, {args.NewPoints?.Count()} added, {args.OldPoints?.Count()} removed");
    }
}
