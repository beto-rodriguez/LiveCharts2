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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveChartsCore.SkiaSharpView.Avalonia
{
    /// <summary>
    /// Defines a default legend for a chart.
    /// </summary>
    public class DefaultLegend : UserControl, IChartLegend<SkiaSharpDrawingContext>
    {
        private readonly DataTemplate _defaultTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLegend"/> class.
        /// </summary>
        /// <exception cref="Exception">default template not found</exception>
        public DefaultLegend()
        {
            InitializeComponent();
            var t = (DataTemplate?)Resources["defaultTemplate"];
            if (t == null) throw new Exception("default template not found");
            _defaultTemplate = t;
        }

        /// <summary>
        /// The orientation property
        /// </summary>
        public static readonly AvaloniaProperty<Orientation> OrientationProperty =
           AvaloniaProperty.Register<CartesianChart, Orientation>(nameof(Orientation), Orientation.Horizontal, inherits: true);

        /// <summary>
        /// The dock property
        /// </summary>
        public static readonly AvaloniaProperty<Dock> DockProperty =
           AvaloniaProperty.Register<CartesianChart, Dock>(nameof(Dock), Dock.Left, inherits: true);

        /// <summary>
        /// Gets or sets the custom template.
        /// </summary>
        /// <value>
        /// The custom template.
        /// </value>
        public DataTemplate? CustomTemplate { get; set; } = null;

        /// <summary>
        /// Gets or sets the series.
        /// </summary>
        /// <value>
        /// The series.
        /// </value>
        public IEnumerable<ISeries> Series { get; set; } = Enumerable.Empty<ISeries>();

        /// <summary>
        /// Gets or sets the text brush.
        /// </summary>
        /// <value>
        /// The text brush.
        /// </value>
        public SolidColorBrush TextBrush { get; set; } = new SolidColorBrush(Color.FromRgb(35, 35, 35));

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

        void IChartLegend<SkiaSharpDrawingContext>.Draw(Chart<SkiaSharpDrawingContext> chart)
        {
            var series = chart.DrawableSeries;
            var legendOrientation = chart.LegendOrientation;
            var legendPosition = chart.LegendPosition;
            Series = series;

            switch (legendPosition)
            {
                case LegendPosition.Hidden:
                    IsVisible = false;
                    break;
                case LegendPosition.Top:
                    IsVisible = true;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = Orientation.Horizontal;
                    Dock = Dock.Top;
                    break;
                case LegendPosition.Left:
                    IsVisible = true;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = Orientation.Vertical;
                    Dock = Dock.Left;
                    break;
                case LegendPosition.Right:
                    IsVisible = true;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = Orientation.Vertical;
                    Dock = Dock.Right;
                    break;
                case LegendPosition.Bottom:
                    IsVisible = true;
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

            var avaloniaChart = (IAvaloniaChart)chart.View;

            CustomTemplate = avaloniaChart.LegendTemplate;
            FontFamily = avaloniaChart.LegendFontFamily;
            FontSize = avaloniaChart.LegendFontSize;
            FontWeight = avaloniaChart.LegendFontWeight;
            FontStyle = avaloniaChart.LegendFontStyle;
            TextBrush = avaloniaChart.LegendTextBrush;

            BuildContent();
            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        }

        /// <summary>
        /// Builds the content.
        /// </summary>
        protected void BuildContent()
        {
            var template = CustomTemplate ?? _defaultTemplate;
            var model = new LegendBindingContext
            {
                Series = Series,
                FontFamily = FontFamily,
                Background = Background,
                FontSize = FontSize,
                FontStyle = FontStyle,
                FontWeight = FontWeight,
                TextBrush = TextBrush
            };

            var templated = template.Build(model);
            if (templated == null) return;

            Content = templated;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
