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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Collections.ObjectModel;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using A = Avalonia;
using Avalonia.Input;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.SkiaSharpView.Avalonia
{
    /// <inheritdoc cref="IPieChartView{TDrawingContext}" />
    public class PieChart : UserControl, IPieChartView<SkiaSharpDrawingContext>, IAvaloniaChart
    {
        #region fields

        /// <summary>
        /// The core
        /// </summary>
        protected Chart<SkiaSharpDrawingContext>? core;

        /// <summary>
        /// The legend
        /// </summary>
        protected IChartLegend<SkiaSharpDrawingContext>? legend;

        /// <summary>
        /// The tooltip
        /// </summary>
        protected IChartTooltip<SkiaSharpDrawingContext>? tooltip;

        private readonly CollectionDeepObserver<ISeries> _seriesObserver;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PieChart"/> class.
        /// </summary>
        /// <exception cref="Exception">Default colors are not valid</exception>
        public PieChart()
        {
            InitializeComponent();

            // workaround to detect mouse events.
            // Avalonia do not seem to detect events if background is not set.
            Background = new SolidColorBrush(Colors.Transparent);

            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetVisualsInitializer();
            if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ApplyStyleToChart(this);

            InitializeCore();

            _seriesObserver = new CollectionDeepObserver<ISeries>(
               (object? sender, NotifyCollectionChangedEventArgs e) =>
               {
                   if (core is null) return;
                   _ = Dispatcher.UIThread.InvokeAsync(() => core.Update(), DispatcherPriority.Background);
               },
               (object? sender, PropertyChangedEventArgs e) =>
               {
                   if (core is null) return;
                   _ = Dispatcher.UIThread.InvokeAsync(() => core.Update(), DispatcherPriority.Background);
               });

            Series = new ObservableCollection<ISeries>();
            PointerLeave += CartesianChart_PointerLeave;

            PointerMoved += CartesianChart_PointerMoved;
        }

        #region avalonia/dependency properties

        /// <summary>
        /// The draw margin property
        /// </summary>
        public static readonly AvaloniaProperty<Margin?> DrawMarginProperty =
           AvaloniaProperty.Register<CartesianChart, Margin?>(nameof(DrawMargin), null, inherits: true);

        /// <summary>
        /// The sync context property.
        /// </summary>
        public static readonly AvaloniaProperty<object> SyncContextProperty =
           AvaloniaProperty.Register<CartesianChart, object>(nameof(SyncContext), new object(), inherits: true);

        /// <summary>
        /// The series property
        /// </summary>
        public static readonly AvaloniaProperty<IEnumerable<ISeries>> SeriesProperty =
            AvaloniaProperty.Register<PieChart, IEnumerable<ISeries>>(nameof(Series), new List<ISeries>(), inherits: true);

        /// <summary>
        /// The initial rotation property
        /// </summary>
        public static readonly AvaloniaProperty<double> InitialRotationProperty =
            AvaloniaProperty.Register<PieChart, double>(nameof(InitialRotation), 0d, inherits: true);

        /// <summary>
        /// The maximum angle property
        /// </summary>
        public static readonly AvaloniaProperty<double> MaxAngleProperty =
            AvaloniaProperty.Register<PieChart, double>(nameof(MaxAngle), 360d, inherits: true);

        /// <summary>
        /// The total property
        /// </summary>
        public static readonly AvaloniaProperty<double?> TotalProperty =
            AvaloniaProperty.Register<PieChart, double?>(nameof(Total), null, inherits: true);

        /// <summary>
        /// The animations speed property
        /// </summary>
        public static readonly AvaloniaProperty<TimeSpan> AnimationsSpeedProperty =
            AvaloniaProperty.Register<PieChart, TimeSpan>(
                nameof(AnimationsSpeed), LiveCharts.CurrentSettings.DefaultAnimationsSpeed, inherits: true);

        /// <summary>
        /// The easing function property
        /// </summary>
        public static readonly AvaloniaProperty<Func<float, float>> EasingFunctionProperty =
            AvaloniaProperty.Register<PieChart, Func<float, float>>(
                nameof(AnimationsSpeed), LiveCharts.CurrentSettings.DefaultEasingFunction, inherits: true);

        /// <summary>
        /// The tool tip template property
        /// </summary>
        public static readonly AvaloniaProperty<DataTemplate?> TooltipTemplateProperty =
            AvaloniaProperty.Register<CartesianChart, DataTemplate?>(nameof(TooltipTemplate), null, inherits: true);

        /// <summary>
        /// The tool tip position property
        /// </summary>
        public static readonly AvaloniaProperty<TooltipPosition> TooltipPositionProperty =
            AvaloniaProperty.Register<CartesianChart, TooltipPosition>(
                nameof(TooltipPosition), LiveCharts.CurrentSettings.DefaultTooltipPosition, inherits: true);

        /// <summary>
        /// The tool tip font family property
        /// </summary>
        public static readonly AvaloniaProperty<A.Media.FontFamily> TooltipFontFamilyProperty =
            AvaloniaProperty.Register<CartesianChart, A.Media.FontFamily>(
                nameof(TooltipFontFamily), new A.Media.FontFamily("Arial"), inherits: true);

        /// <summary>
        /// The tool tip font size property
        /// </summary>
        public static readonly AvaloniaProperty<double> TooltipFontSizeProperty =
            AvaloniaProperty.Register<CartesianChart, double>(nameof(TooltipFontSize), 13d, inherits: true);

        /// <summary>
        /// The tool tip font weight property
        /// </summary>
        public static readonly AvaloniaProperty<FontWeight> TooltipFontWeightProperty =
            AvaloniaProperty.Register<CartesianChart, FontWeight>(nameof(TooltipFontWeight), FontWeight.Normal, inherits: true);

        /// <summary>
        /// The tool tip font style property
        /// </summary>
        public static readonly AvaloniaProperty<A.Media.FontStyle> TooltipFontStyleProperty =
            AvaloniaProperty.Register<CartesianChart, A.Media.FontStyle>(
                nameof(TooltipFontStyle), A.Media.FontStyle.Normal, inherits: true);

        /// <summary>
        /// The tool tip text brush property
        /// </summary>
        public static readonly AvaloniaProperty<SolidColorBrush> TooltipTextBrushProperty =
            AvaloniaProperty.Register<CartesianChart, SolidColorBrush>(
                nameof(TooltipTextBrush), new SolidColorBrush(new A.Media.Color(255, 35, 35, 35)), inherits: true);

        /// <summary>
        /// The tool tip background property
        /// </summary>
        public static readonly AvaloniaProperty<IBrush> TooltipBackgroundProperty =
            AvaloniaProperty.Register<CartesianChart, IBrush>(nameof(TooltipBackground),
                new SolidColorBrush(new A.Media.Color(255, 250, 250, 250)), inherits: true);

        /// <summary>
        /// The legend position property
        /// </summary>
        public static readonly AvaloniaProperty<LegendPosition> LegendPositionProperty =
            AvaloniaProperty.Register<CartesianChart, LegendPosition>(
                nameof(LegendPosition), LiveCharts.CurrentSettings.DefaultLegendPosition, inherits: true);

        /// <summary>
        /// The legend orientation property
        /// </summary>
        public static readonly AvaloniaProperty<LegendOrientation> LegendOrientationProperty =
            AvaloniaProperty.Register<CartesianChart, LegendOrientation>(
                nameof(LegendOrientation), LiveCharts.CurrentSettings.DefaultLegendOrientation, inherits: true);

        /// <summary>
        /// The legend template property
        /// </summary>
        public static readonly AvaloniaProperty<DataTemplate?> LegendTemplateProperty =
            AvaloniaProperty.Register<CartesianChart, DataTemplate?>(nameof(LegendTemplate), null, inherits: true);

        /// <summary>
        /// The legend font family property
        /// </summary>
        public static readonly AvaloniaProperty<A.Media.FontFamily> LegendFontFamilyProperty =
           AvaloniaProperty.Register<CartesianChart, A.Media.FontFamily>(
               nameof(LegendFontFamily), new A.Media.FontFamily("Arial"), inherits: true);

        /// <summary>
        /// The legend font size property
        /// </summary>
        public static readonly AvaloniaProperty<double> LegendFontSizeProperty =
            AvaloniaProperty.Register<CartesianChart, double>(nameof(LegendFontSize), 13d, inherits: true);

        /// <summary>
        /// The legend font weight property
        /// </summary>
        public static readonly AvaloniaProperty<FontWeight> LegendFontWeightProperty =
            AvaloniaProperty.Register<CartesianChart, FontWeight>(nameof(LegendFontWeight), FontWeight.Normal, inherits: true);

        /// <summary>
        /// The legend font style property
        /// </summary>
        public static readonly AvaloniaProperty<A.Media.FontStyle> LegendFontStyleProperty =
            AvaloniaProperty.Register<CartesianChart, A.Media.FontStyle>(
                nameof(LegendFontStyle), A.Media.FontStyle.Normal, inherits: true);

        /// <summary>
        /// The legend text brush property
        /// </summary>
        public static readonly AvaloniaProperty<SolidColorBrush> LegendTextBrushProperty =
            AvaloniaProperty.Register<CartesianChart, SolidColorBrush>(
                nameof(LegendTextBrush), new SolidColorBrush(new A.Media.Color(255, 35, 35, 35)), inherits: true);

        /// <summary>
        /// The legend background property
        /// </summary>
        public static readonly AvaloniaProperty<IBrush> LegendBackgroundProperty =
            AvaloniaProperty.Register<CartesianChart, IBrush>(nameof(LegendBackground),
                new SolidColorBrush(new A.Media.Color(255, 250, 250, 250)), inherits: true);

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
            get => Background is not ISolidColorBrush b
                    ? new System.Drawing.Color()
                    : System.Drawing.Color.FromArgb(b.Color.A, b.Color.R, b.Color.G, b.Color.B);
            set
            {
                Background = new SolidColorBrush(new A.Media.Color(value.R, value.G, value.B, value.A));
                var canvas = this.FindControl<MotionCanvas>("canvas");
                canvas.BackColor = new SkiaSharp.SKColor(value.R, value.G, value.B, value.A);
            }
        }

        SizeF IChartView.ControlSize => new()
        {
            Width = (float)Bounds.Width,
            Height = (float)Bounds.Height
        };

        /// <inheritdoc cref="IChartView.SyncContext" />
        public object SyncContext
        {
            get => GetValue(SyncContextProperty);
            set => SetValue(SyncContextProperty, value);
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => core is null ? throw new Exception("core not found") : core.Canvas;

        PieChart<SkiaSharpDrawingContext> IPieChartView<SkiaSharpDrawingContext>.Core => core is null ? throw new Exception("core not found") : (PieChart<SkiaSharpDrawingContext>)core;

        /// <inheritdoc cref="IChartView.DrawMargin" />
        public Margin? DrawMargin
        {
            get => (Margin?)GetValue(DrawMarginProperty);
            set => SetValue(DrawMarginProperty, value);
        }

        /// <inheritdoc cref="IPieChartView{TDrawingContext}.Series" />
        public IEnumerable<ISeries> Series
        {
            get => (IEnumerable<ISeries>)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }

        /// <inheritdoc cref="IPieChartView{TDrawingContext}.InitialRotation" />
        public double InitialRotation
        {
            get => (double)GetValue(InitialRotationProperty);
            set => SetValue(InitialRotationProperty, value);
        }

        /// <inheritdoc cref="IPieChartView{TDrawingContext}.MaxAngle" />
        public double MaxAngle
        {
            get => (double)GetValue(MaxAngleProperty);
            set => SetValue(MaxAngleProperty, value);
        }

        /// <inheritdoc cref="IPieChartView{TDrawingContext}.Total" />
        public double? Total
        {
            get => (double?)GetValue(TotalProperty);
            set => SetValue(TotalProperty, value);
        }

        /// <inheritdoc cref="IChartView.AnimationsSpeed" />
        public TimeSpan AnimationsSpeed
        {
            get => (TimeSpan)GetValue(AnimationsSpeedProperty);
            set => SetValue(AnimationsSpeedProperty, value);
        }

        /// <inheritdoc cref="IChartView.EasingFunction" />
        public Func<float, float>? EasingFunction
        {
            get => (Func<float, float>)GetValue(EasingFunctionProperty);
            set => SetValue(EasingFunctionProperty, value);
        }

        /// <inheritdoc cref="IChartView.TooltipPosition" />
        public TooltipPosition TooltipPosition
        {
            get => (TooltipPosition)GetValue(TooltipPositionProperty);
            set => SetValue(TooltipPositionProperty, value);
        }

        /// <summary>
        /// Gets or sets the tool data tip template.
        /// </summary>
        /// <value>
        /// The tool tip template.
        /// </value>
        public DataTemplate TooltipTemplate
        {
            get => (DataTemplate)GetValue(TooltipTemplateProperty);
            set => SetValue(TooltipTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the tool tip default font family.
        /// </summary>
        /// <value>
        /// The tool tip font family.
        /// </value>
        public A.Media.FontFamily TooltipFontFamily
        {
            get => (A.Media.FontFamily)GetValue(TooltipFontFamilyProperty);
            set => SetValue(TooltipFontFamilyProperty, value);
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
        /// Gets or sets the default tool tip font style.
        /// </summary>
        /// <value>
        /// The tool tip font style.
        /// </value>
        public A.Media.FontStyle TooltipFontStyle
        {
            get => (A.Media.FontStyle)GetValue(TooltipFontStyleProperty);
            set => SetValue(TooltipFontStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the default tool tip text brush.
        /// </summary>
        /// <value>
        /// The tool tip text brush.
        /// </value>
        public SolidColorBrush TooltipTextBrush
        {
            get => (SolidColorBrush)GetValue(TooltipTextBrushProperty);
            set => SetValue(TooltipTextBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the default tool tip background.
        /// </summary>
        /// <value>
        /// The tool tip background.
        /// </value>
        public IBrush TooltipBackground
        {
            get => (IBrush)GetValue(TooltipBackgroundProperty);
            set => SetValue(TooltipBackgroundProperty, value);
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip" />
        public IChartTooltip<SkiaSharpDrawingContext>? Tooltip => tooltip;

        /// <inheritdoc cref="IChartView.LegendPosition" />
        public LegendPosition LegendPosition
        {
            get => (LegendPosition)GetValue(LegendPositionProperty);
            set => SetValue(LegendPositionProperty, value);
        }

        /// <inheritdoc cref="IChartView.LegendOrientation" />
        public LegendOrientation LegendOrientation
        {
            get => (LegendOrientation)GetValue(LegendOrientationProperty);
            set => SetValue(LegendOrientationProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend template.
        /// </summary>
        /// <value>
        /// The legend template.
        /// </value>
        public DataTemplate LegendTemplate
        {
            get => (DataTemplate)GetValue(LegendTemplateProperty);
            set => SetValue(LegendTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the default legend font family.
        /// </summary>
        /// <value>
        /// The legend font family.
        /// </value>
        public A.Media.FontFamily LegendFontFamily
        {
            get => (A.Media.FontFamily)GetValue(LegendFontFamilyProperty);
            set => SetValue(LegendFontFamilyProperty, value);
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
        /// Gets or sets the default legend font style.
        /// </summary>
        /// <value>
        /// The legend font style.
        /// </value>
        public A.Media.FontStyle LegendFontStyle
        {
            get => (A.Media.FontStyle)GetValue(LegendFontStyleProperty);
            set => SetValue(LegendFontStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the default legend text brush.
        /// </summary>
        /// <value>
        /// The legend text brush.
        /// </value>
        public SolidColorBrush LegendTextBrush
        {
            get => (SolidColorBrush)GetValue(LegendTextBrushProperty);
            set => SetValue(LegendTextBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the default legend background.
        /// </summary>
        /// <value>
        /// The legend background.
        /// </value>
        public IBrush LegendBackground
        {
            get => (IBrush)GetValue(LegendBackgroundProperty);
            set => SetValue(LegendBackgroundProperty, value);
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
            TooltipBackground = new SolidColorBrush(new A.Media.Color(background.A, background.R, background.G, background.B));
            TooltipTextBrush = new SolidColorBrush(new A.Media.Color(textColor.A, textColor.R, textColor.G, textColor.B));
        }

        void IChartView.InvokeOnUIThread(Action action)
        {
            _ = Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Normal);
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
        protected void InitializeCore()
        {
            var canvas = this.FindControl<MotionCanvas>("canvas");
            core = new PieChart<SkiaSharpDrawingContext>(
                this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore, true);

            core.Measuring += OnCoreMeasuring;
            core.UpdateStarted += OnCoreUpdateStarted;
            core.UpdateFinished += OnCoreUpdateFinished;

            legend = this.FindControl<DefaultLegend>("legend");
            tooltip = this.FindControl<DefaultTooltip>("tooltip");
            _ = Dispatcher.UIThread.InvokeAsync(() => core.Update(), DispatcherPriority.Background);
        }

        /// <inheritdoc cref="OnPropertyChanged{T}(AvaloniaPropertyChangedEventArgs{T})" />
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (core is null) return;

            if (change.Property.Name == nameof(SyncContext))
            {
                CoreCanvas.Sync = change.NewValue;
            }

            if (change.Property.Name == nameof(Series))
            {
                _seriesObserver.Dispose((IEnumerable<ISeries>)change.OldValue.Value);
                _seriesObserver.Initialize((IEnumerable<ISeries>)change.NewValue.Value);
                return;
            }

            if (change.Property.Name == nameof(Background))
            {
                var canvas = this.FindControl<MotionCanvas>("canvas");
                var color = Background is not ISolidColorBrush b
                    ? new System.Drawing.Color()
                    : System.Drawing.Color.FromArgb(b.Color.A, b.Color.R, b.Color.G, b.Color.B);
                canvas.BackColor = new SkiaSharp.SKColor(color.R, color.G, color.B, color.A);
            }

            _ = Dispatcher.UIThread.InvokeAsync(() => core.Update(), DispatcherPriority.Background);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void CartesianChart_PointerMoved(object? sender, PointerEventArgs e)
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

        private void CartesianChart_PointerLeave(object? sender, PointerEventArgs e)
        {
            _ = Dispatcher.UIThread.InvokeAsync(HideTooltip, DispatcherPriority.Background);
            core?.InvokePointerLeft();
        }
    }
}
