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
using LiveChartsCore.SkiaSharpView.Drawing;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    public class CartesianChart : Chart, ICartesianChartView<SkiaDrawingContext>
    {
        static CartesianChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CartesianChart), new FrameworkPropertyMetadata(typeof(CartesianChart)));
        }

        public CartesianChart() { }

        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                nameof(Series), typeof(IEnumerable<ICartesianSeries<SkiaDrawingContext>>), 
                typeof(CartesianChart), new PropertyMetadata(new List<ICartesianSeries<SkiaDrawingContext>>()));

        public static readonly DependencyProperty XAxesProperty =
            DependencyProperty.Register(
                nameof(XAxes), typeof(IEnumerable<IAxis<SkiaDrawingContext>>),
                typeof(CartesianChart), new PropertyMetadata(new List<IAxis<SkiaDrawingContext>> { new Axis() }));

        public static readonly DependencyProperty YAxesProperty =
            DependencyProperty.Register(
                nameof(YAxes), typeof(IEnumerable<IAxis<SkiaDrawingContext>>),
                typeof(CartesianChart), new PropertyMetadata(new List<IAxis<SkiaDrawingContext>> { new Axis() }));

        CartesianChart<SkiaDrawingContext> ICartesianChartView<SkiaDrawingContext>.Core => (CartesianChart<SkiaDrawingContext>) core;

        public IEnumerable<ICartesianSeries<SkiaDrawingContext>> Series
        {
            get { return (IEnumerable<ICartesianSeries<SkiaDrawingContext>>)GetValue(SeriesProperty); }
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

        public override void InitializeCore()
        {
            core = new CartesianChart<SkiaDrawingContext>(this, canvas.CanvasCore);
            legend = Template.FindName("legend", this) as IChartLegend<SkiaDrawingContext>;
            tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaDrawingContext>;
            core.Update();
        }
    }
}
