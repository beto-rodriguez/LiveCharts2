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

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace LiveChartsCore.SkiaSharpView.Avalonia
{
    /// <summary>
    /// Defines a default tool tip for a chart control.
    /// </summary>
    public class DefaultTooltip : UserControl, IChartTooltip<SkiaSharpDrawingContext>
    {
        private readonly DataTemplate _defaultTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTooltip"/> class.
        /// </summary>
        /// <exception cref="Exception">default template not found</exception>
        public DefaultTooltip()
        {
            InitializeComponent();

            var template = (DataTemplate?)Resources["defaultTemplate"] ??
                           throw new Exception("default template not found");

            _defaultTemplate = template;
            TooltipTemplate = template;

            Canvas.SetTop(this, 0);
            Canvas.SetLeft(this, 0);
        }

        #region properties

        /// <summary>
        /// Gets or sets the tool tip template.
        /// </summary>
        /// <value>
        /// The tool tip template.
        /// </value>
        public DataTemplate? TooltipTemplate { get; set; }

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public IEnumerable<PointInfo> Points { get; set; } = Enumerable.Empty<PointInfo>();

        /// <summary>
        /// Gets or sets the tool tip font family.
        /// </summary>
        /// <value>
        /// The tool tip font family.
        /// </value>
        public FontFamily TooltipFontFamily { get; set; } = new FontFamily("Trebuchet MS");

        /// <summary>
        /// Gets or sets the size of the tool tip font.
        /// </summary>
        /// <value>
        /// The size of the tool tip font.
        /// </value>
        public double TooltipFontSize { get; set; }

        /// <summary>
        /// Gets or sets the tool tip font weight.
        /// </summary>
        /// <value>
        /// The tool tip font weight.
        /// </value>
        public FontWeight TooltipFontWeight { get; set; }

        /// <summary>
        /// Gets or sets the tool tip font style.
        /// </summary>
        /// <value>
        /// The tool tip font style.
        /// </value>
        public FontStyle TooltipFontStyle { get; set; }

        /// <summary>
        /// Gets or sets the tool tip text brush.
        /// </summary>
        /// <value>
        /// The tool tip text brush.
        /// </value>
        public SolidColorBrush TooltipTextBrush { get; set; } = new SolidColorBrush(Color.FromRgb(35, 35, 35));

        /// <summary>
        /// Gets or sets the tooltip background.
        /// </summary>
        /// <value>
        /// The tooltip background.
        /// </value>
        public IBrush TooltipBackground { get; set; } = new SolidColorBrush(Color.FromRgb(250, 250, 250));

        #endregion

        void IChartTooltip<SkiaSharpDrawingContext>.Show(IEnumerable<PointInfo> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
        {
            var avaloniaChart = (IAvaloniaChart)chart.View;

            var template = avaloniaChart.TooltipTemplate ?? _defaultTemplate;
            if (TooltipTemplate != template) TooltipTemplate = template;

            LvcPoint? location = null;

            if (chart is CartesianChart<SkiaSharpDrawingContext> or PolarChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetCartesianTooltipLocation(
                    chart.TooltipPosition, new LvcSize((float)Bounds.Width, (float)Bounds.Height), chart.ControlSize);
            }
            if (chart is PieChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetPieTooltipLocation(
                    chart.TooltipPosition, new LvcSize((float)Bounds.Width, (float)Bounds.Height));
            }

            if (location is null) throw new Exception("location not found");

            Points = tooltipPoints;
            TooltipBackground = avaloniaChart.TooltipBackground;
            TooltipFontFamily = avaloniaChart.TooltipFontFamily;
            TooltipFontSize = avaloniaChart.TooltipFontSize;
            TooltipFontStyle = avaloniaChart.TooltipFontStyle;
            TooltipFontWeight = avaloniaChart.TooltipFontWeight;
            TooltipTextBrush = avaloniaChart.TooltipTextBrush;
            BuildContent();

            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            double x = location.Value.X;
            double y = location.Value.Y;
            var s = chart.ControlSize;
            var w = s.Width;
            var h = s.Height;
            if (location.Value.X + Bounds.Width > w) x = w - Bounds.Width;
            if (location.Value.X < 0) x = 0;
            if (location.Value.Y < 0) y = 0;
            if (location.Value.Y + Bounds.Height > h) x = h - Bounds.Height;

            Transitions ??= new Transitions
            {
                new DoubleTransition {Property = Canvas.TopProperty, Duration = TimeSpan.FromMilliseconds(300)},
                new DoubleTransition {Property = Canvas.LeftProperty, Duration = TimeSpan.FromMilliseconds(300)},
            };

            Canvas.SetTop(this, y);
            Canvas.SetLeft(this, x);
        }

        /// <summary>
        /// Builds the content.
        /// </summary>
        /// <returns></returns>
        protected void BuildContent()
        {
            var template = TooltipTemplate ?? _defaultTemplate;
            var model = new TooltipBindingContext
            {
                Points = Points,
                TooltipFontFamily = TooltipFontFamily,
                TooltipBackground = TooltipBackground,
                TooltipFontSize = TooltipFontSize,
                TooltipFontStyle = TooltipFontStyle,
                TooltipFontWeight = TooltipFontWeight,
                TooltipTextBrush = TooltipTextBrush
            };

            var templated = template.Build(model);
            if (templated is null) return;

            Content = templated;
        }

        void IChartTooltip<SkiaSharpDrawingContext>.Hide()
        {
            Content = null;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
