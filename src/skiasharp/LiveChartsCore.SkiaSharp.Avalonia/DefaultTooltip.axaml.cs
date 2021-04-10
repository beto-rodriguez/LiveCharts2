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

using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveChartsCore.SkiaSharp.Avalonia
{
    public class DefaultTooltip : UserControl, IChartTooltip<SkiaSharpDrawingContext>
    {
        private readonly DataTemplate defaultTemplate;
        private readonly Dictionary<ChartPoint, object> activePoints = new();

        public DefaultTooltip()
        {
            InitializeComponent();
            var t = (DataTemplate?)Resources["defaultTemplate"];
            if (t == null) throw new Exception("default tempalte not found");
            defaultTemplate = t;
            TooltipTemplate = t;
            Canvas.SetTop(this, 0);
            Canvas.SetLeft(this, 0);
        }

        #region properties

        public DataTemplate TooltipTemplate { get; set; } = null;

        public IEnumerable<TooltipPoint> Points { get; set; } = Enumerable.Empty<TooltipPoint>();

        public FontFamily TooltipFontFamily { get; set; } = new FontFamily("Trebuchet MS");
        public double TooltipFontSize { get; set; }
        public FontWeight TooltipFontWeight { get; set; }
        public FontStyle TooltipFontStyle { get; set; }
        public SolidColorBrush TooltipTextBrush { get; set; } = new SolidColorBrush(Color.FromRgb(35, 35, 35));
        public IBrush TooltipBackground { get; set; } = new SolidColorBrush(Color.FromRgb(250, 250, 250));

        #endregion

        void IChartTooltip<SkiaSharpDrawingContext>.Show(IEnumerable<TooltipPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
        {
            var avaloniaChart = (IAvaloniaChart)chart.View;

            var template = avaloniaChart.TooltipTemplate ?? defaultTemplate;
            if (TooltipTemplate != template) TooltipTemplate = template;

            if (!tooltipPoints.Any())
            {
                foreach (var key in activePoints.Keys.ToArray())
                {
                    key.RemoveFromHoverState();
                    activePoints.Remove(key);
                }
                return;
            }

            System.Drawing.PointF? location = null;

            if (chart is CartesianChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetCartesianTooltipLocation(
                    chart.TooltipPosition, new System.Drawing.SizeF((float)Bounds.Width, (float)Bounds.Height));
            }
            if (chart is PieChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetPieTooltipLocation(
                    chart.TooltipPosition, new System.Drawing.SizeF((float)Bounds.Width, (float)Bounds.Height));
            }

            if (location == null) throw new Exception("location not found");

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

            if (Transitions == null)
                Transitions = new Transitions
                {
                    new DoubleTransition { Property = Canvas.TopProperty, Duration = TimeSpan.FromMilliseconds(300) },
                    new DoubleTransition { Property = Canvas.LeftProperty, Duration = TimeSpan.FromMilliseconds(300) },
                };

            Canvas.SetTop(this, y);
            Canvas.SetLeft(this, x);

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

        protected void BuildContent()
        {
            var template = TooltipTemplate ?? defaultTemplate;
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
            if (templated == null) return;

            Content = templated;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public class TooltipBindingContext
    {
        public IEnumerable<TooltipPoint> Points { get; set; } = Enumerable.Empty<TooltipPoint>();

        public FontFamily TooltipFontFamily { get; set; } = new FontFamily("Trebuchet MS");
        public double TooltipFontSize { get; set; }
        public FontWeight TooltipFontWeight { get; set; }
        public FontStyle TooltipFontStyle { get; set; }
        public SolidColorBrush TooltipTextBrush { get; set; } = new SolidColorBrush(Color.FromRgb(35, 35, 35));
        public IBrush TooltipBackground { get; set; } = new SolidColorBrush(Color.FromRgb(250, 250, 250));
    }
}
