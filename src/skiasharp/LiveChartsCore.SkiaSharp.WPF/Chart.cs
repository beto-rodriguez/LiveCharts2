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
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Brush = System.Windows.Media.Brush;
using FontFamily = System.Windows.Media.FontFamily;
using FontStyle = System.Windows.FontStyle;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    /// <inheritdoc cref="IChartView{TDrawingContext}" />
    public abstract class Chart : Control, IChartView<SkiaSharpDrawingContext>
    {
        #region fields

        /// <summary>
        /// The core
        /// </summary>
        protected Chart<SkiaSharpDrawingContext>? core;

        /// <summary>
        /// The canvas
        /// </summary>
        protected MotionCanvas? canvas;

        /// <summary>
        /// The legend
        /// </summary>
        protected IChartLegend<SkiaSharpDrawingContext>? legend;

        /// <summary>
        /// The tool tip
        /// </summary>
        protected IChartTooltip<SkiaSharpDrawingContext>? tooltip;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Chart"/> class.
        /// </summary>
        /// <exception cref="Exception">Default colors are not valid</exception>
        protected Chart()
        {
            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetVisualsInitializer();
            if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ApplyStyleToChart(this);

            SizeChanged += OnSizeChanged;
            MouseMove += OnMouseMove;
            MouseLeave += OnMouseLeave;
        }

        #region dependency properties

        /// <summary>
        /// The draw margin property
        /// </summary>
        public static readonly DependencyProperty SyncContextProperty =
           DependencyProperty.Register(
               nameof(SyncContext), typeof(object), typeof(Chart), new PropertyMetadata(null,
                   (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                   {
                       var chart = (Chart)o;
                       if (chart.canvas != null) chart.CoreCanvas.Sync = args.NewValue;
                       if (chart.core is null) return;
                       chart.core.Update();
                   }));

        /// <summary>
        /// The draw margin property
        /// </summary>
        public static readonly DependencyProperty DrawMarginProperty =
           DependencyProperty.Register(
               nameof(DrawMargin), typeof(Margin), typeof(Chart), new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// The animations speed property
        /// </summary>
        public static readonly DependencyProperty AnimationsSpeedProperty =
            DependencyProperty.Register(
                nameof(AnimationsSpeed), typeof(TimeSpan), typeof(Chart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultAnimationsSpeed, OnDependencyPropertyChanged));

        /// <summary>
        /// The easing function property
        /// </summary>
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register(
                nameof(EasingFunction), typeof(Func<float, float>), typeof(Chart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultEasingFunction, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend position property
        /// </summary>
        public static readonly DependencyProperty LegendPositionProperty =
            DependencyProperty.Register(
                nameof(LegendPosition), typeof(LegendPosition), typeof(Chart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultLegendPosition, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend orientation property
        /// </summary>
        public static readonly DependencyProperty LegendOrientationProperty =
            DependencyProperty.Register(
                nameof(LegendOrientation), typeof(LegendOrientation), typeof(Chart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultLegendOrientation, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip position property
        /// </summary>
        public static readonly DependencyProperty TooltipPositionProperty =
           DependencyProperty.Register(
               nameof(TooltipPosition), typeof(TooltipPosition), typeof(Chart),
               new PropertyMetadata(LiveCharts.CurrentSettings.DefaultTooltipPosition, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip background property
        /// </summary>
        public static readonly DependencyProperty TooltipBackgroundProperty =
           DependencyProperty.Register(
               nameof(TooltipBackground), typeof(SolidColorBrush), typeof(Chart),
               new PropertyMetadata(new SolidColorBrush(System.Windows.Media.Color.FromRgb(250, 250, 250)), OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip font family property
        /// </summary>
        public static readonly DependencyProperty TooltipFontFamilyProperty =
           DependencyProperty.Register(
               nameof(TooltipFontFamily), typeof(FontFamily), typeof(Chart),
               new PropertyMetadata(new FontFamily("Trebuchet MS"), OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip text color property
        /// </summary>
        public static readonly DependencyProperty TooltipTextBrushProperty =
           DependencyProperty.Register(
               nameof(TooltipTextBrush), typeof(SolidColorBrush), typeof(Chart),
               new PropertyMetadata(new SolidColorBrush(System.Windows.Media.Color.FromRgb(35, 35, 35)), OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip font size property
        /// </summary>
        public static readonly DependencyProperty TooltipFontSizeProperty =
           DependencyProperty.Register(
               nameof(TooltipFontSize), typeof(double), typeof(Chart), new PropertyMetadata(13d, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip font weight property
        /// </summary>
        public static readonly DependencyProperty TooltipFontWeightProperty =
           DependencyProperty.Register(
               nameof(TooltipFontWeight), typeof(FontWeight), typeof(Chart),
               new PropertyMetadata(FontWeights.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip font stretch property
        /// </summary>
        public static readonly DependencyProperty TooltipFontStretchProperty =
           DependencyProperty.Register(
               nameof(TooltipFontStretch), typeof(FontStretch), typeof(Chart),
               new PropertyMetadata(FontStretches.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip font style property
        /// </summary>
        public static readonly DependencyProperty TooltipFontStyleProperty =
           DependencyProperty.Register(
               nameof(TooltipFontStyle), typeof(FontStyle), typeof(Chart),
               new PropertyMetadata(FontStyles.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip template property
        /// </summary>
        public static readonly DependencyProperty TooltipTemplateProperty =
            DependencyProperty.Register(
                nameof(TooltipTemplate), typeof(DataTemplate), typeof(Chart), new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend font family property
        /// </summary>
        public static readonly DependencyProperty LegendFontFamilyProperty =
           DependencyProperty.Register(
               nameof(LegendFontFamily), typeof(FontFamily), typeof(Chart),
               new PropertyMetadata(new FontFamily("Trebuchet MS"), OnDependencyPropertyChanged));

        /// <summary>
        /// The legend text color property
        /// </summary>
        public static readonly DependencyProperty LegendTextBrushProperty =
           DependencyProperty.Register(
               nameof(LegendTextBrush), typeof(SolidColorBrush), typeof(Chart),
               new PropertyMetadata(new SolidColorBrush(System.Windows.Media.Color.FromRgb(35, 35, 35)), OnDependencyPropertyChanged));

        /// <summary>
        /// The legend background property
        /// </summary>
        public static readonly DependencyProperty LegendBackgroundProperty =
           DependencyProperty.Register(
               nameof(LegendBackground), typeof(SolidColorBrush), typeof(Chart),
               new PropertyMetadata(new SolidColorBrush(System.Windows.Media.Color.FromRgb(35, 35, 35)), OnDependencyPropertyChanged));

        /// <summary>
        /// The legend font size property
        /// </summary>
        public static readonly DependencyProperty LegendFontSizeProperty =
           DependencyProperty.Register(
               nameof(LegendFontSize), typeof(double), typeof(Chart), new PropertyMetadata(13d, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend font weight property
        /// </summary>
        public static readonly DependencyProperty LegendFontWeightProperty =
           DependencyProperty.Register(
               nameof(LegendFontWeight), typeof(FontWeight), typeof(Chart),
               new PropertyMetadata(FontWeights.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend font stretch property
        /// </summary>
        public static readonly DependencyProperty LegendFontStretchProperty =
           DependencyProperty.Register(
               nameof(LegendFontStretch), typeof(FontStretch), typeof(Chart),
               new PropertyMetadata(FontStretches.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend font style property
        /// </summary>
        public static readonly DependencyProperty LegendFontStyleProperty =
           DependencyProperty.Register(
               nameof(LegendFontStyle), typeof(FontStyle), typeof(Chart),
               new PropertyMetadata(FontStyles.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend template property
        /// </summary>
        public static readonly DependencyProperty LegendTemplateProperty =
            DependencyProperty.Register(
                nameof(LegendTemplate), typeof(DataTemplate), typeof(Chart), new PropertyMetadata(null, OnDependencyPropertyChanged));

        #endregion

        #region events

        /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? Measuring;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateStarted;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateFinished;

        #endregion

        #region properties

        /// <inheritdoc cref="IChartView.CoreChart" />
        public IChart CoreChart => core ?? throw new Exception("Core not set yet.");

        System.Drawing.Color IChartView.BackColor
        {
            get => Background is not SolidColorBrush b
                    ? new System.Drawing.Color()
                    : System.Drawing.Color.FromArgb(b.Color.A, b.Color.R, b.Color.G, b.Color.B);
            set => SetValueOrCurrentValue(BackgroundProperty, new SolidColorBrush(System.Windows.Media.Color.FromArgb(value.A, value.R, value.G, value.B)));
        }

        /// <inheritdoc cref="IChartView.SyncContext" />
        public object SyncContext
        {
            get => GetValue(SyncContextProperty);
            set => SetValue(SyncContextProperty, value);
        }

        /// <inheritdoc cref="IChartView.DrawMargin" />
        public Margin? DrawMargin
        {
            get => (Margin)GetValue(DrawMarginProperty);
            set => SetValue(DrawMarginProperty, value);
        }

        Margin? IChartView.DrawMargin
        {
            get => DrawMargin;
            set => SetValueOrCurrentValue(DrawMarginProperty, value);
        }

        SizeF IChartView.ControlSize => canvas is null
                    ? throw new Exception("Canvas not found")
                    : (new() { Width = (float)canvas.ActualWidth, Height = (float)canvas.ActualHeight });

        /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => canvas is null ? throw new Exception("Canvas not found") : canvas.CanvasCore;

        /// <inheritdoc cref="IChartView.AnimationsSpeed" />
        public TimeSpan AnimationsSpeed
        {
            get => (TimeSpan)GetValue(AnimationsSpeedProperty);
            set => SetValue(AnimationsSpeedProperty, value);
        }

        TimeSpan IChartView.AnimationsSpeed
        {
            get => AnimationsSpeed;
            set => SetValueOrCurrentValue(AnimationsSpeedProperty, value);
        }

        /// <inheritdoc cref="IChartView.EasingFunction" />
        public Func<float, float> EasingFunction
        {
            get => (Func<float, float>)GetValue(EasingFunctionProperty);
            set => SetValue(EasingFunctionProperty, value);
        }

        Func<float, float>? IChartView.EasingFunction
        {
            get => EasingFunction;
            set => SetValueOrCurrentValue(EasingFunctionProperty, value);
        }

        /// <inheritdoc cref="IChartView.LegendPosition" />
        public LegendPosition LegendPosition
        {
            get => (LegendPosition)GetValue(LegendPositionProperty);
            set => SetValue(LegendPositionProperty, value);
        }

        LegendPosition IChartView.LegendPosition
        {
            get => LegendPosition;
            set => SetValueOrCurrentValue(LegendPositionProperty, value);
        }

        /// <inheritdoc cref="IChartView.LegendOrientation" />
        public LegendOrientation LegendOrientation
        {
            get => (LegendOrientation)GetValue(LegendOrientationProperty);
            set => SetValue(LegendOrientationProperty, value);
        }

        LegendOrientation IChartView.LegendOrientation
        {
            get => LegendOrientation;
            set => SetValueOrCurrentValue(LegendOrientationProperty, value);
        }

        /// <inheritdoc cref="IChartView.TooltipPosition" />
        public TooltipPosition TooltipPosition
        {
            get => (TooltipPosition)GetValue(TooltipPositionProperty);
            set => SetValue(TooltipPositionProperty, value);
        }

        TooltipPosition IChartView.TooltipPosition
        {
            get => TooltipPosition;
            set => SetValueOrCurrentValue(TooltipPositionProperty, value);
        }

        /// <summary>
        /// Gets or sets the tool tip template.
        /// </summary>
        /// <value>
        /// The tool tip template.
        /// </value>
        public DataTemplate? TooltipTemplate
        {
            get => (DataTemplate?)GetValue(TooltipTemplateProperty);
            set => SetValue(TooltipTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the default tool tip background.
        /// </summary>
        /// <value>
        /// The tool tip background.
        /// </value>
        public Brush TooltipBackground
        {
            get => (Brush)GetValue(TooltipBackgroundProperty);
            set => SetValue(TooltipBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the default tool tip font family.
        /// </summary>
        /// <value>
        /// The tool tip font family.
        /// </value>
        public FontFamily TooltipFontFamily
        {
            get => (FontFamily)GetValue(TooltipFontFamilyProperty);
            set => SetValue(TooltipFontFamilyProperty, value);
        }

        /// <summary>
        /// Gets or sets the default color of the tool tip text.
        /// </summary>
        /// <value>
        /// The color of the tool tip text.
        /// </value>
        public SolidColorBrush TooltipTextBrush
        {
            get => (SolidColorBrush)GetValue(TooltipTextBrushProperty);
            set => SetValue(TooltipTextBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the default size of the tool tip font.
        /// </summary>
        /// <value>
        /// The size of the tool tip font.
        /// </value>
        public double TooltipFontSize
        {
            get => (double)GetValue(TooltipFontSizeProperty);
            set => SetValue(TooltipFontSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the default tool tip font weight.
        /// </summary>
        /// <value>
        /// The tool tip font weight.
        /// </value>
        public FontWeight TooltipFontWeight
        {
            get => (FontWeight)GetValue(TooltipFontWeightProperty);
            set => SetValue(TooltipFontWeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the default tool tip font stretch.
        /// </summary>
        /// <value>
        /// The tool tip font stretch.
        /// </value>
        public FontStretch TooltipFontStretch
        {
            get => (FontStretch)GetValue(TooltipFontStretchProperty);
            set => SetValue(TooltipFontStretchProperty, value);
        }

        /// <summary>
        /// Gets or sets the default tool tip font style.
        /// </summary>
        /// <value>
        /// The tool tip font style.
        /// </value>
        public FontStyle TooltipFontStyle
        {
            get => (FontStyle)GetValue(TooltipFontStyleProperty);
            set => SetValue(TooltipFontStyleProperty, value);
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip" />
        public IChartTooltip<SkiaSharpDrawingContext>? Tooltip => tooltip;

        /// <summary>
        /// Gets or sets the legend template.
        /// </summary>
        /// <value>
        /// The legend template.
        /// </value>
        public DataTemplate? LegendTemplate
        {
            get => (DataTemplate?)GetValue(LegendTemplateProperty);
            set => SetValue(LegendTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the default legend font family.
        /// </summary>
        /// <value>
        /// The legend font family.
        /// </value>
        public FontFamily LegendFontFamily
        {
            get => (FontFamily)GetValue(LegendFontFamilyProperty);
            set => SetValue(LegendFontFamilyProperty, value);
        }

        /// <summary>
        /// Gets or sets the default color of the legend text.
        /// </summary>
        /// <value>
        /// The color of the legend text.
        /// </value>
        public SolidColorBrush LegendTextBrush
        {
            get => (SolidColorBrush)GetValue(LegendTextBrushProperty);
            set => SetValue(LegendTextBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend background.
        /// </summary>
        /// <value>
        /// The legend t background.
        /// </value>
        public SolidColorBrush LegendBackground
        {
            get => (SolidColorBrush)GetValue(LegendBackgroundProperty);
            set => SetValue(LegendBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the default size of the legend font.
        /// </summary>
        /// <value>
        /// The size of the legend font.
        /// </value>
        public double LegendFontSize
        {
            get => (double)GetValue(LegendFontSizeProperty);
            set => SetValue(LegendFontSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the default legend font weight.
        /// </summary>
        /// <value>
        /// The legend font weight.
        /// </value>
        public FontWeight LegendFontWeight
        {
            get => (FontWeight)GetValue(LegendFontWeightProperty);
            set => SetValue(LegendFontWeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the default legend font stretch.
        /// </summary>
        /// <value>
        /// The legend font stretch.
        /// </value>
        public FontStretch LegendFontStretch
        {
            get => (FontStretch)GetValue(LegendFontStretchProperty);
            set => SetValue(LegendFontStretchProperty, value);
        }

        /// <summary>
        /// Gets or sets the default legend font style.
        /// </summary>
        /// <value>
        /// The legend font style.
        /// </value>
        public FontStyle LegendFontStyle
        {
            get => (FontStyle)GetValue(LegendFontStyleProperty);
            set => SetValue(LegendFontStyleProperty, value);
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Legend" />
        public IChartLegend<SkiaSharpDrawingContext>? Legend => legend;

        /// <inheritdoc cref="IChartView{TDrawingContext}.PointStates" />
        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; } = new();

        /// <inheritdoc cref="IChartView{TDrawingContext}.AutoUpdateEnabled" />
        public bool AutoUpdateEnabled { get; set; } = true;

        /// <inheritdoc cref="IChartView.UpdaterThrottler" />
        public TimeSpan UpdaterThrottler
        {
            get => core?.UpdaterThrottler ?? throw new Exception("core not set yet.");
            set
            {
                if (core is null) throw new Exception("core not set yet.");
                core.UpdaterThrottler = value;
            }
        }

        #endregion

        /// <inheritdoc cref="OnApplyTemplate" />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template.FindName("canvas", this) is not MotionCanvas canvas)
                throw new Exception(
                    $"{nameof(MotionCanvas)} not found. This was probably caused because the control {nameof(CartesianChart)} template was overridden, " +
                    $"If you override the template please add an {nameof(MotionCanvas)} to the template and name it 'canvas'");

            this.canvas = canvas;

            if (SyncContext != null)
                this.canvas.CanvasCore.Sync = SyncContext;

            InitializeCore();

            if (core is null) throw new Exception("Core not found!");
            core.Measuring += OnCoreMeasuring;
            core.UpdateStarted += OnCoreUpdateStarted;
            core.UpdateFinished += OnCoreUpdateFinished;
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.ShowTooltip(IEnumerable{TooltipPoint})"/>
        public void ShowTooltip(IEnumerable<TooltipPoint> points)
        {
            if (tooltip is null || core is null) return;

            tooltip.Show(points, core);
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.HideTooltip"/>
        public void HideTooltip()
        {
            if (tooltip is null || core is null) return;

            foreach (var state in PointStates.GetStates())
            {
                if (!state.IsHoverState) continue;
                if (state.Fill is not null) state.Fill.ClearGeometriesFromPaintTask(core.Canvas);
                if (state.Stroke is not null) state.Stroke.ClearGeometriesFromPaintTask(core.Canvas);
            }

            tooltip.Hide();
        }

        /// <inheritdoc cref="IChartView.SetTooltipStyle(System.Drawing.Color, System.Drawing.Color)"/>
        public void SetTooltipStyle(System.Drawing.Color background, System.Drawing.Color textColor)
        {
            TooltipBackground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(background.A, background.R, background.G, background.B));
            TooltipTextBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(textColor.A, textColor.R, textColor.G, textColor.B));
        }

        void IChartView.InvokeOnUIThread(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        /// <inheritdoc cref="IChartView.SyncAction(Action)"/>
        public void SyncAction(Action action)
        {
            lock (CoreCanvas.Sync)
            {
                action();
            }
        }

        /// <summary>
        /// Initializes the core.
        /// </summary>
        /// <returns></returns>
        protected abstract void InitializeCore();

        /// <summary>
        /// Called when a dependency property changes.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        protected static void OnDependencyPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            var chart = (Chart)o;
            if (chart.core is null) return;
            chart.core.Update();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (core is null) return;
            core.Update();
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var p = e.GetPosition(this);
            core?.InvokePointerMove(new PointF((float)p.X, (float)p.Y));
        }

        private void OnCoreUpdateFinished(IChartView<SkiaSharpDrawingContext> chart)
        {
            UpdateFinished?.Invoke(this);
        }

        private void OnCoreUpdateStarted(IChartView<SkiaSharpDrawingContext> chart)
        {
            UpdateStarted?.Invoke(this);
        }

        private void OnCoreMeasuring(IChartView<SkiaSharpDrawingContext> chart)
        {
            Measuring?.Invoke(this);
        }

        /// <summary>
        /// Sets the local value of a dependency property, specified by its dependency property identifier.
        /// If the object has not yet finished initializing, does so without changing its value source.
        /// </summary>
        /// <param name="dp">The identifier of the dependency property to set.</param>
        /// <param name="value">The new local value.</param>
        protected void SetValueOrCurrentValue(DependencyProperty dp, object? value)
        {
            if (IsInitialized)
                SetValue(dp, value);
            else
                SetCurrentValue(dp, value);
        }

        private void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            HideTooltip();
            core?.InvokePointerLeft();
        }
    }
}
