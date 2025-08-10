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
using LiveChartsCore.Painting;

namespace LiveChartsCore.VisualElements;

/// <summary>
/// Defines a label visual element.
/// </summary>
public abstract class BaseLabelVisual : VisualElement
{
    internal Paint? _paint;
    internal bool _isVirtual = false;
    internal string _text = string.Empty;
    internal double _textSize = 12;
    internal Align _verticalAlignment = Align.Middle;
    internal Align _horizontalAlignment = Align.Middle;
    internal LvcColor _backgroundColor;
    internal Padding _padding = new(0);
    internal float _lineHeight = 1.45f;
    internal float _maxWidth = float.MaxValue;

    /// <summary>
    /// Gets or sets the fill paint.
    /// </summary>
    public Paint? Paint
    {
        get => _paint;
        set => SetPaintProperty(ref _paint, value, PaintStyle.Text);
    }

    /// <summary>
    /// Gets or sets the label text.
    /// </summary>
    public string Text { get => _text; set => SetProperty(ref _text, value); }

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    public double TextSize { get => _textSize; set => SetProperty(ref _textSize, value); }

    /// <summary>
    /// Gets or sets the vertical alignment.
    /// </summary>
    public Align VerticalAlignment { get => _verticalAlignment; set => SetProperty(ref _verticalAlignment, value); }

    /// <summary>
    /// Gets or sets the horizontal alignment.
    /// </summary>
    public Align HorizontalAlignment { get => _horizontalAlignment; set => SetProperty(ref _horizontalAlignment, value); }

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    public LvcColor BackgroundColor { get => _backgroundColor; set => SetProperty(ref _backgroundColor, value); }

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    public Padding Padding { get => _padding; set => SetProperty(ref _padding, value); }

    /// <summary>
    /// Gets or sets the maximum width.
    /// </summary>
    public float MaxWidth { get => _maxWidth; set => SetProperty(ref _maxWidth, value); }
}

/// <summary>
/// Defines a label visual element.
/// </summary>
/// <typeparam name="TLabelGeometry">The type of the label.</typeparam>
public abstract class BaseLabelVisual<TLabelGeometry> : BaseLabelVisual
    where TLabelGeometry : BaseLabelGeometry, new()
{
    internal TLabelGeometry? _labelGeometry;

    /// <inheritdoc cref="ChartElement.GetPaintTasks"/>
    protected internal override Paint?[] GetPaintTasks() =>
        [_paint];

    /// <inheritdoc cref="VisualElement.GetDrawnGeometries"/>
    protected internal override Animatable?[] GetDrawnGeometries() =>
        [_labelGeometry];

    internal override void AlignToTopLeftCorner()
    {
        VerticalAlignment = Align.Start;
        HorizontalAlignment = Align.Start;
    }

    /// <inheritdoc cref="VisualElement.OnInvalidated(Chart)"/>
    protected internal override void OnInvalidated(Chart chart)
    {
        var x = (float)X;
        var y = (float)Y;
        var clipping = Clipping.GetClipRectangle(ClippingMode, chart);

        if (LocationUnit == MeasureUnit.ChartValues)
        {
            if (PrimaryScaler is null || SecondaryScaler is null)
                throw new Exception($"You can not use {MeasureUnit.ChartValues} scale at this element.");

            x = SecondaryScaler.ToPixels(x);
            y = PrimaryScaler.ToPixels(y);
        }

        InitializeLabel();
        _ = Measure(chart);

        _labelGeometry!.Text = Text;
        _labelGeometry.TextSize = (float)TextSize;
        _labelGeometry.X = x;
        _labelGeometry.Y = y;
        _labelGeometry.RotateTransform = (float)Rotation;
        _labelGeometry.TranslateTransform = Translate;
        _labelGeometry.VerticalAlign = VerticalAlignment;
        _labelGeometry.HorizontalAlign = HorizontalAlignment;
        _labelGeometry.Background = BackgroundColor;
        _labelGeometry.Padding = Padding;
        _labelGeometry.MaxWidth = MaxWidth;

        if (Paint is not null)
        {
            chart.Canvas.AddDrawableTask(Paint);
            Paint.SetClipRectangle(chart.Canvas, clipping);
            Paint.AddGeometryToPaintTask(chart.Canvas, _labelGeometry);
        }
    }

    /// <inheritdoc cref="VisualElement.SetParent(DrawnGeometry)"/>
    protected internal override void SetParent(DrawnGeometry parent)
    {
        if (_labelGeometry is null) return;
        ((IDrawnElement)_labelGeometry).Parent = parent;
    }

    /// <inheritdoc cref="VisualElement.Measure(Chart)"/>
    public override LvcSize Measure(Chart chart)
    {
        ApplyTheme<BaseLabelVisual>(chart.GetTheme());

        InitializeLabel();

        _labelGeometry!.Text = Text;
        _labelGeometry.TextSize = (float)TextSize;
        _labelGeometry.RotateTransform = (float)Rotation;
        _labelGeometry.TranslateTransform = Translate;
        _labelGeometry.VerticalAlign = VerticalAlignment;
        _labelGeometry.HorizontalAlign = HorizontalAlignment;
        _labelGeometry.Background = BackgroundColor;
        _labelGeometry.Padding = Padding;
        _labelGeometry.MaxWidth = MaxWidth;
        _labelGeometry.Paint = _paint;

        return _paint is null
            ? new LvcSize()
            : _labelGeometry.Measure();
    }

    private void InitializeLabel()
    {
        if (_labelGeometry is not null) return;

        var x = (float)X;
        var y = (float)Y;

        if (LocationUnit == MeasureUnit.ChartValues)
        {
            if (PrimaryScaler is null || SecondaryScaler is null)
                throw new Exception($"You can not use {MeasureUnit.ChartValues} scale at this element.");

            x = SecondaryScaler.ToPixels(x);
            y = PrimaryScaler.ToPixels(y);
        }

        _labelGeometry = new()
        {
            X = x,
            Y = y,
        };

        // Enabling animations looks strange for tool tips.
        // we need to improve this, by default there are no animations.
        // if you need to enable the animation you can do it manually:
        // this.Animate(new Animation(...));
        //_labelGeometry.Animate(chart);
    }
}
