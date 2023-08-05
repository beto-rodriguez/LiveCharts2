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
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView;

/// <summary>
/// Defines a helper class to build gauges.
/// </summary>
[Obsolete($"Use {nameof(GaugeGenerator)}.{nameof(GaugeGenerator.BuildSolidGauge)} instead.")]
public class GaugeBuilder : IGaugeBuilder<SkiaSharpDrawingContext>
{
    private readonly Dictionary<ISeries, Tuple<ObservableValue, string?, IPaint<SkiaSharpDrawingContext>?, IPaint<SkiaSharpDrawingContext>?>> _keyValuePairs = new();
    private readonly List<Tuple<ObservableValue, string?, IPaint<SkiaSharpDrawingContext>?, IPaint<SkiaSharpDrawingContext>?>> _tuples = new();
    private List<PieSeries<ObservableValue>>? _builtSeries;

    private RadialAlignment? _radialAlign = null;
    private double? _innerRadius = null;
    private double? _offsetRadius = null;
    private double? _backgroundInnerRadius = null;
    private double? _backgroundOffsetRadius = null;
    private double? _backgroundCornerRadius = null;
    private double? _cornerRadius = null;
    private IPaint<SkiaSharpDrawingContext>? _background = null;
    private double? _labelsSize = null;
    private PolarLabelsPosition? _labelsPosition = null;
    private double? _backgroundMaxRadialColumnWidth = null;
    private double? _maxColumnWidth = null;
    private Func<ChartPoint, string> _labelFormatter = point => point.Coordinate.PrimaryValue.ToString();

    /// <summary>
    /// Gets or sets the inner radius, setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <value>
    /// The inner radius.
    /// </value>
    public double? InnerRadius { get => _innerRadius; set { _innerRadius = value; OnPopertyChanged(); } }

    /// <summary>
    /// Sets the inner radius, setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <param name="value">the value.</param>
    /// <returns></returns>
    public GaugeBuilder WithInnerRadius(double? value)
    {
        InnerRadius = value;
        return this;
    }

    /// <summary>
    /// Gets or sets the offset radius, the separation between each gauge if multiple gauges are nested,
    /// setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public double? OffsetRadius { get => _offsetRadius; set { _offsetRadius = value; OnPopertyChanged(); } }

    /// <summary>
    /// Sets the offset radius, the separation between each gauge if multiple gauges are nested,
    /// setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <param name="value">the value.</param>
    /// <returns></returns>
    public GaugeBuilder WithOffsetRadius(double? value)
    {
        OffsetRadius = value;
        return this;
    }

    /// <summary>
    /// Gets or sets the maximum width of the radial column, setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <value>
    /// The maximum width of the radial column.
    /// </value>
    public double? MaxColumnWidth { get => _maxColumnWidth; set { _maxColumnWidth = value; OnPopertyChanged(); } }

    /// <summary>
    /// Sets the maximum width of the radial column, setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <param name="value">the value.</param>
    public GaugeBuilder WithMaxColumnWidth(double? value)
    {
        MaxColumnWidth = value;
        return this;
    }

    /// <summary>
    /// Gets or sets the corner radius, setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <value>
    /// The corner radius.
    /// </value>
    public double? CornerRadius { get => _cornerRadius; set { _cornerRadius = value; OnPopertyChanged(); } }

    /// <summary>
    /// Sets the corner radius, setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <param name="value">the value.</param>
    public GaugeBuilder WithCornerRadius(double? value)
    {
        CornerRadius = value;
        return this;
    }

    /// <summary>
    /// Gets or sets the radial alignment, setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <value>
    /// The inner radius.
    /// </value>
    public RadialAlignment? RadialAlign { get => _radialAlign; set { _radialAlign = value; OnPopertyChanged(); } }

    /// <summary>
    /// Sets the radial alignment, setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <param name="value">the value.</param>
    public GaugeBuilder WithRadialAlign(RadialAlignment? value)
    {
        RadialAlign = value;
        return this;
    }

    /// <summary>
    /// Gets or sets the background inner radius, setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <value>
    /// The background inner radius.
    /// </value>
    public double? BackgroundInnerRadius { get => _backgroundInnerRadius; set { _backgroundInnerRadius = value; OnPopertyChanged(); } }

    /// <summary>
    /// Sets the background inner radius, setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <param name="value">the value.</param>
    public GaugeBuilder WithBackgroundInnerRadius(double? value)
    {
        BackgroundInnerRadius = value;
        return this;
    }

    /// <summary>
    /// Gets or sets the background offset radius, the separation between each gauge if multiple gauges are nested,
    /// setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <value>
    /// The background relative inner radius.
    /// </value>
    public double? BackgroundOffsetRadius { get => _backgroundOffsetRadius; set { _backgroundOffsetRadius = value; OnPopertyChanged(); } }

