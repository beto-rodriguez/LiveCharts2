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

using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;

namespace LiveChartsCore.SkiaSharpView.WinForms
{
    /// <inheritdoc cref="IPolarChartView{TDrawingContext}" />
    public class PolarChart : Chart, IPolarChartView<SkiaSharpDrawingContext>
    {
        private readonly CollectionDeepObserver<ISeries> _seriesObserver;
        private readonly CollectionDeepObserver<IPolarAxis> _angleObserver;
        private readonly CollectionDeepObserver<IPolarAxis> _radiusObserver;
        private IEnumerable<ISeries> _series = new List<ISeries>();
        private IEnumerable<IPolarAxis> _angleAxes = new List<PolarAxis>();
        private IEnumerable<IPolarAxis> _radiusAxes = new List<PolarAxis>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PolarChart"/> class.
        /// </summary>
        public PolarChart() : this(null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolarChart"/> class.
        /// </summary>
        /// <param name="tooltip">The default tool tip control.</param>
        /// <param name="legend">The default legend control.</param>
        public PolarChart(IChartTooltip<SkiaSharpDrawingContext>? tooltip = null, IChartLegend<SkiaSharpDrawingContext>? legend = null)
            : base(tooltip, legend)
        {
            _seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            _angleObserver = new CollectionDeepObserver<IPolarAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            _radiusObserver = new CollectionDeepObserver<IPolarAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

            AngleAxes = new List<IPolarAxis>() { LiveCharts.CurrentSettings.PolarAxisProvider() };
            RadiusAxes = new List<IPolarAxis>() { LiveCharts.CurrentSettings.PolarAxisProvider() };
            Series = new ObservableCollection<ISeries>();

            var c = Controls[0].Controls[0];

            c.MouseWheel += OnMouseWheel;
            c.MouseDown += OnMouseDown;
            c.MouseUp += OnMouseUp;
        }

        PolarChart<SkiaSharpDrawingContext> IPolarChartView<SkiaSharpDrawingContext>.Core => core is null ? throw new Exception("core not found") : (PolarChart<SkiaSharpDrawingContext>)core;

        /// <inheritdoc cref="IPolarChartView{TDrawingContext}.Series" />

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<ISeries> Series
        {
            get => _series;
            set
            {
                _seriesObserver.Dispose(_series);
                _seriesObserver.Initialize(value);
                _series = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc cref="IPolarChartView{TDrawingContext}.AngleAxes" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IPolarAxis> AngleAxes
        {
            get => _angleAxes;
            set
            {
                _angleObserver.Dispose(_angleAxes);
                _angleObserver.Initialize(value);
                _angleAxes = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc cref="IPolarChartView{TDrawingContext}.RadiusAxes" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<IPolarAxis> RadiusAxes
        {
            get => _radiusAxes;
            set
            {
                _radiusObserver.Dispose(_radiusAxes);
                _radiusObserver.Initialize(value);
                _radiusAxes = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Initializes the core.
        /// </summary>
        protected override void InitializeCore()
        {
            core = new PolarChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, motionCanvas.CanvasCore);
            if (((IChartView)this).DesignerMode) return;
            core.Update();
        }

        /// <inheritdoc cref="IPolarChartView{TDrawingContext}.ScaleUIPoint(LvcPoint, int, int)" />
        public double[] ScaleUIPoint(LvcPoint point, int xAxisIndex = 0, int yAxisIndex = 0)
        {
            return new double[0];
            //if (core is null) throw new Exception("core not found");
            //var cartesianCore = (PolarChart<SkiaSharpDrawingContext>)core;
            //return cartesianCore.ScaleUIPoint(point, xAxisIndex, yAxisIndex);
        }

        private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is IStopNPC stop && !stop.IsNotifyingChanges) return;
            OnPropertyChanged();
        }

        private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is IStopNPC stop && !stop.IsNotifyingChanges) return;
            OnPropertyChanged();
        }

        private void OnMouseWheel(object? sender, MouseEventArgs e)
        {
            //if (core is null) throw new Exception("core not found");
            //var c = (PolarChart<SkiaSharpDrawingContext>)core;
            //var p = e.Location;
            //c.Zoom(new PointF(p.X, p.Y), e.Delta > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
            //Capture = true;
        }

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            core?.InvokePointerDown(new LvcPoint(e.Location.X, e.Location.Y));
        }

        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            core?.InvokePointerUp(new LvcPoint(e.Location.X, e.Location.Y));
        }
    }
}
