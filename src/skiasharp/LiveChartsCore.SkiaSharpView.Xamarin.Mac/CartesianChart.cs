using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using AppKit;
using CoreGraphics;
using Foundation;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace LiveChartsCore.SkiaSharpView.Xamarin.Mac
{
    [Register(nameof(CartesianChart))]
    public class CartesianChart : NSView, ICartesianChartView<SkiaSharpDrawingContext>
    {
        private static readonly ChartUpdateParams s_updateParams = new() { IsAutomaticUpdate = false };
        private CollectionDeepObserver<ISeries> _seriesObserver = null!;
        private CollectionDeepObserver<Section<SkiaSharpDrawingContext>> _sectionsObserver = null!;
        private CollectionDeepObserver<IAxis> _xObserver = null!;
        private CollectionDeepObserver<IAxis> _yObserver = null!;

        private IEnumerable<Section<SkiaSharpDrawingContext>> _sections = new List<Section<SkiaSharpDrawingContext>>();
        private IEnumerable<ISeries> _series = new ObservableCollection<ISeries>();
        private IEnumerable<IAxis> _xAxis = new List<IAxis>() { new Axis() };
        private IEnumerable<IAxis> _yAxis = new List<IAxis>() { new Axis() };

        private LegendPosition _legendPosition = LiveCharts.CurrentSettings.DefaultLegendPosition;
        private LegendOrientation _legendOrientation = LiveCharts.CurrentSettings.DefaultLegendOrientation;
        private TooltipPosition _tooltipPosition = LiveCharts.CurrentSettings.DefaultTooltipPosition;
        private TooltipFindingStrategy _tooltipFindingStrategy =
            LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy;
        private Margin? _drawMargin;
        private DrawMarginFrame<SkiaSharpDrawingContext>? _drawMarginFrame;
        private Color _backColor;

        private MotionCanvas? _motionCanvas;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        // created in code
        public CartesianChart()
        {
            Initialize();
        }

        // created in code
        public CartesianChart(CGRect frame)
            : base(frame)
        {
            Initialize();
        }

        // created via designer
        public CartesianChart(IntPtr p)
            : base(p)
        {
            Initialize();
        }

        private void Initialize()
        {
            AutoUpdateEnabled = false;
            _seriesObserver =
                new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            _sectionsObserver =
                new CollectionDeepObserver<Section<SkiaSharpDrawingContext>>(OnDeepCollectionChanged,
                    OnDeepCollectionPropertyChanged, true);
            _xObserver =
                new CollectionDeepObserver<IAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            _yObserver =
                new CollectionDeepObserver<IAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public event ChartEventHandler<SkiaSharpDrawingContext>? Measuring;

        public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateStarted;

        public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateFinished;

        public TimeSpan UpdaterThrottler
        {
            get => Core?.UpdaterThrottler ?? throw new Exception("core not set yet.");
            set
            {
                if (Core is null) throw new Exception("core not set yet.");
                Core.UpdaterThrottler = value;
            }
        }

        public Margin? DrawMargin
        {
            get => _drawMargin;
            set => SetAndUpdateIfChanged(ref _drawMargin, value);
        }

        public DrawMarginFrame<SkiaSharpDrawingContext>? DrawMarginFrame
        {
            get => _drawMarginFrame;
            set => SetAndUpdateIfChanged(ref _drawMarginFrame, value);
        }

        public IChart CoreChart => Core ?? throw new InvalidOperationException("Core not set yet.");

        Color IChartView.BackColor
        {
            get => _backColor;
            set => SetAndUpdateIfChanged(ref _backColor, value);
        }

        [Export(nameof(BackColor)), Browsable(true)]
        public NSColor BackColor
        {
            get => _backColor.ToNSColor();
            set => SetAndUpdateIfChanged(ref _backColor, value.FromNSColor());
        }

        [Export(nameof(ZoomMode)), Browsable(true)]
        public ZoomAndPanMode ZoomMode { get; set; } = LiveCharts.CurrentSettings.DefaultZoomMode;

        [Export(nameof(ZoomingSpeed)), Browsable(true)]
        public double ZoomingSpeed { get; set; } = LiveCharts.CurrentSettings.DefaultZoomSpeed;

        CartesianChart<SkiaSharpDrawingContext> ICartesianChartView<SkiaSharpDrawingContext>.Core =>
            (CartesianChart<SkiaSharpDrawingContext>?)Core ??
            throw new InvalidOperationException("Core is not initialized.");

        [Export(nameof(AutoUpdateEnabled)), Browsable(true)]
        public bool AutoUpdateEnabled { get; set; }

        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => _motionCanvas?.CanvasCore ??
                                                                   throw new InvalidOperationException(
                                                                       "Chart is not initialized");

        public IChartLegend<SkiaSharpDrawingContext>? Legend { get; set; }

        public IChartTooltip<SkiaSharpDrawingContext>? Tooltip { get; set; }

        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; }

        public TimeSpan AnimationsSpeed { get; set; } = LiveCharts.CurrentSettings.DefaultAnimationsSpeed;

        public Func<float, float>? EasingFunction { get; set; } = LiveCharts.CurrentSettings.DefaultEasingFunction;

        [Export(nameof(LegendPosition)), Browsable(true)]
        public LegendPosition LegendPosition
        {
            get => _legendPosition;
            set => SetAndUpdateIfChanged(ref _legendPosition, value);
        }

        [Export(nameof(LegendOrientation)), Browsable(true)]
        public LegendOrientation LegendOrientation
        {
            get => _legendOrientation;
            set => SetAndUpdateIfChanged(ref _legendOrientation, value);
        }

        [Export(nameof(TooltipPosition)), Browsable(true)]
        public TooltipPosition TooltipPosition
        {
            get => _tooltipPosition;
            set => SetAndUpdateIfChanged(ref _tooltipPosition, value);
        }

        public TooltipFindingStrategy TooltipFindingStrategy
        {
            get => _tooltipFindingStrategy;
            set => SetAndUpdateIfChanged(ref _tooltipFindingStrategy, value);
        }

        public IEnumerable<IAxis> XAxes
        {
            get => _xAxis;
            set
            {
                _xObserver.Dispose(_xAxis);
                _xAxis = value;
                _xObserver.Initialize(value);
                if (Core is null)
                {
                    return;
                }

                BeginUpdateOnMainThread();
            }
        }

        public IEnumerable<IAxis> YAxes
        {
            get => _yAxis;
            set
            {
                _yObserver.Dispose(_yAxis);
                _yAxis = value;
                _yObserver.Initialize(value);
                if (Core is null)
                {
                    return;
                }

                BeginUpdateOnMainThread();
            }
        }

        public IEnumerable<Section<SkiaSharpDrawingContext>> Sections
        {
            get => _sections;
            set
            {
                _sectionsObserver.Dispose(_sections);
                _sections = value;
                _sectionsObserver.Initialize(value);
                if (Core is null) return;
                BeginUpdateOnMainThread();
            }
        }

        public IEnumerable<ISeries> Series
        {
            get => _series;
            set
            {
                _seriesObserver.Dispose(_series);
                _series = value;
                _seriesObserver.Initialize(value);
                if (Core is null)
                {
                    return;
                }

                BeginUpdateOnMainThread();
            }
        }

        public SizeF ControlSize => new()
        {
            Height = (float)Bounds.Height,
            Width = (float)Bounds.Width
        };

        protected Chart<SkiaSharpDrawingContext>? Core { get; set; }

        public override void ViewDidMoveToWindow()
        {
            base.ViewDidMoveToWindow();
            if (Window is not null)
            {
                Window.DidResize -= OnWindowResized;
                Window.DidResize += OnWindowResized;
            }
        }

        public override void DidAddSubview(NSView? subview)
        {
            base.DidAddSubview(subview);

            if (subview is MotionCanvas c)
            {
                _motionCanvas = c;

                if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

                var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
                var initializer = stylesBuilder.GetVisualsInitializer();
                if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
                    throw new Exception("Default colors are not valid");
                initializer.ApplyStyleToChart(this);

                InitializeCore();
            }
        }

        public double[] ScaleUIPoint(PointF point, int xAxisIndex = 0, int yAxisIndex = 0)
        {
            var cartesianCore = (CartesianChart<SkiaSharpDrawingContext>?)Core;
            return cartesianCore?.ScaleUIPoint(point, xAxisIndex, yAxisIndex) ?? new[] { 0d, 0d };
        }

        public void ShowTooltip(IEnumerable<TooltipPoint> points)
        {
            throw new NotImplementedException();
        }

        public void HideTooltip()
        {
            throw new NotImplementedException();
        }

        public void SetTooltipStyle(Color background, Color textColor)
        {
            throw new NotImplementedException();
        }

        protected void InitializeCore()
        {
            if (_motionCanvas is null) return;
            Core = new CartesianChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder,
                _motionCanvas.CanvasCore);

            Core.Measuring += OnCoreMeasuring;
            Core.UpdateStarted += OnCoreUpdateStarted;
            Core.UpdateFinished += OnCoreUpdateFinished;

            BeginUpdateOnMainThread();
        }

        private void OnDeepCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            BeginUpdateOnMainThread();
        }

        private void OnDeepCollectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            BeginUpdateOnMainThread();
        }

        private void OnWindowResized(object sender, EventArgs e)
        {
            Core?.Update(s_updateParams);
        }

        private void SetAndUpdateIfChanged<T>(ref T backingField, T value)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return;
            }

            backingField = value;
            Core?.Update();
        }

        private void BeginUpdateOnMainThread()
        {
            BeginInvokeOnMainThread(() => Core?.Update());
        }

        private void OnCoreMeasuring(IChartView<SkiaSharpDrawingContext> chart)
        {
            Measuring?.Invoke(this);
        }

        private void OnCoreUpdateStarted(IChartView<SkiaSharpDrawingContext> chart)
        {
            UpdateStarted?.Invoke(this);
        }

        private void OnCoreUpdateFinished(IChartView<SkiaSharpDrawingContext> chart)
        {
            UpdateFinished?.Invoke(this);
        }
    }
}
