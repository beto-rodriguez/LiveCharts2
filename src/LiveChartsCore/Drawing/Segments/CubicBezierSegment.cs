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

namespace LiveChartsCore.Drawing.Segments;

/// <summary>
/// Defines a cubic bezier segment, that is part of a sequence.
/// </summary>
public class CubicBezierSegment : Segment
{
    private readonly FloatMotionProperty _xmProperty;
    private readonly FloatMotionProperty _ymProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="CubicBezierSegment"/> class.
    /// </summary>
    public CubicBezierSegment()
    {
        _xmProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Xm), 0f));
        _ymProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Ym), 0f));
    }

    /// <summary>
    /// Gets or sets the middle point in the Y axis.
    /// </summary>
    public float Xm
    {
        get => _xmProperty.GetMovement(this);
        set => _xmProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the middle point in the Y axis.
    /// </summary>
    public float Ym
    {
        get => _ymProperty.GetMovement(this);
        set => _ymProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="Segment.Follows(Segment)"/>
    public override void Follows(Segment segment)
    {
        base.Follows(segment);

        var xProp = segment.MotionProperties[nameof(Xj)];
        var yProp = segment.MotionProperties[nameof(Yj)];

        MotionProperties[nameof(Xm)].CopyFrom(xProp);
        MotionProperties[nameof(Ym)].CopyFrom(yProp);
    }
}