    /// <summary>
    /// Sets the background offset radius, the separation between each gauge if multiple gauges are nested,
    /// setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <param name="value">the value.</param>
    public GaugeBuilder WithBackgroundOffsetRadius(double? value)
    {
        BackgroundOffsetRadius = value;
        return this;
    }

    /// <summary>
    /// Gets or sets the width of the background maximum radial column, setting this property to null will let the theme
    /// decide the value, default is null.
    /// </summary>
    /// <value>
    /// The width of the background maximum radial column.
    /// </value>
    public double? BackgroundMaxRadialColumnWidth
    {
        get => _backgroundMaxRadialColumnWidth;
        set { _backgroundMaxRadialColumnWidth = value; OnPopertyChanged(); }
    }

    /// <summary>
    /// Sets the width of the background maximum radial column, setting this property to null will let the theme
    /// decide the value, default is null.
    /// </summary>
    /// <param name="value">the value.</param>
    public GaugeBuilder WithBackgroundMaxRadialColumnWidth(double? value)
    {
        BackgroundMaxRadialColumnWidth = value;
        return this;
    }

    /// <summary>
    /// Gets or sets the background corner radius, setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <value>
    /// The background corner radius.
    /// </value>
    public double? BackgroundCornerRadius { get => _backgroundCornerRadius; set { _backgroundCornerRadius = value; OnPopertyChanged(); } }

    /// <summary>
    /// Sets the background corner radius, setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <param name="value">the value.</param>
    public GaugeBuilder WithBackgroundCornerRadius(double? value)
    {
        BackgroundMaxRadialColumnWidth = value;
        return this;
    }

    /// <summary>
    /// Gets or sets the background.
    /// </summary>
    /// <value>
    /// The background.
    /// </value>
    public IPaint<SkiaSharpDrawingContext>? Background { get => _background; set { _background = value; OnPopertyChanged(); } }

    /// <summary>
    /// Sets the background.
    /// </summary>
    /// <param name="value">the value.</param>
    public GaugeBuilder WithBackground(IPaint<SkiaSharpDrawingContext>? value)
    {
        Background = value;
        return this;
    }

    /// <summary>
    /// Gets or sets the size of the labels, setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <value>
    /// The size of the labels.
    /// </value>
    public double? LabelsSize { get => _labelsSize; set { _labelsSize = value; OnPopertyChanged(); } }

    /// <summary>
    /// Sets the size of the labels, setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <param name="value">the value.</param>
    public GaugeBuilder WithLabelsSize(double value)
    {
        LabelsSize = value;
        return this;
    }

    /// <summary>
    /// Gets or sets the labels position, setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <value>
    /// The labels position.
    /// </value>
    public PolarLabelsPosition? LabelsPosition { get => _labelsPosition; set { _labelsPosition = value; OnPopertyChanged(); } }

    /// <summary>
    /// Sets  the labels position, setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <param name="value">the value.</param>
    public GaugeBuilder WithLabelsPosition(PolarLabelsPosition value)
    {
        LabelsPosition = value;
        return this;
    }

    /// <summary>
    /// Gets or sets the label formatter.
    /// </summary>
    /// <value>
    /// The label formatter.
    /// </value>
    public Func<ChartPoint, string> LabelFormatter { get => _labelFormatter; set { _labelFormatter = value; OnPopertyChanged(); } }

    /// <summary>
    /// Sets  the labels position, setting this property to null will let the theme decide the value, default is null.
    /// </summary>
    /// <param name="value">the value.</param>
    public GaugeBuilder WithLabelFormatter(Func<ChartPoint, string> value)
    {
        LabelFormatter = value;
        return this;
    }

    /// <summary>
    /// Adds the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="seriesName">The series name.</param>
    /// <param name="seriesPaint">The series paint.</param>
    /// <param name="labelsPaint">The labels paint.</param>
    /// <returns></returns>
    public GaugeBuilder AddValue(
        ObservableValue value,
        string? seriesName,
        IPaint<SkiaSharpDrawingContext>? seriesPaint,
        IPaint<SkiaSharpDrawingContext>? labelsPaint = null)
    {
        _tuples.Add(
            new Tuple<ObservableValue, string?, IPaint<SkiaSharpDrawingContext>?, IPaint<SkiaSharpDrawingContext>?>(
                value, seriesName, seriesPaint, labelsPaint));

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

        return AddValue(value, seriesName, new SolidColorPaint(seriesColor), new SolidColorPaint(labelsColor.Value));
    }

