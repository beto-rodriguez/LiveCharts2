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
    public class MotionCanvas<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        public readonly Stopwatch stopwatch = new();
        private HashSet<IDrawableTask<TDrawingContext>> paintTasks = new();
        private bool isValid;

        public MotionCanvas()
        {
            stopwatch.Start();
        }

        internal HashSet<IDrawable<TDrawingContext>> MeasuredDrawables { get; set; } = new();
        public event Action<MotionCanvas<TDrawingContext>>? Invalidated;
        public bool IsValid { get => isValid; }

        public void DrawFrame(TDrawingContext context)
        {
            var isValid = true;
            var frameTime = stopwatch.ElapsedMilliseconds;
            context.ClearCanvas();

            var toRemoveGeometries = new List<Tuple<IDrawableTask<TDrawingContext>, IDrawable<TDrawingContext>>>();

            foreach (var task in paintTasks.OrderBy(x => x.ZIndex).ToArray())
            {
                task.IsCompleted = true;
                task.CurrentTime = frameTime;
                task.InitializeTask(context);

                foreach (var geometry in task.GetGeometries())
                {
                    geometry.IsCompleted = true;
                    geometry.CurrentTime = frameTime;
                    geometry.Draw(context);

                    isValid = isValid && geometry.IsCompleted;

                    if (geometry.IsCompleted && geometry.RemoveOnCompleted)
                        toRemoveGeometries.Add(
                            new Tuple<IDrawableTask<TDrawingContext>, IDrawable<TDrawingContext>>(task, geometry));
                    //if (!MeasuredDrawables.Contains(geometry))
                    //    toRemoveGeometries.Add(
                    //        new Tuple<IDrawableTask<TDrawingContext>, IDrawable<TDrawingContext>>(task, geometry));
                }

                task.Dispose();

                isValid = isValid && task.IsCompleted;
                task.Dispose();

                if (task.RemoveOnCompleted && task.IsCompleted) paintTasks.Remove(task);
            }

            foreach (var tuple in toRemoveGeometries)
            {
                tuple.Item1.RemoveGeometryFromPainTask(tuple.Item2);
            }

            this.isValid = isValid;
        }

        public int DrawablesCount => paintTasks.Count;

        public void Invalidate()
        {
            isValid = false;
            Invalidated?.Invoke(this);
        }

        public void AddDrawableTask(IDrawableTask<TDrawingContext> task)
        {
            paintTasks.Add(task);
            Invalidate();
#if DEBUG
            Trace.WriteLine("added: " + paintTasks.Count);
#endif
        }

        public void SetPaintTasks(HashSet<IDrawableTask<TDrawingContext>> tasks)
        {
            paintTasks = tasks;
            Invalidate();
#if DEBUG
            Trace.WriteLine("set: " + paintTasks.Count);
#endif
        }

        public void RemovePaintTask(IDrawableTask<TDrawingContext> task)
        {
            paintTasks.Remove(task);
            Invalidate();
#if DEBUG
            Trace.WriteLine("removed: " + paintTasks.Count);
#endif
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
