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
        private readonly CollectionDeepObserver<Section<SkiaSharpDrawingContext>> _sectionsObserverer;
        private IEnumerable<ISeries> _series = new List<ISeries>();
        private IEnumerable<IAxis> _xAxes = new List<Axis> { new Axis() };
        private IEnumerable<IAxis> _yAxes = new List<Axis> { new Axis() };
        private IEnumerable<Section<SkiaSharpDrawingContext>> _sections = new List<Section<SkiaSharpDrawingContext>> ();
        private DrawMarginFrame<SkiaSharpDrawingContext>? _drawMarginFrame;
        private TooltipFindingStrategy _tooltipFindingStrategy = LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianChart"/> class.
        /// </summary>
        public CartesianChart() : this(null, null) { }

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
            _sectionsObserverer = new CollectionDeepObserver<Section<SkiaSharpDrawingContext>>(
                OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

            XAxes = new List<IAxis>() { LiveCharts.CurrentSettings.AxisProvider() };
            YAxes = new List<IAxis>() { LiveCharts.CurrentSettings.AxisProvider() };
            Series = new ObservableCollection<ISeries>();

            var c = Controls[0].Controls[0];

            c.MouseWheel += OnMouseWheel;
            c.MouseDown += OnMouseDown;
            c.MouseUp += OnMouseUp;
        }

        CartesianChart<SkiaSharpDrawingContext> ICartesianChartView<SkiaSharpDrawingContext>.Core => core is null ? throw new Exception("core not found") : (CartesianChart<SkiaSharpDrawingContext>)core;

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

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Sections" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<Section<SkiaSharpDrawingContext>> Sections
        {
            get => _sections;
            set
            {
                _sectionsObserverer.Dispose(_sections);
                _sectionsObserverer.Initialize(value);
                _sections = value;
                core?.Update();
            }
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.DrawMarginFrame" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DrawMarginFrame<SkiaSharpDrawingContext>? DrawMarginFrame
        {
            get => _drawMarginFrame;
            set
            {
                _drawMarginFrame = value;
                core?.Update();
            }
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ZoomMode" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ZoomAndPanMode ZoomMode { get; set; } = LiveCharts.CurrentSettings.DefaultZoomMode;

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ZoomingSpeed" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double ZoomingSpeed { get; set; } = LiveCharts.CurrentSettings.DefaultZoomSpeed;

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.TooltipFindingStrategy" />
        public TooltipFindingStrategy TooltipFindingStrategy { get => _tooltipFindingStrategy; set { _tooltipFindingStrategy = value; OnPropertyChanged(); } }

        /// <summary>
        /// Initializes the core.
        /// </summary>
        protected override void InitializeCore()
        {
            core = new CartesianChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, motionCanvas.CanvasCore);
            core.Update();
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ScaleUIPoint(PointF, int, int)" />
        public PointF ScaleUIPoint(PointF point, int xAxisIndex = 0, int yAxisIndex = 0)
        {
            if (core is null) throw new Exception("core not found");
            var cartesianCore = (CartesianChart<SkiaSharpDrawingContext>)core;
            return cartesianCore.ScaleUIPoint(point, xAxisIndex, yAxisIndex);
        }

        private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (core is null) return;
            core.Update();
        }

        private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (core is null) return;
            core.Update();
        }

        private void OnMouseWheel(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (core is null) throw new Exception("core not found");
            var c = (CartesianChart<SkiaSharpDrawingContext>)core;
            var p = e.Location;
            c.Zoom(new PointF(p.X, p.Y), e.Delta > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
            Capture = true;
        }

        private void OnMouseDown(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            core?.InvokePointerDown(new PointF(e.Location.X, e.Location.Y));
        }

        private void OnMouseUp(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            core?.InvokePointerUp(new PointF(e.Location.X, e.Location.Y));
        }
    }
}
