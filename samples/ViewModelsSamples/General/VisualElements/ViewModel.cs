using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;

namespace ViewModelsSamples.General.VisualElements;

public partial class ViewModel
{
    public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements { get; set; } = new List<ChartElement<SkiaSharpDrawingContext>>
    {
         new RectangleVisualElement
         {
             X = 2.5,
             Y = 3.5,
             LocationUnit = MeasureUnit.ChartValues,
             Width = 4,
             Height = 2,
             SizeUnit = MeasureUnit.ChartValues,
             Fill = new SolidColorPaint(new SKColor(239, 83, 80, 120)) { ZIndex = 10 },
             Stroke = new SolidColorPaint(new SKColor(239, 83, 80)) { ZIndex = 10, StrokeThickness = 1.5f },
         }
    };

    public ISeries[] Series { get; set; } =
    {
        new LineSeries<int>
        {
            GeometrySize = 13,
            Values = new int[] { 1,2,3,4,2,1,3,6,3,5,2,1,4,5,2,3,1,4,5,3,1,6,2,4,5,8,4,5,6,4,7,5,8,4,6,5,4,7,8,9,9,8,7,9,8,7,9,9,8,6,8 }
        }
    };
}
