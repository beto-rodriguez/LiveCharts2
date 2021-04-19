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
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LiveChartsCore.SkiaSharpView.Xamarin.Forms
{
    /// <inheritdoc cref="IChartTooltip{TDrawingContext}" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefaultTooltip : ContentView, IChartTooltip<SkiaSharpDrawingContext>
    {
        private readonly DataTemplate defaultTemplate;
        private readonly Dictionary<ChartPoint, object> activePoints = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTooltip"/> class.
        /// </summary>
        public DefaultTooltip()
        {
            InitializeComponent();
            defaultTemplate = (DataTemplate) Resources["defaultTemplate"];
        }

        /// <summary>
        /// Gets or sets the tooltip template.
        /// </summary>
        /// <value>
        /// The tooltip template.
        /// </value>
        public DataTemplate? TooltipTemplate { get; set; }

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public IEnumerable<TooltipPoint> Points { get; set; } = Enumerable.Empty<TooltipPoint>();

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public string? FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public double FontSize { get; set; }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>
        /// The color of the text.
        /// </value>
        public Color TextColor { get; set; }

        /// <summary>
        /// Gets or sets the font attributes.
        /// </summary>
        /// <value>
        /// The font attributes.
        /// </value>
        public FontAttributes FontAttributes { get; set; }

        void IChartTooltip<SkiaSharpDrawingContext>.Show(IEnumerable<TooltipPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
        {
            var mobileChart = (IMobileChart)chart.View;

            if (!tooltipPoints.Any())
            {
                foreach (var key in activePoints.Keys.ToArray())
                {
                    key.RemoveFromHoverState();
                    activePoints.Remove(key);
                }
                return;
            }

            Points = tooltipPoints;

            System.Drawing.PointF? location = null;
            var size = new Size
            {
                Width = Width * DeviceDisplay.MainDisplayInfo.Density,
                Height = Height * DeviceDisplay.MainDisplayInfo.Density
            };

            if (chart is CartesianChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetCartesianTooltipLocation(
                    chart.TooltipPosition, new System.Drawing.SizeF((float)size.Width, (float)size.Height));
            }
            if (chart is PieChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetPieTooltipLocation(
                    chart.TooltipPosition, new System.Drawing.SizeF((float)size.Width, (float)size.Height));
            }
            if (location == null) throw new Exception("location not supported");

            var template = mobileChart.TooltipTemplate ?? defaultTemplate;
            if (TooltipTemplate != template) TooltipTemplate = template;
            FontFamily = mobileChart.TooltipFontFamily;
            TextColor = mobileChart.TooltipTextColor;
            FontSize = mobileChart.TooltipFontSize;
            FontAttributes = mobileChart.TooltipFontAttributes;
            BuildContent();

            Measure(double.PositiveInfinity, double.PositiveInfinity);
            var chartSize = chart.ControlSize;

            AbsoluteLayout.SetLayoutBounds(
                this,
                new Rectangle(
                    location.Value.X / chartSize.Width, location.Value.Y / chartSize.Height,
                    AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            var o = new object();
            foreach (var tooltipPoint in tooltipPoints)
            {
                tooltipPoint.Point.AddToHoverState();
                activePoints[tooltipPoint.Point] = o;
            }

            foreach (var key in activePoints.Keys.ToArray())
            {
                if (activePoints[key] == o) continue;
                key.RemoveFromHoverState();
                activePoints.Remove(key);
            }

            chart.Canvas.Invalidate();
        }

        /// <summary>
        /// Builds the content.
        /// </summary>
        /// <returns></returns>
        protected void BuildContent()
        {
            var template = TooltipTemplate ?? defaultTemplate;
            if (template.CreateContent() is not View view) return;

            view.BindingContext = new TooltipBindingContext
            {
                Points = Points,
                FontFamily = FontFamily,
                FontSize = FontSize,
                TextColor = TextColor,
                FontAttributes = FontAttributes
            };

            Content = view;
        }
    }
}