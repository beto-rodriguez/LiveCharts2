// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel.Providers;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Motion;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView;

/// <inheritdoc cref="ChartEngine{TDrawingContext}"/>
public class SkiaSharpProvider : ChartEngine<SkiaSharpDrawingContext>
{
    /// <inheritdoc cref="ChartEngine{TDrawingContext}.GetDefaultMapFactory"/>
    public override IMapFactory<SkiaSharpDrawingContext> GetDefaultMapFactory()
        => new MapFactory();

    /// <inheritdoc cref="ChartEngine{TDrawingContext}.GetDefaultCartesianAxis"/>
    public override ICartesianAxis GetDefaultCartesianAxis()
        => new Axis();

    /// <inheritdoc cref="ChartEngine{TDrawingContext}.GetDefaultPolarAxis"/>
    public override IPolarAxis GetDefaultPolarAxis()
        => new PolarAxis();

    /// <inheritdoc cref="ChartEngine{TDrawingContext}.GetSolidColorPaint(LvcColor)"/>
    public override Paint GetSolidColorPaint(LvcColor color)
        => new SolidColorPaint(new SKColor(color.R, color.G, color.B, color.A));

    /// <inheritdoc cref="ChartEngine{TDrawingContext}.InitializeZoommingSection(CoreMotionCanvas)"/>
    public override ISizedGeometry InitializeZoommingSection(CoreMotionCanvas canvas)
    {
        var rectangle = new RectangleGeometry();

        var zoomingSectionPaint = new SolidColorPaint
        {
            IsStroke = false,
            Color = new SKColor(33, 150, 243, 50),
            ZIndex = int.MaxValue
        };

        zoomingSectionPaint.AddGeometryToPaintTask(canvas, rectangle);
        canvas.AddDrawableTask(zoomingSectionPaint);

        return rectangle;
    }
}
