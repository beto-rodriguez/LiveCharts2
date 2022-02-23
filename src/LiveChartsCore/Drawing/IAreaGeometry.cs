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

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines an area geometry.
/// </summary>
/// <typeparam name="TSegment"></typeparam>
/// <typeparam name="TDrawingContext"></typeparam>
public interface IAreaGeometry<TSegment, TDrawingContext> : IDrawable<TDrawingContext>
    where TSegment : IConsecutivePathSegment
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Determines if the area is closed to the <see cref="Pivot"/> value.
    /// </summary>
    bool IsClosed { get; set; }

    /// <summary>
    /// Gets or sets the pivot.
    /// </summary>
    float Pivot { get; set; }

    /// <summary>
    /// Gets the first linked node.
    /// </summary>
    LinkedListNode<TSegment>? FirstCommand { get; }

    /// <summary>
    /// Gets the last linked node.
    /// </summary>
    LinkedListNode<TSegment>? LastCommand { get; }

    /// <summary>
    /// Gets current commands count.
    /// </summary>
    int CountCommands { get; }

    /// <summary>
    /// Adds a path command at the end.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The linked node.</returns>
    LinkedListNode<TSegment> AddLast(TSegment command);

    /// <summary>
    /// Adds a path command at the beginning.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The linked node.</returns>
    LinkedListNode<TSegment> AddFirst(TSegment command);

    /// <summary>
    /// Adds a command after the given liked node.
    /// </summary>
    /// <param name="node">The linked node.</param>
    /// <param name="command"></param>
    /// <returns>The linked node.</returns>
    LinkedListNode<TSegment> AddAfter(LinkedListNode<TSegment> node, TSegment command);

    /// <summary>
    /// Adds a path command before the given linked node.
    /// </summary>
    /// <param name="node">The linked node.</param>
    /// <param name="command"></param>
    /// <returns>The linked node.</returns>
    LinkedListNode<TSegment> AddBefore(LinkedListNode<TSegment> node, TSegment command);

    /// <summary>
    /// Removes a path command.
    /// </summary>
    /// <param name="command">The command.</param>
    bool RemoveCommand(TSegment command);

    /// <summary>
    /// Removes the specified node.
    /// </summary>
    /// <param name="node">The node.</param>
    void RemoveCommand(LinkedListNode<TSegment> node);

    /// <summary>
    /// Determines whether the specified command is contained in the current path.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>
    ///   <c>true</c> if the specified command contains command; otherwise, <c>false</c>.
    /// </returns>
    bool ContainsCommand(TSegment command);

    /// <summary>
    /// Clears the commands.
    /// </summary>
    void ClearCommands();
}
