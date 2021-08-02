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
using System.Drawing;
using System.Linq;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using System.Diagnostics;
using System.Threading;

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

#if DEBUG
        internal ActionThrottler UpdateThrottlerInspector => updateThrottler;
#endif

        /// <summary>
        /// The series context
        /// </summary>
        protected SeriesContext<TDrawingContext> seriesContext = new(Enumerable.Empty<IChartSeries<TDrawingContext>>());

        /// <summary>
        /// The canvas
        /// </summary>
        protected readonly MotionCanvas<TDrawingContext> canvas;

        /// <summary>
        /// The update throttler
        /// </summary>
        protected readonly ActionThrottler updateThrottler;

        /// <summary>
        /// The control size
        /// </summary>
        protected SizeF controlSize = new();

        /// <summary>
        /// The view draw margin
        /// </summary>
        protected Margin? viewDrawMargin = null;

        /// <summary>
        /// The legend position
        /// </summary>
        protected LegendPosition legendPosition;

        /// <summary>
        /// The legend orientation
        /// </summary>
        protected LegendOrientation legendOrientation;

        /// <summary>
        /// The legend
        /// </summary>
        protected IChartLegend<TDrawingContext>? legend;

        /// <summary>
        /// The tool tip position
        /// </summary>
        protected TooltipPosition tooltipPosition;

        /// <summary>
        /// The tool tip finding strategy
        /// </summary>
        protected TooltipFindingStrategy tooltipFindingStrategy;

        /// <summary>
        /// The tool tip
        /// </summary>
        protected IChartTooltip<TDrawingContext>? tooltip;

        /// <summary>
        /// The animations speed
        /// </summary>
        protected TimeSpan animationsSpeed;

        /// <summary>
        /// The easing function
        /// </summary>
        protected Func<float, float>? easingFunction;

        /// <summary>
        /// The draw margin size
        /// </summary>
        protected SizeF drawMarginSize;

        /// <summary>
        /// The draw margin location
        /// </summary>
        protected PointF drawMarginLocation;

        /// <summary>
        /// The previous series
        /// </summary>
        protected IReadOnlyList<IChartSeries<TDrawingContext>> previousSeries = new IChartSeries<TDrawingContext>[0];

        /// <summary>
        /// The previous legend position
        /// </summary>
        protected LegendPosition previousLegendPosition = LegendPosition.Hidden;

        /// <summary>
        /// The preserve first draw
        /// </summary>
        protected bool preserveFirstDraw = false;

        private readonly ActionThrottler _tooltipThrottler;
        private readonly ActionThrottler _panningThrottler;
        private PointF _pointerPosition = new(-10, -10);
        private PointF _pointerPanningPosition = new(-10, -10);
        private PointF _pointerPreviousPanningPosition = new(-10, -10);
        private bool _isPanning = false;
        private bool _isPointerIn = false;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Chart{TDrawingContext}"/> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="defaultPlatformConfig">The default platform configuration.</param>
        /// <param name="lockOnMeasure">Indicates if the thread should lock the measure operation</param>
        protected Chart(
            MotionCanvas<TDrawingContext> canvas,
            Action<LiveChartsSettings> defaultPlatformConfig,
            bool lockOnMeasure = false)
        {
            this.canvas = canvas;
            canvas.Validated += OnCanvasValidated;
            easingFunction = EasingFunctions.QuadraticOut;
            if (!LiveCharts.IsConfigured) LiveCharts.Configure(defaultPlatformConfig);
            updateThrottler = new ActionThrottler(UpdateThrottlerUnlocked, TimeSpan.FromMilliseconds(50));

            PointerDown += Chart_PointerDown;
            PointerMove += Chart_PointerMove;
            PointerUp += Chart_PointerUp;
            PointerLeft += Chart_PointerLeft;

            _tooltipThrottler = new ActionThrottler(TooltipThrottlerUnlocked, TimeSpan.FromMilliseconds(10));
            _panningThrottler = new ActionThrottler(PanningThrottlerUnlocked, TimeSpan.FromMilliseconds(30));
            LockOnMeasure = true;//lockOnMeasure;
        }

        private void TooltipThrottlerUnlocked()
        {
            if (tooltip is null || TooltipPosition == TooltipPosition.Hidden || !_isPointerIn) return;

            View.InvokeOnUIThread(() =>
            {
#if DEBUG
                if (LiveCharts.EnableLogging)
                {
                    Trace.WriteLine(
                        $"[tooltip view thread]".PadRight(60) +
                        $"tread: {Thread.CurrentThread.ManagedThreadId}");
                }
#endif

                var points = FindPointsNearTo(_pointerPosition).ToArray();
                tooltip.Show(points, this);
            });
        }

        private void PanningThrottlerUnlocked()
        {
            if (this is not CartesianChart<TDrawingContext> cartesianChart) return;

            cartesianChart.Pan(
                new PointF(
                (float)(_pointerPanningPosition.X - _pointerPreviousPanningPosition.X),
                (float)(_pointerPanningPosition.Y - _pointerPreviousPanningPosition.Y)));

            _pointerPreviousPanningPosition = new PointF(_pointerPanningPosition.X, _pointerPanningPosition.Y);
        }

        private void Chart_PointerDown(PointF pointerPosition)
        {
            _isPanning = true;
            _pointerPreviousPanningPosition = pointerPosition;
        }

        private void Chart_PointerMove(PointF pointerPosition)
        {
            _pointerPosition = pointerPosition;
            _isPointerIn = true;
            if (tooltip is not null && TooltipPosition != TooltipPosition.Hidden) _tooltipThrottler.Call();
            if (!_isPanning) return;
            _pointerPanningPosition = pointerPosition;
            _panningThrottler.Call();
        }

        private void Chart_PointerLeft()
        {
            _isPointerIn = false;
        }

        private void Chart_PointerUp(PointF pointerPosition)
        {
            if (!_isPanning) return;
            _isPanning = false;
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
        public event ChartEventHandler<TDrawingContext>? Measuring;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
        public event ChartEventHandler<TDrawingContext>? UpdateStarted;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
        public event ChartEventHandler<TDrawingContext>? UpdateFinished;

        internal event Action<PointF> PointerDown;

        internal event Action<PointF> PointerMove;

        internal event Action<PointF> PointerUp;

        internal event Action PointerLeft;

        internal event Action<PanGestureEventArgs> PanGesture;

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
        /// Gets or sets a value indicating whether this it is the first draw of this instance.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this it is the first draw; otherwise, <c>false</c>.
        /// </value>
        public bool IsFirstDraw { get; set; } = true;

        /// <summary>
        /// Gets the series context.
        /// </summary>
        /// <value>
        /// The series context.
        /// </value>
        public SeriesContext<TDrawingContext> SeriesContext => seriesContext;

        /// <summary>
        /// Gets the canvas.
        /// </summary>
        /// <value>
        /// The canvas.
        /// </value>
        public MotionCanvas<TDrawingContext> Canvas => canvas;

        /// <summary>
        /// Gets the drawable series.
        /// </summary>
        /// <value>
        /// The drawable series.
        /// </value>
        public abstract IEnumerable<IChartSeries<TDrawingContext>> DrawableSeries { get; }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        public abstract IChartView<TDrawingContext> View { get; }
        IChartView IChart.View => View;

        /// <summary>
        /// Gets the size of the control.
        /// </summary>
        /// <value>
        /// The size of the control.
        /// </value>
        public SizeF ControlSize => controlSize;

        /// <summary>
        /// Gets the draw margin location.
        /// </summary>
        /// <value>
        /// The draw margin location.
        /// </value>
        public PointF DrawMarginLocation => drawMarginLocation;

        /// <summary>
        /// Gets the size of the draw margin.
        /// </summary>
        /// <value>
        /// The size of the draw margin.
        /// </value>
        public SizeF DrawMarginSize => drawMarginSize;

        /// <summary>
        /// Gets the legend position.
        /// </summary>
        /// <value>
        /// The legend position.
        /// </value>
        public LegendPosition LegendPosition => legendPosition;

        /// <summary>
        /// Gets the legend orientation.
        /// </summary>
        /// <value>
        /// The legend orientation.
        /// </value>
        public LegendOrientation LegendOrientation => legendOrientation;

        /// <summary>
        /// Gets the legend.
        /// </summary>
        /// <value>
        /// The legend.
        /// </value>
        public IChartLegend<TDrawingContext>? Legend => legend;

        /// <summary>
        /// Gets the tooltip position.
        /// </summary>
        /// <value>
        /// The tooltip position.
        /// </value>
        public TooltipPosition TooltipPosition => tooltipPosition;

        /// <summary>
        /// Gets the tooltip finding strategy.
        /// </summary>
        /// <value>
        /// The tooltip finding strategy.
        /// </value>
        public TooltipFindingStrategy TooltipFindingStrategy => tooltipFindingStrategy;

        /// <summary>
        /// Gets the tooltip.
        /// </summary>
        /// <value>
        /// The tooltip.
        /// </value>
        public IChartTooltip<TDrawingContext>? Tooltip => tooltip;

        /// <summary>
        /// Gets the animations speed.
        /// </summary>
        /// <value>
        /// The animations speed.
        /// </value>
        public TimeSpan AnimationsSpeed => animationsSpeed;

        /// <summary>
        /// Gets the easing function.
        /// </summary>
        /// <value>
        /// The easing function.
        /// </value>
        public Func<float, float>? EasingFunction => easingFunction;

        /// <summary>
        /// Gets or sets the updater throttler.
        /// </summary>
        /// <value>
        /// The updater throttler.
        /// </value>
        public TimeSpan UpdaterThrottler { get => updateThrottler.ThrottlerTimeSpan; set => updateThrottler.ThrottlerTimeSpan = value; }

        object IChart.Canvas => Canvas;

        #endregion region

        /// <inheritdoc cref="IChart.Update(ChartUpdateParams?)" />
        public abstract void Update(ChartUpdateParams? chartUpdateParams = null);

        /// <summary>
        /// Finds the points near to the specified point.
        /// </summary>
        /// <param name="pointerPosition">The pointer position.</param>
        /// <returns></returns>
        public abstract IEnumerable<TooltipPoint> FindPointsNearTo(PointF pointerPosition);

        internal void InvokePointerDown(PointF point)
        {
            PointerDown?.Invoke(point);
        }

        internal void InvokePointerMove(PointF point)
        {
            PointerMove?.Invoke(point);
        }

        internal void InvokePointerUp(PointF point)
        {
            PointerUp?.Invoke(point);
        }

        internal void InvokePointerLeft()
        {
            PointerLeft?.Invoke();
        }

        internal void InvokePanGestrue(PanGestureEventArgs eventArgs)
        {
            PanGesture?.Invoke(eventArgs);
        }

        /// <summary>
        /// Measures this chart.
        /// </summary>
        /// <returns></returns>
        protected abstract void Measure();

        /// <summary>
        /// Called when the updated the throttler is unlocked.
        /// </summary>
        /// <returns></returns>
        protected abstract void UpdateThrottlerUnlocked();

        /// <summary>
        /// Sets the draw margin.
        /// </summary>
        /// <param name="controlSize">Size of the control.</param>
        /// <param name="margin">The margin.</param>
        /// <returns></returns>
        protected void SetDrawMargin(SizeF controlSize, Margin margin)
        {
            drawMarginSize = new SizeF
            {
                Width = controlSize.Width - margin.Left - margin.Right,
                Height = controlSize.Height - margin.Top - margin.Bottom
            };

            drawMarginLocation = new PointF(margin.Left, margin.Top);
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

        /// <summary>
        /// SDetermines whether the series miniature changed or not.
        /// </summary>
        /// <param name="newSeries">The new series.</param>
        /// <param name="position">The legend position.</param>
        /// <returns></returns>
        protected bool SeriesMiniatureChanged(IReadOnlyList<IChartSeries<TDrawingContext>> newSeries, LegendPosition position)
        {
            if (position == LegendPosition.Hidden && previousLegendPosition == LegendPosition.Hidden) return false;
            if (position != previousLegendPosition) return true;
            if (previousSeries.Count != newSeries.Count) return true;

            for (var i = 0; i < newSeries.Count; i++)
            {
                if (i + 1 > previousSeries.Count) return true;

                var a = previousSeries[i];
                var b = newSeries[i];

                if (!a.MiniatureEquals(b)) return true;
                //if (a.Name != b.Name || a.Fill != b.Fill || a.Stroke != b.Stroke) return true;
            }

            return false;
        }

        private void OnCanvasValidated(MotionCanvas<TDrawingContext> chart)
        {
            InvokeOnUpdateFinished();
        }
    }
}
