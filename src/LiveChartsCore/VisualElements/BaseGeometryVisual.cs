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
using LiveChartsCore.Painting;

namespace LiveChartsCore.VisualElements;

/// <summary>
/// Defines a visual element that has stroke and fill, it can also be scaled in
/// <see cref="MeasureUnit.Pixels"/> or <see cref="MeasureUnit.ChartValues"/>.
/// </summary>
public abstract class BaseGeometryVisual : VisualElement
{
    /// <summary>
    /// Gets or sets the height of the rectangle [in Pixels or ChartValues, see <see cref="SizeUnit"/>].
    /// </summary>
    public double Width { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets or sets the width of the rectangle [in Pixels or ChartValues, see <see cref="SizeUnit"/>].
    /// </summary>
    public double Height { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets or sets the unit of the <see cref="Height"/> and <see cref="Width"/> properties.
    /// </summary>
    public MeasureUnit SizeUnit { get; set => SetProperty(ref field, value); } = MeasureUnit.Pixels;

    /// <summary>
    /// Gets or sets the fill paint.
    /// </summary>
    public Paint? Fill
    {
        get;
        set => SetPaintProperty(ref field, value);
    }

    /// <summary>
    /// Gets or sets the stroke paint.
    /// </summary>
    public Paint? Stroke
    {
        get;
        set => SetPaintProperty(ref field, value, PaintStyle.Stroke);
    }

    /// <inheritdoc cref="ChartElement.GetPaintTasks"/>
    protected internal override Paint?[] GetPaintTasks() =>
        [Fill, Stroke];

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
