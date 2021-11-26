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
using System.Timers;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.XamarinForms;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LiveChartsCore.SkiaSharpView.Xamarin.Forms
{
    /// <inheritdoc cref="IChartTooltip{TDrawingContext}" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefaultTooltip : ContentView, IChartTooltip<SkiaSharpDrawingContext>
    {
        private Chart<SkiaSharpDrawingContext>? _chart;
        private readonly DataTemplate _defaultTemplate;
        private readonly Timer _closeTimer = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTooltip"/> class.
        /// </summary>
        public DefaultTooltip()
        {
            InitializeComponent();
            _defaultTemplate = (DataTemplate)Resources["defaultTemplate"];
            _closeTimer.Interval = 3000;
            _closeTimer.Elapsed += _closeTimer_Elapsed;
        }

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
        public IEnumerable<ChartPoint> Points { get; set; } = Enumerable.Empty<ChartPoint>();

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public string? TooltipFontFamily { get; set; }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public double TooltipFontSize { get; set; }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>
        /// The color of the text.
        /// </value>
        public Color TooltipTextColor { get; set; }

        /// <summary>
        /// Gets or sets the font attributes.
        /// </summary>
        /// <value>
        /// The font attributes.
        /// </value>
        public FontAttributes TooltipFontAttributes { get; set; }

        /// <summary>
        /// Gets or sets the color of the tool tip background.
        /// </summary>
        /// <value>
        /// The color of the tool tip background.
        /// </value>
        public Color TooltipBackgroundColor { get; set; }

        void IChartTooltip<SkiaSharpDrawingContext>.Show(IEnumerable<ChartPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
        {
            var mobileChart = (IMobileChart)chart.View;

            Points = tooltipPoints;

            LvcPoint? location = null;
            var size = new Size
            {
                Width = Width * DeviceDisplay.MainDisplayInfo.Density,
                Height = Height * DeviceDisplay.MainDisplayInfo.Density
            };

            if (chart is CartesianChart<SkiaSharpDrawingContext> or PolarChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetCartesianTooltipLocation(
                    chart.TooltipPosition, new LvcSize((float)size.Width, (float)size.Height), chart.ControlSize);
            }
            if (chart is PieChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetPieTooltipLocation(
                    chart.TooltipPosition, new LvcSize((float)size.Width, (float)size.Height));
            }
            if (location is null) throw new Exception("location not supported");

            IsVisible = true;
            var template = mobileChart.TooltipTemplate ?? _defaultTemplate;
            if (TooltipTemplate != template) TooltipTemplate = template;
            TooltipFontFamily = mobileChart.TooltipFontFamily;
            TooltipTextColor = mobileChart.TooltipTextBrush;
            TooltipFontSize = mobileChart.TooltipFontSize;
            TooltipFontAttributes = mobileChart.TooltipFontAttributes;
            TooltipBackgroundColor = mobileChart.TooltipBackground;
            BuildContent();
            InvalidateLayout();

            _ = Measure(double.PositiveInfinity, double.PositiveInfinity);
            var chartSize = chart.ControlSize;
            _chart = chart;

            AbsoluteLayout.SetLayoutBounds(
                this,
                new Rectangle(
                    location.Value.X / chartSize.Width,
                    location.Value.Y / chartSize.Height,
                    AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            _closeTimer.Stop();
            _closeTimer.Start();
        }

        /// <summary>
        /// Builds the content.
        /// </summary>
        /// <returns></returns>
        protected void BuildContent()
        {
            var template = TooltipTemplate ?? _defaultTemplate;
            if (template.CreateContent() is not View view) return;

            view.BindingContext = new TooltipBindingContext
            {
                TooltipBackgroundColor = TooltipBackgroundColor,
                Points = Points,
                FontFamily = TooltipFontFamily,
                FontSize = TooltipFontSize,
                TextColor = TooltipTextColor,
                FontAttributes = TooltipFontAttributes
            };

            Content = view;
        }

        void IChartTooltip<SkiaSharpDrawingContext>.Hide()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                AbsoluteLayout.SetLayoutBounds(
                    this,
                    new Rectangle(
                        -1, -1,
                        AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

                _chart?.Update();
            });
        }

        private void _closeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ((IChartTooltip<SkiaSharpDrawingContext>)this).Hide();
            _closeTimer.Stop();
        }
    }
}
