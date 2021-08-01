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

using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    /// <inheritdoc cref="IChartLegend{TDrawingContext}" />
    public partial class DefaultLegend : UserControl, IChartLegend<SkiaSharpDrawingContext>
    {
        private readonly DataTemplate _defaultTempalte;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLegend"/> class.
        /// </summary>
        public DefaultLegend()
        {
            InitializeComponent();
            _defaultTempalte = (DataTemplate)FindResource("defaultTemplate");
        }

        /// <summary>
        /// The custom template property
        /// </summary>
        public static readonly DependencyProperty CustomTemplateProperty =
            DependencyProperty.Register(
                nameof(CustomTemplate), typeof(DataTemplate), typeof(DefaultLegend), new PropertyMetadata(null));

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
        /// The dock property
        /// </summary>
        public static readonly DependencyProperty DockProperty =
            DependencyProperty.Register(
                nameof(Dock), typeof(Dock), typeof(DefaultLegend), new PropertyMetadata(Dock.Right));

        /// <summary>
        /// The text color property
        /// </summary>
        public static readonly DependencyProperty TextColorProperty =
           DependencyProperty.Register(
               nameof(TextColor), typeof(SolidColorBrush), typeof(DefaultLegend), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(35, 35, 35))));

        /// <summary>
        /// The text color property
        /// </summary>
        public static readonly DependencyProperty LegendBackgroundProperty =
           DependencyProperty.Register(
               nameof(LegendBackground), typeof(SolidColorBrush), typeof(DefaultLegend),
               new PropertyMetadata(new SolidColorBrush(Color.FromRgb(35, 35, 35))));

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
        /// Gets or sets the custom template.
        /// </summary>
        /// <value>
        /// The custom template.
        /// </value>
        public DataTemplate CustomTemplate
        {
            get => (DataTemplate)GetValue(CustomTemplateProperty);
            set => SetValue(CustomTemplateProperty, value);
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

        /// <summary>
        /// Gets or sets the dock.
        /// </summary>
        /// <value>
        /// The dock.
        /// </value>
        public Dock Dock
        {
            get => (Dock)GetValue(DockProperty);
            set => SetValue(DockProperty, value);
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>
        /// The color of the text.
        /// </value>
        public SolidColorBrush TextColor
        {
            get => (SolidColorBrush)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>
        /// The color of the text.
        /// </value>
        public SolidColorBrush LegendBackground
        {
            get => (SolidColorBrush)GetValue(LegendBackgroundProperty);
            set => SetValue(LegendBackgroundProperty, value);
        }

        void IChartLegend<SkiaSharpDrawingContext>.Draw(Chart<SkiaSharpDrawingContext> chart)
        {
            var wpfChart = (Chart)chart.View;

            var series = chart.DrawableSeries;
            var legendOrientation = chart.LegendOrientation;
            var legendPosition = chart.LegendPosition;
            var template = wpfChart.LegendTemplate ?? _defaultTempalte;
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
            TextColor = wpfChart.LegendTextBrush;
            FontSize = wpfChart.LegendFontSize;
            FontWeight = wpfChart.LegendFontWeight;
            FontStyle = wpfChart.LegendFontStyle;
            FontStretch = wpfChart.LegendFontStretch;
            LegendBackground = wpfChart.LegendBackground;

            UpdateLayout();
        }
    }
}
