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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LiveChartsCore.Drawing;

namespace LiveChartsCore.Motion;

/// <summary>
/// Defines a canvas that is able to animate the shapes inside it.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
public class MotionCanvas<TDrawingContext> : IDisposable
    where TDrawingContext : DrawingContext
{
    internal HashSet<IPaint<TDrawingContext>> _paintTasks = new();
    private readonly Stopwatch _stopwatch = new();
    private readonly List<double> _fpsStack = new();
    private long _previousFrameTime;
    private long _previousLogTime;
    private object _sync = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MotionCanvas{TDrawingContext}"/> class.
    /// </summary>
    public MotionCanvas()
    {
        _stopwatch.Start();
    }

    internal bool DisableAnimations { get; set; }

    /// <summary>
    /// Occurs when the visual is invalidated.
    /// </summary>
    public event Action<MotionCanvas<TDrawingContext>>? Invalidated;

    /// <summary>
    /// Occurs when all the visuals in the canvas are valid.
    /// </summary>
    public event Action<MotionCanvas<TDrawingContext>>? Validated;

    /// <summary>
    /// Returns true if the visual is valid.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
    /// </value>
    public bool IsValid { get; private set; }

    /// <summary>
    /// Gets the synchronize object.
    /// </summary>
    /// <value>
    /// The synchronize.
    /// </value>
    public object Sync { get => _sync; internal set => _sync = value ?? new object(); }

    /// <summary>
    /// Gets the animatables collection.
    /// </summary>
    public HashSet<IAnimatable> Trackers { get; } = new HashSet<IAnimatable>();

    /// <summary>
    /// Draws the frame.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    public void DrawFrame(TDrawingContext context)
    {
#if DEBUG
        if (LiveCharts.EnableLogging)
            Trace.WriteLine(
                $"[core canvas frame drawn] ".PadRight(60) +
                $"tread: {Environment.CurrentManagedThreadId}");
#endif

        lock (Sync)
        {
            context.OnBeginDraw();

            var isValid = true;
            var frameTime = _stopwatch.ElapsedMilliseconds;

            var toRemoveGeometries = new List<Tuple<IPaint<TDrawingContext>, IDrawable<TDrawingContext>>>();

            foreach (var task in _paintTasks.Where(x => x is not null).OrderBy(x => x.ZIndex))
            {
                if (DisableAnimations) task.CompleteTransition(null);
                task.IsValid = true;
                task.CurrentTime = frameTime;
                task.InitializeTask(context);

                foreach (var geometry in task.GetGeometries(this))
                {
                    if (geometry is null) continue;
                    if (DisableAnimations) geometry.CompleteTransition(null);

                    geometry.IsValid = true;
                    geometry.CurrentTime = frameTime;
                    if (!task.IsPaused) geometry.Draw(context);

                    isValid = isValid && geometry.IsValid;

                    if (geometry.IsValid && geometry.RemoveOnCompleted)
                        toRemoveGeometries.Add(
                            new Tuple<IPaint<TDrawingContext>, IDrawable<TDrawingContext>>(task, geometry));
                }

                isValid = isValid && task.IsValid;

                if (task.RemoveOnCompleted && task.IsValid) _ = _paintTasks.Remove(task);
                task.Dispose();
            }

            foreach (var tracker in Trackers)
            {
                tracker.IsValid = true;
                tracker.CurrentTime = frameTime;
                isValid = isValid && tracker.IsValid;
            }

            foreach (var tuple in toRemoveGeometries)
            {
                tuple.Item1.RemoveGeometryFromPainTask(this, tuple.Item2);

                // if we removed at least one geometry, we need to redraw the control
                // to ensure it is not present in the next frame
                isValid = false;
            }

#if DEBUG
            var dt = frameTime - _previousFrameTime;
            if (dt == 0) dt = 1;
            _fpsStack.Add(1000 / dt);
            if (_fpsStack.Count > 15) _fpsStack.RemoveAt(0);
            if (frameTime - _previousLogTime > 500)
            {
                if (LiveCharts.EnableLogging)
                    Trace.WriteLine($"[LiveCharts] fps = {_fpsStack.DefaultIfEmpty(0).Average():0.00}");
                _previousLogTime = frameTime;
            }
#endif

            _previousFrameTime = frameTime;
            IsValid = isValid;

            context.OnEndDraw();
        }

        if (IsValid) Validated?.Invoke(this);
    }

    /// <summary>
    /// Gets the drawables count.
    /// </summary>
    /// <value>
    /// The drawables count.
    /// </value>
    public int DrawablesCount => _paintTasks.Count;

    /// <summary>
    /// Invalidates this instance.
    /// </summary>
    /// <returns></returns>
    public void Invalidate()
    {
        IsValid = false;
        Invalidated?.Invoke(this);
    }

    /// <summary>
    /// Adds a drawable task.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <returns></returns>
    public void AddDrawableTask(IPaint<TDrawingContext> task)
    {
        _ = _paintTasks.Add(task);
    }

    /// <summary>
    /// Sets the paint tasks.
    /// </summary>
    /// <param name="tasks">The tasks.</param>
    /// <returns></returns>
    public void SetPaintTasks(HashSet<IPaint<TDrawingContext>> tasks)
    {
        _paintTasks = tasks;
    }

    /// <summary>
    /// Removes the paint task.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <returns></returns>
    public void RemovePaintTask(IPaint<TDrawingContext> task)
    {
        task.ReleaseCanvas(this);
        _ = _paintTasks.Remove(task);
    }

    /// <summary>
    /// Clears the canvas and tasks.
    /// </summary>
    public void Clear()
    {
        foreach (var task in _paintTasks)
            task.ReleaseCanvas(this);
        _paintTasks.Clear();
        Invalidate();
    }

    /// <summary>
    /// Counts the geometries.
    /// </summary>
    /// <returns></returns>
    public int CountGeometries()
    {
        var count = 0;

        foreach (var task in _paintTasks)
            foreach (var geometry in task.GetGeometries(this))
                count++;

        return count;
    }

    /// <summary>
    /// Releases the resources.
    /// </summary>
    public void Dispose()
    {
        foreach (var task in _paintTasks)
            task.ReleaseCanvas(this);
        _paintTasks.Clear();
        Trackers.Clear();
        IsValid = true;
    }
}
