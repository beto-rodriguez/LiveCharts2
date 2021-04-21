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
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;

namespace LiveChartsCore.SkiaSharpView.WinForms
{

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}" />
    public class CartesianChart : Chart, ICartesianChartView<SkiaSharpDrawingContext>
    {
        private readonly CollectionDeepObserver<ISeries> seriesObserver;
        private readonly CollectionDeepObserver<IAxis> xObserver;
        private readonly CollectionDeepObserver<IAxis> yObserver;
        private readonly ActionThrottler panningThrottler;
        private Point? previous;
        private Point? current;
        private bool isPanning = false;
        private IEnumerable<ISeries> series = new List<ISeries>();
        private IEnumerable<IAxis> xAxes = new List<Axis> { new Axis() };
        private IEnumerable<IAxis> yAxes = new List<Axis> { new Axis() };

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianChart"/> class.
        /// </summary>
        /// <param name="tooltip">The default tool tip control.</param>
        /// <param name="legend">The default legend control.</param>
        public CartesianChart(IChartTooltip<SkiaSharpDrawingContext>? tooltip = null, IChartLegend<SkiaSharpDrawingContext>? legend = null)
            : base(tooltip, legend)
        {
            seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            xObserver = new CollectionDeepObserver<IAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            yObserver = new CollectionDeepObserver<IAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

            XAxes = new List<IAxis>() { new Axis() };
            YAxes = new List<IAxis>() { new Axis() };
            Series = new ObservableCollection<ISeries>();

            var c = Controls[0].Controls[0];

            c.MouseWheel += OnMouseWheel;
            c.MouseDown += OnMouseDown;
            c.MouseMove += OnMoseMove;
            c.MouseUp += OnMouseUp;

            panningThrottler = new ActionThrottler(DoPan, TimeSpan.FromMilliseconds(30));
        }

        CartesianChart<SkiaSharpDrawingContext> ICartesianChartView<SkiaSharpDrawingContext>.Core => core == null ? throw new Exception("core not found") : (CartesianChart<SkiaSharpDrawingContext>)core;

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Series" />

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<ISeries> Series
        {
            get => series;
            set
            {
                seriesObserver.Dispose(series);
                seriesObserver.Initialize(value);
                series = value;
                core?.Update();
            }
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.XAxes" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IAxis> XAxes
        {
            get => xAxes;
            set
            {
                xObserver.Dispose(xAxes);
                xObserver.Initialize(value);
                xAxes = value;
                core?.Update();
            }
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.YAxes" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IAxis> YAxes
        {
            get => yAxes;
            set
            {
                yObserver.Dispose(yAxes);
                yObserver.Initialize(value);
                yAxes = value;
                core?.Update();
            }
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ZoomMode" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ZoomAndPanMode ZoomMode { get; set; } = LiveCharts.CurrentSettings.DefaultZoomMode;

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ZoomingSpeed" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double ZoomingSpeed { get; set; } = LiveCharts.CurrentSettings.DefaultZoomSpeed;

        /// <summary>
        /// Initializes the core.
        /// </summary>
        protected override void InitializeCore()
        {
            core = new CartesianChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, motionCanvas.CanvasCore);
            //legend = Template.FindName("legend", this) as IChartLegend<SkiaSharpDrawingContext>;
            core.Update();
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ScaleUIPoint(PointF, int, int)" />
        public PointF ScaleUIPoint(PointF point, int xAxisIndex = 0, int yAxisIndex = 0)
        {
            if (core == null) throw new Exception("core not found");
            var cartesianCore = (CartesianChart<SkiaSharpDrawingContext>)core;
            return cartesianCore.ScaleUIPoint(point, xAxisIndex, yAxisIndex);
        }

        private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (core == null) return;
            core.Update();
        }

        private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (core == null) return;
            core.Update();
        }

        private void OnMouseWheel(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (core == null) throw new Exception("core not found");
            var c = (CartesianChart<SkiaSharpDrawingContext>)core;
            var p = e.Location;
            c.Zoom(new PointF(p.X, p.Y), e.Delta > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
            Capture = true;
        }

        private void OnMouseDown(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            isPanning = true;
            previous = e.Location;
        }

        private void OnMoseMove(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!isPanning || previous == null) return;

            current = e.Location;
            panningThrottler.Call();
        }

        private void DoPan()
        {
            if (previous == null || current == null || core == null) return;

            var c = (CartesianChart<SkiaSharpDrawingContext>)core;

            c.Pan(
                new PointF(
                current.Value.X - previous.Value.X,
                current.Value.Y - previous.Value.Y));

            previous = new Point(current.Value.X, current.Value.Y);
        }

        private void OnMouseUp(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!isPanning) return;
            isPanning = false;
            previous = null;
        }
    }
}
