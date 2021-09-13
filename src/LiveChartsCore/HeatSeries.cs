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

using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using System;
using System.Collections.Generic;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a column series.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TVisual"></typeparam>
    /// <typeparam name="TLabel"></typeparam>
    /// <typeparam name="TDrawingContext"></typeparam>
    public abstract class HeatSeries<TModel, TVisual, TLabel, TDrawingContext>
        : CartesianSeries<TModel, TVisual, TLabel, TDrawingContext>, IHeatSeries<TDrawingContext>
            where TVisual : class, ISolidColorChartPoint<TDrawingContext>, new()
            where TDrawingContext : DrawingContext
            where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    {
        private IPaint<TDrawingContext>? _paintTaks;
        private Bounds _weightBounds = new();
        private int _heatKnownLength = 0;
        private List<Tuple<double, LvcColor>> _heatStops = new();
        private LvcColor[] _heatMap = new[]
        {
            LvcColor.FromArgb(255, 87, 103, 222), // cold (min value)
            LvcColor.FromArgb(255, 95, 207, 249) // hot (max value)
        };
        private double[]? _colorStops;
        private Padding _pointPadding = new(4);

        /// <summary>
        /// Initializes a new instance of the <see cref="HeatSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
        /// </summary>
        protected HeatSeries()
            : base(
                 SeriesProperties.Heat | SeriesProperties.PrimaryAxisVerticalOrientation |
                 SeriesProperties.Solid | SeriesProperties.PrefersXYStrategyTooltips)
        {
            DataPadding = new LvcPoint(0, 0);
            TooltipLabelFormatter = (point) => $"{Name}: {point.TertiaryValue:N}";
        }

        /// <inheritdoc cref="IHeatSeries{TDrawingContext}.HeatMap"/>
        public LvcColor[] HeatMap { get => _heatMap; set { _heatMap = value; OnPropertyChanged(); OnSeriesMiniatureChanged(); } }

        /// <inheritdoc cref="IHeatSeries{TDrawingContext}.ColorStops"/>
        public double[]? ColorStops { get => _colorStops; set { _colorStops = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IHeatSeries{TDrawingContext}.PointPadding"/>
        public Padding PointPadding { get => _pointPadding; set { _pointPadding = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ChartElement{TDrawingContext}.Measure(Chart{TDrawingContext})"/>
        public override void Measure(Chart<TDrawingContext> chart)
        {
            _paintTaks ??= LiveCharts.CurrentSettings.GetProvider<TDrawingContext>().GetSolidColorPaint();

            var cartesianChart = (CartesianChart<TDrawingContext>)chart;
            var primaryAxis = cartesianChart.YAxes[ScalesYAt];
            var secondaryAxis = cartesianChart.XAxes[ScalesXAt];

            var drawLocation = cartesianChart.DrawMarginLocation;
            var drawMarginSize = cartesianChart.DrawMarginSize;
            var secondaryScale = new Scaler(drawLocation, drawMarginSize, secondaryAxis);
            var primaryScale = new Scaler(drawLocation, drawMarginSize, primaryAxis);
            var previousPrimaryScale =
                primaryAxis.PreviousDataBounds is null ? null : new Scaler(drawLocation, drawMarginSize, primaryAxis, true);
            var previousSecondaryScale =
                secondaryAxis.PreviousDataBounds is null ? null : new Scaler(drawLocation, drawMarginSize, secondaryAxis, true);

            var uws = secondaryScale.MeasureInPixels(secondaryAxis.UnitWidth);
            var uwp = primaryScale.MeasureInPixels(primaryAxis.UnitWidth);

            var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;
            if (_paintTaks is not null)
            {
                _paintTaks.ZIndex = actualZIndex + 0.2;
                _paintTaks.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
                cartesianChart.Canvas.AddDrawableTask(_paintTaks);
            }
            if (DataLabelsPaint is not null)
            {
                DataLabelsPaint.ZIndex = actualZIndex + 0.3;
                DataLabelsPaint.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
                cartesianChart.Canvas.AddDrawableTask(DataLabelsPaint);
            }

            var dls = (float)DataLabelsSize;
            var toDeletePoints = new HashSet<ChartPoint>(everFetched);

            var p = PointPadding;

            if (_heatKnownLength != HeatMap.Length)
            {
                _heatStops = HeatFunctions.BuildColorStops(HeatMap, ColorStops);
                _heatKnownLength = HeatMap.Length;
            }

            foreach (var point in Fetch(cartesianChart))
            {
                var visual = point.Context.Visual as TVisual;
                var primary = primaryScale.ToPixels(point.PrimaryValue);
                var secondary = secondaryScale.ToPixels(point.SecondaryValue);
                var tertiary = (float)point.TertiaryValue;

                var baseColor = HeatFunctions.InterpolateColor(tertiary, _weightBounds, HeatMap, _heatStops);

                if (point.IsNull)
                {
                    if (visual is not null)
                    {
                        visual.X = secondary - uws * 0.5f;
                        visual.Y = primary - uwp * 0.5f;
                        visual.Width = uws;
                        visual.Height = uwp;
                        visual.RemoveOnCompleted = true;
                        visual.Color = LvcColor.FromArgb(0, visual.Color);
                        point.Context.Visual = null;
                    }
                    continue;
                }

                if (visual is null)
                {
                    var xi = secondary - uws * 0.5f;
                    var yi = primary - uwp * 0.5f;

                    if (previousSecondaryScale is not null && previousPrimaryScale is not null)
                    {
                        var previousP = previousPrimaryScale.ToPixels(pivot);
                        var previousPrimary = previousPrimaryScale.ToPixels(point.PrimaryValue);
                        var bp = Math.Abs(previousPrimary - previousP);
                        var cyp = point.PrimaryValue > pivot ? previousPrimary : previousPrimary - bp;

                        xi = previousSecondaryScale.ToPixels(point.SecondaryValue) - uws * 0.5f;
                        yi = previousPrimaryScale.ToPixels(point.PrimaryValue) - uwp * 0.5f;
                    }

                    var r = new TVisual
                    {
                        X = xi + p.Left,
                        Y = yi + p.Top,
                        Width = uws - p.Left - p.Right,
                        Height = uwp - p.Top - p.Bottom,
                        Color = LvcColor.FromArgb(0, baseColor.R, baseColor.G, baseColor.B)
                    };

                    visual = r;
                    point.Context.Visual = visual;
                    OnPointCreated(point);

                    _ = everFetched.Add(point);
                }

                if (_paintTaks is not null) _paintTaks.AddGeometryToPaintTask(cartesianChart.Canvas, visual);

                visual.X = secondary - uws * 0.5f + p.Left;
                visual.Y = primary - uwp * 0.5f + p.Top;
                visual.Width = uws - p.Left - p.Right;
                visual.Height = uwp - p.Top - p.Bottom;
                visual.Color = LvcColor.FromArgb(baseColor.A, baseColor.R, baseColor.G, baseColor.B);
                visual.RemoveOnCompleted = false;

                var ha = new RectangleHoverArea().SetDimensions(secondary - uws * 0.5f, primary - uwp * 0.5f, uws, uwp);
                point.Context.HoverArea = ha;

                OnPointMeasured(point);
                _ = toDeletePoints.Remove(point);

                if (DataLabelsPaint is not null)
                {
                    var label = (TLabel?)point.Context.Label;

                    if (label is null)
                    {
                        var l = new TLabel { X = secondary - uws * 0.5f, Y = primary - uws * 0.5f };

                        _ = l.TransitionateProperties(nameof(l.X), nameof(l.Y))
                            .WithAnimation(animation =>
                                animation
                                    .WithDuration(AnimationsSpeed ?? cartesianChart.AnimationsSpeed)
                                    .WithEasingFunction(EasingFunction ?? cartesianChart.EasingFunction));

                        l.CompleteAllTransitions();
                        label = l;
                        point.Context.Label = l;
                    }

                    DataLabelsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, label);

                    label.Text = DataLabelsFormatter(new TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>(point));
                    label.TextSize = dls;
                    label.Padding = DataLabelsPadding;
                    var labelPosition = GetLabelPosition(
                         secondary, primary, uws, uws, label.Measure(DataLabelsPaint), DataLabelsPosition, SeriesProperties, point.PrimaryValue > Pivot);
                    label.X = labelPosition.X;
                    label.Y = labelPosition.Y;
                }
            }

            foreach (var point in toDeletePoints)
            {
                if (point.Context.Chart != cartesianChart.View) continue;
                SoftDeleteOrDisposePoint(point, primaryScale, secondaryScale);
                _ = everFetched.Remove(point);
            }
        }

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.GetBounds(CartesianChart{TDrawingContext}, ICartesianAxis, ICartesianAxis)"/>
        public override SeriesBounds GetBounds(
            CartesianChart<TDrawingContext> chart, ICartesianAxis secondaryAxis, ICartesianAxis primaryAxis)
        {
            var baseSeriesBounds = base.GetBounds(chart, secondaryAxis, primaryAxis);
            if (baseSeriesBounds.HasData) return baseSeriesBounds;
            var baseBounds = baseSeriesBounds.Bounds;

            _weightBounds = baseBounds.TertiaryBounds;

            var tickPrimary = primaryAxis.GetTick(chart.ControlSize, baseBounds.VisiblePrimaryBounds);
            var tickSecondary = secondaryAxis.GetTick(chart.ControlSize, baseBounds.VisibleSecondaryBounds);

            var ts = tickSecondary.Value * DataPadding.X;
            var tp = tickPrimary.Value * DataPadding.Y;

            if (baseBounds.VisibleSecondaryBounds.Delta == 0)
            {
                var ms = baseBounds.VisibleSecondaryBounds.Min == 0 ? 1 : baseBounds.VisibleSecondaryBounds.Min;
                ts = 0.1 * ms * DataPadding.X;
            }

            if (baseBounds.VisiblePrimaryBounds.Delta == 0)
            {
                var mp = baseBounds.VisiblePrimaryBounds.Min == 0 ? 1 : baseBounds.VisiblePrimaryBounds.Min;
                tp = 0.1 * mp * DataPadding.Y;
            }

            return
                new SeriesBounds(
                    new DimensionalBounds
                    {
                        SecondaryBounds = new Bounds
                        {
                            Max = baseBounds.SecondaryBounds.Max + 0.5 * secondaryAxis.UnitWidth,
                            Min = baseBounds.SecondaryBounds.Min - 0.5 * secondaryAxis.UnitWidth,
                            PaddingMax = ts,
                            PaddingMin = ts
                        },
                        PrimaryBounds = new Bounds
                        {
                            Max = baseBounds.PrimaryBounds.Max + 0.5 * primaryAxis.UnitWidth,
                            Min = baseBounds.PrimaryBounds.Min - 0.5 * primaryAxis.UnitWidth,
                            PaddingMax = tp,
                            PaddingMin = tp
                        },
                        VisibleSecondaryBounds = new Bounds
                        {
                            Max = baseBounds.VisibleSecondaryBounds.Max + 0.5 * secondaryAxis.UnitWidth,
                            Min = baseBounds.VisibleSecondaryBounds.Min - 0.5 * secondaryAxis.UnitWidth
                        },
                        VisiblePrimaryBounds = new Bounds
                        {
                            Max = baseBounds.VisiblePrimaryBounds.Max + 0.5 * primaryAxis.UnitWidth,
                            Min = baseBounds.VisiblePrimaryBounds.Min - 0.5 * primaryAxis.UnitWidth
                        },
                        MinDeltaPrimary = baseBounds.MinDeltaPrimary,
                        MinDeltaSecondary = baseBounds.MinDeltaSecondary
                    },
                    false);
        }

        /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.SetDefaultPointTransitions(ChartPoint)"/>
        protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
        {
            var chart = chartPoint.Context.Chart;

            if (chartPoint.Context.Visual is not TVisual visual) throw new Exception("Unable to initialize the point instance.");

            _ = visual
                .TransitionateProperties(
                    nameof(visual.X),
                    nameof(visual.Width),
                    nameof(visual.Y),
                    nameof(visual.Height),
                    nameof(visual.Color))
                .WithAnimation(animation =>
                    animation
                        .WithDuration(AnimationsSpeed ?? chart.AnimationsSpeed)
                        .WithEasingFunction(EasingFunction ?? chart.EasingFunction))
                .CompleteCurrentTransitions();
        }

        /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}.SoftDeleteOrDisposePoint(ChartPoint, Scaler, Scaler)"/>
        protected override void SoftDeleteOrDisposePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
        {
            var visual = (TVisual?)point.Context.Visual;
            if (visual is null) return;
            if (DataFactory is null) throw new Exception("Data provider not found");

            var chartView = (ICartesianChartView<TDrawingContext>)point.Context.Chart;
            if (chartView.Core.IsZoomingOrPanning)
            {
                visual.CompleteAllTransitions();
                visual.RemoveOnCompleted = true;
                DataFactory.DisposePoint(point);
                return;
            }

            visual.Color = LvcColor.FromArgb(255, visual.Color);
            visual.RemoveOnCompleted = true;

            var label = (TLabel?)point.Context.Label;
            if (label is null) return;

            label.TextSize = 1;
            label.RemoveOnCompleted = true;
        }

        /// <summary>
        /// Called when the paint context changes.
        /// </summary>
        protected override void OnSeriesMiniatureChanged()
        {
            var context = new CanvasSchedule<TDrawingContext>();
            var w = LegendShapeSize;

            var strokeClone = LiveCharts.CurrentSettings.GetProvider<TDrawingContext>().GetSolidColorPaint();
            var st = strokeClone.StrokeThickness;
            if (st > MaxSeriesStroke)
            {
                st = MaxSeriesStroke;
                strokeClone.StrokeThickness = MaxSeriesStroke;
            }

            var visual = new TVisual
            {
                X = st + MaxSeriesStroke - st,
                Y = st + MaxSeriesStroke - st,
                Height = (float)LegendShapeSize,
                Width = (float)LegendShapeSize,
                Color = HeatMap[0] // ToDo <- draw the gradient?
            };
            strokeClone.ZIndex = 1;
            context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(strokeClone, visual));

            context.Width = w + MaxSeriesStroke * 2;
            context.Height = w + MaxSeriesStroke * 2;

            CanvasSchedule = context;
        }

        /// <inheritdoc cref="ChartSeries{TModel, TVisual, TLabel, TDrawingContext}.MiniatureEquals(IChartSeries{TDrawingContext})"/>
        public override bool MiniatureEquals(IChartSeries<TDrawingContext> instance)
        {
            return instance is HeatSeries<TModel, TVisual, TLabel, TDrawingContext> hSeries
                && Name == instance.Name && HeatMap == hSeries.HeatMap;
        }

        /// <inheritdoc cref="ChartElement{TDrawingContext}.GetPaintTasks"/>
        protected override IPaint<TDrawingContext>?[] GetPaintTasks()
        {
            return new[] { _paintTaks, hoverPaint };
        }
    }
}
