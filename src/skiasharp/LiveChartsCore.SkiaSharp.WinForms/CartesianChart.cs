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
        private readonly CollectionDeepObserver<ISeries> _seriesObserver;
        private readonly CollectionDeepObserver<IAxis> _xObserver;
        private readonly CollectionDeepObserver<IAxis> _yObserver;
        private readonly ActionThrottler _panningThrottler;
        private Point? _previous;
        private Point? _current;
        private bool _isPanning = false;
        private IEnumerable<ISeries> _series = new List<ISeries>();
        private IEnumerable<IAxis> _xAxes = new List<Axis> { new Axis() };
        private IEnumerable<IAxis> _yAxes = new List<Axis> { new Axis() };

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianChart"/> class.
        /// </summary>
        /// <param name="tooltip">The default tool tip control.</param>
        /// <param name="legend">The default legend control.</param>
        public CartesianChart(IChartTooltip<SkiaSharpDrawingContext>? tooltip = null, IChartLegend<SkiaSharpDrawingContext>? legend = null)
            : base(tooltip, legend)
        {
            _seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            _xObserver = new CollectionDeepObserver<IAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            _yObserver = new CollectionDeepObserver<IAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

            XAxes = new List<IAxis>() { LiveCharts.CurrentSettings.AxisProvider() };
            YAxes = new List<IAxis>() { LiveCharts.CurrentSettings.AxisProvider() };
            Series = new ObservableCollection<ISeries>();

            var c = Controls[0].Controls[0];

            c.MouseWheel += OnMouseWheel;
            c.MouseDown += OnMouseDown;
            c.MouseMove += OnMoseMove;
            c.MouseUp += OnMouseUp;

            _panningThrottler = new ActionThrottler(DoPan, TimeSpan.FromMilliseconds(30));
        }

        CartesianChart<SkiaSharpDrawingContext> ICartesianChartView<SkiaSharpDrawingContext>.Core => core == null ? throw new Exception("core not found") : (CartesianChart<SkiaSharpDrawingContext>)core;

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Series" />

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<ISeries> Series
        {
            get => _series;
            set
            {
                _seriesObserver.Dispose(_series);
                _seriesObserver.Initialize(value);
                _series = value;
                core?.Update();
            }
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.XAxes" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IAxis> XAxes
        {
            get => _xAxes;
            set
            {
                _xObserver.Dispose(_xAxes);
                _xObserver.Initialize(value);
                _xAxes = value;
                core?.Update();
            }
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.YAxes" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IAxis> YAxes
        {
            get => _yAxes;
            set
            {
                _yObserver.Dispose(_yAxes);
                _yObserver.Initialize(value);
                _yAxes = value;
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
            _isPanning = true;
            _previous = e.Location;
        }

        private void OnMoseMove(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!_isPanning || _previous == null) return;

            _current = e.Location;
            _panningThrottler.Call();
        }

        private void DoPan()
        {
            if (_previous == null || _current == null || core == null) return;

            var c = (CartesianChart<SkiaSharpDrawingContext>)core;

            c.Pan(
                new PointF(
                _current.Value.X - _previous.Value.X,
                _current.Value.Y - _previous.Value.Y));

            _previous = new Point(_current.Value.X, _current.Value.Y);
        }

        private void OnMouseUp(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!_isPanning) return;
            _isPanning = false;
            _previous = null;
        }
    }
}
