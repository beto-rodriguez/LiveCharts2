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
        private readonly List<Tuple<ObservableValue, IDrawableTask<SkiaSharpDrawingContext>?>> _tuples = new();

        /// <summary>
        /// Gets or sets the inner radius.
        /// </summary>
        /// <value>
        /// The inner radius.
        /// </value>
        public double InnerRadius { get; set; }

        /// <summary>
        /// Gets or sets the offset radius, the separation between each gauge if multiple gauges are nested.
        /// </summary>
        /// <value>
        /// The relative inner radius.
        /// </value>
        public double OffsetRadius { get; set; }

        /// <summary>
        /// Gets or sets the background inner radius.
        /// </summary>
        /// <value>
        /// The background inner radius.
        /// </value>
        public double BackgroundInnerRadius { get; set; }

        /// <summary>
        /// Gets or sets the background offset radius, the separation between each gauge if multiple gauges are nested.
        /// </summary>
        /// <value>
        /// The background relative inner radius.
        /// </value>
        public double BackgroundOffsetRadius { get; set; }

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public IDrawableTask<SkiaSharpDrawingContext> Background { get; set; } = new SolidColorPaintTask(new SKColor(0, 0, 0, 50));

        /// <summary>
        /// Gets or sets the size of the labels.
        /// </summary>
        /// <value>
        /// The size of the labels.
        /// </value>
        public double LabelsSize { get; set; } = 18;

        /// <summary>
        /// Gets or sets the labels position.
        /// </summary>
        /// <value>
        /// The labels position.
        /// </value>
        public PolarLabelsPosition LabelsPosition { get; set; }

        /// <summary>
        /// Gets or sets the label formatter.
        /// </summary>
        /// <value>
        /// The label formatter.
        /// </value>
        public Func<ChartPoint, string> LabelFormatter { get; set; } = point => point.PrimaryValue.ToString();

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="labelsDrawableTask">The labels drawable task.</param>
        /// <returns></returns>
        public GaugeBuilder AddValue(ObservableValue value, IDrawableTask<SkiaSharpDrawingContext>? labelsDrawableTask)
        {
            _tuples.Add(
                new Tuple<ObservableValue, IDrawableTask<SkiaSharpDrawingContext>?>(value, labelsDrawableTask));

            return this;
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="labelsColor">Color of the labels.</param>
        /// <returns></returns>
        public GaugeBuilder AddValue(ObservableValue value, SKColor labelsColor)
        {
            return AddValue(value, new SolidColorPaintTask(labelsColor));
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public GaugeBuilder AddValue(ObservableValue value)
        {
            return AddValue(value, new SolidColorPaintTask(new SKColor(35, 35, 35)));
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
                        Values = list,
                        DataLabelsDrawableTask = item.Item2,
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

            return series;
        }
    }
}
