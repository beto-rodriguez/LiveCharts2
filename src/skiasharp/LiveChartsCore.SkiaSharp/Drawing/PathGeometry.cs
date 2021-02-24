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

namespace LiveChartsCore.SkiaSharpView.Drawing
{
    public class PathGeometry : Drawable, IPathGeometry<SkiaDrawingContext, SKPath>
    {
        private readonly HashSet<IPathCommand<SKPath>> commands = new HashSet<IPathCommand<SKPath>>();

        public PathGeometry()
        {
        }

        public bool IsClosed { get; set; }

        public override void Draw(SkiaDrawingContext context)
        {
            if (commands.Count == 0) return;

            SKPath path = new SKPath();

            var isValid = true;
            foreach (var segment in commands)
            {
                segment.IsCompleted = true;
                segment.Execute(path, GetCurrentTime(), this);
                isValid = isValid && segment.IsCompleted;
            }

            if (IsClosed) path.Close();
            context.Canvas.DrawPath(path, context.Paint);

            if (!isValid) Invalidate();
        }

        public void AddCommand(IPathCommand<SKPath> segment)
        {
            commands.Add(segment);
            Invalidate();
        }

        public bool ContainsCommand(IPathCommand<SKPath> segment)
        {
            return commands.Contains(segment);
        }

        public void RemoveCommand(IPathCommand<SKPath> segment)
        {
            commands.Remove(segment);
            Invalidate();
        }

        public void ClearCommands()
        {
            commands.Clear();
        }
    }
}
