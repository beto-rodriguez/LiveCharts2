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

using System.Collections.Generic;
using LiveChartsCore.Drawing.Segments;
using LiveChartsCore.Motion;
using LiveChartsCore.Painting;

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines an area geometry.
/// </summary>
/// <typeparam name="TSegment">The type of the segment.</typeparam>
public abstract class CoreVectorGeometry<TSegment> : Animatable, IDrawable
    where TSegment : Segment
{
    private readonly FloatMotionProperty _pivotProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreVectorGeometry{TSegment}"/> class.
    /// </summary>
    public CoreVectorGeometry()
    {
        _pivotProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Pivot), 0f));
    }

    /// <summary>
    /// Gets the commands in the vector.
    /// </summary>
    public LinkedList<TSegment> Commands { get; } = new();

    /// <inheritdoc cref="CoreVectorGeometry{TSegment}.ClosingMethod" />
    public VectorClosingMethod ClosingMethod { get; set; }

    /// <inheritdoc cref="CoreVectorGeometry{TSegment}.Pivot" />
    public float Pivot { get => _pivotProperty.GetMovement(this); set => _pivotProperty.SetMovement(value, this); }

    /// <inheritdoc cref="Animatable.CompleteTransition(string[])" />
    public override void CompleteTransition(params string[]? propertyName)
    {
        foreach (var segment in Commands)
        {
            segment.CompleteTransition(propertyName);
        }

        base.CompleteTransition(propertyName);
    }

    /// <inheritdoc cref="IDrawable.OnMeasure(Paint)" />
    public LvcSize OnMeasure(Paint paint) => new();
}
