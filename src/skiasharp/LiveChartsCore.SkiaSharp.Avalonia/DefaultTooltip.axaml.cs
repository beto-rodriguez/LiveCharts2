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
        private Border border;
        private readonly Dictionary<ChartPoint, object> activePoints = new();
        private TimeSpan animationsSpeed = TimeSpan.FromMilliseconds(200);
        private double hideoutCount = 1500;

        public DefaultTooltip()
        {
            InitializeComponent();
            var t = (DataTemplate?)Resources["defaultTemplate"];
            if (t == null) throw new Exception("default tempalte not found");
            defaultTemplate = t;
            border = this.FindControl<Border>("border");
            Canvas.SetTop(this, 0);
            Canvas.SetLeft(this, 0);
        }

        #region dependency properties

        public static readonly AvaloniaProperty<DataTemplate> TooltipTemplateProperty =
            AvaloniaProperty.Register<DefaultTooltip, DataTemplate>(nameof(Template), null, inherits: true);

        public static readonly AvaloniaProperty<IEnumerable<TooltipPoint>> PointsProperty =
            AvaloniaProperty.Register<DefaultTooltip, IEnumerable<TooltipPoint>>(nameof(Points), null, inherits: true);

        public static readonly AvaloniaProperty<IBrush> TextColorProperty =
            AvaloniaProperty.Register<DefaultTooltip, IBrush>(nameof(TextColor), new SolidColorBrush(Color.FromRgb(250, 250, 250)), inherits: true);

        #endregion

        #region properties

        public TimeSpan AnimationsSpeed { get => animationsSpeed; set => animationsSpeed = value; }

        public double HideoutCount { get => hideoutCount; set => hideoutCount = value; }

        public DataTemplate TooltipTemplate
        {
            get { return (DataTemplate)GetValue(TooltipTemplateProperty); }
            set { SetValue(TooltipTemplateProperty, value); }
        }

        public IEnumerable<TooltipPoint> Points
        {
            get { return (IEnumerable<TooltipPoint>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        public SolidColorBrush TextColor
        {
            get { return (SolidColorBrush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

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

            FontFamily = avaloniaChart.TooltipFontFamily ?? new FontFamily("Trebuchet MS");
            TextColor = avaloniaChart.TooltipTextBrush ?? new SolidColorBrush(Color.FromRgb(35, 35, 35));
            FontSize = avaloniaChart.TooltipFontSize;
            FontWeight = avaloniaChart.TooltipFontWeight;
            FontStyle = avaloniaChart.TooltipFontStyle;

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

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
