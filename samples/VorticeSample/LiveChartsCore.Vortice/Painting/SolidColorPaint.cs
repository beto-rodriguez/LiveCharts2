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
using LiveChartsCore.Painting;
using LiveChartsCore.Vortice.Drawing;
using Vortice.Direct2D1;
using Vortice.Mathematics;

namespace LiveChartsCore.Vortice.Painting;

public class SolidColorPaint : VorticePaint
{
    public SolidColorPaint()
        : base()
    { }

    public SolidColorPaint(Color4 color)
        : base()
    {
        Color = color;
    }

    public SolidColorPaint(Color4 color, float strokeWidth)
        : base(strokeWidth)
    {
        Color = color;
    }

    public Color4 Color { get; set; }

    /// <inheritdoc cref="Paint.CloneTask" />
    public override Paint CloneTask()
    {
        var clone = new SolidColorPaint { Color = Color };
        return clone;
    }

    internal override void OnPaintStarted(DrawingContext drawingContext, IDrawnElement? drawnElement)
    {
        var vorticeContext = (VorticeDrawingContext)drawingContext;

        if (_lastTarget != vorticeContext.RenderTarget)
        {
            DisposeTask();
            _lastTarget = vorticeContext.RenderTarget;
        }

        _brush ??= vorticeContext.RenderTarget.CreateSolidColorBrush(Color);
        vorticeContext.ActiveBrush = _brush;
    }

    internal override Paint Transitionate(float progress, Paint target)
    {
        if (target is not SolidColorPaint toPaint) return target;

        Color = new Color4(
            (byte)(Color.R + progress * (toPaint.Color.R - Color.R)),
            (byte)(Color.G + progress * (toPaint.Color.G - Color.G)),
            (byte)(Color.B + progress * (toPaint.Color.B - Color.B)),
            (byte)(Color.A + progress * (toPaint.Color.A - Color.A)));

        ((ID2D1SolidColorBrush)_brush!).Color = Color;

        return this;
    }

    internal override void ApplyOpacityMask(DrawingContext context, float opacity, IDrawnElement? drawnElement)
    {
        _brush.Opacity = opacity;
    }

    internal override void RestoreOpacityMask(DrawingContext context, float opacity, IDrawnElement? drawnElement)
    {
        _brush.Opacity = 1;
    }
}
