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
using System;
using System.Collections.Generic;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class PathGeometry : Geometry, IPathGeometry<SkiaDrawingContext>
    {
        private readonly HashSet<PathCommand> commands = new HashSet<PathCommand>();

        public PathGeometry()
        {

        }

        public bool IsClosed { get; set; }

        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            throw new NotImplementedException();
        }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            if (commands.Count == 0) return;

            SKPath path = new SKPath();

            foreach (var segment in commands)
            {
                segment.Excecute(path);
            }

             if (IsClosed) path.Close();
            context.Canvas.DrawPath(path, paint);
        }

        public void AddCommand(PathCommand segment)
        {
            commands.Add(segment);
            Invalidate();
        }

        public bool ContainesCommad(PathCommand segment)
        {
            return commands.Contains(segment);
        }

        public void RemoveCommand(PathCommand segment)
        {
            commands.Remove(segment);
            Invalidate();
        }

        public void CubicBezierTo(float x0, float y0, float x1, float y1, float x2, float y2)
        {
            var bezier = new CubicBezierSegment(this)
            {
                X0 = x0,
                Y0 = y0,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };
            AddCommand(bezier);
        }

        public void LineTo(float x, float y)
        {
            var line = new LineSegment(this)
            {
                X = x,
                Y = y,
            };
            AddCommand(line);
        }

        public void MoveTo(float x, float y)
        {
            var moveTo = new MoveToPathCommand(this)
            {
                X = x,
                Y = y,
            };
            AddCommand(moveTo);
        }

        public void ClearSegments()
        {
            commands.Clear();
        }
    }
}
