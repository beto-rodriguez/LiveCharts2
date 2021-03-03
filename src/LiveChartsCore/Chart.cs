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

using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using LiveChartsCore.Rx;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore
{
    public abstract class Chart<TDrawingContext> : IChart
        where TDrawingContext : DrawingContext
    {
        protected object measureWorker = new object();
        protected HashSet<IDrawable<TDrawingContext>> measuredDrawables = new HashSet<IDrawable<TDrawingContext>>();
        protected SeriesContext<TDrawingContext> seriesContext = new SeriesContext<TDrawingContext>(Enumerable.Empty<IDrawableSeries<TDrawingContext>>());
        protected readonly Canvas<TDrawingContext> canvas;
        protected readonly ActionThrottler updateThrottler;
        private readonly Dictionary<string, IDrawableTask<TDrawingContext>> states = new Dictionary<string, IDrawableTask<TDrawingContext>>();

        // view copied properties
        protected SizeF controlSize = new SizeF();
        protected Margin? viewDrawMargin = null;
        protected LegendPosition legendPosition;
        protected LegendOrientation legendOrientation;
        protected IChartLegend<TDrawingContext>? legend;
        protected TooltipPosition tooltipPosition;
        protected TooltipFindingStrategy tooltipFindingStrategy;
        protected IChartTooltip<TDrawingContext>? tooltip;
        protected TimeSpan animationsSpeed;
        protected Func<float, float> easingFunction;
        protected SizeF drawMarginSize;
        protected PointF drawMaringLocation;

        public Chart(Canvas<TDrawingContext> canvas, Action<LiveChartsSettings> defaultPlatformConfig)
        {
            this.canvas = canvas;
            updateThrottler = new ActionThrottler(TimeSpan.FromSeconds(300));
            updateThrottler.Unlocked += UpdateThrottlerUnlocked;
            easingFunction = EasingFunctions.QuadraticOut;
            if (!LiveCharts.IsConfigured) LiveCharts.Configure(defaultPlatformConfig);
        }

        public object MeasureWorker => measureWorker;
        public HashSet<IDrawable<TDrawingContext>> MeasuredDrawables => measuredDrawables;
        public SeriesContext<TDrawingContext> SeriesContext => seriesContext;
        public Canvas<TDrawingContext> Canvas => canvas;
        public abstract IEnumerable<IDrawableSeries<TDrawingContext>> DrawableSeries { get; }
        public abstract IChartView<TDrawingContext> View { get; }
        IChartView IChart.View => View;

        public SizeF ControlSize => controlSize;
        public PointF DrawMaringLocation => drawMaringLocation;
        public SizeF DrawMarginSize => drawMarginSize;

        public LegendPosition LegendPosition => legendPosition;
        public LegendOrientation LegendOrientation => legendOrientation;
        public IChartLegend<TDrawingContext>? Legend => legend;
        public TooltipPosition TooltipPosition => tooltipPosition;
        public TooltipFindingStrategy TooltipFindingStrategy => tooltipFindingStrategy;
        public IChartTooltip<TDrawingContext>? Tooltip => tooltip;
        public TimeSpan AnimationsSpeed => animationsSpeed;
        public Func<float, float> EasingFunction => easingFunction;

        public abstract void Update();

        protected abstract void Measure();

        protected abstract void UpdateThrottlerUnlocked();

        public abstract IEnumerable<TooltipPoint> FindPointsNearTo(PointF pointerPosition);

        protected void SetDrawMargin(SizeF controlSize, Margin margin)
        {
            drawMarginSize = new SizeF
            {
                Width = controlSize.Width - margin.Left - margin.Right,
                Height = controlSize.Height - margin.Top - margin.Bottom
            };

            drawMaringLocation = new PointF(margin.Left, margin.Top);
        }
    }
}
