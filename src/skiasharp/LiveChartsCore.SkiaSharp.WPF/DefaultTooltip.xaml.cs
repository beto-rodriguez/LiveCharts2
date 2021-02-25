using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    /// <summary>
    /// Interaction logic for DefaultTooltip.xaml
    /// </summary>
    public partial class DefaultTooltip : Popup, IChartTooltip<SkiaDrawingContext>
    {
        private TimeSpan animationsSpeed = TimeSpan.FromMilliseconds(200);
        private IEasingFunction easingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
        private Timer clearHighlightTimer = new Timer();
        private Dictionary<IDrawableTask<SkiaDrawingContext>, HashSet<IDrawable<SkiaDrawingContext>>> highlited;
        private Chart chart;
        private double hideoutCount = 1500;
        private System.Drawing.PointF previousLocation = new System.Drawing.PointF();

        public DefaultTooltip()
        {
            InitializeComponent();
            PopupAnimation = PopupAnimation.Fade;
            Placement = PlacementMode.Relative;

            clearHighlightTimer.Interval = hideoutCount;
            clearHighlightTimer.Elapsed += clearHidelightTimerElapsed;
        }

        private void DefaultTooltip_LayoutUpdated(object sender, EventArgs e)
        {
            Trace.WriteLine(ActualWidth);
        }

        #region dependency properties

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

        void IChartTooltip<SkiaDrawingContext>.Show(IEnumerable<TooltipPoint> foundPoints, IChartView<SkiaDrawingContext> view)
        {
            System.Drawing.PointF? location = null;
            
            if (view is ICartesianChartView<SkiaDrawingContext>)
            {
                location = foundPoints.GetCartesianTooltipLocation(
                    view.TooltipPosition, new System.Drawing.SizeF((float)border.ActualWidth, (float)border.ActualHeight));
            }
            if (view is IPieChartView<SkiaDrawingContext>)
            {
                location = foundPoints.GetPieTooltipLocation(
                    view.TooltipPosition, new System.Drawing.SizeF((float)border.ActualWidth, (float)border.ActualHeight));
            }

            if (location == null) return;
            if (previousLocation.X == location.Value.X && previousLocation.Y == location.Value.Y) return;
            previousLocation = location.Value;

            IsOpen = true;
            Points = foundPoints;

            //UpdateLayout();
            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            var from = PlacementRectangle;
            var to = new Rect(location.Value.X, location.Value.Y, 0, 0);
            if (from == Rect.Empty) from = to;
            var animation = new RectAnimation(from, to, animationsSpeed) { EasingFunction = easingFunction };
            BeginAnimation(PlacementRectangleProperty, animation);

            var wpfChart = (Chart)view;
            FontFamily = wpfChart.TooltipFontFamily ?? new FontFamily("Trebuchet MS");
            TextColor = wpfChart.TooltipTextColor ?? new SolidColorBrush(Color.FromRgb(35, 35, 35));
            FontSize = wpfChart.TooltipFontSize ?? 13;
            FontWeight = wpfChart.TooltipFontWeight ?? FontWeights.Normal;
            FontStyle = wpfChart.TooltipFontStyle ?? FontStyles.Normal;
            FontStretch = wpfChart.TooltipFontStretch ?? FontStretches.Normal;

            var highlightTasks = new Dictionary<IDrawableTask<SkiaDrawingContext>, HashSet<IDrawable<SkiaDrawingContext>>>();
            highlited = highlightTasks;

            void highlightGeometries(TooltipPoint point, IDrawableTask<SkiaDrawingContext> highlightPaintTask)
            {
                // if we have not cleared the geometries of the current series... we do it!
                if (!highlightTasks.TryGetValue(highlightPaintTask, out var highlighPaint))
                {
                    // create a new empty collection (hashSet) to draw our geometries using the highlight paint.
                    highlighPaint = new HashSet<IDrawable<SkiaDrawingContext>>();
                    highlightPaintTask.SetGeometries(highlighPaint);
                    highlightTasks.Add(highlightPaintTask, highlighPaint);
                }

                highlighPaint.Add(((IHighlightableGeometry<SkiaDrawingContext>) point.Point.PointContext.Visual).HighlightableGeometry);
            }

            foreach (var point in foundPoints)
            {
                var hlf = (point.Series as IDrawableSeries<SkiaDrawingContext>)?.HighlightFill;
                var hls = (point.Series as IDrawableSeries<SkiaDrawingContext>)?.HighlightStroke;

                if (hlf != null) highlightGeometries(point, hlf);
                if (hls != null) highlightGeometries(point, hls);
            }

            wpfChart.CoreCanvas.Invalidate();
            chart = wpfChart;

            clearHighlightTimer.Stop();
            clearHighlightTimer.Start();
        }

        private void clearHidelightTimerElapsed(object sender, ElapsedEventArgs e)
        {
            clearHighlightTimer.Stop();
            Dispatcher.Invoke(() =>
            {
                IsOpen = false;

                if (highlited == null || highlited.Count == 0) return;

                foreach (var item in highlited) item.Value.Clear();

                chart.CoreCanvas.Invalidate();
            });
        }
    }
}
