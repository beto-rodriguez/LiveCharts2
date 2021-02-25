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
using System.Windows;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    public class PieChart : Chart, IPieChartView<SkiaDrawingContext>
    {
        static PieChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PieChart), new FrameworkPropertyMetadata(typeof(PieChart)));
        }

        public PieChart() { }
        PieChart<SkiaDrawingContext> IPieChartView<SkiaDrawingContext>.Core => (PieChart<SkiaDrawingContext>)core;

        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                nameof(Series), typeof(IEnumerable<IPieSeries<SkiaDrawingContext>>), typeof(PieChart), new PropertyMetadata(null));

        public IEnumerable<IPieSeries<SkiaDrawingContext>> Series
        {
            get { return (IEnumerable<IPieSeries<SkiaDrawingContext>>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        public override void InitializeCore()
        {
            core = new PieChart<SkiaDrawingContext>(this, canvas.CanvasCore);
            legend = Template.FindName("legend", this) as IChartLegend<SkiaDrawingContext>;
            tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaDrawingContext>;
            core.Update();
        }
    }
}
