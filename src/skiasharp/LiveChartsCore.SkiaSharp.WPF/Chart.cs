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

using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    public abstract class Chart: Control, IChartView<SkiaSharpDrawingContext>
    {
        protected Chart<SkiaSharpDrawingContext> core;
        protected MotionCanvas canvas;
        protected IChartLegend<SkiaSharpDrawingContext> legend;
        protected IChartTooltip<SkiaSharpDrawingContext> tooltip;
        private readonly ActionThrottler mouseMoveThrottler;
        private PointF mousePosition = new PointF();

        public Chart()
        {
            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetStylesBuilder<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetInitializer();
            if (stylesBuilder.CurrentColors == null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ConstructChart(this);

            SizeChanged += OnSizeChanged;
            MouseMove += OnMouseMove;
            mouseMoveThrottler = new ActionThrottler(MouseMoveThrottlerUnlocked, TimeSpan.FromMilliseconds(10));
        }

        public static readonly DependencyProperty AnimationsSpeedProperty =
            DependencyProperty.Register(
                nameof(AnimationsSpeed), typeof(TimeSpan), typeof(Chart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultAnimationsSpeed));

        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register(
                nameof(EasingFunction), typeof(Func<float, float>), typeof(Chart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultEasingFunction));

        public static readonly DependencyProperty LegendPositionProperty =
            DependencyProperty.Register(
                nameof(LegendPosition), typeof(LegendPosition), typeof(Chart), 
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultLegendPosition));

        public static readonly DependencyProperty LegendOrientationProperty =
            DependencyProperty.Register(
                nameof(LegendOrientation), typeof(LegendOrientation), typeof(Chart), 
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultLegendOrientation));

        public static readonly DependencyProperty TooltipPositionProperty =
           DependencyProperty.Register(
               nameof(TooltipPosition), typeof(TooltipPosition), typeof(Chart), 
               new PropertyMetadata(LiveCharts.CurrentSettings.DefaultTooltipPosition));

        public static readonly DependencyProperty TooltipFindingStrategyProperty =
            DependencyProperty.Register(
                nameof(TooltipFindingStrategy), typeof(TooltipFindingStrategy), typeof(Chart), 
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy));

        public TimeSpan AnimationsSpeed
        {
            get { return (TimeSpan)GetValue(AnimationsSpeedProperty); }
            set { SetValue(AnimationsSpeedProperty, value); }
        }

        public Func<float, float> EasingFunction
        {
            get { return (Func<float, float>)GetValue(EasingFunctionProperty); }
            set { SetValue(AnimationsSpeedProperty, value); }
        }

        public LegendPosition LegendPosition
        {
            get { return (LegendPosition)GetValue(LegendPositionProperty); }
            set { SetValue(LegendPositionProperty, value); }
        }

        public LegendOrientation LegendOrientation
        {
            get { return (LegendOrientation)GetValue(LegendOrientationProperty); }
            set { SetValue(LegendOrientationProperty, value); }
        }

        public TooltipPosition TooltipPosition
        {
            get { return (TooltipPosition)GetValue(TooltipPositionProperty); }
            set { SetValue(TooltipPositionProperty, value); }
        }

        public TooltipFindingStrategy TooltipFindingStrategy
        {
            get { return (TooltipFindingStrategy)GetValue(TooltipFindingStrategyProperty); }
            set { SetValue(TooltipFindingStrategyProperty, value); }
        }

        SizeF IChartView.ControlSize => new SizeF { Width = (float)canvas.ActualWidth, Height = (float)canvas.ActualHeight };

        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => canvas.CanvasCore;

        public FontFamily LegendFontFamily { get; set; }

        public SolidColorBrush LegendTextColor { get; set; }

        public double? LegendFontSize { get; set; }

        public FontWeight? LegendFontWeight { get; set; }

        public FontStretch? LegendFontStretch { get; set; }

        public FontStyle? LegendFontStyle { get; set; }

        public IChartLegend<SkiaSharpDrawingContext> Legend => legend;

        public Margin DrawMargin { get; set; }

        public FontFamily TooltipFontFamily { get; set; }

        public SolidColorBrush TooltipTextColor { get; set; }

        public double? TooltipFontSize { get; set; }

        public FontWeight? TooltipFontWeight { get; set; }

        public FontStretch? TooltipFontStretch { get; set; }

        public FontStyle? TooltipFontStyle { get; set; }

        public IChartTooltip<SkiaSharpDrawingContext> Tooltip => tooltip;

        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; }

        protected abstract void InitializeCore();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!(Template.FindName("canvas", this) is MotionCanvas canvas))
                throw new Exception(
                    $"{nameof(MotionCanvas)} not found. This was probably caused because the control {nameof(CartesianChart)} template was overridden, " +
                    $"If you override the template please add an {nameof(MotionCanvas)} to the template and name it 'canvas'");

            this.canvas = canvas;
            InitializeCore();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => core.Update());
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var p = e.GetPosition(canvas);
            mousePosition = new PointF((float)p.X, (float)p.Y);
            mouseMoveThrottler.Call();
        }

        private void MouseMoveThrottlerUnlocked()
        {
            if (TooltipPosition == TooltipPosition.Hidden) return;
            tooltip.Show(core.FindPointsNearTo(mousePosition), core);
        }
    }
}
