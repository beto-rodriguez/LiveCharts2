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

using Avalonia.Media;
using LiveChartsCore.AvaloniaView.Drawing;
using LiveChartsCore.AvaloniaView.Motion;
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.Motion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveChartsCore.AvaloniaView.Painting
{
    public abstract class PaintTask : Animatable, IDisposable, IDrawableTask<AvaloniaDrawingContext>
    {
        protected FloatMotionProperty strokeThicknessTransition;
        private readonly ColorMotionProperty colorTransition;
        private HashSet<IDrawable<AvaloniaDrawingContext>> geometries = new();

        public PaintTask()
        {
            strokeThicknessTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeThickness), 0f));
            colorTransition = RegisterMotionProperty(new ColorMotionProperty(nameof(Color), new Color()));
        }

        public PaintTask(Color color)
        {
            strokeThicknessTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeThickness), 0f));
            colorTransition = RegisterMotionProperty(
                new ColorMotionProperty(nameof(Color), new Color(color.A, color.R, color.G, color.B)));
        }

        public int ZIndex { get; set; }
        public bool IsStroke { get; set; }
        public bool IsFill { get; set; }
        public float StrokeThickness { get => strokeThicknessTransition.GetMovement(this); set => strokeThicknessTransition.SetMovement(value, this); }
        public Color Color { get => colorTransition.GetMovement(this); set => colorTransition.SetMovement(value, this); }

        public abstract void InitializeTask(AvaloniaDrawingContext drawingContext);

        public IEnumerable<IDrawable<AvaloniaDrawingContext>> GetGeometries()
        {
            foreach (var item in geometries.ToArray())
            {
                yield return item;
            }
        }

        public void SetGeometries(HashSet<IDrawable<AvaloniaDrawingContext>> geometries)
        {
            this.geometries = geometries;
            Invalidate();
        }

        public void AddGeometyToPaintTask(IDrawable<AvaloniaDrawingContext> geometry)
        {
            geometries.Add(geometry);
            Invalidate();
        }

        public void RemoveGeometryFromPainTask(IDrawable<AvaloniaDrawingContext> geometry)
        {
            geometries.Remove(geometry);
            Invalidate();
        }

        public abstract IDrawableTask<AvaloniaDrawingContext> CloneTask();

        public void Dispose()
        {
           
        }
    }


}
