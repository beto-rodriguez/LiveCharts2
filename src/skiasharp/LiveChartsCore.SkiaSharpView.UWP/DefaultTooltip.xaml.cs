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
using System.Runtime.InteropServices.WindowsRuntime;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LiveChartsCore.SkiaSharpView.UWP
{
    public sealed partial class DefaultTooltip : ToolTip, IChartTooltip<SkiaSharpDrawingContext>
    {
        private readonly Dictionary<ChartPoint, object> _activePoints = new();

        /// <summary>
        /// 
        /// </summary>
        public DefaultTooltip()
        {
            DataContext = this;
        }

        /// <summary>
        /// The points property
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
           DependencyProperty.Register(
               nameof(Points), typeof(IEnumerable<TooltipPoint>),
               typeof(DefaultTooltip), new PropertyMetadata(new List<TooltipPoint>()));

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>
        /// The points.
        /// </value>
        public IEnumerable<TooltipPoint> Points
        {
            get => (IEnumerable<TooltipPoint>)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        void IChartTooltip<SkiaSharpDrawingContext>.Show(IEnumerable<TooltipPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
        {
            var winuiChart = (IUwpChart)chart.View;
            //var template = wpfChart.TooltipTemplate ?? _defaultTempalte;
            //if (Template != template) Template = template;

            if (!tooltipPoints.Any())
            {
                foreach (var key in _activePoints.Keys.ToArray())
                {
                    key.RemoveFromHoverState();
                    _ = _activePoints.Remove(key);
                }
                return;
            }

            if (_activePoints.Count > 0 && tooltipPoints.All(x => _activePoints.ContainsKey(x.Point))) return;

            System.Drawing.PointF? location = null;

            IsOpen = true;
            Points = tooltipPoints;

            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            if (chart is CartesianChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetCartesianTooltipLocation(
                    chart.TooltipPosition, new System.Drawing.SizeF((float)DesiredSize.Width, (float)DesiredSize.Height), chart.ControlSize);
            }
            if (chart is PieChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetPieTooltipLocation(
                    chart.TooltipPosition, new System.Drawing.SizeF((float)DesiredSize.Width, (float)DesiredSize.Height));
            }

            if (location is null) throw new Exception("location not supported");

            PlacementRect = new Rect(location.Value.X, location.Value.Y, 0, 0);

            Background = winuiChart.TooltipBackground;
            FontFamily = winuiChart.TooltipFontFamily;
            //TextColor = wpfChart.TooltipTextBrush;
            FontSize = winuiChart.TooltipFontSize;
            FontWeight = winuiChart.TooltipFontWeight;
            FontStyle = winuiChart.TooltipFontStyle;
            FontStretch = winuiChart.TooltipFontStretch;

            var o = new object();
            foreach (var tooltipPoint in tooltipPoints)
            {
                tooltipPoint.Point.AddToHoverState();
                _activePoints[tooltipPoint.Point] = o;
            }

            foreach (var key in _activePoints.Keys.ToArray())
            {
                if (_activePoints[key] == o) continue;
                key.RemoveFromHoverState();
                _ = _activePoints.Remove(key);
            }

            //winuiChart.CoreCanvas.Invalidate();
        }

        void IChartTooltip<SkiaSharpDrawingContext>.Hide()
        {
            IsOpen = false;
        }
    }
}
