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

public class CustomLegend : IChartLegend<SkiaSharpDrawingContext>
{
    private static readonly int s_zIndex = 10050;
    private readonly StackPanel<RoundedRectangleGeometry, SkiaSharpDrawingContext> _stackPanel = new();
    private readonly SolidColorPaint _backgroundPaint = new(new SKColor(28, 49, 58)) { ZIndex = s_zIndex };
    private readonly SolidColorPaint _fontPaint = new(new SKColor(230, 230, 230)) { ZIndex = s_zIndex + 1 };

    public void Draw(Chart<SkiaSharpDrawingContext> chart)
    {
        var legendPosition = chart.GetLegendPosition();

        _stackPanel.X = legendPosition.X;
        _stackPanel.Y = legendPosition.Y;

        chart.AddVisual(_stackPanel);
        if (chart.LegendPosition == LegendPosition.Hidden) chart.RemoveVisual(_stackPanel);
    }

    public LvcSize Measure(Chart<SkiaSharpDrawingContext> chart)
    {
        _stackPanel.Orientation = ContainerOrientation.Vertical;
        _stackPanel.MaxWidth = double.MaxValue;
        _stackPanel.MaxHeight = chart.ControlSize.Height;
        _stackPanel.BackgroundPaint = _backgroundPaint;

        // clear the previous elements.
        foreach (var visual in _stackPanel.Children.ToArray())
        {
            _ = _stackPanel.Children.Remove(visual);
            chart.RemoveVisual(visual);
        }

        foreach (var series in chart.ChartSeries)
        {
            if (!series.IsVisibleAtLegend) continue;

            _stackPanel.Children.Add(new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
            {
                Padding = new Padding(12, 6),
                VerticalAlignment = Align.Middle,
                HorizontalAlignment = Align.Middle,
                Children =
                {
                    series.GetMiniaturesSketch().AsDrawnControl(),
                    new LabelVisual
                    {
                        Text = series.Name ?? string.Empty,
                        Paint = _fontPaint,
                        TextSize = 10,
                        Padding = new Padding(8, 0, 0, 0),
                        VerticalAlignment = Align.Start,
                        HorizontalAlignment = Align.Start
                    }
                }
            });
        }

        return _stackPanel.Measure(chart);
    }
}
