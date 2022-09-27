using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;

namespace ViewModelsSamples.General.VisualElements;

[ObservableObject]
public partial class ViewModel
{
    public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements { get; set; } = new List<ChartElement<SkiaSharpDrawingContext>>
    {
         new LabelVisual
         {
             Text = "What happened here?",
             X = new DateTime(2022, 1, 2).Ticks,
             Y = 1,
             TextSize = 16,
             Paint = new SolidColorPaint(new SKColor(250, 250, 250)) { ZIndex = 11 },
             BackgroundColor = new LvcColor(55, 71, 79),
             Padding = new Padding(12),
             LocationUnit = MeasureUnit.ChartValues,
             Translate = new LvcPoint(0, -35)
         }
    };

    public ISeries[] Series { get; set; } =
    {
        new LineSeries<DateTimePoint>
        {
            GeometrySize = 13,
            Values = new DateTimePoint[]
            {
                new(new DateTime(2022, 1, 1), 1),
                new(new DateTime(2022, 1, 2), 2),
                new(new DateTime(2022, 1, 3), 1),
                new(new DateTime(2022, 1, 4), 2)
            }
        }
    };
}
