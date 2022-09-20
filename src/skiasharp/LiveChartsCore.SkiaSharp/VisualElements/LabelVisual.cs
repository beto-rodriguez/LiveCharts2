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
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace LiveChartsCore.SkiaSharpView.VisualElements;

/// <summary>
/// Defines a visual element with stroke and fill properties.
/// </summary>
public class LabelVisual : BaseVisual
{
    internal LabelGeometry? _labelGeometry;
    internal IPaint<SkiaSharpDrawingContext>? _paint;
    internal double _x;
    internal double _y;
    internal string _text = string.Empty;
    internal double _textSize = 12;
    internal Align _verticalAlignment = Align.Middle;
    internal Align _horizontalAlignment = Align.Middle;
    internal LvcColor _backgroundColor;
    internal Padding _padding = new(0);
    internal double _rotation;
    internal LvcPoint _translate = new();

    /// <summary>
    /// Gets or sets the fill paint.
    /// </summary>
    public IPaint<SkiaSharpDrawingContext>? Paint
    {
        get => _paint;
        set => SetPaintProperty(ref _paint, value);
    }

    /// <summary>
    /// Gets or sets the X coordinate [in Pixels or ChartValues, see <see cref="LocationUnit"/>].
    /// </summary>
    public double X { get => _x; set { _x = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the Y coordinate [in Pixels or ChartValues, see <see cref="LocationUnit"/>].
    /// </summary>
    public double Y { get => _y; set { _y = value; OnPropertyChanged(); } }

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

    /// <summary>
    /// Gets or sets the unit of the <see cref="X"/> and <see cref="Y"/> properties.
    /// </summary>
    public MeasureUnit LocationUnit { get; set; } = MeasureUnit.Pixels;

    /// <inheritdoc cref="ChartElement{TDrawingContext}.GetPaintTasks"/>
    internal override IPaint<SkiaSharpDrawingContext>?[] GetPaintTasks()
    {
        return new[] { _paint };
    }

    /// <inheritdoc cref="BaseVisual.Draw"/>
    protected override void Draw(Chart<SkiaSharpDrawingContext> chart, Scaler primaryAxisScale, Scaler secondaryAxisScale)
    {
        var x = (float)X;
        var y = (float)Y;

        if (LocationUnit == MeasureUnit.ChartValues)
        {
            x = secondaryAxisScale.ToPixels(x);
            y = primaryAxisScale.ToPixels(y);
        }

        if (_labelGeometry is null)
        {
            _labelGeometry = new LabelGeometry
            {
                TextSize = (float)TextSize,
                X = x,
                Y = y,
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
}
