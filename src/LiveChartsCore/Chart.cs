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

using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using System;
using System.Collections.Generic;
using System.Linq;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a chart,
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="IChart" />
    public abstract class Chart<TDrawingContext> : IChart
        where TDrawingContext : DrawingContext
    {
        #region fields

        /// <summary>
        /// The preserve first draw
        /// </summary>
        protected bool preserveFirstDraw = false;
        private readonly ActionThrottler _updateThrottler;
        private readonly ActionThrottler _tooltipThrottler;
        private readonly ActionThrottler _panningThrottler;
        private LvcPoint _pointerPosition = new(-10, -10);
        private LvcPoint _pointerPanningPosition = new(-10, -10);
        private LvcPoint _pointerPreviousPanningPosition = new(-10, -10);
        private bool _isPanning = false;
        private bool _isPointerIn = false;
        private readonly Dictionary<ChartPoint, object> _activePoints = new();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Chart{TDrawingContext}"/> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="defaultPlatformConfig">The default platform configuration.</param>
        /// <param name="view">The chart view.</param>
        /// <param name="lockOnMeasure">Indicates if the thread should lock the measure operation</param>
        protected Chart(
            MotionCanvas<TDrawingContext> canvas,
            Action<LiveChartsSettings> defaultPlatformConfig,
            IChartView view,
            bool lockOnMeasure = false)
        {
            Canvas = canvas;
            canvas.Validated += OnCanvasValidated;
            EasingFunction = EasingFunctions.QuadraticOut;
            if (!LiveCharts.IsConfigured) LiveCharts.Configure(defaultPlatformConfig);

            _updateThrottler = view.DesignerMode
                    ? new ActionThrottler(() => Task.CompletedTask, TimeSpan.FromMilliseconds(50))
                    : new ActionThrottler(UpdateThrottlerUnlocked, TimeSpan.FromMilliseconds(50));

            PointerDown += Chart_PointerDown;
            PointerMove += Chart_PointerMove;
            PointerUp += Chart_PointerUp;
            PointerLeft += Chart_PointerLeft;

            _tooltipThrottler = new ActionThrottler(TooltipThrottlerUnlocked, TimeSpan.FromMilliseconds(10));
            _panningThrottler = new ActionThrottler(PanningThrottlerUnlocked, TimeSpan.FromMilliseconds(30));
            LockOnMeasure = true;//lockOnMeasure;
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
        public event ChartEventHandler<TDrawingContext>? Measuring;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
        public event ChartEventHandler<TDrawingContext>? UpdateStarted;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
        public event ChartEventHandler<TDrawingContext>? UpdateFinished;

        internal event Action<LvcPoint> PointerDown;

        internal event Action<LvcPoint> PointerMove;

        internal event Action<LvcPoint> PointerUp;

        internal event Action PointerLeft;

        internal event Action<PanGestureEventArgs>? PanGesture;

        #region properties

        /// <summary>
        /// Indicates whether the thread should lock the measure operation.
        /// </summary>
        protected bool LockOnMeasure { get; }

        /// <summary>
        /// Gets the measure work.
        /// </summary>
        /// <value>
        /// The measure work.
        /// </value>
        public object MeasureWork { get; protected set; } = new object();

        /// <summary>
        /// Gets or sets the theme identifier.
        /// </summary>
        /// <value>
        /// The theme identifier.
        /// </value>
        public object ThemeId { get; protected set; } = new object();

        /// <summary>
        /// Gets whether the control is loaded.
        /// </summary>
        public bool IsLoaded { get; protected set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether this it is the first draw of this instance.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this it is the first draw; otherwise, <c>false</c>.
        /// </value>
        public bool IsFirstDraw { get; protected set; } = true;

        /// <summary>
        /// Gets the canvas.
        /// </summary>
        /// <value>
        /// The canvas.
        /// </value>
        public MotionCanvas<TDrawingContext> Canvas { get; private set; }

        /// <summary>
        /// Gets the drawable series.
        /// </summary>
        /// <value>
        /// The drawable series.
        /// </value>
        public abstract IEnumerable<IChartSeries<TDrawingContext>> ChartSeries { get; }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        public abstract IChartView<TDrawingContext> View { get; }

        IChartView IChart.View => View;

        /// <summary>
        /// The series context
        /// </summary>
        public SeriesContext<TDrawingContext> SeriesContext { get; protected set; } = new(Enumerable.Empty<IChartSeries<TDrawingContext>>());

        /// <summary>
        /// Gets the size of the control.
        /// </summary>
        /// <value>
        /// The size of the control.
        /// </value>
        public LvcSize ControlSize { get; protected set; } = new LvcSize();

        /// <summary>
        /// Gets the draw margin location.
        /// </summary>
        /// <value>
        /// The draw margin location.
        /// </value>
        public LvcPoint DrawMarginLocation { get; protected set; } = new LvcPoint();

        /// <summary>
        /// Gets the size of the draw margin.
        /// </summary>
        /// <value>
        /// The size of the draw margin.
        /// </value>
        public LvcSize DrawMarginSize { get; protected set; } = new LvcSize();

        /// <summary>
        /// Gets the legend position.
        /// </summary>
        /// <value>
        /// The legend position.
        /// </value>
        public LegendPosition LegendPosition { get; protected set; }

        /// <summary>
        /// Gets the legend orientation.
        /// </summary>
        /// <value>
        /// The legend orientation.
        /// </value>
        public LegendOrientation LegendOrientation { get; protected set; }

        /// <summary>
        /// Gets the legend.
        /// </summary>
        /// <value>
        /// The legend.
        /// </value>
        public IChartLegend<TDrawingContext>? Legend { get; protected set; }

        /// <summary>
        /// Gets the tooltip position.
        /// </summary>
        /// <value>
        /// The tooltip position.
        /// </value>
        public TooltipPosition TooltipPosition { get; protected set; }

        /// <summary>
        /// Gets the tooltip finding strategy.
        /// </summary>
        /// <value>
        /// The tooltip finding strategy.
        /// </value>
        public TooltipFindingStrategy TooltipFindingStrategy { get; protected set; }

        /// <summary>
        /// Gets the tooltip.
        /// </summary>
        /// <value>
        /// The tooltip.
        /// </value>
        public IChartTooltip<TDrawingContext>? Tooltip { get; protected set; }

        /// <summary>
        /// Gets the animations speed.
        /// </summary>
        /// <value>
        /// The animations speed.
        /// </value>
        public TimeSpan AnimationsSpeed { get; protected set; }

        /// <summary>
        /// Gets the easing function.
        /// </summary>
        /// <value>
        /// The easing function.
        /// </value>
        public Func<float, float>? EasingFunction { get; protected set; }

        /// <summary>
        /// Gets or sets the updater throttler.
        /// </summary>
        /// <value>
        /// The updater throttler.
        /// </value>
        public TimeSpan UpdaterThrottler
        {
            get => _updateThrottler.ThrottlerTimeSpan;
            set => _updateThrottler.ThrottlerTimeSpan = value;
        }

        /// <summary>
        /// Gets the previous legend position.
        /// </summary>
        public LegendPosition PreviousLegendPosition { get; protected set; }

        /// <summary>
        /// Gets the previous series.
        /// </summary>
        public IReadOnlyList<IChartSeries<TDrawingContext>> PreviousSeries { get; protected set; } = new IChartSeries<TDrawingContext>[0];

        object IChart.Canvas => Canvas;

        #endregion region

        /// <inheritdoc cref="IChart.Update(ChartUpdateParams?)" />
        public virtual void Update(ChartUpdateParams? chartUpdateParams = null)
        {
            chartUpdateParams ??= new ChartUpdateParams();

            if (chartUpdateParams.IsAutomaticUpdate && !View.AutoUpdateEnabled) return;

            if (!chartUpdateParams.Throttling)
            {
                _updateThrottler.ForceCall();
                return;
            }

            _updateThrottler.Call();
        }

        /// <summary>
        /// Finds the points near to the specified point.
        /// </summary>
        /// <param name="pointerPosition">The pointer position.</param>
        /// <returns></returns>
        public abstract TooltipPoint[] FindPointsNearTo(LvcPoint pointerPosition);

        /// <summary>
        /// Loads the control resources.
        /// </summary>
        public virtual void Load()
        {
            IsLoaded = true;
            IsFirstDraw = true;
            Update();
        }

        /// <summary>
        /// Unloads the control.
        /// </summary>
        public virtual void Unload()
        {
            IsLoaded = false;
        }

        internal void ClearTooltipData()
        {
            foreach (var point in _activePoints.Keys.ToArray())
            {
                point.Context.Series.OnPointerLeft(point);
                _ = _activePoints.Remove(point);
            }

            Canvas.Invalidate();
        }

        internal void InvokePointerDown(LvcPoint point)
        {
            PointerDown?.Invoke(point);
        }

        internal void InvokePointerMove(LvcPoint point)
        {
            PointerMove?.Invoke(point);
        }

        internal void InvokePointerUp(LvcPoint point)
        {
            PointerUp?.Invoke(point);
        }

        internal void InvokePointerLeft()
        {
            PointerLeft?.Invoke();
        }

        /// <summary>
        /// Measures this chart.
        /// </summary>
        /// <returns></returns>
        protected abstract void Measure();

        /// <summary>
        /// Sets the draw margin.
        /// </summary>
        /// <param name="controlSize">Size of the control.</param>
        /// <param name="margin">The margin.</param>
        /// <returns></returns>
        protected void SetDrawMargin(LvcSize controlSize, Margin margin)
        {
            DrawMarginSize = new LvcSize
            {
                Width = controlSize.Width - margin.Left - margin.Right,
                Height = controlSize.Height - margin.Top - margin.Bottom
            };

            DrawMarginLocation = new LvcPoint(margin.Left, margin.Top);
        }

        /// <summary>
        /// Invokes the <see cref="Measuring"/> event.
        /// </summary>
        /// <returns></returns>
        protected void InvokeOnMeasuring()
        {
            Measuring?.Invoke(View);
        }

        /// <summary>
        /// Invokes the on update started.
        /// </summary>
        /// <returns></returns>
        protected void InvokeOnUpdateStarted()
        {
            UpdateStarted?.Invoke(View);
        }

        /// <summary>
        /// Invokes the on update finished.
        /// </summary>
        /// <returns></returns>
        protected void InvokeOnUpdateFinished()
        {
            UpdateFinished?.Invoke(View);
        }

        internal void InvokePanGestrue(PanGestureEventArgs eventArgs)
        {
            PanGesture?.Invoke(eventArgs);
        }

        /// <summary>
        /// SDetermines whether the series miniature changed or not.
        /// </summary>
        /// <param name="newSeries">The new series.</param>
        /// <param name="position">The legend position.</param>
        /// <returns></returns>
        protected virtual bool SeriesMiniatureChanged(IReadOnlyList<IChartSeries<TDrawingContext>> newSeries, LegendPosition position)
        {
            if (position == LegendPosition.Hidden && PreviousLegendPosition == LegendPosition.Hidden) return false;
            if (position != PreviousLegendPosition) return true;
            if (PreviousSeries.Count != newSeries.Count) return true;

            for (var i = 0; i < newSeries.Count; i++)
            {
                if (i + 1 > PreviousSeries.Count) return true;

                var a = PreviousSeries[i];
                var b = newSeries[i];

                if (!a.MiniatureEquals(b)) return true;
                //if (a.Name != b.Name || a.Fill != b.Fill || a.Stroke != b.Stroke) return true;
            }

            return false;
        }

        /// <summary>
        /// Called when the updated the throttler is unlocked.
        /// </summary>
        /// <returns></returns>
        protected virtual Task UpdateThrottlerUnlocked()
        {
            return Task.Run(() =>
            {
                View.InvokeOnUIThread(() =>
                {
                    lock (Canvas.Sync)
                    {
                        Measure();
                    }
                });
            });
        }

        private Task TooltipThrottlerUnlocked()
        {
            return Task.Run(() =>
                 View.InvokeOnUIThread(() =>
                 {
                     lock (Canvas.Sync)
                     {
#if DEBUG
                         if (LiveCharts.EnableLogging)
                         {
                             Trace.WriteLine(
                                 $"[tooltip view thread]".PadRight(60) +
                                 $"tread: {Thread.CurrentThread.ManagedThreadId}");
                         }
#endif
                         if (Tooltip is null || TooltipPosition == TooltipPosition.Hidden || !_isPointerIn) return;

                         // TODO:
                         // all this needs a performance review...
                         // it should not be crital, should not be even close to be the 'bottle neck' in a case where
                         // we face perfomance issues.

                         var points = FindPointsNearTo(_pointerPosition).ToArray();

                         if (!points.Any())
                         {
                             ClearTooltipData();
                             return;
                         }

                         if (_activePoints.Count > 0 && points.All(x => _activePoints.ContainsKey(x.Point))) return;

                         var o = new object();
                         foreach (var tooltipPoint in points)
                         {
                             tooltipPoint.Point.Context.Series.OnPointerEnter(tooltipPoint.Point);
                             _activePoints[tooltipPoint.Point] = o;
                         }

                         foreach (var point in _activePoints.Keys.ToArray())
                         {
                             if (_activePoints[point] == o) continue;
                             point.Context.Series.OnPointerLeft(point);
                             _ = _activePoints.Remove(point);
                         }

                         Tooltip.Show(points, this);

                         Canvas.Invalidate();
                     }
                 }));
        }

        private Task PanningThrottlerUnlocked()
        {
            return Task.Run(() =>
                View.InvokeOnUIThread(() =>
                {
                    if (this is not CartesianChart<TDrawingContext> cartesianChart) return;

                    lock (Canvas.Sync)
                    {
                        cartesianChart.Pan(
                        new LvcPoint(
                            (float)(_pointerPanningPosition.X - _pointerPreviousPanningPosition.X),
                            (float)(_pointerPanningPosition.Y - _pointerPreviousPanningPosition.Y)));
                        _pointerPreviousPanningPosition = new LvcPoint(_pointerPanningPosition.X, _pointerPanningPosition.Y);
                    }
                }));
        }

        private void OnCanvasValidated(MotionCanvas<TDrawingContext> chart)
        {
            InvokeOnUpdateFinished();
        } 

        private void Chart_PointerDown(LvcPoint pointerPosition)
        {
            _isPanning = true;
            _pointerPreviousPanningPosition = pointerPosition;
        }

        private void Chart_PointerMove(LvcPoint pointerPosition)
        {
            _pointerPosition = pointerPosition;
            _isPointerIn = true;
            if (Tooltip is not null && TooltipPosition != TooltipPosition.Hidden) _tooltipThrottler.Call();
            if (!_isPanning) return;
            _pointerPanningPosition = pointerPosition;
            _panningThrottler.Call();
        }

        private void Chart_PointerLeft()
        {
            _isPointerIn = false;
        }

        private void Chart_PointerUp(LvcPoint pointerPosition)
        {
            if (!_isPanning) return;
            _isPanning = false;
        }
    }
}
