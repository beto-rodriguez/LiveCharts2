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

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.VisualElements;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LiveChartsCore.SkiaSharpView.Blazor;

/// <inheritdoc cref="ICartesianChartView"/>
public partial class CartesianChart : Chart, ICartesianChartView
{
    private CollectionDeepObserver<ISeries>? _seriesObserver;
    private CollectionDeepObserver<ICartesianAxis>? _xObserver;
    private CollectionDeepObserver<ICartesianAxis>? _yObserver;
    private CollectionDeepObserver<CoreSection>? _sectionsObserver;

    private IEnumerable<ISeries> _series = new ObservableCollection<ISeries>();
    private IEnumerable<ICartesianAxis>? _xAxes;
    private IEnumerable<ICartesianAxis>? _yAxes;
    private IEnumerable<CoreSection> _sections = [];
    private CoreDrawMarginFrame? _drawMarginFrame;
    private FindingStrategy _findingStrategy = LiveCharts.DefaultSettings.FindingStrategy;

    /// <inheritdoc cref="Chart.OnInitialized"/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        _seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _xObserver = new CollectionDeepObserver<ICartesianAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _yObserver = new CollectionDeepObserver<ICartesianAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _sectionsObserver = new CollectionDeepObserver<CoreSection>(
            OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

        if (_xAxes is null)
            XAxes =
            [
                LiveCharts.DefaultSettings.GetProvider().GetDefaultCartesianAxis()
            ];

        if (_yAxes is null)
            YAxes =
            [
                LiveCharts.DefaultSettings.GetProvider().GetDefaultCartesianAxis()
            ];

        // ToDo: pointer events
        //var c = Controls[0].Controls[0];

        //c.MouseWheel += OnMouseWheel;
        //c.MouseDown += OnMouseDown;
        //c.MouseUp += OnMouseUp;
    }

    CartesianChartEngine ICartesianChartView.Core =>
    core is null ? throw new Exception("core not found") : (CartesianChartEngine)core;

    /// <inheritdoc cref="ICartesianChartView.Series" />
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

    /// <inheritdoc cref="ICartesianChartView.XAxes" />
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

    /// <inheritdoc cref="ICartesianChartView.YAxes" />
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

    /// <inheritdoc cref="ICartesianChartView.Sections" />
    [Parameter]
    public IEnumerable<CoreSection> Sections
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

    /// <inheritdoc cref="ICartesianChartView.DrawMarginFrame" />
    [Parameter]
    public CoreDrawMarginFrame? DrawMarginFrame
    {
        get => _drawMarginFrame;
        set
        {
            _drawMarginFrame = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="ICartesianChartView.ZoomMode" />
    [Parameter]
    public ZoomAndPanMode ZoomMode { get; set; } = LiveCharts.DefaultSettings.ZoomMode;

    /// <inheritdoc cref="ICartesianChartView.ZoomingSpeed" />
    [Parameter]
    public double ZoomingSpeed { get; set; } = LiveCharts.DefaultSettings.ZoomSpeed;

    /// <inheritdoc cref="ICartesianChartView.FindingStrategy" />
    [Parameter]
    [Obsolete($"Renamed to {nameof(FindingStrategy)}")]
    public TooltipFindingStrategy TooltipFindingStrategy
    {
        get => FindingStrategy.AsOld();
        set => FindingStrategy = value.AsNew();
    }

    /// <inheritdoc cref="ICartesianChartView.FindingStrategy" />
    [Parameter]
    public FindingStrategy FindingStrategy
    {
        get => _findingStrategy;
        set
        {
            _findingStrategy = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="ICartesianChartView.ScalePixelsToData(LvcPointD, int, int)"/>
    public LvcPointD ScalePixelsToData(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        if (core is not CartesianChartEngine cc) throw new Exception("core not found");
        var xScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.XAxes[xAxisIndex]);
        var yScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.YAxes[yAxisIndex]);

        return new LvcPointD { X = xScaler.ToChartValues(point.X), Y = yScaler.ToChartValues(point.Y) };
    }

    /// <inheritdoc cref="ICartesianChartView.ScaleDataToPixels(LvcPointD, int, int)"/>
    public LvcPointD ScaleDataToPixels(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        if (core is not CartesianChartEngine cc) throw new Exception("core not found");

        var xScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.XAxes[xAxisIndex]);
        var yScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.YAxes[yAxisIndex]);

        return new LvcPointD { X = xScaler.ToPixels(point.X), Y = yScaler.ToPixels(point.Y) };
    }

    /// <inheritdoc cref="IChartView.GetPointsAt(LvcPointD, FindingStrategy, FindPointFor)"/>
    public override IEnumerable<ChartPoint> GetPointsAt(LvcPointD point, FindingStrategy strategy = FindingStrategy.Automatic, FindPointFor findPointFor = FindPointFor.HoverEvent)
    {
        if (core is not CartesianChartEngine cc) throw new Exception("core not found");

        if (strategy == FindingStrategy.Automatic)
            strategy = cc.Series.GetFindingStrategy();

        return cc.Series.SelectMany(series => series.FindHitPoints(cc, new(point), strategy, findPointFor));
    }

    /// <inheritdoc cref="IChartView.GetVisualsAt(LvcPointD)"/>
    public override IEnumerable<IChartElement> GetVisualsAt(LvcPointD point)
    {
        return core is not CartesianChartEngine cc
            ? throw new Exception("core not found")
            : cc.VisualElements.SelectMany(visual => ((CoreVisualElement)visual).IsHitBy(core, new(point)));
    }

    /// <inheritdoc cref="Chart.InitializeCore" />
    protected override void InitializeCore()
    {
        if (motionCanvas is null) throw new Exception("MotionCanvas component was not found");

        core = new CartesianChartEngine(
            this, config => config.UseDefaults(), motionCanvas.CanvasCore);

        if (((IChartView)this).DesignerMode) return;
        core.Update();
    }

    /// <inheritdoc cref="Chart.OnWheel(WheelEventArgs)" />
    protected override void OnWheel(WheelEventArgs e)
    {
        if (core is null) throw new Exception("core not found");
        var c = (CartesianChartEngine)core;
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

        Series = [];
        XAxes = [];
        YAxes = [];
        Sections = [];
        VisualElements = [];
        _seriesObserver = null!;
        _xObserver = null!;
        _yObserver = null!;
        _sectionsObserver = null!;
    }

    private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        OnPropertyChanged();

    private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e) =>
        OnPropertyChanged();
}
