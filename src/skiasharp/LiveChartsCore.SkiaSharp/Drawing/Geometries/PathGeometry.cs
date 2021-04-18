// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

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
        private readonly HashSet<IPathCommand<SKPath>> commands = new HashSet<IPathCommand<SKPath>>();
        private IPathCommand<SKPath>[] drawingCommands = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathGeometry"/> class.
        /// </summary>
        public PathGeometry()
        {
        }

        /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.IsClosed" />
        public bool IsClosed { get; set; }

        /// <inheritdoc cref="Geometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
        public override void Draw(SkiaSharpDrawingContext context)
        {
            if (commands.Count == 0) return;

            var toExecute = drawingCommands ?? (drawingCommands = commands.ToArray());

            var path = new SKPath();
            var isValid = true;

            foreach (var segment in toExecute)
            {
                segment.IsCompleted = true;
                segment.Execute(path, GetCurrentTime(), this);
                isValid = isValid && segment.IsCompleted;
            }

            if (IsClosed) path.Close();
            context.Canvas.DrawPath(path, context.Paint);

            if (!isValid) Invalidate();

        }

        /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.AddCommand(IPathCommand{TPathArgs})" />
        public void AddCommand(IPathCommand<SKPath> segment)
        {
            commands.Add(segment);
            drawingCommands = null;
            Invalidate();
        }

        /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.ContainsCommand(IPathCommand{TPathArgs})" />
        public bool ContainsCommand(IPathCommand<SKPath> segment)
        {
            return commands.Contains(segment);
        }

        /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.RemoveCommand(IPathCommand{TPathArgs})" />
        public void RemoveCommand(IPathCommand<SKPath> segment)
        {
            commands.Remove(segment);
            drawingCommands = null;
            Invalidate();
        }

        /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.ClearCommands" />
        public void ClearCommands()
        {
            commands.Clear();
        }
    }
}
