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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;

namespace LiveChartsCore;

/// <summary>
/// Defines a bar series point.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TLabel">The type of the label.</typeparam>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}" />
/// <seealso cref="IBarSeries{TDrawingContext}" />
public abstract class BarSeries<TModel, TVisual, TLabel, TDrawingContext>
    : StrokeAndFillCartesianSeries<TModel, TVisual, TLabel, TDrawingContext>, IBarSeries<TDrawingContext>
        where TVisual : class, ISizedGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
{
    private double _pading = 2;
    private double _maxBarWidth = 50;
    private bool _ignoresBarPosition = false;
    private double _rx;
    private double _ry;
    private IPaint<TDrawingContext>? _errorPaint;

    /// <summary>
    /// Initializes a new instance of the <see cref="BarSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
    /// </summary>
    /// <param name="properties">The properties.</param>
    protected BarSeries(SeriesProperties properties)
        : base(properties)
    { }

    /// <inheritdoc cref="IBarSeries{TDrawingContext}.GroupPadding"/>
    [Obsolete($"Replace by {nameof(Padding)} property.")]
    public double GroupPadding { get => _pading; set => SetProperty(ref _pading, value); }

    /// <inheritdoc cref="IBarSeries{TDrawingContext}.Padding"/>
    public double Padding { get => _pading; set => SetProperty(ref _pading, value); }

    /// <inheritdoc cref="IBarSeries{TDrawingContext}.MaxBarWidth"/>
    public double MaxBarWidth { get => _maxBarWidth; set => SetProperty(ref _maxBarWidth, value); }

    /// <inheritdoc cref="IBarSeries{TDrawingContext}.IgnoresBarPosition"/>
    public bool IgnoresBarPosition { get => _ignoresBarPosition; set => SetProperty(ref _ignoresBarPosition, value); }

    /// <inheritdoc cref="IBarSeries{TDrawingContext}.Rx"/>
    public double Rx { get => _rx; set => SetProperty(ref _rx, value); }

    /// <inheritdoc cref="IBarSeries{TDrawingContext}.Ry"/>
    public double Ry { get => _ry; set => SetProperty(ref _ry, value); }

    /// <inheritdoc cref="IErrorSeries{TDrawingContext}.ErrorPaint"/>
    public IPaint<TDrawingContext>? ErrorPaint
    {
        get => _errorPaint;
        set => SetPaintProperty(ref _errorPaint, value, true);
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.GetMiniaturesSketch"/>
    public override Sketch<TDrawingContext> GetMiniaturesSketch()
    {
        var schedules = new List<PaintSchedule<TDrawingContext>>();

        if (Fill is not null) schedules.Add(BuildMiniatureSchedule(Fill, new TVisual()));
        if (Stroke is not null) schedules.Add(BuildMiniatureSchedule(Stroke, new TVisual()));

        return new Sketch<TDrawingContext>(MiniatureShapeSize, MiniatureShapeSize, GeometrySvg)
        {
            PaintSchedules = schedules
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
            CartesianChart<TDrawingContext> cartesianChart,
            IBarSeries<TDrawingContext> barSeries,
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

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.OnPointerEnter(ChartPoint)"/>
    protected override void OnPointerEnter(ChartPoint point)
    {
        var visual = (TVisual?)point.Context.Visual;
        if (visual is null) return;
        visual.Opacity = 0.8f;

        base.OnPointerEnter(point);
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.OnPointerLeft(ChartPoint)"/>
    protected override void OnPointerLeft(ChartPoint point)
    {
        var visual = (TVisual?)point.Context.Visual;
        if (visual is null) return;
        visual.Opacity = 1;

        base.OnPointerLeft(point);
    }

    internal override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return new[] { Stroke, Fill, DataLabelsPaint, _errorPaint };
    }
}
