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
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Segments;

namespace LiveChartsCore.Measure;

/// <summary>
/// Defines the vector manager class.
/// </summary>
/// <typeparam name="TSegment">The type of the segment.</typeparam>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
public class VectorManager<TSegment, TDrawingContext>
    where TDrawingContext : DrawingContext
    where TSegment : IPathSegment, IAnimatable
{
    private readonly IAreaGeometry<TSegment, TDrawingContext> _areaGeometry;
    private LinkedListNode<TSegment>? _activeNode;
    private TSegment? _lastActiveSegment;

    /// <summary>
    /// Initializes a new instance of the <see cref="VectorManager{TSegment, TDrawingContext}"/> class.
    /// </summary>
    /// <param name="areaGeometry">The area geometry</param>
    public VectorManager(IAreaGeometry<TSegment, TDrawingContext> areaGeometry)
    {
        _areaGeometry = areaGeometry;
        _activeNode = areaGeometry.FirstCommand;
    }

    /// <summary>
    /// Adds a segment to the area geometry.
    /// </summary>
    /// <param name="segment">The segment.</param>
    /// <param name="copyPrevious">Indicates whether the data of the previous segment should be copied to the just added segment.</param>
    public void AddConsecutiveSegment(TSegment segment, bool copyPrevious = false)
    {
        if (_activeNode == null)
        {
            _activeNode = _areaGeometry.AddFirst(segment);
            return;
        }

        while (_activeNode.Next is not null && _activeNode.Next.Value.Id < segment.Id)
        {
            _lastActiveSegment = _activeNode.Next.Value;
            _areaGeometry.RemoveCommand(_activeNode.Next);
        }

        if (segment.Id == _activeNode.Value.Id)
        {
            _lastActiveSegment = _activeNode.Value;
            return;
        }

        if (copyPrevious)
        {
            var source = _lastActiveSegment ?? _activeNode.Value;
            source.CurrentTime = _areaGeometry.CurrentTime;
            source.CopyTo(segment);
            segment.CompleteAllTransitions();
        }

        _activeNode = _areaGeometry.AddAfter(_activeNode, segment);
        _lastActiveSegment = _activeNode.Value;
    }
}
