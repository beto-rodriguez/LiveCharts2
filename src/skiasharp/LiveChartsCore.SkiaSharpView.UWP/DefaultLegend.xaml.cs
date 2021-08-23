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

using System.Collections.Generic;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LiveChartsCore.SkiaSharpView.UWP
{
    public sealed partial class DefaultLegend : UserControl, IChartLegend<SkiaSharpDrawingContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLegend"/> class.
        /// </summary>
        public DefaultLegend()
        {
            InitializeComponent();

            DataContext = this;
        }

        /// <summary>
        /// The series property
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                nameof(Series), typeof(IEnumerable<IChartSeries<SkiaSharpDrawingContext>>),
                typeof(DefaultLegend), new PropertyMetadata(new List<IChartSeries<SkiaSharpDrawingContext>>()));

        /// <summary>
        /// The orientation property
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation), typeof(Orientation), typeof(DefaultLegend), new PropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// Gets or sets the series.
        /// </summary>
        /// <value>
        /// The series.
        /// </value>
        public IEnumerable<IChartSeries<SkiaSharpDrawingContext>> Series
        {
            get => (IEnumerable<IChartSeries<SkiaSharpDrawingContext>>)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        void IChartLegend<SkiaSharpDrawingContext>.Draw(Chart<SkiaSharpDrawingContext> chart)
        {
            var winuiChart = (IUwpChart)chart.View;

            var series = chart.ChartSeries;
            var legendOrientation = chart.LegendOrientation;
            var legendPosition = chart.LegendPosition;
            //var template = wpfChart.LegendTemplate ?? _defaultTempalte;
            //if (CustomTemplate != template) CustomTemplate = template;

            Series = series;

            switch (legendPosition)
            {
                case LegendPosition.Hidden:
                    Visibility = Visibility.Collapsed;
                    break;
                case LegendPosition.Top:
                    Visibility = Visibility.Visible;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = Orientation.Horizontal;
                    Grid.SetColumn(winuiChart.Legend, 1);
                    Grid.SetRow(winuiChart.Legend, 0);
                    break;
                case LegendPosition.Left:
                    Visibility = Visibility.Visible;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = Orientation.Vertical;
                    Grid.SetColumn(winuiChart.Legend, 0);
                    Grid.SetRow(winuiChart.Legend, 1);
                    break;
                case LegendPosition.Right:
                    Visibility = Visibility.Visible;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = Orientation.Vertical;
                    Grid.SetColumn(winuiChart.Legend, 2);
                    Grid.SetRow(winuiChart.Legend, 1);
                    break;
                case LegendPosition.Bottom:
                    Visibility = Visibility.Visible;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = Orientation.Horizontal;
                    Grid.SetColumn(winuiChart.Legend, 1);
                    Grid.SetRow(winuiChart.Legend, 2);
                    break;
                default:
                    break;
            }

            if (legendOrientation != LegendOrientation.Auto)
                Orientation = legendOrientation == LegendOrientation.Horizontal
                    ? Orientation.Horizontal
                    : Orientation.Vertical;

            FontFamily = winuiChart.LegendFontFamily;
            //TextColor = wpfChart.LegendTextBrush;
            FontSize = winuiChart.LegendFontSize;
            FontWeight = winuiChart.LegendFontWeight;
            FontStyle = winuiChart.LegendFontStyle;
            FontStretch = winuiChart.LegendFontStretch;
            //LegendBackground = wpfChart.LegendBackground;

            UpdateLayout();
        }
    }
}
