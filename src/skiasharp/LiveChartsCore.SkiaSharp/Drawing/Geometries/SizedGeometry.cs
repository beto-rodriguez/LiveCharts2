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
using LiveChartsCore.Motion;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <inheritdoc cref="ISizedGeometry{TDrawingContext}" />
public abstract class SizedGeometry : Geometry, ISizedGeometry<SkiaSharpDrawingContext>
{
    /// <summary>
    /// The width
    /// </summary>
    protected FloatMotionProperty widthProperty;

    /// <summary>
    /// The height
    /// </summary>
    protected FloatMotionProperty heightProperty;

    /// <summary>
    /// The match dimensions
    /// </summary>
    protected bool matchDimensions = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="SizedGeometry"/> class.
    /// </summary>
    protected SizedGeometry() : base()
    {
        widthProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Width), 0));
        heightProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Height), 0));
    }

    /// <inheritdoc cref="ISizedGeometry{TDrawingContext}.Width" />
    public float Width { get => widthProperty.GetMovement(this); set => widthProperty.SetMovement(value, this); }

    /// <inheritdoc cref="ISizedGeometry{TDrawingContext}.Height" />
    public float Height
    {
        get => matchDimensions ? widthProperty.GetMovement(this) : heightProperty.GetMovement(this);
        set
        {
            if (matchDimensions)
            {
                widthProperty.SetMovement(value, this);
                return;
            }
            heightProperty.SetMovement(value, this);
        }
    }

    /// <inheritdoc cref="Geometry.OnMeasure(IPaint{SkiaSharpDrawingContext})" />
    protected override LvcSize OnMeasure(IPaint<SkiaSharpDrawingContext> paint)
    {
        return new LvcSize(Width, Height);
    }
}
