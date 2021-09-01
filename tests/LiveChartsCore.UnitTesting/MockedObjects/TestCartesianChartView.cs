using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;

namespace LiveChartsCore.UnitTesting.MockedObjects
{
    public class TestCartesianChartView : ICartesianChartView<SkiaSharpDrawingContext>
    {
        public TestCartesianChartView()
        {
            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetVisualsInitializer();
            if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ApplyStyleToChart(this);

            Core = new CartesianChart<SkiaSharpDrawingContext>(
                this, LiveChartsSkiaSharp.DefaultPlatformBuilder, CoreCanvas);
        }

        public bool DesignerMode => false;

        public bool IsInVisualTree => true;

        IChart IChartView.CoreChart => Core;

        public CartesianChart<SkiaSharpDrawingContext> Core { get; }

        public IEnumerable<ICartesianAxis> XAxes { get; set; }

        public IEnumerable<ICartesianAxis> YAxes { get; set; }

        public IEnumerable<ISeries> Series { get; set; }

        public ZoomAndPanMode ZoomMode { get; set; }

        public double ZoomingSpeed { get; set; }

        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas { get; } = new();

        public IChartLegend<SkiaSharpDrawingContext> Legend => null;

        public IChartTooltip<SkiaSharpDrawingContext> Tooltip => null;

        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; }

        public LvcSize ControlSize => new(100, 100);

        public Margin DrawMargin { get; set; }

        public TimeSpan AnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(1);

        public Func<float, float> EasingFunction { get; set; } = EasingFunctions.Lineal;

        public LegendPosition LegendPosition { get; set; }

        public LegendOrientation LegendOrientation { get; set; }

        public TooltipPosition TooltipPosition { get; set; }

        public TooltipFindingStrategy TooltipFindingStrategy { get; set; }

        public LvcColor BackColor { get; set; }
        public bool AutoUpdateEnabled { get; set; } = true;
        public TimeSpan UpdaterThrottler { get; set; }
        public DrawMarginFrame<SkiaSharpDrawingContext> DrawMarginFrame { get; set; }
        public IEnumerable<Section<SkiaSharpDrawingContext>> Sections { get; set; }
        public object SyncContext { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event ChartEventHandler<SkiaSharpDrawingContext> Measuring;
        public event ChartEventHandler<SkiaSharpDrawingContext> UpdateStarted;
        public event ChartEventHandler<SkiaSharpDrawingContext> UpdateFinished;

        public void DummyRaiseEvents()
        {
            Measuring?.Invoke(this);
            UpdateStarted?.Invoke(this);
            UpdateFinished?.Invoke(this);
        }

        public void HideTooltip() { }

        public double[] ScaleUIPoint(LvcPoint point, int xAxisIndex = 0, int yAxisIndex = 0)
        {
            return new double[2];
        }

        public void ShowTooltip(IEnumerable<TooltipPoint> points) { }

        public void SetTooltipStyle(LvcColor background, LvcColor textColor) { }

        public void InvokeOnUIThread(Action action)
        {
            action();
        }

        public void SyncAction(Action action)
        {
            action();
        }
    }
}
