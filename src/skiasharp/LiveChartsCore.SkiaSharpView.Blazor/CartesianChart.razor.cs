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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.VisualElements;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LiveChartsCore.SkiaSharpView.Blazor;

/// <inheritdoc cref="ICartesianChartView"/>
public partial class CartesianChart : Chart, ICartesianChartView
{
    private ICollection<ISeries> _series = [];
    private ICollection<ICartesianAxis> _xAxes = [];
    private ICollection<ICartesianAxis> _yAxes = [];
    private ICollection<IChartElement> _sections = [];
    private IChartElement? _drawMarginFrame;
    private FindingStrategy _findingStrategy = LiveCharts.DefaultSettings.FindingStrategy;
    private bool _matchAxesScreenDataRatio;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartesianChart"/> class.
    /// </summary>
    public CartesianChart()
    {
        _ = Observe
            .Collection(nameof(Series))
            .Collection(nameof(XAxes))
            .Collection(nameof(YAxes))
            .Collection(nameof(Sections))
            .Property(nameof(DrawMarginFrame));

        XAxes = new ObservableCollection<ICartesianAxis>();
        YAxes = new ObservableCollection<ICartesianAxis>();
        Series = new ObservableCollection<ISeries>();
        VisualElements = new ObservableCollection<IChartElement>();
        Sections = new ObservableCollection<IChartElement>();
    }

    CartesianChartEngine ICartesianChartView.Core =>
    core is null ? throw new Exception("core not found") : (CartesianChartEngine)core;

    /// <inheritdoc cref="ICartesianChartView.Series" />
    [Parameter]
    public ICollection<ISeries> Series
    {
        get => _series;
        set { _series = value; Observe[nameof(Series)].Initialize(value); OnPropertyChanged(); }
    }

    /// <inheritdoc cref="ICartesianChartView.XAxes" />
    [Parameter]
    public ICollection<ICartesianAxis> XAxes
    {
        get => _xAxes;
        set { _xAxes = value; Observe[nameof(XAxes)].Initialize(value); OnPropertyChanged(); }
    }

    /// <inheritdoc cref="ICartesianChartView.YAxes" />
    [Parameter]
    public ICollection<ICartesianAxis> YAxes
    {
        get => _yAxes;
        set { _yAxes = value; Observe[nameof(YAxes)].Initialize(value); OnPropertyChanged(); }
    }

    /// <inheritdoc cref="ICartesianChartView.Sections" />
    [Parameter]
    public ICollection<IChartElement> Sections
    {
        get => _sections;
        set { _sections = value; Observe[nameof(Sections)].Initialize(value); OnPropertyChanged(); }
    }

    /// <inheritdoc cref="ICartesianChartView.DrawMarginFrame" />
    [Parameter]
    public IChartElement? DrawMarginFrame
    {
        get => _drawMarginFrame;
        set { _drawMarginFrame = value; Observe[nameof(DrawMarginFrame)].Initialize(value); OnPropertyChanged(); }
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

    /// <inheritdoc cref="ICartesianChartView.MatchAxesScreenDataRatio" />
    [Parameter]
    public bool MatchAxesScreenDataRatio
    {
        get => _matchAxesScreenDataRatio;
        set
        {
            _matchAxesScreenDataRatio = value;
            MatchAxesScreenDataRatioChanged();
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
            : cc.VisualElements.SelectMany(visual => ((VisualElement)visual).IsHitBy(core, new(point)));
    }

    /// <inheritdoc cref="Chart.InitializeCore" />
    protected override void InitializeCore()
    {
        if (motionCanvas is null) throw new Exception("MotionCanvas component was not found");

        core = new CartesianChartEngine(
            this, config => config.UseDefaults(), motionCanvas.CanvasCore);

        MatchAxesScreenDataRatioChanged();

        if (((IChartView)this).DesignerMode) return;
        core.Update();
    }

    /// <inheritdoc cref="Chart.OnWheel(WheelEventArgs)" />
    protected override void OnWheel(WheelEventArgs e)
    {
        if (core is null) throw new Exception("core not found");
        var c = (CartesianChartEngine)core;
        var p = new LvcPoint((float)e.OffsetX, (float)e.OffsetY);

        c.Zoom(p, e.DeltaY < 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);

        // ToDo:
        // capute pointer??
        // Capture = true;
    }

    /// <inheritdoc cref="Chart.OnDisposing"/>
    protected override void OnDisposing()
    {
        base.OnDisposing();
        core?.Unload();

        Series = [];
        XAxes = [];
        YAxes = [];
        Sections = [];
        VisualElements = [];
    }

    private void MatchAxesScreenDataRatioChanged()
    {
        if (core is null) return;
        if (MatchAxesScreenDataRatio) SharedAxes.MatchAxesScreenDataRatio(this);
        else SharedAxes.DisposeMatchAxesScreenDataRatio(this);
    }
}
