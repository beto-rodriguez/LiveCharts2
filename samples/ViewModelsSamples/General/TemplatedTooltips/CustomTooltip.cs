using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace ViewModelsSamples.General.TemplatedTooltips;

public class CustomTooltip : IChartTooltip<SkiaSharpDrawingContext>, IImageControl
{
    private StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>? _stackPanel;
    private static readonly int s_zIndex = 10050;
    private readonly SolidColorPaint _backgroundPaint = new(new SKColor(28, 49, 58)) { ZIndex = s_zIndex };
    private readonly SolidColorPaint _fontPaint = new(new SKColor(230, 230, 230)) { ZIndex = s_zIndex + 1 };

    public LvcSize Size { get; private set; }

    public void Show(IEnumerable<ChartPoint> foundPoints, Chart<SkiaSharpDrawingContext> chart)
    {
        _stackPanel ??= new StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Padding(25),
            Orientation = ContainerOrientation.Vertical,
            HorizontalAlignment = Align.Start,
            VerticalAlignment = Align.Middle,
            BackgroundPaint = _backgroundPaint
        };

        // clear the previous elements.
        foreach (var child in _stackPanel.Children.ToArray())
        {
            _ = _stackPanel.Children.Remove(child);
            chart.RemoveVisual(child);
        }

        foreach (var point in foundPoints)
        {
            var sketch = ((IChartSeries<SkiaSharpDrawingContext>)point.Context.Series).GetMiniatresSketch();
            var relativePanel = sketch.AsDrawnControl();

            var label = new LabelVisual
            {
                //Text = point.AsTooltipString,
                Paint = _fontPaint,
                TextSize = 15,
                Padding = new Padding(8, 0, 0, 0),
                VerticalAlignment = Align.Start,
                HorizontalAlignment = Align.Start
            };

            var sp = new StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>
            {
                Padding = new Padding(0, 4),
                VerticalAlignment = Align.Middle,
                HorizontalAlignment = Align.Middle,
                Children =
                {
                    relativePanel,
                    label
                }
            };

            _stackPanel?.Children.Add(sp);
        }

        Measure(chart);

        var location = foundPoints.GetTooltipLocation(Size, chart);

        _stackPanel.X = location.X;
        _stackPanel.Y = location.Y;

        chart.AddVisual(_stackPanel);
    }

    public void Measure(IChart chart)
    {
        if (_stackPanel is null) return;
        Size = _stackPanel.Measure((Chart<SkiaSharpDrawingContext>)chart);
    }

    public void Hide(Chart<SkiaSharpDrawingContext> chart)
    {
        if (chart is null || _stackPanel is null) return;
        chart.RemoveVisual(_stackPanel);
    }
}
