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
using System.Text;
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
    private int _frames = 0;
    private Stopwatch? _fspSw;
    private int _jitteredDrawCount;
    private double _totalDrawTime = 0;
    private double _lastDrawTime = 0;
    private double _totalFrames = 0;
    private double _totalSeconds = 0;
    internal TimeSpan _nextFrameDelay = s_baseFrameDelay;
    private static readonly double s_ticksPerMillisecond = Stopwatch.Frequency / 1000d;
    private static readonly TimeSpan s_baseFrameDelay = TimeSpan.FromMilliseconds(1000d / LiveCharts.RenderingSettings.LiveChartsRenderLoopFPS);
    private static readonly long s_jitterThreshold = s_baseFrameDelay.Ticks / 2;
    internal LvcColor _virtualBackgroundColor;
    internal static string? s_externalRenderer;
    internal static string? s_rendererName;
    internal static string? s_tickerName;

    static CoreMotionCanvas()
    {
        s_clock.Start();
    }

    internal delegate void FrameRequestHandler(DrawingContext context);

    /// <summary>
    /// Gets the clock elapsed time in milliseconds.
    /// </summary>
    public static long ElapsedMilliseconds
    {
        get
        {
#if DEBUG
            if (DebugElapsedMilliseconds > -1)
                return DebugElapsedMilliseconds;
#endif
            return s_clock.ElapsedMilliseconds;
        }
    }

#if DEBUG
    internal static long DebugElapsedMilliseconds { get; set; } = -1;
#endif

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
    public object Sync { get; internal set => field = value ?? new object(); } = new();

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

        var showFps = LiveCharts.RenderingSettings.ShowFPS;
        var drawStartTime = s_clock.ElapsedTicks;

        lock (Sync)
        {
            context.OnBeginDraw();

            var isValid = true;

            var toRemoveGeometries = new List<Tuple<Paint, IDrawnElement>>();

            foreach (var task in _paintTasks.Where(x => x is not null && x != Paint.Default).OrderBy(x => x.ZIndex))
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
                MeasureFPS(drawStartTime);

                if (_totalSeconds > 0)
                {
                    var sb = new StringBuilder();

#if DEBUG
                    sb.Append($"[~~ {_totalFrames / _totalSeconds:N2} ~~] FPS (DEBUG)");
#else
                    sb.Append($"[ {_totalFrames / _totalSeconds:N2} ] FPS");
#endif
                    sb.Append($"`[ {_lastDrawTime:N2}ms / {_totalDrawTime / _totalFrames:N2}ms ] render time last / avrg");

                    if (s_externalRenderer is null)
                    {
                        sb.Append($"`[ {(LiveCharts.RenderingSettings.UseGPU ? "GPU" : "CPU")} ] via {s_rendererName}");
                        var isVSynced = LiveCharts.RenderingSettings.UseGPU && LiveCharts.RenderingSettings.TryUseVSync;
                        sb.Append($"`[ {(isVSynced ? "VSync" : "VSync disabled")} ] handled by {s_tickerName}");
                    }
                    else
                    {
                        sb.Append($"`{s_externalRenderer} handling GPU / VSync");
                    }

                    if (_jitteredDrawCount > 0)
                        sb.Append(
                              $"`[ {_jitteredDrawCount} ] jittered draws");

                    context.LogOnCanvas(sb.ToString());
                }
            }

            IsValid = isValid;

            context.OnEndDraw();
        }

        if (IsValid)
        {
            Validated?.Invoke(this);

            if (showFps)
            {
                // restart the count when the canvas is valid.
                _frames = 0;
                _fspSw = null;
                _totalDrawTime = 0;
                _lastDrawTime = 0;
                _totalFrames = 0;
                _totalSeconds = 0;
            }
        }

        if (!LiveCharts.RenderingSettings.TryUseVSync)
        {
            var timeInDrawOperation = s_clock.ElapsedTicks - drawStartTime;
            var delay = s_baseFrameDelay.Ticks - timeInDrawOperation;

            TimeSpan frameDelay;

            if (delay <= s_jitterThreshold)
            {
                _jitteredDrawCount++;
                frameDelay = s_baseFrameDelay;
            }
            else
            {
                frameDelay = new TimeSpan(delay);
            }

            _nextFrameDelay = frameDelay;
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

    private void MeasureFPS(long drawStartTime)
    {
        if (s_clock.ElapsedMilliseconds < 3000)
            return; // we only start measuring after 3 seconds, to improve accuracy.

        _totalDrawTime += (s_clock.ElapsedTicks - drawStartTime) / s_ticksPerMillisecond;

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

            _totalFrames += logEach;
            _totalSeconds += elapsedSeconds;

            // it not exactly the last frame time, is the 20th frame time
            // so we can actually read the time in the log.
            _lastDrawTime = (s_clock.ElapsedTicks - drawStartTime) / s_ticksPerMillisecond;

            _fspSw.Restart();
        }
    }
}
