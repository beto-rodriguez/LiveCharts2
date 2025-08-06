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

using LiveChartsCore.Generators;

namespace LiveChartsCore.Drawing.Segments;

/// <summary>
/// Defines a path segment that is part of a sequence.
/// </summary>
public partial class Segment : Animatable
{
    /// <summary>
    /// Gets or sets the segment id, a unique and consecutive integer.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets the start point in the X axis.
    /// </summary>
    [MotionProperty]
    public partial float Xi { get; set; }

    /// <summary>
    /// Gets the start point in the X axis.
    /// </summary>
    [MotionProperty]
    public partial float Yi { get; set; }

    /// <summary>
    /// Gets the end point in the X axis.
    /// </summary>
    [MotionProperty]
    public partial float Xj { get; set; }

    /// <summary>
    /// Gets the end point in the Y axis.
    /// </summary>
    [MotionProperty]
    public partial float Yj { get; set; }

    internal virtual void Follows(Segment segment)
    {
        IsValid = segment.IsValid;
        RemoveOnCompleted = segment.RemoveOnCompleted;

        var xiPropertyGetter = XiProperty.GetMotion!;
        var xjPropertyGetter = XjProperty.GetMotion!;
        var yiPropertyGetter = YiProperty.GetMotion!;
        var yjPropertyGetter = YjProperty.GetMotion!;

        var xProp = xjPropertyGetter(segment)!;
        var yProp = yjPropertyGetter(segment)!;

        xiPropertyGetter(this)!.CopyFrom(xProp);
        xjPropertyGetter(this)!.CopyFrom(xProp);
        yiPropertyGetter(this)!.CopyFrom(yProp);
        yjPropertyGetter(this)!.CopyFrom(yProp);
    }

    internal virtual void Copy(Segment segment)
    {
        IsValid = segment.IsValid;
        RemoveOnCompleted = segment.RemoveOnCompleted;

        var xiPropertyGetter = XiProperty.GetMotion!;
        var xjPropertyGetter = XjProperty.GetMotion!;
        var yiPropertyGetter = YiProperty.GetMotion!;
        var yjPropertyGetter = YjProperty.GetMotion!;

        xiPropertyGetter(this)!.CopyFrom(xiPropertyGetter(segment));
        xjPropertyGetter(this)!.CopyFrom(xjPropertyGetter(segment));
        yiPropertyGetter(this)!.CopyFrom(yiPropertyGetter(segment));
        yjPropertyGetter(this)!.CopyFrom(yjPropertyGetter(segment));
    }

    /// <inheritdoc/>
    public override string ToString() => $"[{Id}] ({Xi:N2} - {Xj:N2}, {Yj:N2}{Yi:N2})";
}
