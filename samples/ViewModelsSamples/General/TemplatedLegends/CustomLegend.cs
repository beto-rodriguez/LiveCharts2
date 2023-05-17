using System.Linq;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace ViewModelsSamples.General.TemplatedLegends;

public class CustomLegend : IChartLegend<SkiaSharpDrawingContext>, IImageControl
{
    private static readonly int s_zIndex = 10050;
    private StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>? _stackPanel;
    private readonly SolidColorPaint _backgroundPaint = new(new SKColor(28, 49, 58)) { ZIndex = s_zIndex };
    private readonly SolidColorPaint _fontPaint = new(new SKColor(230, 230, 230)) { ZIndex = s_zIndex + 1 };

    public LvcSize Size { get; set; }

    public void Draw(Chart<SkiaSharpDrawingContext> chart)
    {
        if (chart.LegendPosition == LegendPosition.Hidden) return;

        Measure(chart);
        if (_stackPanel is null) return;
        var actualChartSize = chart.ControlSize;

        if (chart.LegendPosition == LegendPosition.Top)
        {
            chart.Canvas.StartPoint = new LvcPoint(0, Size.Height);
            _stackPanel.X = actualChartSize.Width * 0.5f - Size.Width * 0.5f;
            _stackPanel.Y = -Size.Height;
        }
        if (chart.LegendPosition == LegendPosition.Bottom)
        {
            _stackPanel.X = actualChartSize.Width * 0.5f - Size.Width * 0.5f;
            _stackPanel.Y = actualChartSize.Height;
        }
        if (chart.LegendPosition == LegendPosition.Left)
        {
            chart.Canvas.StartPoint = new LvcPoint(Size.Width, 0);
            _stackPanel.X = -Size.Width;
            _stackPanel.Y = actualChartSize.Height * 0.5f - Size.Height * 0.5f;
        }
        if (chart.LegendPosition == LegendPosition.Right)
        {
            _stackPanel.X = actualChartSize.Width;
            _stackPanel.Y = actualChartSize.Height * 0.5f - Size.Height * 0.5f;
        }

        chart.AddVisual(_stackPanel);
    }

    public void Measure(IChart chart)
    {
        var skiaChart = (Chart<SkiaSharpDrawingContext>)chart;
        BuildLayout(skiaChart);
        if (_stackPanel is null) return;
        Size = _stackPanel.Measure(skiaChart);
    }

    private void BuildLayout(Chart<SkiaSharpDrawingContext> chart)
    {
        _stackPanel ??= new StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext>
        {
            Padding = new Padding(15),
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

        foreach (var series in chart.ChartSeries)
        {
            var sketch = series.GetMiniatresSketch();
            var relativePanel = sketch.AsDrawnControl();

            var label = new LabelVisual
            {
                Text = series.Name ?? string.Empty,
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

            _ = _stackPanel?.Children.Add(sp);
        }
    }
}
