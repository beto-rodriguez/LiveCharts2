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
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace LiveChartsCore.SkiaSharpView.WinForms
{
    /// <inheritdoc cref="IPieChartView{TDrawingContext}" />
    public class PieChart : Chart, IPieChartView<SkiaSharpDrawingContext>
    {
        private readonly CollectionDeepObserver<ISeries> _seriesObserver;
        private IEnumerable<ISeries> _series = new List<ISeries>();
        private double _initialRotation;
        private double? _total;

        /// <summary>
        /// Initializes a new instance of the <see cref="PieChart"/> class.
        /// </summary>
        /// <param name="tooltip">The default tool tip control.</param>
        /// <param name="legend">The default legend.</param>
        public PieChart(IChartTooltip<SkiaSharpDrawingContext>? tooltip = null, IChartLegend<SkiaSharpDrawingContext>? legend = null)
            : base(tooltip, legend)
        {
            _seriesObserver = new CollectionDeepObserver<ISeries>(
               (object? sender, NotifyCollectionChangedEventArgs e) =>
               {
                   if (core == null) return;
                   core.Update();
               },
               (object? sender, PropertyChangedEventArgs e) =>
               {
                   if (core == null) return;
                   core.Update();
               },
               true);
        }

        PieChart<SkiaSharpDrawingContext> IPieChartView<SkiaSharpDrawingContext>.Core =>
            core == null ? throw new Exception("core not found") : (PieChart<SkiaSharpDrawingContext>)core;

        /// <inheritdoc cref="IPieChartView{TDrawingContext}.Series" />
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

        /// <inheritdoc cref="IPieChartView{TDrawingContext}.InitialRotation" />
        public double InitialRotation { get => _initialRotation; set { _initialRotation = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IPieChartView{TDrawingContext}.Total" />
        public double? Total { get => _total; set { _total = value; OnPropertyChanged(); } }

        /// <summary>
        /// Initializes the core.
        /// </summary>
        protected override void InitializeCore()
        {
            core = new PieChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, motionCanvas.CanvasCore);
            core.Update();
        }
    }

}
