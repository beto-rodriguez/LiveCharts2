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

using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    public class CartesianChart : Chart, ICartesianChartView<SkiaSharpDrawingContext>
    {
        private readonly CollectionDeepObserver<ISeries> seriesObserver;
        private readonly CollectionDeepObserver<IAxis> xObserver;
        private readonly CollectionDeepObserver<IAxis> yObserver;
        private readonly ActionThrottler panningThrottler;
        private System.Windows.Point? previous;
        private System.Windows.Point? current;
        private bool isPanning = false;

        static CartesianChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CartesianChart), new FrameworkPropertyMetadata(typeof(CartesianChart)));
        }

        public CartesianChart()
        {
            seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            xObserver = new CollectionDeepObserver<IAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            yObserver = new CollectionDeepObserver<IAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

            XAxes = new List<IAxis>() { new Axis() };
            YAxes = new List<IAxis>() { new Axis() };
            Series = new ObservableCollection<ISeries>();

            MouseWheel += OnMouseWheel;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;

            panningThrottler = new ActionThrottler(DoPan, TimeSpan.FromMilliseconds(30));
        }

        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                nameof(Series), typeof(IEnumerable<ISeries>), typeof(CartesianChart), new PropertyMetadata(null,
                    (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                    {
                        var chart = (CartesianChart)o;
                        var seriesObserver = chart.seriesObserver;
                        seriesObserver.Dispose((IEnumerable<ISeries>)args.OldValue);
                        seriesObserver.Initialize((IEnumerable<ISeries>)args.NewValue);
                        if (chart.core == null) return;
                        Application.Current.Dispatcher.Invoke(() => chart.core.Update());
                    }));

        public static readonly DependencyProperty XAxesProperty =
            DependencyProperty.Register(
                nameof(XAxes), typeof(IEnumerable<IAxis>), typeof(CartesianChart), new PropertyMetadata(null,
                    (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                    {
                        var chart = (CartesianChart)o;
                        var observer = chart.xObserver;
                        observer.Dispose((IEnumerable<IAxis>)args.OldValue);
                        observer.Initialize((IEnumerable<IAxis>)args.NewValue);
                        if (chart.core == null) return;
                        Application.Current.Dispatcher.Invoke(() => chart.core.Update());
                    }));

        public static readonly DependencyProperty YAxesProperty =
            DependencyProperty.Register(
                nameof(YAxes), typeof(IEnumerable<IAxis>), typeof(CartesianChart), new PropertyMetadata(null,
                    (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                    {
                        var chart = (CartesianChart)o;
                        var observer = chart.yObserver;
                        observer.Dispose((IEnumerable<IAxis>)args.OldValue);
                        observer.Initialize((IEnumerable<IAxis>)args.NewValue);
                        if (chart.core == null) return;
                        Application.Current.Dispatcher.Invoke(() => chart.core.Update());
                    }));

        public static readonly DependencyProperty ZoomModeProperty =
            DependencyProperty.Register(
                nameof(ZoomMode), typeof(ZoomAndPanMode), typeof(CartesianChart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultZoomMode));

        public static readonly DependencyProperty ZoomingSpeedProperty =
            DependencyProperty.Register(
                nameof(ZoomingSpeed), typeof(double), typeof(CartesianChart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultZoomSpeed));

        CartesianChart<SkiaSharpDrawingContext> ICartesianChartView<SkiaSharpDrawingContext>.Core => (CartesianChart<SkiaSharpDrawingContext>)core;

        public IEnumerable<ISeries> Series
        {
            get { return (IEnumerable<ISeries>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        public IEnumerable<IAxis> XAxes
        {
            get { return (IEnumerable<IAxis>)GetValue(XAxesProperty); }
            set { SetValue(XAxesProperty, value); }
        }

        public IEnumerable<IAxis> YAxes
        {
            get { return (IEnumerable<IAxis>)GetValue(YAxesProperty); }
            set { SetValue(YAxesProperty, value); }
        }

        public ZoomAndPanMode ZoomMode
        {
            get { return (ZoomAndPanMode)GetValue(ZoomModeProperty); }
            set { SetValue(ZoomModeProperty, value); }
        }

        public double ZoomingSpeed
        {
            get { return (double)GetValue(ZoomingSpeedProperty); }
            set { SetValue(ZoomingSpeedProperty, value); }
        }

        protected override void InitializeCore()
        {
            core = new CartesianChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore);
            legend = Template.FindName("legend", this) as IChartLegend<SkiaSharpDrawingContext>;
            tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaSharpDrawingContext>;
            Application.Current.Dispatcher.Invoke(() => core.Update());
        }

        public PointF ScaleUIPoint(PointF point, int xAxisIndex = 0, int yAxisIndex = 0)
        {
            var cartesianCore = (CartesianChart<SkiaSharpDrawingContext>)core;
            return cartesianCore.ScaleUIPoint(point, xAxisIndex, yAxisIndex);
        }

        private void OnDeepCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (core == null) return;
            Application.Current.Dispatcher.Invoke(() => core.Update());
        }

        private void OnDeepCollectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (core == null) return;
            Application.Current.Dispatcher.Invoke(() => core.Update());
        }

        private void OnMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var c = (CartesianChart<SkiaSharpDrawingContext>)core;
            var p = e.GetPosition(this);
            c.Zoom(new PointF((float)p.X, (float)p.Y), e.Delta > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
        }

        private void OnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isPanning = true;
            previous = e.GetPosition(this);
            CaptureMouse();
            Trace.WriteLine("down");
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isPanning || previous == null) return;

            current = e.GetPosition(this);
            panningThrottler.Call();
        }

        private void DoPan()
        {
            if (previous == null || current == null) return;

            var c = (CartesianChart<SkiaSharpDrawingContext>)core;

            c.Pan(
                new PointF(
                (float)(current.Value.X - previous.Value.X),
                (float)(current.Value.Y - previous.Value.Y)));

            previous = new System.Windows.Point(current.Value.X, current.Value.Y);
        }

        private void OnMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Trace.WriteLine("up");
            if (!isPanning) return;
            isPanning = false;
            previous = null;
            ReleaseMouseCapture();
        }
    }
}

