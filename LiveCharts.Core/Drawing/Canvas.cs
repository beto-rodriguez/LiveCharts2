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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LiveChartsCore.Drawing
{
    public class Canvas<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        public readonly Stopwatch stopwatch = new Stopwatch();
        private HashSet<IDrawableTask<TDrawingContext>> paintTasks = new HashSet<IDrawableTask<TDrawingContext>>();
        private bool isValid;

        public Canvas()
        {
            stopwatch.Start();
        }

        public event Action<Canvas<TDrawingContext>> Invalidated;
        public bool IsValid { get => isValid; }

        public void DrawFrame(TDrawingContext context)
        {
            var isValid = true;
            var frameTime = stopwatch.ElapsedMilliseconds;
            context.ClearCanvas();

            foreach (var paint in paintTasks.OrderBy(x => x.ZIndex))
            {
                paint.IsCompleted = true;
                paint.CurrentTime = frameTime;
                paint.InitializeTask(context);

                foreach (var geometry in paint.GetGeometries())
                {
                    geometry.IsCompleted = true;
                    geometry.CurrentTime = frameTime;
                    geometry.Draw(context);

                    isValid = isValid && geometry.IsCompleted;
                    if (geometry.RemoveOnCompleted && geometry.IsCompleted) paint.RemoveGeometryFromPainTask(geometry);
                }

                paint.Dispose();

                isValid = isValid && paint.IsCompleted;
                paint.Dispose();

                if (paint.RemoveOnCompleted && paint.IsCompleted) paintTasks.Remove(paint);
            }

            this.isValid = isValid;
        }

        public void Invalidate()
        {
            isValid = false;
            Invalidated?.Invoke(this);
        }

        public void AddPaintTask(IDrawableTask<TDrawingContext> task)
        {
            paintTasks.Add(task);
            Invalidate();
        }

        public void SetPaintTasks(HashSet<IDrawableTask<TDrawingContext>> tasks)
        {
            paintTasks = tasks;
            Invalidate();
        }

        public void RemovePaintTask(IDrawableTask<TDrawingContext> task)
        {
            paintTasks.Remove(task);
            Invalidate();
        }

        public void ForEachGeometry(Action<IDrawable<TDrawingContext>> predicate) => ForEachGeometry((geometry, paint) => predicate(geometry));

        public void ForEachGeometry(Action<IDrawable<TDrawingContext>, IDrawableTask<TDrawingContext>> predicate)
        {
            foreach (var paint in paintTasks)
                foreach (var geometry in paint.GetGeometries())
                    predicate(geometry, paint);
        }
    }
}
