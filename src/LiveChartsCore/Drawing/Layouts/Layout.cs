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

using System;
using LiveChartsCore.Generators;
using LiveChartsCore.Painting;

namespace LiveChartsCore.Drawing.Layouts;

/// <summary>
/// Defines a layout for drawable elements.
/// </summary>
public abstract partial class Layout<TDrawingContext> : Animatable, IDrawnElement
    where TDrawingContext : DrawingContext
{
    private IDrawnElement? _parent;

    /// <summary>
    /// Initializes a new instance of the <see cref="Layout{TDrawingContext}"/> class.
    /// </summary>
    protected Layout()
    {
        _OpacityMotionProperty = new(1f);
        _TransformOriginMotionProperty = new(new(0.5f, 0.5f));
        _ScaleTransformMotionProperty = new(new(1f, 1f));
        _SkewTransformMotionProperty = new(new(1f, 1f));
        _PaddingMotionProperty = new(new(0f));
    }

    /// <inheritdoc cref="IDrawnElement.Parent"/>
    IDrawnElement? IDrawnElement.Parent { get => _parent; set => _parent = value; }

    /// <inheritdoc cref="IDrawnElement.Opacity"/>
    [MotionProperty]
    public partial float Opacity { get; set; }

    /// <inheritdoc cref="IDrawnElement.X"/>
    [MotionProperty(HasExplicitAcessors = true)]
    public partial float X
    {
        get => _parent is null
            ? _XMotionProperty.GetMovement(this)
            : _XMotionProperty.GetMovement(this) + _parent.X;
        set => _XMotionProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="IDrawnElement.Y"/>
    [MotionProperty(HasExplicitAcessors = true)]
    public partial float Y
    {
        get => _parent is null
            ? _YMotionProperty.GetMovement(this)
            : _YMotionProperty.GetMovement(this) + _parent.Y;
        set => _YMotionProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="IDrawnElement.TransformOrigin"/>
    [MotionProperty]
    public partial LvcPoint TransformOrigin { get; set; }

    /// <inheritdoc cref="IDrawnElement.TranslateTransform"/>
    [MotionProperty]
    public partial LvcPoint TranslateTransform { get; set; }

    /// <inheritdoc cref="IDrawnElement.RotateTransform"/>
    [MotionProperty]
    public partial float RotateTransform { get; set; }

    /// <inheritdoc cref="IDrawnElement.ScaleTransform"/>
    [MotionProperty]
    public partial LvcPoint ScaleTransform { get; set; }

    /// <inheritdoc cref="IDrawnElement.SkewTransform"/>
    [MotionProperty]
    public partial LvcPoint SkewTransform { get; set; }

    /// <inheritdoc cref="IDrawnElement.HasTransform"/>
    public bool HasTransform { get; protected set; }

    /// <inheritdoc cref="IDrawnElement.HasTranslate"/>
    public bool HasTranslate
    {
        get
        {
            var t = TranslateTransform;
            return t.X != 0 || t.Y != 0;
        }
    }

    /// <inheritdoc cref="IDrawnElement.HasScale"/>
    public bool HasScale
    {
        get
        {
            var s = ScaleTransform;
            return s.X != 1 || s.Y != 1;
        }
    }

    /// <inheritdoc cref="IDrawnElement.HasSkew"/>
    public bool HasSkew
    {
        get
        {
            var s = SkewTransform;
            return s.X != 1 || s.Y != 1;
        }
    }

    /// <inheritdoc cref="IDrawnElement.HasSkew"/>
    public bool HasRotation => Math.Abs(RotateTransform) > 0;

    Paint? IDrawnElement.Stroke
    {
        get => MeasureTask.Instance;
        set => throw new NotImplementedException(
            $"Layouts can not have a {nameof(IDrawnElement.Stroke)}, instead place the layout as the child of another geometry.");
    }

    Paint? IDrawnElement.Fill
    {
        get => null;
        set => throw new NotImplementedException(
            $"Layouts can not have a {nameof(IDrawnElement.Fill)}, instead place the layout as the child of another geometry.");
    }

    Paint? IDrawnElement.Paint
    {
        get => null;
        set => throw new NotImplementedException(
            $"Layouts can not have a {nameof(IDrawnElement.Paint)}, instead place the layout as the child of another geometry.");
    }

    LvcDropShadow? IDrawnElement.DropShadow { get; set; }

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    [MotionProperty]
    public partial Padding Padding { get; set; }

    /// <inheritdoc cref="IDrawnElement.Measure()"/>
    public abstract LvcSize Measure();

    void IDrawnElement.DisposePaints()
    {

    }
}
