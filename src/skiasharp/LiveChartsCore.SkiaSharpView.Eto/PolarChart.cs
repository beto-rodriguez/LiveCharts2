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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Eto.Forms;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.SkiaSharpView.Eto;

/// <inheritdoc cref="IPolarChartView{TDrawingContext}" />
public class PolarChart : Chart, IPolarChartView<SkiaSharpDrawingContext>
{
    private bool _fitToBounds = false;
    private double _totalAngle = 360;
    private double _innerRadius;
    private double _initialRotation = LiveCharts.DefaultSettings.PolarInitialRotation;
    private CollectionDeepObserver<ISeries> _seriesObserver;
    private CollectionDeepObserver<IPolarAxis> _angleObserver;
    private CollectionDeepObserver<IPolarAxis> _radiusObserver;
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

        AngleAxes = new List<IPolarAxis>()
            {
                LiveCharts.DefaultSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultPolarAxis()
            };
        RadiusAxes = new List<IPolarAxis>()
            {
                LiveCharts.DefaultSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultPolarAxis()
            };
        Series = new ObservableCollection<ISeries>();

        var c = motionCanvas;

        c.MouseWheel += OnMouseWheel;
        c.MouseDown += OnMouseDown;
        c.MouseUp += OnMouseUp;
    }

    PolarChart<SkiaSharpDrawingContext> IPolarChartView<SkiaSharpDrawingContext>.Core =>
        core is null ? throw new Exception("core not found") : (PolarChart<SkiaSharpDrawingContext>)core;

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.FitToBounds" />
    public bool FitToBounds
    {
        get => _fitToBounds;
        set
        {
            _fitToBounds = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.TotalAngle" />
    public double TotalAngle
    {
        get => _totalAngle;
        set
        {
            _totalAngle = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.InnerRadius" />
    public double InnerRadius
    {
        get => _innerRadius;
        set
        {
            _innerRadius = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.InitialRotation" />
    public double InitialRotation
    {
        get => _initialRotation;
        set
        {
            _initialRotation = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.Series" />
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
        core = new PolarChart<SkiaSharpDrawingContext>(
            this, config => config.UseDefaults(), motionCanvas.CanvasCore, true);
        core.Update();
    }

    /// <inheritdoc cref="Chart.OnUnloading"/>
    protected override void OnUnloading()
    {
        Series = Array.Empty<ISeries>();
        AngleAxes = Array.Empty<IPolarAxis>();
        RadiusAxes = Array.Empty<IPolarAxis>();
        VisualElements = Array.Empty<ChartElement<SkiaSharpDrawingContext>>();
        _seriesObserver = null!;
        _angleObserver = null!;
        _radiusObserver = null!;
    }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.ScalePixelsToData(LvcPointD, int, int)"/>
    public LvcPointD ScalePixelsToData(LvcPointD point, int angleAxisIndex = 0, int radiusAxisIndex = 0)
    {
        if (core is not PolarChart<SkiaSharpDrawingContext> cc) throw new Exception("core not found");

        var scaler = new PolarScaler(
            cc.DrawMarginLocation, cc.DrawMarginSize, cc.AngleAxes[angleAxisIndex], cc.RadiusAxes[radiusAxisIndex],
            cc.InnerRadius, cc.InitialRotation, cc.TotalAnge);

        return scaler.ToChartValues(point.X, point.Y);
    }

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.ScaleDataToPixels(LvcPointD, int, int)"/>
    public LvcPointD ScaleDataToPixels(LvcPointD point, int angleAxisIndex = 0, int radiusAxisIndex = 0)
    {
        if (core is not PolarChart<SkiaSharpDrawingContext> cc) throw new Exception("core not found");

        var scaler = new PolarScaler(
            cc.DrawMarginLocation, cc.DrawMarginSize, cc.AngleAxes[angleAxisIndex], cc.RadiusAxes[radiusAxisIndex],
            cc.InnerRadius, cc.InitialRotation, cc.TotalAnge);

        var r = scaler.ToPixels(point.X, point.Y);

        return new LvcPointD { X = (float)r.X, Y = (float)r.Y };
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetPointsAt(LvcPoint, TooltipFindingStrategy)"/>
    public override IEnumerable<ChartPoint> GetPointsAt(LvcPoint point, TooltipFindingStrategy strategy = TooltipFindingStrategy.Automatic)
    {
        if (core is not PolarChart<SkiaSharpDrawingContext> cc) throw new Exception("core not found");

        if (strategy == TooltipFindingStrategy.Automatic)
            strategy = cc.Series.GetTooltipFindingStrategy();

        return cc.Series.SelectMany(series => series.FindHitPoints(cc, point, strategy));
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetVisualsAt(LvcPoint)"/>
    public override IEnumerable<VisualElement<SkiaSharpDrawingContext>> GetVisualsAt(LvcPoint point)
    {
        return core is not PolarChart<SkiaSharpDrawingContext> cc
            ? throw new Exception("core not found")
            : cc.VisualElements.SelectMany(visual => ((VisualElement<SkiaSharpDrawingContext>)visual).IsHitBy(core, point));
    }

    private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged();
    }

    private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
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
        core?.InvokePointerDown(new LvcPoint(e.Location.X, e.Location.Y), false);
    }

    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        core?.InvokePointerUp(new LvcPoint(e.Location.X, e.Location.Y), false);
    }
}
