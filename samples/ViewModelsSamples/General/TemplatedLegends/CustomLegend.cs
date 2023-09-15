using System.Linq;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
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
    private readonly SolidColorPaint _fontPaint = new(new SKColor(30, 20, 30))
    {
        SKTypeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
        ZIndex = s_zIndex + 1
    };

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

        // clear the previous elements.
        foreach (var visual in _stackPanel.Children.ToArray())
        {
            _ = _stackPanel.Children.Remove(visual);
            chart.RemoveVisual(visual);
        }

        var theme = LiveCharts.DefaultSettings.GetTheme<SkiaSharpDrawingContext>();

        foreach (var series in chart.Series.Where(x => x.IsVisibleAtLegend))
        {
            var panel = new StackPanel<RectangleGeometry, SkiaSharpDrawingContext>
            {
                Padding = new Padding(12, 6),
                VerticalAlignment = Align.Middle,
                HorizontalAlignment = Align.Middle,
                Children =
                {
                    new SVGVisual
                    {
                        Path = SKPath.ParseSvgPathData(SVGPoints.Star),
                        Width = 25,
                        Height = 25,
                        ClippingMode = ClipMode.None, // required on legends // mark
                        Fill = new SolidColorPaint(theme.GetSeriesColor(series).AsSKColor())
                        {
                            ZIndex = s_zIndex + 1
                        }
                    },
                    new LabelVisual
                    {
                        Text = series.Name ?? string.Empty,
                        Paint = _fontPaint,
                        TextSize = 15,
                        ClippingMode = ClipMode.None, // required on legends // mark
                        Padding = new Padding(8, 0, 0, 0),
                        VerticalAlignment = Align.Start,
                        HorizontalAlignment = Align.Start
                    }
                }
            };

            panel.PointerDown += GetPointerDownHandler(series);
            _stackPanel.Children.Add(panel);
        }

        return _stackPanel.Measure(chart);
    }

    private static VisualElementHandler<SkiaSharpDrawingContext> GetPointerDownHandler(
        IChartSeries<SkiaSharpDrawingContext> series)
    {
        return (visual, args) =>
        {
            series.IsVisible = !series.IsVisible;
        };
    }
}
