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

namespace LiveChartsCore.SkiaSharpView.Painting;

/// <summary>
/// A paint that is resolved at runtime, based on the active color.
/// </summary>
public class ClonePaint : SkiaPaint
{
    private SkiaPaint _clone = null!;

    /// <inheritdoc cref="Paint.ApplyOpacityMask(DrawingContext, float)"/>
    public override void ApplyOpacityMask(DrawingContext context, float opacity) => _clone.ApplyOpacityMask(context, opacity);

    /// <inheritdoc cref="Paint.CloneTask"/>
    public override Paint CloneTask() => this;

    /// <inheritdoc cref="Paint.Dispose"/>
    public override void Dispose() => _clone.Dispose();

    /// <inheritdoc cref="Paint.InitializeTask(DrawingContext)"/>
    public override void InitializeTask(DrawingContext drawingContext) => _clone.InitializeTask(drawingContext);

    /// <inheritdoc cref="Paint.RestoreOpacityMask(DrawingContext, float)"/>
    public override void RestoreOpacityMask(DrawingContext context, float opacity) => _clone.RestoreOpacityMask(context, opacity);

    /// <inheritdoc cref="Paint.Transitionate(float, Paint)"/>
    public override Paint Transitionate(float progress, Paint target) =>
        _clone.Transitionate(progress, target);

    /// <inheritdoc cref="Paint.ResolveActiveColor(Paint)"/>
    public override void ResolveActiveColor(Paint active)
    {
        _clone ??= (SkiaPaint)active.CloneTask();
        _source = _clone;

        _clone.IsAntialias = IsAntialias;
        _clone.StrokeThickness = StrokeThickness;
        _clone.StrokeCap = StrokeCap;
        _clone.StrokeMiter = StrokeMiter;
        _clone.StrokeJoin = StrokeJoin;
        _clone.ImageFilter = ImageFilter;
        _clone.PathEffect = PathEffect;
        _clone.SKFontStyle = SKFontStyle;
        _clone.SKTypeface = SKTypeface;

        _clone.ResolveActiveColor(active);
    }
}
