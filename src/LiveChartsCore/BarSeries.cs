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
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Painting;

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
            where TVisual : BoundedDrawnGeometry, new()
            where TLabel : BaseLabelGeometry, new()
{
    private bool _showError;

    /// <inheritdoc cref="IBarSeries.Padding"/>
    public double Padding { get; set => SetProperty(ref field, value); } = 2;

    /// <inheritdoc cref="IBarSeries.MaxBarWidth"/>
    public double MaxBarWidth { get; set => SetProperty(ref field, value); } = 50;

    /// <inheritdoc cref="IBarSeries.IgnoresBarPosition"/>
    public bool IgnoresBarPosition { get; set => SetProperty(ref field, value); } = false;

    /// <inheritdoc cref="IBarSeries.Rx"/>
    public double Rx { get; set => SetProperty(ref field, value); }

    /// <inheritdoc cref="IBarSeries.Ry"/>
    public double Ry { get; set => SetProperty(ref field, value); }

    /// <inheritdoc cref="IErrorSeries.ShowError"/>
    public bool ShowError
    {
        get => _showError;
        set
        {
            SetProperty(ref _showError, value);
            ErrorPaint?.IsPaused = !value;
        }
    }

    /// <inheritdoc cref="IErrorSeries.ErrorPaint"/>
    public Paint? ErrorPaint
    {
        get;
        set
        {
            SetPaintProperty(ref field, value, PaintStyle.Stroke);
            _showError = value is not null && value != Paint.Default;
        }
    } = Paint.Default;

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel}.GetMiniatureGeometry"/>
    public override IDrawnElement GetMiniatureGeometry(ChartPoint? point)
    {
        var v = point?.Context.Visual;

        var m = new TVisual
        {
            Fill = v?.Fill ?? Fill,
            Stroke = v?.Stroke ?? Stroke,
            StrokeThickness = (float)MiniatureStrokeThickness,
            DrawEffect = DrawEffect.Local,
            ClippingBounds = LvcRectangle.Empty,
            Width = (float)MiniatureShapeSize,
            Height = (float)MiniatureShapeSize
        };

        if (m is IVariableSvgPath svg) svg.SVGPath = GeometrySvg;

        return m;
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

    /// <inheritdoc cref="ChartElement.GetPaintTasks"/>
    protected internal override Paint?[] GetPaintTasks() =>
        [Stroke, Fill, DataLabelsPaint, ErrorPaint];

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
