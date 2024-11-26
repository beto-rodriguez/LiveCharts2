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

using LiveChartsCore.Motion;

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines a label geometry in the user interface.
/// </summary>
public abstract class CoreLabelGeometry : CoreGeometry
{
    private readonly FloatMotionProperty _textSizeProperty;
    private readonly ColorMotionProperty _backgroundProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreLabelGeometry"/> class.
    /// </summary>
    public CoreLabelGeometry()
        : base(true)
    {
        _textSizeProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(TextSize), 11));
        _backgroundProperty = RegisterMotionProperty(new ColorMotionProperty(nameof(Background), LvcColor.Empty));
        TransformOrigin = new LvcPoint(0f, 0f);
    }

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
}
