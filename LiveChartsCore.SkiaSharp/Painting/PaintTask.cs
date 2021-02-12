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
using LiveChartsCore.SkiaSharp.Drawing;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace LiveChartsCore.SkiaSharp.Painting
{
    /// <summary>
    /// Defines a brush that support animations, this class is based on <see cref="SKPaint"/> 
    /// class (https://docs.microsoft.com/en-us/dotnet/api/skiasharp.skpaint?view=skiasharp-1.68.2). Also see https://api.skia.org/classSkPaint.html
    /// </summary>
    public abstract class PaintTask : Animatable, IDisposable, IDrawableTask<SkiaDrawingContext>
    {
        protected SKPaint skiaPaint;
        private HashSet<IGeometry<SkiaDrawingContext>> geometries = new HashSet<IGeometry<SkiaDrawingContext>>();
        protected FloatMotionProperty strokeWidthTransition;

        public PaintTask()
        {
            strokeWidthTransition = RegisterTransitionProperty(new FloatMotionProperty(nameof(StrokeWidth), 0f));
        }

        public int ZIndex { get; set; }
        public float StrokeWidth { get => strokeWidthTransition.GetMovement(this); set => strokeWidthTransition.SetMovement(value, this); }
        public SKPaintStyle Style { get; set; }
        public bool IsStroke { get; set; }
        public bool IsFill { get; set; }
        

        public abstract void InitializeTask(SkiaDrawingContext drawingContext);

        public IEnumerable<IGeometry<SkiaDrawingContext>> GetGeometries()
        {
            foreach (var item in geometries)
            {
                yield return item;
            }
        }

        public void SetGeometries(HashSet<IGeometry<SkiaDrawingContext>> geometries)
        {
            this.geometries = geometries;
            Invalidate();
        }

        public void AddGeometyToPaintTask(IGeometry<SkiaDrawingContext> geometry)
        {
            geometries.Add(geometry);
            Invalidate();
        }

        public void RemoveGeometryFromPainTask(IGeometry<SkiaDrawingContext> geometry)
        {
            geometries.Remove(geometry);
            Invalidate();
        }

        public abstract IDrawableTask<SkiaDrawingContext> CloneTask();

        public void Dispose()
        {
            skiaPaint?.Dispose();
            skiaPaint = null;
        }
    }
}
