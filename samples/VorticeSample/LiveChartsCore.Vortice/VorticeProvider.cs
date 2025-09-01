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
using LiveChartsCore.Vortice.Drawing.Geometries;
using LiveChartsCore.Vortice.Painting;
using Vortice.Mathematics;

namespace LiveChartsCore.Vortice;

/// <inheritdoc cref="ChartEngine"/>
public class VorticeProvider : ChartEngine
{
    /// <inheritdoc cref="ChartEngine.GetDefaultMapFactory"/>
    public override IMapFactory GetDefaultMapFactory()
        => throw new NotImplementedException();

    /// <inheritdoc cref="ChartEngine.GetDefaultCartesianAxis"/>
    public override ICartesianAxis GetDefaultCartesianAxis()
        => new Axis();

    /// <inheritdoc cref="ChartEngine.GetDefaultPolarAxis"/>
    public override IPolarAxis GetDefaultPolarAxis()
        => throw new NotImplementedException();

    /// <inheritdoc cref="ChartEngine.GetSolidColorPaint(LvcColor)"/>
    public override Paint GetSolidColorPaint(LvcColor color)
        => new SolidColorPaint(new Color4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));

    /// <inheritdoc cref="ChartEngine.InitializeZoommingSection(CoreMotionCanvas)"/>
    public override BoundedDrawnGeometry InitializeZoommingSection(CoreMotionCanvas canvas)
    {
        var rectangle = new RectangleGeometry();

        var zoomingSectionPaint = new SolidColorPaint
        {
            PaintStyle = PaintStyle.Fill,
            Color = new Color4(33 / 255f, 150 / 255f, 243 / 255f, 50 / 255f),
            ZIndex = int.MaxValue
        };

        zoomingSectionPaint.AddGeometryToPaintTask(canvas, rectangle);
        canvas.AddDrawableTask(zoomingSectionPaint);

        return rectangle;
    }
}
