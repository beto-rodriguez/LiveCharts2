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
using System.Linq;
using Eto.Forms;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.SkiaSharpView.Eto;

/// <inheritdoc cref="IPieChartView" />
public class PieChart : Chart, IPieChartView
{
    private ICollection<ISeries> _series = [];
    private bool _isClockwise = true;
    private double _initialRotation;
    private double _maxAngle = 360;
    private double _maxValue = double.NaN;
    private double _minValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="PieChart"/> class.
    /// </summary>
    public PieChart() : this(null, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PieChart"/> class.
    /// </summary>
    /// <param name="tooltip">The default tool tip control.</param>
    /// <param name="legend">The default legend.</param>
    public PieChart(IChartTooltip? tooltip = null, IChartLegend? legend = null)
        : base(tooltip, legend)
    {
        _ = Observe
           .Collection(nameof(Series));

        motionCanvas.MouseDown += OnMouseDown;
    }

    PieChartEngine IPieChartView.Core =>
        core is null ? throw new Exception("core not found") : (PieChartEngine)core;

    /// <inheritdoc cref="IPieChartView.Series" />
    public ICollection<ISeries> Series
    {
        get => _series;
        set { _series = value; Observe[nameof(Series)].Initialize(value); OnPropertyChanged(); }
    }

    /// <inheritdoc cref="IPieChartView.IsClockwise" />
    public bool IsClockwise { get => _isClockwise; set { _isClockwise = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPieChartView.InitialRotation" />
    public double InitialRotation { get => _initialRotation; set { _initialRotation = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPieChartView.MaxAngle" />
    public double MaxAngle { get => _maxAngle; set { _maxAngle = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPieChartView.MaxValue" />
    public double MaxValue { get => _maxValue; set { _maxValue = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IPieChartView.MinValue" />
    public double MinValue { get => _minValue; set { _minValue = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.GetPointsAt(LvcPointD, FindingStrategy, FindPointFor)"/>
    public override IEnumerable<ChartPoint> GetPointsAt(LvcPointD point, FindingStrategy strategy = FindingStrategy.Automatic, FindPointFor findPointFor = FindPointFor.HoverEvent)
    {
        if (core is not PieChartEngine cc) throw new Exception("core not found");

        if (strategy == FindingStrategy.Automatic)
            strategy = cc.Series.GetFindingStrategy();

        return cc.Series.SelectMany(series => series.FindHitPoints(cc, new(point), strategy, findPointFor));
    }

    /// <inheritdoc cref="IChartView.GetVisualsAt(LvcPointD)"/>
    public override IEnumerable<IChartElement> GetVisualsAt(LvcPointD point)
    {
        return core is not PieChartEngine cc
            ? throw new Exception("core not found")
            : cc.VisualElements.SelectMany(visual => ((VisualElement)visual).IsHitBy(core, new(point)));
    }

    /// <summary>
    /// Initializes the core.
    /// </summary>
    protected override void InitializeCore()
    {
        core = new PieChartEngine(
            this, config => config.UseDefaults(), motionCanvas.CanvasCore);
        core.Update();
    }

    /// <inheritdoc cref="Chart.OnUnloading"/>
    protected override void OnUnloading()
    {
        Series = [];
        VisualElements = [];
    }

    private void OnMouseDown(object? sender, MouseEventArgs e) =>
        core?.InvokePointerDown(new LvcPoint(e.Location.X, e.Location.Y), false);
}
