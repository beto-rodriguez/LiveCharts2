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

using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Drawing;
using LiveChartsCore.Measure;

namespace LiveChartsCore.SkiaSharpView
{
    /// <summary>
    /// Defines an object that must initialize live charts visual objects, this object defines how things will 
    /// be drawn by default, it is highly related to themes.
    /// </summary>
    public class DefaultInitializer : LiveChartsInitializer<SkiaSharpDrawingContext>
    {
        /// <summary>
        /// Constructs a chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public override void ConstructChart(IChartView<SkiaSharpDrawingContext> chart)
        {
            var defaultHoverColor = new SKColor(255, 255, 255, 180);
            chart.PointStates = new PointStatesDictionary<SkiaSharpDrawingContext>
            {
                [LiveCharts.BarSeriesHoverKey] = new StrokeAndFillDrawable<SkiaSharpDrawingContext>(null, new SolidColorPaintTask(defaultHoverColor)),
                [LiveCharts.LineSeriesHoverKey] = new StrokeAndFillDrawable<SkiaSharpDrawingContext>(null, new SolidColorPaintTask(defaultHoverColor)),
                [LiveCharts.PieSeriesHoverKey] = new StrokeAndFillDrawable<SkiaSharpDrawingContext>(null, new SolidColorPaintTask(defaultHoverColor)),
                [LiveCharts.ScatterSeriesHoverKey] = new StrokeAndFillDrawable<SkiaSharpDrawingContext>(null, new SolidColorPaintTask(defaultHoverColor)),
                [LiveCharts.StackedBarSeriesHoverKey] = new StrokeAndFillDrawable<SkiaSharpDrawingContext>(null, new SolidColorPaintTask(defaultHoverColor)),
            };
        }

        /// <summary>
        /// Constructs an axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        public override void ConstructAxis(IAxis<SkiaSharpDrawingContext> axis)
        {
            axis.ShowSeparatorLines = true;
            axis.TextBrush = LiveChartsSkiaSharp.DefaultPaint;
            axis.SeparatorsBrush = LiveChartsSkiaSharp.DefaultPaint;
        }

        /// <summary>
        /// Constructs a series.
        /// </summary>
        /// <param name="series">The series.</param>
        public override void ConstructSeries(IDrawableSeries<SkiaSharpDrawingContext> series)
        {
            if ((series.SeriesProperties & SeriesProperties.PieSeries) == SeriesProperties.PieSeries)
            {
                var pieSeries = (IPieSeries<SkiaSharpDrawingContext>)series;

                pieSeries.Fill = LiveChartsSkiaSharp.DefaultPaint;
                pieSeries.Stroke = null;
                pieSeries.Pushout = 0;

                return;
            }

            if ((series.SeriesProperties & SeriesProperties.Line) == SeriesProperties.Line)
            {
                var lineSeries = (ILineSeries<SkiaSharpDrawingContext>)series;

                if ((series.SeriesProperties & SeriesProperties.Stacked) == SeriesProperties.Stacked)
                {
                    lineSeries.GeometrySize = 0;
                    lineSeries.GeometryFill = null;
                    lineSeries.GeometryStroke = null;
                    lineSeries.Stroke = null;
                    series.Fill = LiveChartsSkiaSharp.DefaultPaint;

                    return;
                }

                lineSeries.GeometrySize = 18;
                lineSeries.GeometryFill = new SolidColorPaintTask(new SKColor(250, 250, 250));
                lineSeries.GeometryStroke = LiveChartsSkiaSharp.DefaultPaint;
            }

            series.Fill = LiveChartsSkiaSharp.DefaultPaint;
            series.Stroke = LiveChartsSkiaSharp.DefaultPaint;
        }

        /// <summary>
        /// Resolves the series defaults.
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <param name="series">The series.</param>
        public override void ResolveSeriesDefaults(Color[] colors, IDrawableSeries<SkiaSharpDrawingContext> series)
        {
            var color = colors[series.SeriesId % colors.Length];

            if (series.Name == null) series.Name = $"Series {series.SeriesId + 1}";

            if ((series.SeriesProperties & SeriesProperties.PieSeries) == SeriesProperties.PieSeries)
            {
                if (series.Fill == LiveChartsSkiaSharp.DefaultPaint) series.Fill = new SolidColorPaintTask(ColorAsSKColor(color));
                if (series.Stroke == LiveChartsSkiaSharp.DefaultPaint) series.Stroke = new SolidColorPaintTask(ColorAsSKColor(color), 3);

                return;
            }

            if (series.Fill == LiveChartsSkiaSharp.DefaultPaint)
            {
                var mask = SeriesProperties.Line | SeriesProperties.Stacked;
                var opacity = (series.SeriesProperties & mask) == mask ? 1 : 0.5;

                series.Fill = new SolidColorPaintTask(ColorAsSKColor(color, (byte)(opacity * 255)));
            }
            if (series.Stroke == LiveChartsSkiaSharp.DefaultPaint) series.Stroke = new SolidColorPaintTask(ColorAsSKColor(color), 3.5f);

            if ((series.SeriesProperties & SeriesProperties.Line) == SeriesProperties.Line)
            {
                var lineSeries = (ILineSeries<SkiaSharpDrawingContext>)series;
                if (lineSeries.GeometryFill == LiveChartsSkiaSharp.DefaultPaint)
                    lineSeries.GeometryFill = new SolidColorPaintTask(ColorAsSKColor(color));
                if (lineSeries.GeometryStroke == LiveChartsSkiaSharp.DefaultPaint)
                    lineSeries.GeometryStroke = new SolidColorPaintTask(ColorAsSKColor(color), lineSeries.Stroke?.StrokeThickness ?? 3.5f);
            }
        }

        /// <summary>
        /// Resolves the axis defaults.
        /// </summary>
        /// <param name="axis">The axis.</param>
        public override void ResolveAxisDefaults(IAxis<SkiaSharpDrawingContext> axis)
        {
            if (axis.SeparatorsBrush == LiveChartsSkiaSharp.DefaultPaint)
                axis.SeparatorsBrush = axis.Orientation == AxisOrientation.X
                    ? null
                    : new SolidColorPaintTask(new SKColor(225, 225, 225));

            if (axis.TextBrush == LiveChartsSkiaSharp.DefaultPaint)
                axis.TextBrush = new SolidColorPaintTask(new SKColor(180, 180, 180));
        }

        private SKColor ColorAsSKColor(Color color, byte? alphaOverrides = null)
        {
            return new SKColor(color.R, color.G, color.B, alphaOverrides ?? color.A);
        }
    }
}

