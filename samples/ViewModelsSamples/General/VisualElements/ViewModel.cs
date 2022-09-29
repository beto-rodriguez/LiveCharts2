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
         new GeometryVisual<RectangleGeometry>
         {
             X = 2.5,
             Y = 3.5,
             LocationUnit = MeasureUnit.ChartValues,
             Width = 4,
             Height = 2,
             SizeUnit = MeasureUnit.ChartValues,
             Fill = new SolidColorPaint(new SKColor(239, 83, 80, 50)) { ZIndex = 10 },
             Stroke = new SolidColorPaint(new SKColor(239, 83, 80)) { ZIndex = 10, StrokeThickness = 1.5f },
         },
         new GeometryVisual<OvalGeometry>
         {
             X = 5.5,
             Y = 6,
             LocationUnit = MeasureUnit.ChartValues,
             Width = 4,
             Height = 5,
             SizeUnit = MeasureUnit.ChartValues,
             Fill = new SolidColorPaint(new SKColor(100, 221, 23, 50)) { ZIndex = - 10 },
             Stroke = new SolidColorPaint(new SKColor(100, 221, 23)) { ZIndex = -10, StrokeThickness = 1.5f },
         },
         new GeometryVisual<MyGeometry>
         {
             X = 18,
             Y = 6,
             LocationUnit = MeasureUnit.ChartValues,
             Width = 100,
             Height = 100,
             SizeUnit = MeasureUnit.Pixels,
             Fill = new SolidColorPaint(new SKColor(251, 192, 45, 50)) { ZIndex = 10 },
             Stroke = new SolidColorPaint(new SKColor(251, 192, 45)) { ZIndex = 10, StrokeThickness = 1.5f },
         },
         new LabelVisual
         {
             Text = "What happened here?",
             X = 11,
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
