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
using System.Collections.Generic;
using LiveChartsCore.Drawing.Segments;
using LiveChartsCore.Motion;
using LiveChartsCore.Painting;

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines an area geometry.
/// </summary>
/// <typeparam name="TSegment">The type of the segment.</typeparam>
public abstract class BaseVectorGeometry<TSegment> : Animatable, IDrawnElement
    where TSegment : Segment
{
    private readonly FloatMotionProperty _xProperty;
    private readonly FloatMotionProperty _yProperty;
    private readonly FloatMotionProperty _rotationProperty;
    private readonly PointMotionProperty _transformOriginProperty;
    private readonly PointMotionProperty _scaleProperty;
    private readonly PointMotionProperty _skewProperty;
    private readonly PointMotionProperty _translateProperty;
    private readonly FloatMotionProperty _opacityProperty;
    private readonly FloatMotionProperty _pivotProperty;
    private Paint? _stroke;
    private Paint? _fill;
    private IDrawnElement? _parent;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseVectorGeometry{TSegment}"/> class.
    /// </summary>
    public BaseVectorGeometry()
    {
        _xProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(X), 0));
        _yProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Y), 0));
        _transformOriginProperty = RegisterMotionProperty(
            new PointMotionProperty(nameof(TransformOrigin), new LvcPoint(0.5f, 0.5f)));
        _translateProperty = RegisterMotionProperty(
            new PointMotionProperty(nameof(TranslateTransform), new LvcPoint(0, 0)));
        _rotationProperty = RegisterMotionProperty(
            new FloatMotionProperty(nameof(RotateTransform), 0));
        _scaleProperty = RegisterMotionProperty(
            new PointMotionProperty(nameof(ScaleTransform), new LvcPoint(1, 1)));
        _skewProperty = RegisterMotionProperty(
            new PointMotionProperty(nameof(SkewTransform), new LvcPoint(1, 1)));
        _opacityProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Opacity), 1));
        _pivotProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Pivot), 0f));
    }

    /// <inheritdoc cref="IDrawnElement.Parent"/>
    IDrawnElement? IDrawnElement.Parent { get => _parent; set => _parent = value; }

    /// <inheritdoc cref="IDrawnElement.Opacity"/>
    public float Opacity
    {
        get => _opacityProperty.GetMovement(this);
        set => _opacityProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="IDrawnElement.X"/>
    public float X
    {
        get => _parent is null
            ? _xProperty.GetMovement(this)
            : _xProperty.GetMovement(this) + _parent.X;
        set => _xProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="IDrawnElement.Y"/>
    public float Y
    {
        get => _parent is null
            ? _yProperty.GetMovement(this)
            : _yProperty.GetMovement(this) + _parent.Y;
        set => _yProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="IDrawnElement.TransformOrigin"/>
    public LvcPoint TransformOrigin
    {
        get => _transformOriginProperty.GetMovement(this);
        set => _transformOriginProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="IDrawnElement.TranslateTransform"/>
    public LvcPoint TranslateTransform
    {
        get => _translateProperty.GetMovement(this);
        set
        {
            _translateProperty.SetMovement(value, this);
            HasTransform = true;
        }
    }

    /// <inheritdoc cref="IDrawnElement.RotateTransform"/>
    public float RotateTransform
    {
        get => _rotationProperty.GetMovement(this);
        set
        {
            _rotationProperty.SetMovement(value, this);
            HasTransform = true;
        }
    }

    /// <inheritdoc cref="IDrawnElement.ScaleTransform"/>
    public LvcPoint ScaleTransform
    {
        get => _scaleProperty.GetMovement(this);
        set
        {
            _scaleProperty.SetMovement(value, this);
            HasTransform = true;
        }
    }

    /// <inheritdoc cref="IDrawnElement.SkewTransform"/>
    public LvcPoint SkewTransform
    {
        get => _skewProperty.GetMovement(this);
        set
        {
            _skewProperty.SetMovement(value, this);
            HasTransform = true;
        }
    }

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

    /// <inheritdoc cref="IDrawnElement.Stroke"/>
    public Paint? Stroke
    {
        get => _stroke;
        set
        {
            _stroke = value;
            if (_stroke is not null) _stroke.PaintStyle = PaintStyle.Stroke;
        }
    }

    /// <inheritdoc cref="IDrawnElement.Fill"/>
    public Paint? Fill
    {
        get => _fill;
        set
        {
            _fill = value;
            if (_fill is not null) _fill.PaintStyle = PaintStyle.Fill;
        }
    }

    Paint? IDrawnElement.Paint { get; set; }

    /// <summary>
    /// Gets the commands in the vector.
    /// </summary>
    public LinkedList<TSegment> Commands { get; } = new();

    /// <summary>
    /// Gets or sets the closing method.
    /// </summary>
    public VectorClosingMethod ClosingMethod { get; set; }

    /// <summary>
    /// Gets or sets the pivot.
    /// </summary>
    public float Pivot
    {
        get => _pivotProperty.GetMovement(this);
        set => _pivotProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="Animatable.CompleteTransition(string[])" />
    public override void CompleteTransition(params string[]? propertyName)
    {
        foreach (var segment in Commands)
        {
            segment.CompleteTransition(propertyName);
        }

        base.CompleteTransition(propertyName);
    }

    /// <inheritdoc cref="IDrawnElement.Measure()" />
    public LvcSize Measure() => new();
}
