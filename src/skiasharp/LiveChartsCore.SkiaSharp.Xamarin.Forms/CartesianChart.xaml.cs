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
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LiveChartsCore.SkiaSharpView.Xamarin.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CartesianChart : ContentView, IChartView<SkiaDrawingContext>
    {
        protected ChartCore<SkiaDrawingContext> core;

        public CartesianChart()
        {
            InitializeComponent();

            if (!(FindByName("canvas") is NaturalVisualCanvas canvas))
                throw new Exception(
                    $"SkiaElement not found. This was probably caused because the control {nameof(CartesianChart)} template was overridden, " +
                    $"If you override the template please add an {nameof(NaturalVisualCanvas)} to the template and name it 'canvas'");

            core = new ChartCore<SkiaDrawingContext>(this, canvas.CanvasCore);
            core.Update();

            SizeChanged += CartesianChart_SizeChanged;
        }

        ChartCore<SkiaDrawingContext> IChartView<SkiaDrawingContext>.Core => core;
        public Canvas<SkiaDrawingContext> CoreCanvas => canvas.CanvasCore;

        System.Drawing.SizeF IChartView<SkiaDrawingContext>.ControlSize
        {
            get
            {
                var i = DeviceDisplay.MainDisplayInfo;
                unchecked
                {
                    return new System.Drawing.SizeF
                    {
                        Width = (float)(i.Width),
                        Height = (float)(i.Height)
                    };
                }
            }
        }

        public static readonly BindableProperty SeriesProperty =
            BindableProperty.Create(
                nameof(Series), typeof(IEnumerable<ISeries<SkiaDrawingContext>>), typeof(CartesianChart),
                new List<ISeries<SkiaDrawingContext>>(), BindingMode.Default, null);

        public static readonly BindableProperty XAxesProperty =
            BindableProperty.Create(
                nameof(XAxes), typeof(IList<IAxis<SkiaDrawingContext>>),
                typeof(CartesianChart), new List<IAxis<SkiaDrawingContext>>(), BindingMode.Default, null);

        public static readonly BindableProperty YAxesProperty =
            BindableProperty.Create(
                nameof(YAxes), typeof(IList<IAxis<SkiaDrawingContext>>),
                typeof(CartesianChart), new List<IAxis<SkiaDrawingContext>>(), BindingMode.Default, null);

        public IEnumerable<ISeries<SkiaDrawingContext>> Series
        {
            get { return (IEnumerable<ISeries<SkiaDrawingContext>>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        public IList<IAxis<SkiaDrawingContext>> XAxes
        {
            get { return (IList<IAxis<SkiaDrawingContext>>)GetValue(XAxesProperty); }
            set { SetValue(XAxesProperty, value); }
        }

        public IList<IAxis<SkiaDrawingContext>> YAxes
        {
            get { return (IList<IAxis<SkiaDrawingContext>>)GetValue(YAxesProperty); }
            set { SetValue(YAxesProperty, value); }
        }

        public LegendPosition LegendPosition { get; set; }

        public LegendOrientation LegendOrientation { get; set; }

        public IChartLegend<SkiaDrawingContext> Legend => null;

        public Margin DrawMargin { get; set; }

        public TimeSpan AnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(500);

        public Func<float, float> EasingFunction { get; set; } = EasingFunctions.Lineal;

        public TooltipFindingStrategy TooltipFindingStrategy { get; set; }

        public IChartTooltip<SkiaDrawingContext> Tooltip => null;

        public TooltipPosition TooltipPosition { get; set; }

        private void CartesianChart_SizeChanged(object sender, EventArgs e)
        {
            core.Update();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            core.Update();
        }
    }
}