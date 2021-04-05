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
using LiveChartsCore.Motion;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore.Kernel
{
    public class DefaultPaintTask<TDrawingContext> : IDrawableTask<TDrawingContext>
        where TDrawingContext: DrawingContext
    {
        public bool IsStroke { get; set; }
        public bool IsFill { get; set; }
        public double ZIndex { get; set; }
        public float StrokeThickness { get; set; }
        public bool IsCompleted { get; set; }
        public long CurrentTime { get; set; }
        public bool RemoveOnCompleted { get; set; }
        public RectangleF ClipRectangle { get; set; }

        public void AddGeometyToPaintTask(IDrawable<TDrawingContext> geometry)
        {
            
        }

        public IDrawableTask<TDrawingContext> CloneTask()
        {
            return this;
        }

        public void CompleteTransitions(params string[] propertyName)
        {
        }

        public void Dispose()
        {
        }

        public IEnumerable<IDrawable<TDrawingContext>> GetGeometries()
        {
            return Enumerable.Empty<IDrawable<TDrawingContext>>();
        }

        public IMotionProperty GetTransitionProperty(string propertyName)
        {
            throw new System.NotImplementedException();
        }

        public void InitializeTask(TDrawingContext context)
        {
        }

        public void RemoveGeometryFromPainTask(IDrawable<TDrawingContext> geometry)
        {
        }

        public void RemovePropertyTransition(string propertyName)
        {
        }

        public void SetGeometries(HashSet<IDrawable<TDrawingContext>> geometries)
        {
            throw new System.NotImplementedException();
        }

        public void SetPropertiesTransitions(Animation animation, params string[] propertyName)
        {
            throw new System.NotImplementedException();
        }

        public void CompleteAllTransitions()
        {
            throw new System.NotImplementedException();
        }
    }
}
