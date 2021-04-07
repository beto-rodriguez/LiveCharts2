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
using am = Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Collections.ObjectModel;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;

namespace LiveChartsCore.SkiaSharp.Avalonia
{
    public class PieChart : UserControl, IPieChartView<SkiaSharpDrawingContext>, IAvaloniaChart
    {
        protected Chart<SkiaSharpDrawingContext> core;
        //protected MotionCanvas motionCanvas;
        protected IChartLegend<SkiaSharpDrawingContext> legend;
        protected IChartTooltip<SkiaSharpDrawingContext> tooltip;
        private readonly ActionThrottler mouseMoveThrottler;
        private PointF mousePosition = new PointF();
        private CollectionDeepObserver<ISeries> seriesObserver;

        public PieChart()
        {
            InitializeComponent();

            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetStylesBuilder<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetInitializer();
            if (stylesBuilder.CurrentColors == null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ConstructChart(this);

            InitializeCore();

            mouseMoveThrottler = new ActionThrottler(MouseMoveThrottlerUnlocked, TimeSpan.FromMilliseconds(10));

            seriesObserver = new CollectionDeepObserver<ISeries>(
               (object? sender, NotifyCollectionChangedEventArgs e) =>
               {
                   if (core == null) return;
                   Dispatcher.UIThread.InvokeAsync(() => core.Update(), DispatcherPriority.Background);
               },
               (object? sender, PropertyChangedEventArgs e) =>
               {
                   if (core == null) return;
                   Dispatcher.UIThread.InvokeAsync(() => core.Update(), DispatcherPriority.Background);
               });

            Series = new ObservableCollection<ISeries>();
        }

        PieChart<SkiaSharpDrawingContext> IPieChartView<SkiaSharpDrawingContext>.Core => (PieChart<SkiaSharpDrawingContext>)core;

        public static readonly AvaloniaProperty<IEnumerable<ISeries>> SeriesProperty =
            AvaloniaProperty.Register<PieChart, IEnumerable<ISeries>>(nameof(Series), new List<ISeries>(), inherits: true);

        public static readonly AvaloniaProperty<TimeSpan> AnimationsSpeedProperty =
            AvaloniaProperty.Register<PieChart, TimeSpan>(
                nameof(AnimationsSpeed), LiveCharts.CurrentSettings.DefaultAnimationsSpeed, inherits: true);

        public static readonly AvaloniaProperty<Func<float, float>> EasingFunctionProperty =
            AvaloniaProperty.Register<PieChart, Func<float, float>>(
                nameof(AnimationsSpeed), LiveCharts.CurrentSettings.DefaultEasingFunction, inherits: true);

        public static readonly AvaloniaProperty<LegendPosition> LegendPositionProperty =
            AvaloniaProperty.Register<PieChart, LegendPosition>(
                nameof(LegendPosition), LiveCharts.CurrentSettings.DefaultLegendPosition, inherits: true);

        public static readonly AvaloniaProperty<LegendOrientation> LegendOrientationProperty =
            AvaloniaProperty.Register<PieChart, LegendOrientation>(
                nameof(LegendOrientation), LiveCharts.CurrentSettings.DefaultLegendOrientation, inherits: true);

        public static readonly AvaloniaProperty<TooltipPosition> TooltipPositionProperty =
            AvaloniaProperty.Register<PieChart, TooltipPosition>(
                nameof(TooltipPosition), LiveCharts.CurrentSettings.DefaultTooltipPosition, inherits: true);

        public static readonly AvaloniaProperty<TooltipFindingStrategy> TooltipFindingStrategyProperty =
            AvaloniaProperty.Register<PieChart, TooltipFindingStrategy>(
                nameof(LegendPosition), LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy, inherits: true);

        public static readonly AvaloniaProperty<DataTemplate> TooltipTemplateProperty =
           AvaloniaProperty.Register<CartesianChart, DataTemplate>(nameof(TooltipTemplate), null, inherits: true);

        public IEnumerable<ISeries> Series
        {
            get { return (IEnumerable<ISeries>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        public TimeSpan AnimationsSpeed
        {
            get { return (TimeSpan)GetValue(AnimationsSpeedProperty); }
            set { SetValue(AnimationsSpeedProperty, value); }
        }

        public Func<float, float> EasingFunction
        {
            get { return (Func<float, float>)GetValue(EasingFunctionProperty); }
            set { SetValue(AnimationsSpeedProperty, value); }
        }

        public LegendPosition LegendPosition
        {
            get { return (LegendPosition)GetValue(LegendPositionProperty); }
            set { SetValue(LegendPositionProperty, value); }
        }

        public LegendOrientation LegendOrientation
        {
            get { return (LegendOrientation)GetValue(LegendOrientationProperty); }
            set { SetValue(LegendOrientationProperty, value); }
        }

        public TooltipPosition TooltipPosition
        {
            get { return (TooltipPosition)GetValue(TooltipPositionProperty); }
            set { SetValue(TooltipPositionProperty, value); }
        }

        public TooltipFindingStrategy TooltipFindingStrategy
        {
            get { return (TooltipFindingStrategy)GetValue(TooltipFindingStrategyProperty); }
            set { SetValue(TooltipFindingStrategyProperty, value); }
        }

        public DataTemplate TooltipTemplate
        {
            get { return (DataTemplate)GetValue(TooltipTemplateProperty); }
            set { SetValue(TooltipTemplateProperty, value); }
        }

        SizeF IChartView.ControlSize
        {
            get
            {
                //Measure(new Avalonia.Size(double.PositiveInfinity, double.PositiveInfinity));
                return new SizeF
                {
                    Width = (float)Bounds.Width,
                    Height = (float)Bounds.Height
                };
            }
        }

        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => core.Canvas;

        public IChartLegend<SkiaSharpDrawingContext> Legend => null;

        public Margin DrawMargin { get; set; }

        public IChartTooltip<SkiaSharpDrawingContext> Tooltip => null;

        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; }
        public am.FontFamily TooltipFontFamily { get; set; }
        public double TooltipFontSize { get; set; }
        public FontWeight TooltipFontWeight { get; set; }
        public am.FontStyle TooltipFontStyle { get; set; }
        public SolidColorBrush TooltipTextBrush { get; set; }

        protected void InitializeCore()
        {
            var canvas = this.FindControl<MotionCanvas>("canvas");
            core = new PieChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore);
            //legend = Template.FindName("legend", this) as IChartLegend<SkiaSharpDrawingContext>;
            //tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaSharpDrawingContext>;
            Dispatcher.UIThread.InvokeAsync(() => core.Update(), DispatcherPriority.Background);
        }

        private void MouseMoveThrottlerUnlocked()
        {
            if (TooltipPosition == TooltipPosition.Hidden) return;
            tooltip.Show(core.FindPointsNearTo(mousePosition), core);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property.Name == nameof(Series))
            {
                seriesObserver.Dispose((IEnumerable<ISeries>)change.OldValue.Value);
                seriesObserver.Initialize((IEnumerable<ISeries>)change.NewValue.Value);
                return;
            }

            // is this how the size event is handled?
            // https://github.com/AvaloniaUI/Avalonia/issues/3237
            if (change.Property.Name != nameof(Bounds)) return;
            Dispatcher.UIThread.InvokeAsync(() => core.Update(), DispatcherPriority.Background);
        }
    }
}
