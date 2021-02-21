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
using SkiaSharp.Views.WPF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    public class CartesianChart : Control, ICartesianChartView<SkiaDrawingContext>
    {
        protected CartesianChartCore<SkiaDrawingContext> core;
        protected NaturalGeometriesCanvas canvas;
        protected IChartLegend<SkiaDrawingContext> legend;
        protected IChartTooltip<SkiaDrawingContext> tooltip;
        private readonly ActionThrottler mouseMoveThrottler;
        private PointF mousePosition = new PointF();

        static CartesianChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CartesianChart), new FrameworkPropertyMetadata(typeof(CartesianChart)));
        }

        public CartesianChart()
        {
            SizeChanged += OnSizeChanged;
            MouseMove += OnMouseMove;
            mouseMoveThrottler = new ActionThrottler(TimeSpan.FromMilliseconds(10));
            mouseMoveThrottler.Unlocked += MouseMoveThrottlerUnlocked;
        }

        CartesianChartCore<SkiaDrawingContext> ICartesianChartView<SkiaDrawingContext>.Core => core;
        public Canvas<SkiaDrawingContext> CoreCanvas => canvas.CanvasCore;

        SizeF ICartesianChartView<SkiaDrawingContext>.ControlSize
        { 
            get
            {
                unchecked
                {
                    return new SizeF { Width = (float) canvas.ActualWidth, Height = (float) canvas.ActualHeight };
                }
            }
        }

        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                nameof(Series), typeof(IEnumerable<ISeries<SkiaDrawingContext>>), 
                typeof(CartesianChart), new PropertyMetadata(new List<ISeries<SkiaDrawingContext>>()));

        public static readonly DependencyProperty XAxesProperty =
            DependencyProperty.Register(
                nameof(XAxes), typeof(IEnumerable<IAxis<SkiaDrawingContext>>),
                typeof(CartesianChart), new PropertyMetadata(new List<IAxis<SkiaDrawingContext>> { new Axis() }));

        public static readonly DependencyProperty YAxesProperty =
            DependencyProperty.Register(
                nameof(YAxes), typeof(IEnumerable<IAxis<SkiaDrawingContext>>),
                typeof(CartesianChart), new PropertyMetadata(new List<IAxis<SkiaDrawingContext>> { new Axis() }));

        public IEnumerable<ISeries<SkiaDrawingContext>> Series
        {
            get { return (IEnumerable<ISeries<SkiaDrawingContext>>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        public IEnumerable<IAxis<SkiaDrawingContext>> XAxes
        {
            get { return (IEnumerable<IAxis<SkiaDrawingContext>>)GetValue(XAxesProperty); }
            set { SetValue(XAxesProperty, value); }
        }

        public IEnumerable<IAxis<SkiaDrawingContext>> YAxes
        {
            get { return (IEnumerable<IAxis<SkiaDrawingContext>>)GetValue(YAxesProperty); }
            set { SetValue(YAxesProperty, value); }
        }

        public LegendPosition LegendPosition { get; set; }
        public LegendOrientation LegendOrientation { get; set; }
        public FontFamily LegendFontFamily { get; set; }
        public SolidColorBrush LegendTextColor { get; set; }
        public double? LegendFontSize { get; set; }
        public FontWeight? LegendFontWeight { get; set; }
        public FontStretch? LegendFontStretch { get; set; }
        public FontStyle? LegendFontStyle { get; set; }
        public IChartLegend<SkiaDrawingContext> Legend => legend;

        public FontFamily TooltipFontFamily { get; set; }
        public SolidColorBrush TooltipTextColor { get; set; }
        public double? TooltipFontSize { get; set; }
        public FontWeight? TooltipFontWeight { get; set; }
        public FontStretch? TooltipFontStretch { get; set; }
        public FontStyle? TooltipFontStyle { get; set; }
        public TooltipPosition TooltipPosition { get; set; }
        public TooltipFindingStrategy TooltipFindingStrategy { get; set; }
        public IChartTooltip<SkiaDrawingContext> Tooltip => tooltip;

        public Margin DrawMargin { get; set; }

        public TimeSpan AnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(500);

        public Func<float, float> EasingFunction { get; set; } = EasingFunctions.Lineal;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!(Template.FindName("canvas", this) is NaturalGeometriesCanvas canvas))
                throw new Exception(
                    $"{nameof(SKElement)} not found. This was probably caused because the control {nameof(CartesianChart)} template was overridden, " +
                    $"If you override the template please add an {nameof(NaturalGeometriesCanvas)} to the template and name it 'canvas'");

            this.canvas = canvas;
            core = new CartesianChartCore<SkiaDrawingContext>(this, canvas.CanvasCore);
            legend = Template.FindName("legend", this) as IChartLegend<SkiaDrawingContext>;
            tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaDrawingContext>;
            core.Update();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            core.Update();
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var p = e.GetPosition(canvas);
            mousePosition = unchecked(new PointF((float)p.X, (float)p.Y));
            mouseMoveThrottler.TryRun();
        }

        private void MouseMoveThrottlerUnlocked()
        {
            tooltip.Show(core.FindPointsNearTo(mousePosition), this);
        }
    }
}
