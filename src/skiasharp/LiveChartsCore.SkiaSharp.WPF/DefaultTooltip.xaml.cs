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

using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    /// <inheritdoc cref="IChartTooltip{TDrawingContext}" />
    public partial class DefaultTooltip : Popup, IChartTooltip<SkiaSharpDrawingContext>
    {
        private readonly DataTemplate _defaultTempalte;
        private readonly Dictionary<ChartPoint, object> _activePoints = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTooltip"/> class.
        /// </summary>
        public DefaultTooltip()
        {
            InitializeComponent();
            SetCurrentValue(PopupAnimationProperty, PopupAnimation.Fade);
            SetCurrentValue(PlacementProperty, PlacementMode.Relative);
            _defaultTempalte = (DataTemplate)FindResource("defaultTemplate");
        }

        #region dependency properties

        /// <summary>
        /// The template property
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
           DependencyProperty.Register(
               nameof(Template), typeof(DataTemplate), typeof(DefaultTooltip), new PropertyMetadata(null));

        /// <summary>
        /// The points property
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
           DependencyProperty.Register(
               nameof(Points), typeof(IEnumerable<TooltipPoint>),
               typeof(DefaultTooltip), new PropertyMetadata(new List<TooltipPoint>()));

        /// <summary>
        /// The background property
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
           DependencyProperty.Register(
               nameof(Background), typeof(Brush), typeof(DefaultTooltip),
               new PropertyMetadata(new SolidColorBrush(Color.FromRgb(250, 250, 250))));

        /// <summary>
        /// The font family property
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
           DependencyProperty.Register(
               nameof(FontFamily), typeof(FontFamily), typeof(DefaultTooltip), new PropertyMetadata(new FontFamily("Trebuchet MS")));

        /// <summary>
        /// The font size property
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
           DependencyProperty.Register(
               nameof(FontSize), typeof(double), typeof(DefaultTooltip), new PropertyMetadata(13d));

        /// <summary>
        /// The font weight property
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty =
           DependencyProperty.Register(
               nameof(FontWeightProperty), typeof(FontWeight), typeof(DefaultTooltip), new PropertyMetadata(FontWeights.Normal));

        /// <summary>
        /// The font style property
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty =
           DependencyProperty.Register(
               nameof(FontStyle), typeof(FontStyle), typeof(DefaultTooltip), new PropertyMetadata(FontStyles.Normal));

        /// <summary>
        /// The font stretch property
        /// </summary>
        public static readonly DependencyProperty FontStretchProperty =
           DependencyProperty.Register(
               nameof(FontStretch), typeof(FontStretch), typeof(DefaultTooltip), new PropertyMetadata(FontStretches.Normal));

        /// <summary>
        /// The text color property
        /// </summary>
        public static readonly DependencyProperty TextColorProperty =
          DependencyProperty.Register(
              nameof(TextColor), typeof(SolidColorBrush), typeof(DefaultTooltip), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(250, 250, 250))));

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the animations speed.
        /// </summary>
        /// <value>
        /// The animations speed.
        /// </value>
        public TimeSpan AnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(200);

        /// <summary>
        /// Gets or sets the easing function.
        /// </summary>
        /// <value>
        /// The easing function.
        /// </value>
        public IEasingFunction EasingFunction { get; set; } = new CubicEase() { EasingMode = EasingMode.EaseOut };

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>
        /// The template.
        /// </value>
        public DataTemplate Template
        {
            get => (DataTemplate)GetValue(TemplateProperty);
            set => SetValue(TemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

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

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public FontFamily FontFamily
        {
            get => (FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        /// <value>
        /// The font weight.
        /// </value>
        public FontWeight FontWeight
        {
            get => (FontWeight)GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        /// <value>
        /// The font style.
        /// </value>
        public FontStyle FontStyle
        {
            get => (FontStyle)GetValue(FontStyleProperty);
            set => SetValue(FontStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the font stretch.
        /// </summary>
        /// <value>
        /// The font stretch.
        /// </value>
        public FontStretch FontStretch
        {
            get => (FontStretch)GetValue(FontStretchProperty);
            set => SetValue(FontStretchProperty, value);
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

        #endregion

        void IChartTooltip<SkiaSharpDrawingContext>.Show(IEnumerable<TooltipPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
        {
            var wpfChart = (Chart)chart.View;
            var template = wpfChart.TooltipTemplate ?? _defaultTempalte;
            if (Template != template) Template = template;

            if (!tooltipPoints.Any())
            {
                foreach (var key in _activePoints.Keys.ToArray())
                {
                    key.RemoveFromHoverState();
                    _ = _activePoints.Remove(key);
                }
                return;
            }

            System.Drawing.PointF? location = null;

            if (chart is CartesianChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetCartesianTooltipLocation(
                    chart.TooltipPosition, new System.Drawing.SizeF((float)border.ActualWidth, (float)border.ActualHeight), chart.ControlSize);
            }
            if (chart is PieChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetPieTooltipLocation(
                    chart.TooltipPosition, new System.Drawing.SizeF((float)border.ActualWidth, (float)border.ActualHeight));
            }

            if (location is null) throw new Exception("location not supported");

            IsOpen = true;
            Points = tooltipPoints;

            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            var from = PlacementRectangle;
            var to = new Rect(location.Value.X, location.Value.Y, 0, 0);
            if (from == Rect.Empty) from = to;
            var animation = new RectAnimation(from, to, AnimationsSpeed) { EasingFunction = EasingFunction };
            BeginAnimation(PlacementRectangleProperty, animation);

            Background = wpfChart.TooltipBackground;
            FontFamily = wpfChart.TooltipFontFamily;
            TextColor = wpfChart.TooltipTextBrush;
            FontSize = wpfChart.TooltipFontSize;
            FontWeight = wpfChart.TooltipFontWeight;
            FontStyle = wpfChart.TooltipFontStyle;
            FontStretch = wpfChart.TooltipFontStretch;

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

            wpfChart.CoreCanvas.Invalidate();
        }

        void IChartTooltip<SkiaSharpDrawingContext>.Hide()
        {
            IsOpen = false;
        }
    }
}
