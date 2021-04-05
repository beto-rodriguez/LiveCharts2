using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    /// <summary>
    /// Interaction logic for DefaultTooltip.xaml
    /// </summary>
    public partial class DefaultTooltip : Popup, IChartTooltip<SkiaSharpDrawingContext>
    {
        private readonly DataTemplate defaultTempalte;
        private TimeSpan animationsSpeed = TimeSpan.FromMilliseconds(200);
        private IEasingFunction easingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
        private double hideoutCount = 1500;
        private Dictionary<ChartPoint, object> activePoints = new();

        public DefaultTooltip()
        {
            InitializeComponent();
            PopupAnimation = PopupAnimation.Fade;
            Placement = PlacementMode.Relative;
            defaultTempalte = FindResource("defaultTemplate") as DataTemplate;
        }

        #region dependency properties

        public static readonly DependencyProperty TemplateProperty =
           DependencyProperty.Register(
               nameof(Template), typeof(DataTemplate), typeof(DefaultTooltip), new PropertyMetadata(null));

        public static readonly DependencyProperty PointsProperty =
           DependencyProperty.Register(
               nameof(Points), typeof(IEnumerable<TooltipPoint>),
               typeof(DefaultTooltip), new PropertyMetadata(new List<TooltipPoint>()));

        public static readonly DependencyProperty FontFamilyProperty =
           DependencyProperty.Register(
               nameof(FontFamily), typeof(FontFamily), typeof(DefaultTooltip), new PropertyMetadata(new FontFamily("Trebuchet MS")));

        public static readonly DependencyProperty FontSizeProperty =
           DependencyProperty.Register(
               nameof(FontSize), typeof(double), typeof(DefaultTooltip), new PropertyMetadata(13d));

        public static readonly DependencyProperty FontWeightProperty =
           DependencyProperty.Register(
               nameof(FontWeightProperty), typeof(FontWeight), typeof(DefaultTooltip), new PropertyMetadata(FontWeights.Normal));

        public static readonly DependencyProperty FontStyleProperty =
           DependencyProperty.Register(
               nameof(FontStyle), typeof(FontStyle), typeof(DefaultTooltip), new PropertyMetadata(FontStyles.Normal));

        public static readonly DependencyProperty FontStretchProperty =
           DependencyProperty.Register(
               nameof(FontStretch), typeof(FontStretch), typeof(DefaultTooltip), new PropertyMetadata(FontStretches.Normal));

        public static readonly DependencyProperty TextColorProperty =
          DependencyProperty.Register(
              nameof(TextColor), typeof(SolidColorBrush), typeof(DefaultTooltip), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(250, 250, 250))));

        #endregion

        #region properties

        public TimeSpan AnimationsSpeed { get => animationsSpeed; set => animationsSpeed = value; }
        public IEasingFunction EasingFunction { get => easingFunction; set => easingFunction = value; }
        public double HideoutCount { get => hideoutCount; set => hideoutCount = value; }

        public DataTemplate Template
        {
            get { return (DataTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        public IEnumerable<TooltipPoint> Points
        {
            get { return (IEnumerable<TooltipPoint>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        public SolidColorBrush TextColor
        {
            get { return (SolidColorBrush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        #endregion

        void IChartTooltip<SkiaSharpDrawingContext>.Show(IEnumerable<TooltipPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
        {
            var wpfChart = (Chart)chart.View;
            var template = wpfChart.TooltipTemplate ?? defaultTempalte;
            if (Template != template) Template = template;

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
                    chart.TooltipPosition, new System.Drawing.SizeF((float)border.ActualWidth, (float)border.ActualHeight));
            }
            if (chart is PieChart<SkiaSharpDrawingContext>)
            {
                location = tooltipPoints.GetPieTooltipLocation(
                    chart.TooltipPosition, new System.Drawing.SizeF((float)border.ActualWidth, (float)border.ActualHeight));
            }

            IsOpen = true;
            Points = tooltipPoints;

            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            var from = PlacementRectangle;
            var to = new Rect(location.Value.X, location.Value.Y, 0, 0);
            if (from == Rect.Empty) from = to;
            var animation = new RectAnimation(from, to, animationsSpeed) { EasingFunction = easingFunction };
            BeginAnimation(PlacementRectangleProperty, animation);

            FontFamily = wpfChart.TooltipFontFamily ?? new FontFamily("Trebuchet MS");
            TextColor = wpfChart.TooltipTextColor ?? new SolidColorBrush(Color.FromRgb(35, 35, 35));
            FontSize = wpfChart.TooltipFontSize ?? 13;
            FontWeight = wpfChart.TooltipFontWeight ?? FontWeights.Normal;
            FontStyle = wpfChart.TooltipFontStyle ?? FontStyles.Normal;
            FontStretch = wpfChart.TooltipFontStretch ?? FontStretches.Normal;

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

            wpfChart.CoreCanvas.Invalidate();
        }
    }
}
