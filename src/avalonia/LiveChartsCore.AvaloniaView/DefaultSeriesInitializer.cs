// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.AvaloniaView.Drawing;
using LiveChartsCore.AvaloniaView.Painting;
using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System.Drawing;

namespace LiveChartsCore.AvaloniaView
{
    public class DefaultSeriesInitializer : LiveChartsInitializer<AvaloniaDrawingContext>
    {
        public override void ConstructChart(IChartView<AvaloniaDrawingContext> chart)
        {
            var defaultHoverColor = new Avalonia.Media.Color(180, 255, 255, 255);
            chart.PointStates = new PointStatesDictionary<AvaloniaDrawingContext>
            {
                [LiveCharts.BarSeriesHoverKey] = new StrokeAndFillDrawable<AvaloniaDrawingContext>(null, new SolidColorPaintTask(defaultHoverColor)),
                [LiveCharts.LineSeriesHoverKey] = new StrokeAndFillDrawable<AvaloniaDrawingContext>(null, new SolidColorPaintTask(defaultHoverColor)),
                [LiveCharts.PieSeriesHoverKey] = new StrokeAndFillDrawable<AvaloniaDrawingContext>(null, new SolidColorPaintTask(defaultHoverColor)),
                [LiveCharts.ScatterSeriesHoverKey] = new StrokeAndFillDrawable<AvaloniaDrawingContext>(null, new SolidColorPaintTask(defaultHoverColor)),
                [LiveCharts.StackedBarSeriesHoverKey] = new StrokeAndFillDrawable<AvaloniaDrawingContext>(null, new SolidColorPaintTask(defaultHoverColor)),
            };
        }

        public override void ConstructSeries(IDrawableSeries<AvaloniaDrawingContext> series)
        {
            if ((series.SeriesProperties & SeriesProperties.PieSeries) == SeriesProperties.PieSeries)
            {
                var pieSeries = (IPieSeries<AvaloniaDrawingContext>)series;

                pieSeries.Fill = LiveChartsAvalonia.DefaultPaint;
                pieSeries.Stroke = null;
                pieSeries.Pushout = 0;

                pieSeries.OnPointCreated =
                    (IDoughnutVisualChartPoint<AvaloniaDrawingContext> visual, IChartView<AvaloniaDrawingContext> chart) =>
                    {
                        visual
                            .TransitionateProperties(
                                nameof(visual.StartAngle),
                                nameof(visual.SweepAngle),
                                nameof(visual.PushOut))
                            .WithAnimation(animation =>
                                animation
                                    .WithDuration(chart.AnimationsSpeed)
                                    .WithEasingFunction(EasingFunctions.SinOut));
                    };

                return;
            }

            if ((series.SeriesProperties & SeriesProperties.Line) == SeriesProperties.Line)
            {
                var lineSeries = (ILineSeries<AvaloniaDrawingContext>)series;

                if ((series.SeriesProperties & SeriesProperties.Stacked) == SeriesProperties.Stacked)
                {
                    lineSeries.GeometrySize = 0;
                    lineSeries.ShapesFill = null;
                    lineSeries.ShapesStroke = null;
                    lineSeries.Stroke = null;
                    series.Fill = LiveChartsAvalonia.DefaultPaint;

                    return;
                }

                lineSeries.GeometrySize = 18;
                lineSeries.ShapesFill = new SolidColorPaintTask(new Avalonia.Media.Color(255, 250, 250, 250));
                lineSeries.ShapesStroke = LiveChartsAvalonia.DefaultPaint;
            }

            series.Fill = LiveChartsAvalonia.DefaultPaint;
            series.Stroke = LiveChartsAvalonia.DefaultPaint;
        }

        public override void ResolveSeriesDefaults(Color[] colors, IDrawableSeries<AvaloniaDrawingContext> series)
        {
            var color = colors[series.SeriesId % colors.Length];

            if (series.Name == null) series.Name = $"Series {series.SeriesId + 1}";

            if ((series.SeriesProperties & SeriesProperties.PieSeries) == SeriesProperties.PieSeries)
            {
                if (series.Fill == LiveChartsAvalonia.DefaultPaint) series.Fill = new SolidColorPaintTask(ColorAsAvaloniaColor(color));
                if (series.Stroke == LiveChartsAvalonia.DefaultPaint) series.Stroke = new SolidColorPaintTask(ColorAsAvaloniaColor(color), 3);

                return;
            }

            if (series.Fill == LiveChartsAvalonia.DefaultPaint)
            {
                var mask = SeriesProperties.Line | SeriesProperties.Stacked;
                var opacity = (series.SeriesProperties & mask) == mask ? 1 : 0.5;

                series.Fill = new SolidColorPaintTask(ColorAsAvaloniaColor(color, (byte)(opacity * 255)));
            }
            if (series.Stroke == LiveChartsAvalonia.DefaultPaint) series.Stroke = new SolidColorPaintTask(ColorAsAvaloniaColor(color), 3.5f);

            if ((series.SeriesProperties & SeriesProperties.Line) == SeriesProperties.Line)
            {
                var lineSeries = (ILineSeries<AvaloniaDrawingContext>)series;
                if (lineSeries.ShapesFill == LiveChartsAvalonia.DefaultPaint)
                    lineSeries.ShapesFill = new SolidColorPaintTask(ColorAsAvaloniaColor(color));
                if (lineSeries.ShapesStroke == LiveChartsAvalonia.DefaultPaint)
                    lineSeries.ShapesStroke = new SolidColorPaintTask(ColorAsAvaloniaColor(color), lineSeries.Stroke?.StrokeThickness ?? 3.5f);
            }
        }

        private Avalonia.Media.Color ColorAsAvaloniaColor(Color color, byte? alphaOverrides = null)
        {
            return new Avalonia.Media.Color(alphaOverrides ?? color.A, color.R, color.G, color.B);
        }
    }
}

