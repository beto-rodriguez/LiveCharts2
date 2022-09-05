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

using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace LiveChartsCore.SkiaSharpView.VisualElements;

/// <summary>
/// Defines a visual element in a chart that draws a rectangle geometry in the user interface.
/// </summary>
public class RectangleVisualElement : GeometryVisualElement
{
    private RectangleGeometry? _rectangleGeometry;
    private double _x;
    private double _y;
    private double _width;
    private double _height;

    /// <summary>
    /// Gets or sets the X coordinate [in Pixels or ChartValues, see <see cref="LocationUnit"/>].
    /// </summary>
    public double X { get => _x; set { _x = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the Y coordinate [in Pixels or ChartValues, see <see cref="LocationUnit"/>].
    /// </summary>
    public double Y { get => _y; set { _y = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the unit of the <see cref="X"/> and <see cref="Y"/> properties.
    /// </summary>
    public MeasureUnit LocationUnit { get; set; } = MeasureUnit.Pixels;

    /// <summary>
    /// Gets or sets the height of the rectangle [in Pixels or ChartValues, see <see cref="SizeUnit"/>].
    /// </summary>
    public double Width { get => _width; set { _width = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the width of the rectangle [in Pixels or ChartValues, see <see cref="SizeUnit"/>].
    /// </summary>
    public double Height { get => _height; set { _height = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the unit of the <see cref="Height"/> and <see cref="Width"/> properties.
    /// </summary>
    public MeasureUnit SizeUnit { get; set; } = MeasureUnit.Pixels;

    /// <inheritdoc cref="GeometryVisualElement.Draw"/>
    protected override void Draw(Chart<SkiaSharpDrawingContext> chart, Scaler primaryAxisScale, Scaler secondaryAxisScale)
    {
        var x = (float)X;
        var y = (float)Y;
        var w = (float)Width;
        var h = (float)Height;

        if (SizeUnit == MeasureUnit.ChartValues)
        {
            x = secondaryAxisScale.ToPixels(x);
            y = primaryAxisScale.ToPixels(y);
        }

        if (LocationUnit == MeasureUnit.ChartValues)
        {
            w = secondaryAxisScale.MeasureInPixels(w);
            h = primaryAxisScale.MeasureInPixels(h);
        }

        if (_rectangleGeometry is null)
        {
            _rectangleGeometry = new RectangleGeometry { X = x, Y = y, Width = w, Height = h };

            _ = _rectangleGeometry
                .TransitionateProperties()
                .WithAnimation(chart)
                .CompleteCurrentTransitions();
        }

        _rectangleGeometry.X = x;
        _rectangleGeometry.Y = y;
        _rectangleGeometry.Width = w;
        _rectangleGeometry.Height = h;

        var drawing = chart.Canvas.Draw();
        if (Fill is not null) _ = drawing.SelectPaint(Fill).Draw(_rectangleGeometry);
        if (Stroke is not null) _ = drawing.SelectPaint(Stroke).Draw(_rectangleGeometry);
    }
}
