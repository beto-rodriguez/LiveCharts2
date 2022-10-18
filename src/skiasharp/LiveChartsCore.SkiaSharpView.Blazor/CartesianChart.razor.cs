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

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LiveChartsCore.SkiaSharpView.Blazor;

/// <inheritdoc cref="ICartesianChartView{TDrawingContext}"/>
public partial class CartesianChart : Chart, ICartesianChartView<SkiaSharpDrawingContext>
{
    private CollectionDeepObserver<ISeries>? _seriesObserver;
    private CollectionDeepObserver<ICartesianAxis>? _xObserver;
    private CollectionDeepObserver<ICartesianAxis>? _yObserver;
    private CollectionDeepObserver<Section<SkiaSharpDrawingContext>>? _sectionsObserver;
    private CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>>? _visualsObserver;
    private IEnumerable<ISeries> _series = new ObservableCollection<ISeries>();
    private IEnumerable<ICartesianAxis>? _xAxes;
    private IEnumerable<ICartesianAxis>? _yAxes;
    private IEnumerable<Section<SkiaSharpDrawingContext>> _sections = new List<Section<SkiaSharpDrawingContext>>();
    private IEnumerable<ChartElement<SkiaSharpDrawingContext>> _visuals = new List<ChartElement<SkiaSharpDrawingContext>>();
    private DrawMarginFrame<SkiaSharpDrawingContext>? _drawMarginFrame;
    private TooltipFindingStrategy _tooltipFindingStrategy = LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy;

    /// <inheritdoc cref="Chart.OnInitialized"/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        _seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _xObserver = new CollectionDeepObserver<ICartesianAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _yObserver = new CollectionDeepObserver<ICartesianAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _sectionsObserver = new CollectionDeepObserver<Section<SkiaSharpDrawingContext>>(
            OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _visualsObserver = new CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>>(
            OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

        if (_xAxes is null)
            XAxes = new List<ICartesianAxis>()
            {
                LiveCharts.CurrentSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultCartesianAxis()
            };

        if (_yAxes is null)
            YAxes = new List<ICartesianAxis>()
            {
                LiveCharts.CurrentSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultCartesianAxis()
            };

        // ToDo: pointer events
        //var c = Controls[0].Controls[0];

        //c.MouseWheel += OnMouseWheel;
        //c.MouseDown += OnMouseDown;
        //c.MouseUp += OnMouseUp;
    }

    CartesianChart<SkiaSharpDrawingContext> ICartesianChartView<SkiaSharpDrawingContext>.Core =>
    core is null ? throw new Exception("core not found") : (CartesianChart<SkiaSharpDrawingContext>)core;

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Series" />
    [Parameter]
    public IEnumerable<ISeries> Series
    {
        get => _series;
        set
        {
            _seriesObserver?.Dispose(_series);
            _seriesObserver?.Initialize(value);
            _series = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.XAxes" />
    [Parameter]
    public IEnumerable<ICartesianAxis> XAxes
    {
        get => _xAxes ?? throw new Exception($"{nameof(XAxes)} can not be null");
        set
        {
            _xObserver?.Dispose(_xAxes);
            _xObserver?.Initialize(value);
            _xAxes = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.YAxes" />
    [Parameter]
    public IEnumerable<ICartesianAxis> YAxes
    {
        get => _yAxes ?? throw new Exception($"{nameof(YAxes)} can not be null");
        set
        {
            _yObserver?.Dispose(_yAxes);
            _yObserver?.Initialize(value);
            _yAxes = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Sections" />
    [Parameter]
    public IEnumerable<Section<SkiaSharpDrawingContext>> Sections
    {
        get => _sections;
        set
        {
            _sectionsObserver?.Dispose(_sections);
            _sectionsObserver?.Initialize(value);
            _sections = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.VisualElements" />
    [Parameter]
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

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.DrawMarginFrame" />
    [Parameter]
    public DrawMarginFrame<SkiaSharpDrawingContext>? DrawMarginFrame
    {
        get => _drawMarginFrame;
        set
        {
            _drawMarginFrame = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ZoomMode" />
    [Parameter]
    public ZoomAndPanMode ZoomMode { get; set; } = LiveCharts.CurrentSettings.DefaultZoomMode;

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ZoomingSpeed" />
    [Parameter]
    public double ZoomingSpeed { get; set; } = LiveCharts.CurrentSettings.DefaultZoomSpeed;

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.TooltipFindingStrategy" />
    [Parameter]
    public TooltipFindingStrategy TooltipFindingStrategy
    {
        get => _tooltipFindingStrategy;
        set
        {
            _tooltipFindingStrategy = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ScaleUIPoint(LvcPoint, int, int)" />
    public double[] ScaleUIPoint(LvcPoint point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        if (core is null) throw new Exception("core not found");
        var cartesianCore = (CartesianChart<SkiaSharpDrawingContext>)core;
        return cartesianCore.ScaleUIPoint(point, xAxisIndex, yAxisIndex);
    }

    /// <inheritdoc cref="Chart.InitializeCore" />
    protected override void InitializeCore()
    {
        if (motionCanvas is null) throw new Exception("MotionCanvas component was not found");

        var zoomingSection = new RectangleGeometry();
        var zoomingSectionPaint = new SolidColorPaint
        {
            IsFill = true,
            Color = new SkiaSharp.SKColor(33, 150, 243, 50),
            ZIndex = int.MaxValue
        };
        zoomingSectionPaint.AddGeometryToPaintTask(motionCanvas.CanvasCore, zoomingSection);
        motionCanvas.CanvasCore.AddDrawableTask(zoomingSectionPaint);

        core = new CartesianChart<SkiaSharpDrawingContext>(
            this, LiveChartsSkiaSharp.DefaultPlatformBuilder, motionCanvas.CanvasCore, zoomingSection);
        if (((IChartView)this).DesignerMode) return;
        core.Update();
    }

    /// <inheritdoc cref="Chart.OnWheel(WheelEventArgs)" />
    protected override void OnWheel(WheelEventArgs e)
    {
        if (core is null) throw new Exception("core not found");
        var c = (CartesianChart<SkiaSharpDrawingContext>)core;
        var p = new LvcPoint((float)e.OffsetX, (float)e.OffsetY);

        Console.WriteLine($"dx {e.DeltaX}, dy {e.DeltaY}, dz {e.DeltaZ}, dm {e.DeltaMode}");

        c.Zoom(p, e.DeltaY < 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);

        // ToDo:
        // capute pointer??
        // Capture = true;
    }

    /// <inheritdoc cref="Chart.OnDisposing"/>
    protected override void OnDisposing()
    {
        core?.Unload();

        Series = Array.Empty<ISeries>();
        XAxes = Array.Empty<ICartesianAxis>();
        YAxes = Array.Empty<ICartesianAxis>();
        Sections = Array.Empty<RectangularSection>();
        VisualElements = Array.Empty<ChartElement<SkiaSharpDrawingContext>>();
        _seriesObserver = null!;
        _xObserver = null!;
        _yObserver = null!;
        _sectionsObserver = null!;
        _visuals = null!;
    }

    private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if(sender is ChartElement<SkiaSharpDrawingContext> stop && stop._isInternalSet) return;
        OnPropertyChanged();
    }

    private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(sender is ChartElement<SkiaSharpDrawingContext> stop && stop._isInternalSet) return;
        OnPropertyChanged();
    }
}
