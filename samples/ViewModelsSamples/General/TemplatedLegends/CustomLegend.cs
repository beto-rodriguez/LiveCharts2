using System.Linq;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Layouts;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.General.TemplatedLegends;

public class CustomLegend : IChartLegend
{
    private readonly Container _container;
    private readonly StackLayout _stackLayout;
    private bool _isInCanvas = false;
    private DrawablesTask? _drawableTask;

    public CustomLegend()
    {
        _container = new Container
        {
            Content = _stackLayout = new()
            {
                Padding = new Padding(15, 4),
                HorizontalAlignment = Align.Start,
                VerticalAlignment = Align.Middle
            }
        };
    }

    public void Draw(Chart chart)
    {
        var legendPosition = chart.GetLegendPosition();

        _container.X = legendPosition.X;
        _container.Y = legendPosition.Y;

        if (!_isInCanvas)
        {
            _drawableTask = chart.Canvas.AddGeometry(_container);
            _drawableTask.ZIndex = 10099;
            _isInCanvas = true;
        }

        if (chart.LegendPosition == LegendPosition.Hidden && _drawableTask is not null)
        {
            chart.Canvas.RemovePaintTask(_drawableTask);
            _isInCanvas = false;
            _drawableTask = null;
        }
    }

    public LvcSize Measure(Chart chart)
    {
        BuildLayout(chart);

        return _container.Measure();
    }

    public void Hide(Chart chart)
    {
        if (_drawableTask is not null)
        {
            chart.Canvas.RemovePaintTask(_drawableTask);
            _isInCanvas = false;
            _drawableTask = null;
        }
    }

    private void BuildLayout(Chart chart)
    {
        _stackLayout.Orientation = chart.LegendPosition is LegendPosition.Left or LegendPosition.Right
            ? ContainerOrientation.Vertical
            : ContainerOrientation.Horizontal;

        if (_stackLayout.Orientation == ContainerOrientation.Horizontal)
        {
            _stackLayout.MaxWidth = chart.ControlSize.Width;
            _stackLayout.MaxHeight = double.MaxValue;
        }
        else
        {
            _stackLayout.MaxWidth = double.MaxValue;
            _stackLayout.MaxHeight = chart.ControlSize.Height;
        }

        foreach (var visual in _stackLayout.Children.ToArray())
            _ = _stackLayout.Children.Remove(visual);

        foreach (var series in chart.Series.Where(x => x.IsVisibleAtLegend))
        {
            var sl = new StackLayout
            {
                Padding = new Padding(12, 6),
                VerticalAlignment = Align.Middle,
                HorizontalAlignment = Align.Middle,
                Children =
                {
                    new RectangleGeometry
                    {
                        Fill = (series as IStrokedAndFilled)?.Fill?.CloneTask(),
                        Stroke = new SolidColorPaint(new SKColor(30, 30, 30), 3),
                        Width = 20,
                        Height = 50
                    },
                    new LabelGeometry
                    {
                        Text = series.Name ?? string.Empty,
                        Paint = new SolidColorPaint(new SKColor(30, 30, 30)),
                        TextSize = 20,
                        Padding = new Padding(8, 2, 0, 2),
                        MaxWidth = (float)LiveCharts.DefaultSettings.MaxTooltipsAndLegendsLabelsWidth,
                        VerticalAlign = Align.Start,
                        HorizontalAlign = Align.Start
                    }
                }
            };

            _stackLayout.Children.Add(sl);
        }
    }
}
