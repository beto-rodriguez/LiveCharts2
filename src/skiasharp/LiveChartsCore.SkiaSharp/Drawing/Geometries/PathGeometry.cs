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
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries
{
    /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}" />
    public class PathGeometry : Drawable, IPathGeometry<SkiaSharpDrawingContext, SKPath>
    {
        /// <summary>
        /// The commands
        /// </summary>
        protected readonly HashSet<IPathCommand<SKPath>> _commands = new();

        /// <summary>
        /// The drawing commands
        /// </summary>
        protected IPathCommand<SKPath>[]? _drawingCommands = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathGeometry"/> class.
        /// </summary>
        public PathGeometry() { }

        /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.IsClosed" />
        public bool IsClosed { get; set; }

        /// <inheritdoc cref="Geometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
        public override void Draw(SkiaSharpDrawingContext context)
        {
            if (_commands.Count == 0) return;

            var toExecute = _drawingCommands ??= _commands.ToArray();

            var toRemoveSegments = new List<IPathCommand<SKPath>>();

            using var path = new SKPath();
            var isValid = true;

            foreach (var segment in toExecute)
            {
                segment.IsValid = true;
                segment.Execute(path, GetCurrentTime(), this);
                isValid = isValid && segment.IsValid;

                if (segment.IsValid && segment.RemoveOnCompleted) toRemoveSegments.Add(segment);
            }

            foreach (var segment in toRemoveSegments)
            {
                _ = _commands.Remove(segment);
                isValid = false;
            }

            if (IsClosed) path.Close();

            context.Canvas.DrawPath(path, context.Paint);

            if (!isValid) Invalidate();
        }

        /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.AddCommand(IPathCommand{TPathArgs})" />
        public void AddCommand(IPathCommand<SKPath> segment)
        {
            _ = _commands.Add(segment);
            _drawingCommands = null;
            Invalidate();
        }

        /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.ContainsCommand(IPathCommand{TPathArgs})" />
        public bool ContainsCommand(IPathCommand<SKPath> segment)
        {
            return _commands.Contains(segment);
        }

        /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.RemoveCommand(IPathCommand{TPathArgs})" />
        public void RemoveCommand(IPathCommand<SKPath> segment)
        {
            _ = _commands.Remove(segment);
            _drawingCommands = null;
            Invalidate();
        }

        /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.ClearCommands" />
        public void ClearCommands()
        {
            _commands.Clear();
        }

        /// <inheritdoc cref="IAnimatable.CompleteAllTransitions" />
        public override void CompleteAllTransitions()
        {
            var toExecute = _drawingCommands ??= _commands.ToArray();

            foreach (var segment in toExecute)
            {
                segment.CompleteAllTransitions();
            }

            base.CompleteAllTransitions();
        }
    }
}
