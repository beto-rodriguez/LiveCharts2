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
        /// The series context
        /// </summary>
        protected SeriesContext<TDrawingContext> seriesContext = new(Enumerable.Empty<IDrawableSeries<TDrawingContext>>());

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
        protected Func<float, float> easingFunction;

        /// <summary>
        /// The draw margin size
        /// </summary>
        protected SizeF drawMarginSize;

        /// <summary>
        /// The draw margin location
        /// </summary>
        protected PointF drawMaringLocation;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Chart{TDrawingContext}"/> class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="defaultPlatformConfig">The default platform configuration.</param>
        public Chart(MotionCanvas<TDrawingContext> canvas, Action<LiveChartsSettings> defaultPlatformConfig)
        {
            this.canvas = canvas;
            canvas.Validated += OnCanvasValidated;
            easingFunction = EasingFunctions.QuadraticOut;
            if (!LiveCharts.IsConfigured) LiveCharts.Configure(defaultPlatformConfig);
            updateThrottler = new ActionThrottler(UpdateThrottlerUnlocked, TimeSpan.FromMilliseconds(10));
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
        public event ChartEventHandler<TDrawingContext>? Measuring;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
        public event ChartEventHandler<TDrawingContext>? UpdateStarted;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
        public event ChartEventHandler<TDrawingContext>? UpdateFinished;

        #region properties

        /// <summary>
        /// Gets or sets the measure work.
        /// </summary>
        /// <value>
        /// The measure work.
        /// </value>
        public object MeasureWork { get; set; } = new object();

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
        public abstract IEnumerable<IDrawableSeries<TDrawingContext>> DrawableSeries { get; }

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
        /// Gets the draw maring location.
        /// </summary>
        /// <value>
        /// The draw maring location.
        /// </value>
        public PointF DrawMaringLocation => drawMaringLocation;

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
        public Func<float, float> EasingFunction => easingFunction;

        #endregion region

        /// <inheritdoc cref="IChart.Update(ChartUpdateParams?)" />
        public abstract void Update(ChartUpdateParams? chartUpdateParams = null);

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
        /// Finds the points near to the specified point.
        /// </summary>
        /// <param name="pointerPosition">The pointer position.</param>
        /// <returns></returns>
        public abstract IEnumerable<TooltipPoint> FindPointsNearTo(PointF pointerPosition);

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

            drawMaringLocation = new PointF(margin.Left, margin.Top);
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

        private void OnCanvasValidated(MotionCanvas<TDrawingContext> chart)
        {
            InvokeOnUpdateFinished();
        }
    }
}
