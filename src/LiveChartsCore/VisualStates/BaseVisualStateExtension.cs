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

namespace LiveChartsCore.VisualStates;

/// <summary>
/// Defines a base visual state extension.
/// </summary>
public abstract class BaseVisualStateExtension
{
    /// <summary>
    /// Gets or sets the opacity.
    /// </summary>
    public float Opacity { get; set; }

    /// <summary>
    /// Gets or sets the translate transform.
    /// </summary>
    public string? TranslateTransform { get; set; }

    /// <summary>
    /// Gets or sets the scale transform.
    /// </summary>
    public string? ScaleTransform { get; set; }

    /// <summary>
    /// Gets or sets the rotate transform.
    /// </summary>
    public float RotateTransform { get; set; }

    /// <summary>
    /// Gets or sets the skew transform.
    /// </summary>
    public string? SkewTransform { get; set; }

    /// <summary>
    /// Gets or sets the transform origin.
    /// </summary>
    public string? TransformOrigin { get; set; }

    /// <summary>
    /// Gets or sets the fill.
    /// </summary>
    public Paint? Fill { get; set; } = Paint.Default;

    /// <summary>
    /// Gets or sets the stroke.
    /// </summary>
    public Paint? Stroke { get; set; } = Paint.Default;

    /// <summary>
    /// Gets or sets the paint.
    /// </summary>
    public Paint? Paint { get; set; } = Paint.Default;

    protected static LvcPoint ParsePoint(string? point, LvcPoint @default)
    {
        if (string.IsNullOrWhiteSpace(point)) return @default;

        var split = point.Split(',');

        if (split.Length != 2) return @default;
#pragma warning disable IDE0046 // Convert to conditional expression
        if (!float.TryParse(split[0], out var x) || !float.TryParse(split[1], out var y)) return @default;
#pragma warning restore IDE0046 // Convert to conditional expression

        return new LvcPoint(x, y);
    }
}
