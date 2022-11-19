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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.SkiaSharpView.VisualElements;

/// <summary>
/// Defines a visual element with stroke and fill properties.
/// </summary>
public class LabelVisual : VisualElement<SkiaSharpDrawingContext>
{
    internal LabelGeometry? _labelGeometry;
    internal IPaint<SkiaSharpDrawingContext>? _paint;
    internal bool _isVirtual = false;
    internal string _text = string.Empty;
    internal double _textSize = 12;
    internal Align _verticalAlignment = Align.Middle;
    internal Align _horizontalAlignment = Align.Middle;
    internal LvcColor _backgroundColor;
    internal Padding _padding = new(0);
    internal double _rotation;
    internal LvcPoint _translate = new();
    private LvcSize _actualSize = new();
    private LvcPoint _actualLocation = new();

    /// <summary>
    /// Gets or sets the fill paint.
    /// </summary>
    public IPaint<SkiaSharpDrawingContext>? Paint
    {
        get => _paint;
        set => SetPaintProperty(ref _paint, value);
    }

    /// <summary>
    /// Gets or sets the label text.
    /// </summary>
    public string Text { get => _text; set { _text = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    public double TextSize { get => _textSize; set { _textSize = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the rotation.
    /// </summary>
    public double Rotation { get => _rotation; set { _rotation = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the translate transform.
    /// </summary>
    public LvcPoint Translate { get => _translate; set { _translate = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the vertical alignment.
    /// </summary>
    public Align VerticalAlignment { get => _verticalAlignment; set { _verticalAlignment = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the horizontal alignment.
    /// </summary>
    public Align HorizontalAlignment { get => _horizontalAlignment; set { _horizontalAlignment = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    public LvcColor BackgroundColor { get => _backgroundColor; set { _backgroundColor = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    public Padding Padding { get => _padding; set { _padding = value; OnPropertyChanged(); } }

    internal override IPaint<SkiaSharpDrawingContext>?[] GetPaintTasks()
    {
        return new[] { _paint };
    }

    internal override void AlignToTopLeftCorner()
    {
        VerticalAlignment = Align.Start;
        HorizontalAlignment = Align.Start;
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.OnInvalidated(Chart{TDrawingContext}, Scaler, Scaler)"/>
    protected internal override void OnInvalidated(Chart<SkiaSharpDrawingContext> chart, Scaler? primaryScaler, Scaler? secondaryScaler)
    {
        var x = (float)(X + _parentX);
        var y = (float)(Y + _parentY);

        if (LocationUnit == MeasureUnit.ChartValues)
        {
            if (primaryScaler is null || secondaryScaler is null)
                throw new Exception($"You can not use {MeasureUnit.ChartValues} scale at this element.");

            x = secondaryScaler.ToPixels(x);
            y = primaryScaler.ToPixels(y);
        }

        _actualLocation = new(x, y);
        _ = Measure(chart, primaryScaler, secondaryScaler);

        if (_labelGeometry is null)
        {
            var parentX = x;
            var parentY = y;

            if (_parent is not null)
            {
                var xProperty = (FloatMotionProperty)_parent.MotionProperties[nameof(_parent.X)];
                var yProperty = (FloatMotionProperty)_parent.MotionProperties[nameof(_parent.Y)];
                parentX = xProperty.GetCurrentValue((Animatable)_parent) + _parentPaddingX;
                parentY = yProperty.GetCurrentValue((Animatable)_parent) + _parentPaddingY;
            }

            _labelGeometry = new LabelGeometry
            {
                Text = Text,
                TextSize = (float)TextSize,
                X = parentX, //(_parent?.X + _parentPaddingX) ?? x,
                Y = parentY,///(_parent?.Y + _parentPaddingY) ?? y,
                RotateTransform = (float)Rotation,
                TranslateTransform = Translate,
                VerticalAlign = VerticalAlignment,
                HorizontalAlign = HorizontalAlignment,
                Background = BackgroundColor,
                Padding = Padding
            };

            _ = _labelGeometry
                .TransitionateProperties()
                .WithAnimation(chart)
                .CompleteCurrentTransitions();
        }

        _labelGeometry.Text = Text;
        _labelGeometry.TextSize = (float)TextSize;
        _labelGeometry.X = x;
        _labelGeometry.Y = y;
        _labelGeometry.RotateTransform = (float)Rotation;
        _labelGeometry.TranslateTransform = Translate;
        _labelGeometry.VerticalAlign = VerticalAlignment;
        _labelGeometry.HorizontalAlign = HorizontalAlignment;
        _labelGeometry.Background = BackgroundColor;
        _labelGeometry.Padding = Padding;

        var drawing = chart.Canvas.Draw();
        if (Paint is not null) _ = drawing.SelectPaint(Paint).Draw(_labelGeometry);
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.Measure(Chart{TDrawingContext}, Scaler, Scaler)"/>
    public override LvcSize Measure(Chart<SkiaSharpDrawingContext> chart, Scaler? primaryScaler, Scaler? secondaryScaler)
    {
        var l = _labelGeometry ?? new LabelGeometry()
        {
            Text = Text,
            TextSize = (float)TextSize,
            RotateTransform = (float)Rotation,
            TranslateTransform = Translate,
            VerticalAlign = VerticalAlignment,
            HorizontalAlign = HorizontalAlignment,
            Background = BackgroundColor,
            Padding = Padding,
        };

        return _actualSize = _paint is null
            ? new LvcSize()
            : l.Measure(_paint);
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.GetActualSize"/>
    public override LvcSize GetActualSize()
    {
        return _actualSize;
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.GetActualLocation"/>
    public override LvcPoint GetActualLocation()
    {
        var x = _actualLocation.X;
        var y = _actualLocation.Y;

        x += Translate.X;
        y += Translate.Y;

        var size = GetActualSize();
        if (HorizontalAlignment == Align.Middle) x -= size.Width * 0.5f;
        if (HorizontalAlignment == Align.End) x -= size.Width;

        if (VerticalAlignment == Align.Middle) y -= size.Height * 0.5f;
        if (VerticalAlignment == Align.End) y -= size.Height;

        return new(x, y);
    }
}
