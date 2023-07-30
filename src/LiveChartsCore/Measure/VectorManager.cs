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
using System.Diagnostics;
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
    where TSegment : IConsecutivePathSegment, IAnimatable
{
    private LinkedListNode<TSegment>? _nextNode;
    private LinkedListNode<TSegment>? _currentNode;

    /// <summary>
    /// Initializes a new instance of the <see cref="VectorManager{TSegment, TDrawingContext}"/> class.
    /// </summary>
    /// <param name="areaGeometry">The area geometry</param>
    public VectorManager(IVectorGeometry<TSegment, TDrawingContext> areaGeometry)
    {
        AreaGeometry = areaGeometry;
        _nextNode = areaGeometry.Commands.First;
    }

    /// <summary>
    /// Gets the area geometry.
    /// </summary>
    public IVectorGeometry<TSegment, TDrawingContext> AreaGeometry { get; private set; }

    /// <summary>
    /// Adds a segment to the area geometry.
    /// </summary>
    /// <param name="segment">The segment.</param>
    /// <param name="followsPrevious">Indicates whether the segment follows the previous segment visual state.</param>
    public void AddConsecutiveSegment(TSegment segment, bool followsPrevious)
    {
        while (
            _nextNode is not null &&
            _nextNode.Next is not null &&
            segment.Id >= _nextNode.Next.Value.Id)
        {
            _nextNode = _nextNode.Next;
            if (_nextNode.Previous is null) continue;
            AreaGeometry.Commands.Remove(_nextNode.Previous);
        }

        // at this points "_nextNode" is:
        // the next node after "segment"
        // or it could also be "segment"
        // or null in case there are no more segments.

        if (_nextNode is null)
        {
            if (_currentNode is not null && followsPrevious) segment.Follows(_currentNode.Value);
            _currentNode = AreaGeometry.Commands.AddLast(segment);
            return;
        }

        if (_nextNode.Value.Id == segment.Id)
        {
            if (!Equals(_nextNode.Value, segment))
            {
                if (followsPrevious) segment.Follows(_nextNode.Value);
                _nextNode.Value = segment; // <- ensure it is the same instance
            }
            _currentNode = _nextNode;
            _nextNode = _currentNode.Next;
            return;
        }

        _currentNode ??= _nextNode;

        if (followsPrevious) segment.Follows(_currentNode.Value);
        _currentNode = AreaGeometry.Commands.AddBefore(_nextNode, segment);
        _nextNode = _currentNode.Next;
    }

    /// <summary>
    /// Clears the current vector segments.
    /// </summary>
    public void Clear()
    {
        AreaGeometry.Commands.Clear();
    }

    /// <summary>
    /// Ends the vector.
    /// </summary>
    public void End()
    {
        while (_currentNode?.Next is not null)
        {
            AreaGeometry.Commands.Remove(_currentNode.Next);
        }

        AreaGeometry.IsValid = false;
    }

    /// <summary>
    /// Logs the path segments ids.
    /// </summary>
    public void LogPath()
    {
        var a = "";
        var c = AreaGeometry.Commands.First;

        while (c != null)
        {
            a += $"{c.Value.Id}, ";
            c = c.Next;
        }

        Trace.WriteLine(a);
    }
}

