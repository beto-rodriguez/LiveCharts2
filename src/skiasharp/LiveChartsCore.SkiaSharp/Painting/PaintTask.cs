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
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Motion;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveChartsCore.SkiaSharpView.Painting
{
    /// <summary>
    /// Defines a brush that support animations, this class is based on <see cref="SKPaint"/> 
    /// class (https://docs.microsoft.com/en-us/dotnet/api/skiasharp.skpaint?view=skiasharp-1.68.2). Also see https://api.skia.org/classSkPaint.html
    /// </summary>
    public abstract class PaintTask : Animatable, IDisposable, IDrawableTask<SkiaSharpDrawingContext>
    {
        protected SKPaint skiaPaint;
        private HashSet<IDrawable<SkiaSharpDrawingContext>> geometries = new HashSet<IDrawable<SkiaSharpDrawingContext>>();
        private IDrawable<SkiaSharpDrawingContext>[] actualGeometries = null;
        protected FloatMotionProperty strokeWidthTransition;
        private readonly ColorMotionProperty colorTransition;

        public PaintTask()
        {
            strokeWidthTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeThickness), 0f));
            colorTransition = RegisterMotionProperty(new ColorMotionProperty(nameof(Color), new SKColor()));
        }

        public PaintTask(SKColor color)
        {
            strokeWidthTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeThickness), 0f));
            colorTransition = RegisterMotionProperty(
                new ColorMotionProperty(nameof(Color), new SKColor(color.Red, color.Green, color.Blue, color.Alpha)));
        }

        double IDrawableTask<SkiaSharpDrawingContext>.ZIndex { get; set; }
        public float StrokeThickness { get => strokeWidthTransition.GetMovement(this); set => strokeWidthTransition.SetMovement(value, this); }
        public SKPaintStyle Style { get; set; }
        public bool IsStroke { get; set; }
        public bool IsFill { get; set; }
        public bool IsAntialias { get; set; } = true;

        public SKColor Color { get => colorTransition.GetMovement(this); set => colorTransition.SetMovement(value, this); }

        public abstract void InitializeTask(SkiaSharpDrawingContext drawingContext);

        public IEnumerable<IDrawable<SkiaSharpDrawingContext>> GetGeometries()
        {
            var g = actualGeometries ?? (actualGeometries = geometries.ToArray());
            foreach (var item in g)
            {
                yield return item;
            }
        }

        public void SetGeometries(HashSet<IDrawable<SkiaSharpDrawingContext>> geometries)
        {
            this.geometries = geometries;
            actualGeometries = null;
            Invalidate();
        }

        public void AddGeometyToPaintTask(IDrawable<SkiaSharpDrawingContext> geometry)
        {
            geometries.Add(geometry);
            actualGeometries = null;
            Invalidate();
        }

        public void RemoveGeometryFromPainTask(IDrawable<SkiaSharpDrawingContext> geometry)
        {
            geometries.Remove(geometry);
            actualGeometries = null;
            Invalidate();
        }

        public abstract IDrawableTask<SkiaSharpDrawingContext> CloneTask();

        public void Dispose()
        {
            skiaPaint?.Dispose();
            skiaPaint = null;
        }
    }
}
