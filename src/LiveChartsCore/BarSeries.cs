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

using System.Collections.Generic;
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Painting;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore;

/// <summary>
/// Defines a bar series point.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TLabel">The type of the label.</typeparam>
/// <seealso cref="CartesianSeries{TModel, TVisual, TLabel}" />
/// <seealso cref="IBarSeries" />
/// <remarks>
/// Initializes a new instance of the <see cref="BarSeries{TModel, TVisual, TLabel}"/> class.
/// </remarks>
/// <param name="properties">The properties.</param>
/// <param name="values">The values.</param>
public abstract class BarSeries<TModel, TVisual, TLabel>(
    SeriesProperties properties,
    IReadOnlyCollection<TModel>? values)
        : StrokeAndFillCartesianSeries<TModel, TVisual, TLabel>(properties, values), IBarSeries
            where TVisual : CoreSizedGeometry, new()
            where TLabel : CoreLabelGeometry, new()
{
    private double _pading = 2;
    private double _maxBarWidth = 50;
    private bool _ignoresBarPosition = false;
    private double _rx;
    private double _ry;
    private Paint? _errorPaint;

    /// <inheritdoc cref="IBarSeries.Padding"/>
    public double Padding { get => _pading; set => SetProperty(ref _pading, value); }

    /// <inheritdoc cref="IBarSeries.MaxBarWidth"/>
    public double MaxBarWidth { get => _maxBarWidth; set => SetProperty(ref _maxBarWidth, value); }

    /// <inheritdoc cref="IBarSeries.IgnoresBarPosition"/>
    public bool IgnoresBarPosition { get => _ignoresBarPosition; set => SetProperty(ref _ignoresBarPosition, value); }

    /// <inheritdoc cref="IBarSeries.Rx"/>
    public double Rx { get => _rx; set => SetProperty(ref _rx, value); }

    /// <inheritdoc cref="IBarSeries.Ry"/>
    public double Ry { get => _ry; set => SetProperty(ref _ry, value); }

    /// <inheritdoc cref="IErrorSeries.ErrorPaint"/>
    public Paint? ErrorPaint
    {
        get => _errorPaint;
        set => SetPaintProperty(ref _errorPaint, value, PaintStyle.Stroke);
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel}.GetMiniaturesSketch"/>
    [System.Obsolete]
    public override Sketch GetMiniaturesSketch()
    {
        var schedules = new List<PaintSchedule>();

        if (Fill is not null) schedules.Add(BuildMiniatureSchedule(Fill, new TVisual()));
        if (Stroke is not null) schedules.Add(BuildMiniatureSchedule(Stroke, new TVisual()));

        return new Sketch(MiniatureShapeSize, MiniatureShapeSize, GeometrySvg)
        {
            PaintSchedules = schedules
        };
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel}.GetMiniature"/>"/>
    public override IChartElement GetMiniature(ChartPoint? point, int zindex)
    {
        return new GeometryVisual<TVisual, TLabel>
        {
            Fill = GetMiniatureFill(point, zindex + 1),
            Stroke = GetMiniatureStroke(point, zindex + 2),
            Width = MiniatureShapeSize,
            Height = MiniatureShapeSize,
            Svg = GeometrySvg,
            ClippingMode = ClipMode.None
        };
    }

    /// <summary>
    /// A mesure helper class.
    /// </summary>
    protected class MeasureHelper
    {
        /// <summary>
        /// Initializes a new instance of the measue helper class.
        /// </summary>
        /// <param name="scaler">The scaler.</param>
        /// <param name="cartesianChart">The chart.</param>
        /// <param name="barSeries">The series.</param>
        /// <param name="axis">The axis.</param>
        /// <param name="p">The pivot.</param>
        /// <param name="minP">The min pivot allowed.</param>
        /// <param name="maxP">The max pivot allowed.</param>
        /// <param name="isStacked">Indicates whether the series is stacked or not.</param>
        /// <param name="isRow">Indicates whether the serie is row or not.</param>
        public MeasureHelper(
            Scaler scaler,
            CartesianChartEngine cartesianChart,
            IBarSeries barSeries,
            ICartesianAxis axis,
            float p,
            float minP,
            float maxP,
            bool isStacked,
            bool isRow)
        {
            this.p = p;
            if (p < minP) this.p = minP;
            if (p > maxP) this.p = maxP;

            uw = scaler.MeasureInPixels(axis.UnitWidth);
            actualUw = uw;

            var gp = (float)barSeries.Padding;

            if (uw - gp < 1) gp -= uw - gp;

            uw -= gp;
            uwm = 0.5f * uw;

            int pos, count;

            if (isStacked)
            {
                pos = isRow
                    ? cartesianChart.SeriesContext.GetStackedRowPostion(barSeries)
                    : cartesianChart.SeriesContext.GetStackedColumnPostion(barSeries);
                count = isRow
                    ? cartesianChart.SeriesContext.GetStackedRowSeriesCount()
                    : cartesianChart.SeriesContext.GetStackedColumnSeriesCount();
            }
            else
            {
                pos = isRow
                    ? cartesianChart.SeriesContext.GetRowPosition(barSeries)
                    : cartesianChart.SeriesContext.GetColumnPostion(barSeries);
                count = isRow
                    ? cartesianChart.SeriesContext.GetRowSeriesCount()
                    : cartesianChart.SeriesContext.GetColumnSeriesCount();
            }

            cp = 0f;

            var padding = (float)barSeries.Padding;
            if (barSeries.IgnoresBarPosition) count = 1;

            uw /= count;
            var mw = (float)barSeries.MaxBarWidth;
            if (uw > mw) uw = mw;
            uwm = 0.5f * uw;
            cp = barSeries.IgnoresBarPosition
                ? 0
                : (pos - count / 2f) * uw + uwm;

            // apply the pading
            uw -= padding;
            cp += padding * 0.5f;

            if (uw < 1)
            {
                uw = 1;
                uwm = 0.5f;
            }
        }

        /// <summary>
        /// helper units.
        /// </summary>
        public float uw, uwm, cp, p, actualUw;
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel}.OnPointerEnter(ChartPoint)"/>
    protected override void OnPointerEnter(ChartPoint point)
    {
        var visual = (TVisual?)point.Context.Visual;
        if (visual is null) return;
        visual.Opacity = 0.8f;

        base.OnPointerEnter(point);
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel}.OnPointerLeft(ChartPoint)"/>
    protected override void OnPointerLeft(ChartPoint point)
    {
        var visual = (TVisual?)point.Context.Visual;
        if (visual is null) return;
        visual.Opacity = 1;

        base.OnPointerLeft(point);
    }

    /// <inheritdoc cref="ChartElement.GetPaintTasks"/>
    protected internal override Paint?[] GetPaintTasks() =>
        [Stroke, Fill, DataLabelsPaint, _errorPaint];

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel}.FindPointsInPosition(Chart, LvcPoint, FindingStrategy, FindPointFor)"/>
    protected override IEnumerable<ChartPoint> FindPointsInPosition(
        Chart chart, LvcPoint pointerPosition, FindingStrategy strategy, FindPointFor findPointFor)
    {
        return strategy == FindingStrategy.ExactMatch
            ? Fetch(chart)
                .Where(point =>
                {
                    var v = (TVisual?)point.Context.Visual;

                    return
                        v is not null &&
                        pointerPosition.X > v.X && pointerPosition.X < v.X + v.Width &&
                        pointerPosition.Y > v.Y && pointerPosition.Y < v.Y + v.Height;
                })
            : base.FindPointsInPosition(chart, pointerPosition, strategy, findPointFor);
    }
}