    /// <summary>
    /// Adds the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="seriesName">The series name.</param>
    /// <param name="seriesColor">Color of the series.</param>
    /// <param name="labelsColor">Color of the labels.</param>
    /// <returns></returns>
    public GaugeBuilder AddValue(double value, string seriesName, SKColor seriesColor, SKColor? labelsColor = null)
    {
        return AddValue(new ObservableValue(value), seriesName, seriesColor, labelsColor);
    }

    /// <summary>
    /// Adds the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public GaugeBuilder AddValue(ObservableValue value)
    {
        return AddValue(value, null, null, null);
    }

    /// <summary>
    /// Adds the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public GaugeBuilder AddValue(double value)
    {
        return AddValue(new ObservableValue(value));
    }

    /// <summary>
    /// Adds the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="seriesName">The series name.</param>
    /// <returns></returns>
    public GaugeBuilder AddValue(ObservableValue value, string? seriesName)
    {
        return AddValue(value, seriesName, null, null);
    }

    /// <summary>
    /// Adds the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="seriesName">The series name.</param>
    /// <returns></returns>
    public GaugeBuilder AddValue(double value, string? seriesName)
    {
        return AddValue(new ObservableValue(value), seriesName);
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
                list.Add(new ObservableValue(null));
            }
            list.Insert(i, item.Item1);

            var sf = new PieSeries<ObservableValue>(true)
            {
                ZIndex = i + 1,
                Values = list,
                Name = item.Item2,
                HoverPushout = 0,
                //DataLabelsPosition = PolarLabelsPosition.ChartCenter
            };

            if (item.Item3 is not null) sf.Fill = item.Item3;
            if (item.Item4 is not null) sf.DataLabelsPaint = item.Item4;
            if (LabelFormatter is not null) sf.DataLabelsFormatter = LabelFormatter;

            var a = sf.Stroke;

            ApplyStyles(sf);
            series.Add(sf);
            _keyValuePairs.Add(sf, item);

            i++;
        }

        var fillSeriesValues = new List<ObservableValue>();
        while (fillSeriesValues.Count < _tuples.Count) fillSeriesValues.Add(new ObservableValue(0));

        var s = new PieSeries<ObservableValue>(true, true)
        {
            ZIndex = -1,
            IsFillSeries = true,
            Values = fillSeriesValues
        };
        ApplyStyles(s);
        series.Add(s);

        _builtSeries = series;

        return series;
    }

    /// <summary>
    /// Applies the styles the specified series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns></returns>
    public void ApplyStyles(PieSeries<ObservableValue> series)
    {
        if (series.SeriesProperties.HasFlag(SeriesProperties.GaugeFill))
        {
            ApplyStylesToFill(series);
            return;
        }

        ApplyStylesToSeries(series);
    }

    /// <summary>
    /// Applies the styles to fill series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns></returns>
    public void ApplyStylesToFill(PieSeries<ObservableValue> series)
    {
        if (Background is not null) series.Fill = Background;
        if (BackgroundInnerRadius is not null) series.InnerRadius = BackgroundInnerRadius.Value;
        if (BackgroundOffsetRadius is not null)
        {
            series.RelativeOuterRadius = BackgroundOffsetRadius.Value;
            series.RelativeInnerRadius = BackgroundOffsetRadius.Value;
        }
        if (BackgroundMaxRadialColumnWidth is not null) series.MaxRadialColumnWidth = BackgroundMaxRadialColumnWidth.Value;
        if (RadialAlign is not null) series.RadialAlign = RadialAlign.Value;
    }

    /// <summary>
    /// Applies the styles to series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public void ApplyStylesToSeries(PieSeries<ObservableValue> series)
    {
        if (_keyValuePairs.TryGetValue(series, out var t)) if (t.Item3 is not null) series.Fill = t.Item3;
        if (LabelsSize is not null) series.DataLabelsSize = LabelsSize.Value;
        if (LabelsPosition is not null) series.DataLabelsPosition = LabelsPosition.Value;
        if (InnerRadius is not null) series.InnerRadius = InnerRadius.Value;
        if (OffsetRadius is not null)
        {
            series.RelativeInnerRadius = OffsetRadius.Value;
            series.RelativeOuterRadius = OffsetRadius.Value;
        }
        if (MaxColumnWidth is not null) series.MaxRadialColumnWidth = MaxColumnWidth.Value;
        if (RadialAlign is not null) series.RadialAlign = RadialAlign.Value;

        series.DataLabelsFormatter = LabelFormatter;
    }

    private void OnPopertyChanged()
    {
        if (_builtSeries is null) return;

        foreach (var item in _builtSeries)
        {
            ApplyStyles(item);
        }
    }
}
