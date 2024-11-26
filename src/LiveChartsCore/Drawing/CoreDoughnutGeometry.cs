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
/// Defines a doughnut geometry.
/// </summary>
public abstract class CoreDoughnutGeometry : CoreSizedGeometry
{
    private readonly FloatMotionProperty _cxProperty;
    private readonly FloatMotionProperty _cyProperty;
    private readonly FloatMotionProperty _startProperty;
    private readonly FloatMotionProperty _sweepProperty;
    private readonly FloatMotionProperty _pushoutProperty;
    private readonly FloatMotionProperty _innerRadiusProperty;
    private readonly FloatMotionProperty _cornerRadiusProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreDoughnutGeometry"/> class.
    /// </summary>
    public CoreDoughnutGeometry()
    {
        _cxProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(CenterX)));
        _cyProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(CenterY)));
        _startProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(StartAngle)));
        _sweepProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(SweepAngle)));
        _pushoutProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(PushOut)));
        _innerRadiusProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(InnerRadius)));
        _cornerRadiusProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(CornerRadius)));
    }

    /// <summary>
    /// Gets or sets the center x.
    /// </summary>
    public float CenterX
    {
        get => _cxProperty.GetMovement(this);
        set => _cxProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the center y.
    /// </summary>
    public float CenterY
    {
        get => _cyProperty.GetMovement(this);
        set => _cyProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the start angle in degrees.
    /// </summary>
    public float StartAngle
    {
        get => _startProperty.GetMovement(this);
        set => _startProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the sweep angle in degrees.
    /// </summary>
    public float SweepAngle
    {
        get => _sweepProperty.GetMovement(this);
        set => _sweepProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the push out.
    /// </summary>
    public float PushOut
    {
        get => _pushoutProperty.GetMovement(this);
        set => _pushoutProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the inner radius.
    /// </summary>
    public float InnerRadius
    {
        get => _innerRadiusProperty.GetMovement(this);
        set => _innerRadiusProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    public float CornerRadius
    {
        get => _cornerRadiusProperty.GetMovement(this);
        set => _cornerRadiusProperty.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the direction of the corner radius is inverted.
    /// </summary>
    public bool InvertedCornerRadius { get; set; }
}
