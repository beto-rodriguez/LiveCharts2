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
using LiveChartsCore.Motion;
using LiveChartsCore.Painting;

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines a label geometry in the user interface.
/// </summary>
public abstract class CoreLabelGeometry : Animatable, IDrawable
{
    private readonly FloatMotionProperty _xProperty;
    private readonly FloatMotionProperty _yProperty;
    private readonly FloatMotionProperty _rotationProperty;
    private readonly PointMotionProperty _transformOriginProperty;
    private readonly PointMotionProperty _scaleProperty;
    private readonly PointMotionProperty _skewProperty;
    private readonly PointMotionProperty _translateProperty;
    private readonly FloatMotionProperty _opacityProperty;
    private readonly FloatMotionProperty _textSizeProperty;
    private readonly ColorMotionProperty _backgroundProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreLabelGeometry"/> class.
    /// </summary>
    public CoreLabelGeometry()
    {
        HasTransform = true;
        _textSizeProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(TextSize), 11));
        _backgroundProperty = RegisterMotionProperty(new ColorMotionProperty(nameof(Background), LvcColor.Empty));
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
        TransformOrigin = new LvcPoint(0f, 0f);
    }

    /// <summary>
    /// Gets or sets the opacity.
    /// </summary>
    public float Opacity
    {
        get => _opacityProperty.GetMovement(this);
        set => _opacityProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="IDrawable.X"/>
    public float X
    {
        get => Parent is null
            ? _xProperty.GetMovement(this)
            : _xProperty.GetMovement(this) + Parent.X;
        set => _xProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="IDrawable.Y"/>
    public float Y
    {
        get => Parent is null
            ? _yProperty.GetMovement(this)
            : _yProperty.GetMovement(this) + Parent.Y;
        set => _yProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="IDrawable.TransformOrigin"/>
    public LvcPoint TransformOrigin
    {
        get => _transformOriginProperty.GetMovement(this);
        set => _transformOriginProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="IDrawable.TranslateTransform"/>
    public LvcPoint TranslateTransform
    {
        get => _translateProperty.GetMovement(this);
        set
        {
            _translateProperty.SetMovement(value, this);
            HasTransform = true;
        }
    }

    /// <inheritdoc cref="IDrawable.RotateTransform"/>
    public float RotateTransform
    {
        get => _rotationProperty.GetMovement(this);
        set
        {
            _rotationProperty.SetMovement(value, this);
            HasTransform = true;
        }
    }

    /// <inheritdoc cref="IDrawable.ScaleTransform"/>
    public LvcPoint ScaleTransform
    {
        get => _scaleProperty.GetMovement(this);
        set
        {
            _scaleProperty.SetMovement(value, this);
            HasTransform = true;
        }
    }

    /// <inheritdoc cref="IDrawable.SkewTransform"/>
    public LvcPoint SkewTransform
    {
        get => _skewProperty.GetMovement(this);
        set
        {
            _skewProperty.SetMovement(value, this);
            HasTransform = true;
        }
    }

    /// <inheritdoc cref="IDrawable.HasTransform"/>
    public bool HasTransform { get; protected set; }

    /// <inheritdoc cref="IDrawable.HasTranslate"/>
    public bool HasTranslate
    {
        get
        {
            var t = TranslateTransform;
            return t.X != 0 || t.Y != 0;
        }
    }

    /// <inheritdoc cref="IDrawable.HasScale"/>
    public bool HasScale
    {
        get
        {
            var s = ScaleTransform;
            return s.X != 1 || s.Y != 1;
        }
    }

    /// <inheritdoc cref="IDrawable.HasSkew"/>
    public bool HasSkew
    {
        get
        {
            var s = SkewTransform;
            return s.X != 1 || s.Y != 1;
        }
    }

    /// <inheritdoc cref="IDrawable.HasSkew"/>
    public bool HasRotation => Math.Abs(RotateTransform) > 0;

    /// <summary>
    /// Gets or sets the vertical align.
    /// </summary>
    /// <value>
    /// The vertical align.
    /// </value>
    public Align VerticalAlign { get; set; } = Align.Middle;

    /// <summary>
    /// Gets or sets the horizontal align.
    /// </summary>
    /// <value>
    /// The horizontal align.
    /// </value>
    public Align HorizontalAlign { get; set; } = Align.Middle;

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the size of the text.
    /// </summary>
    public float TextSize
    {
        get => _textSizeProperty.GetMovement(this);
        set => _textSizeProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    public LvcColor Background
    {
        get => _backgroundProperty.GetMovement(this);
        set => _backgroundProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    public Padding Padding { get; set; } = new();

    /// <summary>
    /// Gets or sets the line height [in times the text height].
    /// </summary>
    public float LineHeight { get; set; } = 1.45f;

    /// <summary>
    /// Gets or sets the maximum width, when the text exceeds this width, it will be wrapped.
    /// </summary>
    public float MaxWidth { get; set; } = float.MaxValue;

#if DEBUG
    /// <summary>
    /// This property is only available on debug mode, it indicates if the debug lines should be shown.
    /// </summary>
    public static bool ShowDebugLines { get; set; }
#endif

    /// <summary>
    /// Gets or sets the paint.
    /// </summary>
    public Paint? Paint
    {
        get => ((IDrawable)this).Fill;
        set => ((IDrawable)this).Fill = value;
    }

    Paint? IDrawable.Stroke { get; set; }
    Paint? IDrawable.Fill { get; set; }

    /// <inheritdoc cref="IDrawable.Measure()"/>
    public abstract LvcSize Measure();
}
