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
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Drawing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LiveChartsCore.SkiaSharpView.Uno
{
    public abstract partial class Chart : Control, IChartView<SkiaSharpDrawingContext>
    {
        protected Chart<SkiaSharpDrawingContext> core;
        protected MotionCanvas canvas;
        protected IChartLegend<SkiaSharpDrawingContext> legend = null;
        protected IChartTooltip<SkiaSharpDrawingContext> tooltip = null;
        private readonly ActionThrottler mouseMoveThrottler;
        private PointF mousePosition = new PointF();

        public Chart()
        {
            DefaultStyleKey = typeof(Chart);

            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSK.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetStylesBuilder<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetInitializer();
            if (stylesBuilder.CurrentColors == null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ConstructChart(this);

            SizeChanged += OnSizeChanged;
            PointerMoved += OnPointerMoved;
            mouseMoveThrottler = new ActionThrottler(TimeSpan.FromMilliseconds(10));
            mouseMoveThrottler.Unlocked += MouseMoveThrottlerUnlocked;
        }

        SizeF IChartView.ControlSize
        {
            get
            {
                unchecked
                {
                    return new SizeF { Width = (float)canvas.ActualWidth, Height = (float)canvas.ActualHeight };
                }
            }
        }

        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => canvas.CanvasCore;

        public LegendPosition LegendPosition { get; set; }
        public LegendOrientation LegendOrientation { get; set; }
        public IChartLegend<SkiaSharpDrawingContext> Legend => legend;
        public Margin DrawMargin { get; set; }
        public TimeSpan AnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(500);
        public Func<float, float> EasingFunction { get; set; } = EasingFunctions.Lineal;

        public TooltipPosition TooltipPosition { get; set; }
        public TooltipFindingStrategy TooltipFindingStrategy { get; set; }
        public IChartTooltip<SkiaSharpDrawingContext> Tooltip => tooltip;

        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; }

        protected abstract void InitializeCore();

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var canvas = GetTemplateChild("canvas") as MotionCanvas;
            if (canvas == null)
                throw new Exception(
                    $"{nameof(MotionCanvas)} not found. This was probably caused because the control {nameof(CartesianChart)} template was overridden, " +
                    $"If you override the template please add an {nameof(MotionCanvas)} to the template and name it 'canvas'");

            this.canvas = canvas;
            InitializeCore();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            core.Update();
        }

        private void OnPointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(this);
            mousePosition = new PointF((float)p.Position.X, (float)p.Position.Y);
            mouseMoveThrottler.TryRun();
        }

        private void MouseMoveThrottlerUnlocked()
        {
            if (TooltipPosition == TooltipPosition.Hidden) return;
            tooltip.Show(core.FindPointsNearTo(mousePosition), core);
        }
    }
}
