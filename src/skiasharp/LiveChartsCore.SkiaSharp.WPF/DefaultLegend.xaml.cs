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
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    /// <summary>
    /// Interaction logic for DefaultLegend.xaml
    /// </summary>
    public partial class DefaultLegend : UserControl, IChartLegend<SkiaSharpDrawingContext>
    {
        private readonly DataTemplate defaultTempalte;

        public DefaultLegend()
        {
            InitializeComponent();
            defaultTempalte = (DataTemplate)FindResource("defaultTemplate");
        }

        public static readonly DependencyProperty CustomTemplateProperty =
            DependencyProperty.Register(
                nameof(CustomTemplate), typeof(DataTemplate), typeof(DefaultLegend), new PropertyMetadata(null));

        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                nameof(Series), typeof(IEnumerable<IDrawableSeries<SkiaSharpDrawingContext>>),
                typeof(DefaultLegend), new PropertyMetadata(new List<IDrawableSeries<SkiaSharpDrawingContext>>()));

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation), typeof(Orientation), typeof(DefaultLegend), new PropertyMetadata(Orientation.Horizontal));

        public static readonly DependencyProperty DockProperty =
            DependencyProperty.Register(
                nameof(Dock), typeof(Dock), typeof(DefaultLegend), new PropertyMetadata(Dock.Right));

        public static readonly DependencyProperty TextColorProperty =
           DependencyProperty.Register(
               nameof(TextColor), typeof(SolidColorBrush), typeof(DefaultLegend), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(35, 35, 35))));

        public IEnumerable<IDrawableSeries<SkiaSharpDrawingContext>> Series
        {
            get { return (IEnumerable<IDrawableSeries<SkiaSharpDrawingContext>>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        public DataTemplate CustomTemplate
        {
            get { return (DataTemplate)GetValue(CustomTemplateProperty); }
            set { SetValue(CustomTemplateProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public Dock Dock
        {
            get { return (Dock)GetValue(DockProperty); }
            set { SetValue(DockProperty, value); }
        }

        public SolidColorBrush TextColor
        {
            get { return (SolidColorBrush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        void IChartLegend<SkiaSharpDrawingContext>.Draw(Chart<SkiaSharpDrawingContext> chart)
        {
            var wpfChart = (Chart)chart.View;

            var series = chart.DrawableSeries;
            var legendOrientation = chart.LegendOrientation;
            var legendPosition = chart.LegendPosition;
            var template = wpfChart.LegendTemplate ?? defaultTempalte;
            if (CustomTemplate != template) CustomTemplate = template;

            Series = series;

            switch (legendPosition)
            {
                case LegendPosition.Hidden:
                    Visibility = Visibility.Collapsed;
                    break;
                case LegendPosition.Top:
                    Visibility = Visibility.Visible;
                   if (legendOrientation == LegendOrientation.Auto) Orientation = Orientation.Horizontal;
                    Dock = Dock.Top;
                    break;
                case LegendPosition.Left:
                    Visibility = Visibility.Visible;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = Orientation.Vertical;
                    Dock = Dock.Left;
                    break;
                case LegendPosition.Right:
                    Visibility = Visibility.Visible;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = Orientation.Vertical;
                    Dock = Dock.Right;
                    break;
                case LegendPosition.Bottom:
                    Visibility = Visibility.Visible;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = Orientation.Horizontal;
                    Dock = Dock.Bottom;
                    break;
                default:
                    break;
            }

            if (legendOrientation != LegendOrientation.Auto)
                Orientation = legendOrientation == LegendOrientation.Horizontal
                    ? Orientation.Horizontal
                    : Orientation.Vertical;

            FontFamily = wpfChart.LegendFontFamily;
            TextColor = wpfChart.LegendTextColor;
            FontSize = wpfChart.LegendFontSize;
            FontWeight = wpfChart.LegendFontWeight;
            FontStyle = wpfChart.LegendFontStyle;
            FontStretch = wpfChart.LegendFontStretch;

            UpdateLayout();
        }
    }
}
