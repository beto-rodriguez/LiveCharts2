﻿// The MIT License(MIT)
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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Eto.Forms;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace LiveChartsCore.SkiaSharpView.Eto;

/// <inheritdoc cref="IPieChartView{TDrawingContext}" />
public class PieChart : Chart, IPieChartView<SkiaSharpDrawingContext>
{
    private CollectionDeepObserver<ISeries> _seriesObserver;
    private IEnumerable<ISeries> _series = new List<ISeries>();
    private CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>> _visualsObserver;
    private IEnumerable<ChartElement<SkiaSharpDrawingContext>> _visuals = new List<ChartElement<SkiaSharpDrawingContext>>();
    private bool _isClockwise = true;
    private double _initialRotation;
    private double _maxAngle = 360;
    private double? _total;

    /// <summary>
    /// Initializes a new instance of the <see cref="PieChart"/> class.
    /// </summary>
    public PieChart() : this(null, null) { }

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
               if (sender is IStopNPC stop && !stop.IsNotifyingChanges) return;
               OnPropertyChanged();
           },
           (object? sender, PropertyChangedEventArgs e) =>
           {
               if (sender is IStopNPC stop && !stop.IsNotifyingChanges) return;
               OnPropertyChanged();
           },
           true);
        _visualsObserver = new CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>>(
           (object? sender, NotifyCollectionChangedEventArgs e) =>
           {
               if (sender is IStopNPC stop && !stop.IsNotifyingChanges) return;
               OnPropertyChanged();
           },
           (object? sender, PropertyChangedEventArgs e) =>
           {
               if (sender is IStopNPC stop && !stop.IsNotifyingChanges) return;
               OnPropertyChanged();
           },
           true);

        motionCanvas.MouseDown += OnMouseDown;
    }

    PieChart<SkiaSharpDrawingContext> IPieChartView<SkiaSharpDrawingContext>.Core =>
        core is null ? throw new Exception("core not found") : (PieChart<SkiaSharpDrawingContext>)core;

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.Series" />
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

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.VisualElements" />
    public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements
    {
        get => _visuals;
        set
        {
            _visualsObserver?.Dispose(_visuals);
            _visualsObserver?.Initialize(value);
            _visuals = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.IsClockwise" />
    public bool IsClockwise { get => _isClockwise; set { _isClockwise = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.InitialRotation" />
    public double InitialRotation { get => _initialRotation; set { _initialRotation = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.MaxAngle" />
    public double MaxAngle { get => _maxAngle; set { _maxAngle = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.Total" />
    public double? Total { get => _total; set { _total = value; OnPropertyChanged(); } }

    /// <summary>
    /// Initializes the core.
    /// </summary>
    protected override void InitializeCore()
    {
        core = new PieChart<SkiaSharpDrawingContext>(
            this, LiveChartsSkiaSharp.DefaultPlatformBuilder, motionCanvas.CanvasCore, true);
        core.Update();
    }

    /// <inheritdoc cref="Chart.OnUnloading"/>
    protected override void OnUnloading()
    {
        Series = Array.Empty<ISeries>();
        _seriesObserver = null!;
        VisualElements = Array.Empty<ChartElement<SkiaSharpDrawingContext>>();
        _visualsObserver = null!;
    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        core?.InvokePointerDown(new LvcPoint(e.Location.X, e.Location.Y), false);
    }
}
