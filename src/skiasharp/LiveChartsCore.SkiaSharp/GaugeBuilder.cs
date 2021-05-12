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
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView
{
    /// <summary>
    /// Defines a helper class to build gauges.
    /// </summary>
    public class GaugeBuilder
    {
        private readonly List<Tuple<ObservableValue, string?, IDrawableTask<SkiaSharpDrawingContext>?, IDrawableTask<SkiaSharpDrawingContext>?>> _tuples = new();
        private List<PieSeries<ObservableValue>>? _builtSeries;
        private double _innerRadius;
        private double _offsetRadius;
        private double _backgroundInnerRadius;
        private double _backgroundOffsetRadius;
        private IDrawableTask<SkiaSharpDrawingContext> _background = new SolidColorPaintTask(new SKColor(0, 0, 0, 20));
        private double _labelsSize = 18;
        private PolarLabelsPosition _labelsPosition;
        private Func<ChartPoint, string> _labelFormatter = point => point.PrimaryValue.ToString();

        /// <summary>
        /// Gets or sets the inner radius.
        /// </summary>
        /// <value>
        /// The inner radius.
        /// </value>
        public double InnerRadius { get => _innerRadius; set { _innerRadius = value; OnPopertyChanged(); } }

        /// <summary>
        /// Gets or sets the offset radius, the separation between each gauge if multiple gauges are nested.
        /// </summary>
        /// <value>
        /// The relative inner radius.
        /// </value>
        public double OffsetRadius { get => _offsetRadius; set { _offsetRadius = value; OnPopertyChanged(); } }

        /// <summary>
        /// Gets or sets the background inner radius.
        /// </summary>
        /// <value>
        /// The background inner radius.
        /// </value>
        public double BackgroundInnerRadius { get => _backgroundInnerRadius; set { _backgroundInnerRadius = value; OnPopertyChanged(); } }

        /// <summary>
        /// Gets or sets the background offset radius, the separation between each gauge if multiple gauges are nested.
        /// </summary>
        /// <value>
        /// The background relative inner radius.
        /// </value>
        public double BackgroundOffsetRadius { get => _backgroundOffsetRadius; set { _backgroundOffsetRadius = value; OnPopertyChanged(); } }

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public IDrawableTask<SkiaSharpDrawingContext> Background { get => _background; set { _background = value; OnPopertyChanged(); } }

        /// <summary>
        /// Gets or sets the size of the labels.
        /// </summary>
        /// <value>
        /// The size of the labels.
        /// </value>
        public double LabelsSize { get => _labelsSize; set { _labelsSize = value; OnPopertyChanged(); } }

        /// <summary>
        /// Gets or sets the labels position.
        /// </summary>
        /// <value>
        /// The labels position.
        /// </value>
        public PolarLabelsPosition LabelsPosition { get => _labelsPosition; set { _labelsPosition = value; OnPopertyChanged(); } }

        /// <summary>
        /// Gets or sets the label formatter.
        /// </summary>
        /// <value>
        /// The label formatter.
        /// </value>
        public Func<ChartPoint, string> LabelFormatter { get => _labelFormatter; set { _labelFormatter = value; OnPopertyChanged(); } }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="seriesName">The series name.</param>
        /// <param name="seriesDrawableTask">The series drawable task.</param>
        /// <param name="labelsDrawableTask">The labels drawable task.</param>
        /// <returns></returns>
        public GaugeBuilder AddValue(
            ObservableValue value,
            string? seriesName,
            IDrawableTask<SkiaSharpDrawingContext>? seriesDrawableTask,
            IDrawableTask<SkiaSharpDrawingContext>? labelsDrawableTask = null)
        {
            labelsDrawableTask ??= new SolidColorPaintTask(new SKColor(35, 35, 35));

            _tuples.Add(
                new Tuple<ObservableValue, string?, IDrawableTask<SkiaSharpDrawingContext>?, IDrawableTask<SkiaSharpDrawingContext>?>(
                    value, seriesName, seriesDrawableTask, labelsDrawableTask));

            return this;
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="seriesName">The series name.</param>
        /// <param name="seriesColor">Color of the series.</param>
        /// <param name="labelsColor">Color of the labels.</param>
        /// <returns></returns>
        public GaugeBuilder AddValue(ObservableValue value, string seriesName, SKColor seriesColor, SKColor? labelsColor = null)
        {
            labelsColor ??= new SKColor(35, 35, 35);

            return AddValue(value, seriesName, new SolidColorPaintTask(seriesColor), new SolidColorPaintTask(labelsColor.Value));
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public GaugeBuilder AddValue(ObservableValue value)
        {
            return AddValue(value, null, LiveChartsSkiaSharp.DefaultPaintTask, new SolidColorPaintTask(new SKColor(35, 35, 35)));
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="seriesName">The series name.</param>
        /// <returns></returns>
        public GaugeBuilder AddValue(ObservableValue value, string? seriesName)
        {
            return AddValue(value, seriesName, LiveChartsSkiaSharp.DefaultPaintTask, new SolidColorPaintTask(new SKColor(35, 35, 35)));
        }

        /// <summary>
        /// Builds the series.
        /// </summary>
        /// <returns></returns>
        public List<PieSeries<ObservableValue>> BuildSeries()
        {
            var series = new List<PieSeries<ObservableValue>>();

            var i = 0;
            foreach (var item in _tuples)
            {
                var list = new List<ObservableValue>();
                while (list.Count < _tuples.Count - 1)
                {
                    list.Add(new ObservableValue(0));
                }
                list.Insert(i, item.Item1);

                series.Add(
                    new PieSeries<ObservableValue>
                    {
                        ZIndex = i + 1,
                        Values = list,
                        Name = item.Item2,
                        Fill = item.Item3,
                        DataLabelsDrawableTask = item.Item4,
                        DataLabelsSize = LabelsSize,
                        DataLabelsFormatter = LabelFormatter,
                        DataLabelsPosition = LabelsPosition,
                        InnerRadius = InnerRadius,
                        RelativeInnerRadius = OffsetRadius,
                        RelativeOuterRadius = OffsetRadius
                    });

                i++;
            }

            var fillSeriesValues = new List<ObservableValue>();
            while (fillSeriesValues.Count < _tuples.Count) fillSeriesValues.Add(new ObservableValue(0));

            series.Add(
                new PieSeries<ObservableValue>
                {
                    ZIndex = -1,
                    IsFillSeries = true,
                    Fill = Background,
                    Values = fillSeriesValues,
                    InnerRadius = BackgroundInnerRadius,
                    RelativeInnerRadius = BackgroundOffsetRadius,
                    RelativeOuterRadius = BackgroundOffsetRadius
                });

            _builtSeries = series;

            return series;
        }

        private void OnPopertyChanged()
        {
            if (_builtSeries == null) return;

            var i = 1;
            foreach (var item in _builtSeries)
            {
                if (item.IsFillSeries)
                {
                    item.Fill = Background;
                    item.InnerRadius = BackgroundInnerRadius;
                    item.RelativeInnerRadius = BackgroundOffsetRadius;
                    item.RelativeOuterRadius = BackgroundOffsetRadius;

                    continue;
                }
                item.InnerRadius = InnerRadius;
                item.RelativeInnerRadius = OffsetRadius;
                item.RelativeOuterRadius = OffsetRadius;
                item.DataLabelsPosition = LabelsPosition;
                item.DataLabelsFormatter = LabelFormatter;
                item.DataLabelsSize = LabelsSize;
            }
        }
    }
}
