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

using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Drawing;

namespace LiveChartsCore.SkiaSharpView
{
    public class DefaultSeriesInitializer : SeriesInitializer<SkiaSharpDrawingContext>
    {
        public override void ConstructSeries(IDrawableSeries<SkiaSharpDrawingContext> series)
        {
            if ((series.SeriesProperties & SeriesProperties.PieSeries) == SeriesProperties.PieSeries)
            {
                var pieSeries = (IPieSeries<SkiaSharpDrawingContext>)series;

                pieSeries.Fill = LiveChartsSK.DefaultPaint;
                pieSeries.Stroke = null;
                pieSeries.PushOut = 6;

                pieSeries.TransitionsSetter =
                    (IDoughnutVisualChartPoint<SkiaSharpDrawingContext> visual, Animation defaultAnimation) =>
                    {
                        visual
                            .DefinePropertyTransitions(nameof(visual.StartAngle), nameof(visual.SweepAngle))
                            .DefineAnimation(animation =>
                            {
                                animation.Duration = defaultAnimation.Duration;
                                animation.EasingFunction = EasingFunctions.BounceOut;
                            });
                    };

                return;
            }

            series.Fill = LiveChartsSK.DefaultPaint;
            series.Stroke = LiveChartsSK.DefaultPaint;
        }

        public override void ResolveDefaults(Color color, IDrawableSeries<SkiaSharpDrawingContext> series)
        {
            if (series.Name == null) series.Name = $"Series {series.SeriesId + 1}";

            if ((series.SeriesProperties & SeriesProperties.PieSeries) == SeriesProperties.PieSeries)
            {
                if (series.Fill == LiveChartsSK.DefaultPaint) series.Fill = new SolidColorPaintTask(ColorAsSKColor(color));
                if (series.Stroke == LiveChartsSK.DefaultPaint) series.Stroke = new SolidColorPaintTask(ColorAsSKColor(color), 3);

                return;
            }

            if ((series.SeriesProperties & SeriesProperties.Line) == SeriesProperties.Line)
            {
                var lineSeries = (ILineSeries<SkiaSharpDrawingContext>)series;
                if (lineSeries.ShapesFill == LiveChartsSK.DefaultPaint) lineSeries.ShapesFill = new SolidColorPaintTask(ColorAsSKColor(color));
                if (lineSeries.ShapesStroke == LiveChartsSK.DefaultPaint) lineSeries.ShapesStroke = null;
            }

            if (series.Fill == LiveChartsSK.DefaultPaint) series.Fill = new SolidColorPaintTask(ColorAsSKColor(color, (byte)(0.7*255)));
            if (series.Stroke == LiveChartsSK.DefaultPaint) series.Stroke = new SolidColorPaintTask(ColorAsSKColor(color), 3);
        }

        private SKColor ColorAsSKColor(Color color, byte? alphaOverrides = null)
        {
            return new SKColor(color.R, color.G, color.B, alphaOverrides ?? color.A);
        }
    }
}

