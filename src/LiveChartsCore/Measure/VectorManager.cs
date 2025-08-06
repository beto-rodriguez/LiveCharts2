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
using System.Collections.Generic;
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Segments;

namespace LiveChartsCore.Measure;

internal class VectorManager<TSegment>(BaseVectorGeometry<TSegment> areaGeometry)
    where TSegment : Segment
{
    private LinkedListNode<TSegment>? _currentNode = areaGeometry.Commands.First;

    public void AddConsecutiveSegment(TSegment segment, bool followsPrevious)
    {
        var list = areaGeometry.Commands;
        LinkedListNode<TSegment>? replaceCandidate = null;
        List<LinkedListNode<TSegment>>? deleteCandidates = null;

        // look for the segment in the list
        while (_currentNode is not null && _currentNode.Value != segment)
        {
            if (_currentNode.Value.Id == segment.Id)
            {
                // save this node, if we can not find the segment
                // but we found a node with the same id,
                // we have a candidate to do a replace.
                replaceCandidate = _currentNode;
            }

            deleteCandidates ??= [];
            deleteCandidates.Add(_currentNode);

            _currentNode = _currentNode?.Next;
        }

        if (_currentNode is null)
        {
            // if we did not find the segment
            // we have to options
            //   1. add it to the end of the list
            //   2. replace the node with the same id

            if (segment.Id <= list.Last?.Value.Id)
            {
                // at this point we know that the path contains this segment
                // but the instance changed, so we replace the node

                if (replaceCandidate is null)
                    throw new InvalidOperationException("This should not happen :(");

                if (followsPrevious)
                    segment.Copy(replaceCandidate.Value);

                replaceCandidate.Value = segment;
                _currentNode = replaceCandidate.Next;
            }
            else
            {
                // this is a new segment

                if (followsPrevious && list.Last is not null)
                    segment.Follows(list.Last.Value);

                _currentNode = list.AddLast(segment);
            }
        }
        else
        {
            // we found the segment

            // lets clean any outdated segments
            // the deleteCandidates list contains all the segments
            // that are not used before the current segment
            foreach (var node in deleteCandidates ?? [])
                list.Remove(node);

            _currentNode = _currentNode.Next;
        }
    }

    public void End() =>
        areaGeometry.IsValid = false;
}
