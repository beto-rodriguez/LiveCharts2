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
using LiveChartsCore.Painting;

namespace LiveChartsCore.Motion;

/// <summary>
/// Defines a canvas that is able to animate the shapes inside it.
/// </summary>
public class CoreMotionCanvas : IDisposable
{
    private static readonly Stopwatch s_clock = new();
    internal HashSet<Paint> _paintTasks = [];
    private object _sync = new();

    private int _frames = 0;
    private Stopwatch? _fspSw;
    private double _lastKnowFps = 0;
    private double _totalFrames = 0;
    private double _totalSeconds = 0;

    static CoreMotionCanvas()
    {
        s_clock.Start();
    }

    /// <summary>
    /// Gets the clock elapsed time in milliseconds.
    /// </summary>
    public static long ElapsedMilliseconds => s_clock.ElapsedMilliseconds;

    internal bool DisableAnimations { get; set; }

    /// <summary>
    /// Occurs when the visual is invalidated.
    /// </summary>
    public event Action<CoreMotionCanvas>? Invalidated;

    /// <summary>
    /// Occurs when all the visuals in the canvas are valid.
    /// </summary>
    public event Action<CoreMotionCanvas>? Validated;

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
    public HashSet<Animatable> Trackers { get; } = [];

    /// <summary>
    /// Draws the frame.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    public void DrawFrame<TDrawingContext>(TDrawingContext context)
        where TDrawingContext : DrawingContext
    {
#if DEBUG
        if (LiveCharts.EnableLogging)
            Trace.WriteLine(
                $"[core canvas frame drawn] ".PadRight(60) +
                $"thread: {Environment.CurrentManagedThreadId}");
#endif

        var showFps = LiveCharts.ShowFPS;

        lock (Sync)
        {
            context.OnBeginDraw();

            var isValid = true;

            var toRemoveGeometries = new List<Tuple<Paint, IDrawnElement>>();

            foreach (var task in _paintTasks.Where(x => x is not null).OrderBy(x => x.ZIndex))
            {
                if (DisableAnimations) task.CompleteTransition(null);
                task.IsValid = true;

                context.InitializePaintTask(task);

                foreach (var geometry in task.GetGeometries(this))
                {
                    if (geometry is null) continue;
                    if (DisableAnimations) geometry.CompleteTransition(null);

                    geometry.IsValid = true;

                    if (!task.IsPaused)
                    {
                        context.ActiveOpacity = geometry.Opacity;
                        context.Draw(geometry);
                    }

                    isValid = isValid && geometry.IsValid;

                    if (geometry.IsValid && geometry.RemoveOnCompleted)
                        toRemoveGeometries.Add(
                            new Tuple<Paint, IDrawnElement>(task, geometry));
                }

                isValid = isValid && task.IsValid;

                if (task.RemoveOnCompleted && task.IsValid) _ = _paintTasks.Remove(task);

                context.DisposePaintTask(task);
            }

            foreach (var tracker in Trackers)
            {
                tracker.IsValid = true;
                isValid = isValid && tracker.IsValid;
            }

            foreach (var tuple in toRemoveGeometries)
            {
                tuple.Item1.RemoveGeometryFromPaintTask(this, tuple.Item2);

                // if we removed at least one geometry, we need to redraw the control
                // to ensure it is not present in the next frame
                isValid = false;
            }

            if (showFps)
            {
                MeasureFPS();

                if (_totalSeconds > 0)
                    context.LogOnCanvas(
                        $"[fps] last {_lastKnowFps:N2}, average {_totalFrames / _totalSeconds:N2}");
            }

            IsValid = isValid;

            context.OnEndDraw();
        }

        if (IsValid)
        {
            Validated?.Invoke(this);

            if (showFps)
            {
                _frames = 0;
                _fspSw = null;
            }
        }
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
    public void AddDrawableTask(Paint task)
    {
        if (task == Paint.Default) return;
        _ = _paintTasks.Add(task);
    }

    /// <summary>
    /// Adds a geometry (or geometries) to the canvas.
    /// </summary>
    /// <returns>
    /// The task created to manage the geometries.
    /// </returns>
    public DrawnTask AddGeometry(params IDrawnElement[] geometries)
    {
        var task = new DrawnTask(this, geometries);
        _ = _paintTasks.Add(task);
        return task;
    }

    /// <summary>
    /// Sets the paint tasks.
    /// </summary>
    /// <param name="tasks">The tasks.</param>
    /// <returns></returns>
    public void SetPaintTasks(HashSet<Paint> tasks) => _paintTasks = tasks;

    /// <summary>
    /// Removes the paint task.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <returns></returns>
    public void RemovePaintTask(Paint task)
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

    private void MeasureFPS()
    {
        if (_fspSw is null)
        {
            _fspSw = new();
            _fspSw.Start();
        }

        const int logEach = 20;
        _frames++;

        if (_frames % logEach == 0)
        {
            var elapsedSeconds = _fspSw.ElapsedMilliseconds / 1000d;
            _lastKnowFps = logEach / elapsedSeconds;

            _totalFrames += logEach;
            _totalSeconds += elapsedSeconds;

            _fspSw.Restart();
        }
    }
}
