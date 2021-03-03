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
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    public abstract class Chart: Control, IChartView<SkiaSharpDrawingContext>
    {
        protected Chart<SkiaSharpDrawingContext> core;
        protected NaturalGeometriesCanvas canvas;

        protected IChartLegend<SkiaSharpDrawingContext> legend;
        protected IChartTooltip<SkiaSharpDrawingContext> tooltip;
        private readonly ActionThrottler mouseMoveThrottler;
        private PointF mousePosition = new PointF();

        public Chart()
        {
            SizeChanged += OnSizeChanged;
            MouseMove += OnMouseMove;
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

        public Canvas<SkiaSharpDrawingContext> CoreCanvas => canvas.CanvasCore;

        public LegendPosition LegendPosition { get; set; }
        public LegendOrientation LegendOrientation { get; set; }
        public FontFamily LegendFontFamily { get; set; }
        public SolidColorBrush LegendTextColor { get; set; }
        public double? LegendFontSize { get; set; }
        public FontWeight? LegendFontWeight { get; set; }
        public FontStretch? LegendFontStretch { get; set; }
        public FontStyle? LegendFontStyle { get; set; }
        public IChartLegend<SkiaSharpDrawingContext> Legend => legend;
        public Margin DrawMargin { get; set; }
        public TimeSpan AnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(500);
        public Func<float, float> EasingFunction { get; set; } = EasingFunctions.Lineal;

        public FontFamily TooltipFontFamily { get; set; }
        public SolidColorBrush TooltipTextColor { get; set; }
        public double? TooltipFontSize { get; set; }
        public FontWeight? TooltipFontWeight { get; set; }
        public FontStretch? TooltipFontStretch { get; set; }
        public FontStyle? TooltipFontStyle { get; set; }
        public TooltipPosition TooltipPosition { get; set; }
        public TooltipFindingStrategy TooltipFindingStrategy { get; set; }
        public IChartTooltip<SkiaSharpDrawingContext> Tooltip => tooltip;

        public PointStates<SkiaSharpDrawingContext> PointStates { get; set; }

        public abstract void InitializeCore();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!(Template.FindName("canvas", this) is NaturalGeometriesCanvas canvas))
                throw new Exception(
                    $"{nameof(SKElement)} not found. This was probably caused because the control {nameof(CartesianChart)} template was overridden, " +
                    $"If you override the template please add an {nameof(NaturalGeometriesCanvas)} to the template and name it 'canvas'");

            this.canvas = canvas;
            InitializeCore();
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
            tooltip.Show(core.FindPointsNearTo(mousePosition), core);
        }
    }
}
