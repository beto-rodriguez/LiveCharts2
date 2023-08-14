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
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;

namespace LiveChartsCore.VisualElements;

/// <summary>
/// Defines a visual element that has stroke and fill, it can also be scaled in
/// <see cref="MeasureUnit.Pixels"/> or <see cref="MeasureUnit.ChartValues"/>.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context,</typeparam>
public abstract class BaseGeometryVisual<TDrawingContext> : VisualElement<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    private double _width;
    private double _height;
    private IPaint<TDrawingContext>? _fill;
    private IPaint<TDrawingContext>? _stroke;
    private MeasureUnit _sizeUnit = MeasureUnit.Pixels;

    /// <summary>
    /// Gets or sets the height of the rectangle [in Pixels or ChartValues, see <see cref="SizeUnit"/>].
    /// </summary>
    public double Width { get => _width; set => SetProperty(ref _width, value); }

    /// <summary>
    /// Gets or sets the width of the rectangle [in Pixels or ChartValues, see <see cref="SizeUnit"/>].
    /// </summary>
    public double Height { get => _height; set => SetProperty(ref _height, value); }

    /// <summary>
    /// Gets or sets the unit of the <see cref="Height"/> and <see cref="Width"/> properties.
    /// </summary>
    public MeasureUnit SizeUnit { get => _sizeUnit; set => SetProperty(ref _sizeUnit, value); }

    /// <summary>
    /// Gets or sets the fill paint.
    /// </summary>
    public IPaint<TDrawingContext>? Fill
    {
        get => _fill;
        set => SetPaintProperty(ref _fill, value);
    }

    /// <summary>
    /// Gets or sets the stroke paint.
    /// </summary>
    public IPaint<TDrawingContext>? Stroke
    {
        get => _stroke;
        set => SetPaintProperty(ref _stroke, value, true);
    }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.GetPaintTasks"/>
    internal override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return new[] { _fill, _stroke };
    }

    /// <summary>
    /// Called when [paint changed].
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    protected override void OnPaintChanged(string? propertyName)
    {
        base.OnPaintChanged(propertyName);
        OnPropertyChanged(propertyName);
    }
}
