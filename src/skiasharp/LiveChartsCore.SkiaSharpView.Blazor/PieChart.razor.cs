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

using System.Collections.Specialized;
using System.ComponentModel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.VisualElements;
using Microsoft.AspNetCore.Components;

namespace LiveChartsCore.SkiaSharpView.Blazor;

/// <inheritdoc cref="IPieChartView{TDrawingContext}"/>
public partial class PieChart : Chart, IPieChartView<SkiaSharpDrawingContext>
{
    private CollectionDeepObserver<ISeries>? _seriesObserver;
    private IEnumerable<ISeries> _series = new List<ISeries>();
    private double _initialRotation;
    private bool _isClockwise = true;
    private double _maxAngle = 360;
    private double? _maxValue;
    private double _minValue;

    /// <summary>
    /// Called when the control is initialized.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        _seriesObserver = new CollectionDeepObserver<ISeries>(
            (object? sender, NotifyCollectionChangedEventArgs e) => OnPropertyChanged(),
            (object? sender, PropertyChangedEventArgs e) => OnPropertyChanged(),
            true);
    }

    PieChart<SkiaSharpDrawingContext> IPieChartView<SkiaSharpDrawingContext>.Core =>
        core is null ? throw new Exception("core not found") : (PieChart<SkiaSharpDrawingContext>)core;

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.Series" />
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

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.IsClockwise" />
    [Parameter]
    public bool IsClockwise { get => _isClockwise; set { _isClockwise = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.InitialRotation" />
    [Parameter]
    public double InitialRotation { get => _initialRotation; set { _initialRotation = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.MaxAngle" />
    [Parameter]
    public double MaxAngle { get => _maxAngle; set { _maxAngle = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.Total" />
    [Parameter]
    [Obsolete($"Use {nameof(MaxValue)} instead.")]
    public double? Total { get => _maxValue; set { _maxValue = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.MaxValue" />
    [Parameter]
    public double? MaxValue { get => _maxValue; set { _maxValue = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.MinValue" />
    [Parameter]
    public double MinValue { get => _minValue; set { _minValue = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetPointsAt(LvcPoint, TooltipFindingStrategy)"/>
    public override IEnumerable<ChartPoint> GetPointsAt(LvcPoint point, TooltipFindingStrategy strategy = TooltipFindingStrategy.Automatic)
    {
        if (core is not PieChart<SkiaSharpDrawingContext> cc) throw new Exception("core not found");

        if (strategy == TooltipFindingStrategy.Automatic)
            strategy = cc.Series.GetTooltipFindingStrategy();

        return cc.Series.SelectMany(series => series.FindHitPoints(cc, point, strategy));
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.GetVisualsAt(LvcPoint)"/>
    public override IEnumerable<VisualElement<SkiaSharpDrawingContext>> GetVisualsAt(LvcPoint point)
    {
        return core is not PieChart<SkiaSharpDrawingContext> cc
            ? throw new Exception("core not found")
            : cc.VisualElements.SelectMany(visual => ((VisualElement<SkiaSharpDrawingContext>)visual).IsHitBy(core, point));
    }

    /// <summary>
    /// Initializes the core.
    /// </summary>
    protected override void InitializeCore()
    {
        if (motionCanvas is null) throw new Exception("MotionCanvas component was not found");

        core = new PieChart<SkiaSharpDrawingContext>(
            this, config => config.UseDefaults(), motionCanvas.CanvasCore);
        if (((IChartView)this).DesignerMode) return;
        core.Update();
    }

    /// <inheritdoc cref="Chart.OnDisposing"/>
    protected override void OnDisposing()
    {
        core?.Unload();

        Series = Array.Empty<ISeries>();
        _seriesObserver = null!;
        VisualElements = Array.Empty<ChartElement<SkiaSharpDrawingContext>>();
    }
}
