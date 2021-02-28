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
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Drawing;

namespace LiveChartsCore.SkiaSharpView
{
    public class DefaultSeriesInitializer : SeriesInitializer<SkiaSharpDrawingContext>
    {
        public override void ApplyDefaultsTo(int nextSeriesCount, Color nextColor, IDrawableSeries<SkiaSharpDrawingContext> series)
        {
            if (series.Name == null) series.Name = $"Series {nextSeriesCount + 1}";

            if ((series.SeriesProperties & SeriesProperties.PieSeries) == SeriesProperties.PieSeries)
            {
                // pie special case...
                var pieSeries = (IPieSeries<SkiaSharpDrawingContext>)series;

                if (pieSeries.Fill == null) pieSeries.Fill = new SolidColorPaintTask(ColorAsSKColor(nextColor));
                if (pieSeries.Stroke == null) pieSeries.Stroke = null;
                 pieSeries.PushOut = 8;

                return;
            }

            series.Fill = new SolidColorPaintTask(ColorAsSKColor(nextColor, (byte)(0.7 * 255)));
            series.Stroke = new SolidColorPaintTask(ColorAsSKColor(nextColor));
        }

        private SKColor ColorAsSKColor(Color color, byte? alphaOverrides = null)
        {
            return new SKColor(color.R, color.G, color.B, alphaOverrides ?? color.A);
        }
    }
}

