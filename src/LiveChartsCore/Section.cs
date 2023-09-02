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

using System.ComponentModel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;

namespace LiveChartsCore;

/// <summary>
/// Defines a visual section in a chart.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="ChartElement{TDrawingContext}" />
public abstract class Section<TDrawingContext> : ChartElement<TDrawingContext>, INotifyPropertyChanged
    where TDrawingContext : DrawingContext
{
    private IPaint<TDrawingContext>? _stroke = null;
    private IPaint<TDrawingContext>? _fill = null;
    private IPaint<TDrawingContext>? _labelPaint = null;
    private double? _xi;
    private double? _xj;
    private double? _yi;
    private double? _yj;
    private int _scalesXAt;
    private int _scalesYAt;
    private int? _zIndex;
    private bool _isVisible = true;

    /// <summary>
    /// Gets or sets the stroke.
    /// </summary>
    /// <value>
    /// The stroke.
    /// </value>
    public IPaint<TDrawingContext>? Stroke
    {
        get => _stroke;
        set => SetPaintProperty(ref _stroke, value, true);
    }

    /// <summary>
    /// Gets or sets the fill.
    /// </summary>
    /// <value>
    /// The fill.
    /// </value>
    public IPaint<TDrawingContext>? Fill
    {
        get => _fill;
        set => SetPaintProperty(ref _fill, value);
    }

    /// <summary>
    /// Gets or sets the label paint.
    /// </summary>
    /// <value>
    /// The fill.
    /// </value>
    public IPaint<TDrawingContext>? LabelPaint
    {
        get => _labelPaint;
        set => SetPaintProperty(ref _labelPaint, value);
    }

    /// <summary>
    /// Gets or sets whether the section is visible or not.
    /// </summary>
    public bool IsVisible { get => _isVisible; set => SetProperty(ref _isVisible, value); }

    /// <summary>
    /// Gets or sets the xi, the value where the section starts at the X axis,
    /// set the property to null to indicate that the section must start at the beginning of the X axis, default is null.
    /// </summary>
    /// <value>
    /// The xi.
    /// </value>
    public double? Xi { get => _xi; set => SetProperty(ref _xi, value); }

    /// <summary>
    /// Gets or sets the xj, the value where the section ends and the X axis.
    /// set the property to null to indicate that the section must go to the end of the X axis, default is null.
    /// </summary>
    /// <value>
    /// The xj.
    /// </value>
    public double? Xj { get => _xj; set => SetProperty(ref _xj, value); }

    /// <summary>
    /// Gets or sets the yi, the value where the section starts and the Y axis.
    /// set the property to null to indicate that the section must start at the beginning of the Y axis, default is null.
    /// </summary>
    /// <value>
    /// The yi.
    /// </value>
    public double? Yi { get => _yi; set => SetProperty(ref _yi, value); }

    /// <summary>
    /// Gets or sets the yj, the value where the section ends and the Y axis.
    /// set the property to null to indicate that the section must go to the end of the Y axis, default is null.
    /// </summary>
    /// <value>
    /// The yj.
    /// </value>
    public double? Yj { get => _yj; set => SetProperty(ref _yj, value); }

    /// <summary>
    /// Gets or sets the axis index where the section is scaled in the X plane, the index must exist 
    /// in the <see cref="ICartesianChartView{TDrawingContext}.XAxes"/> collection.
    /// </summary>
    /// <value>
    /// The index of the axis.
    /// </value>
    public int ScalesXAt { get => _scalesXAt; set => SetProperty(ref _scalesXAt, value); }

    /// <summary>
    /// Gets or sets the axis index where the section is scaled in the Y plane, the index must exist 
    /// in the <see cref="ICartesianChartView{TDrawingContext}.YAxes"/> collection.
    /// </summary>
    /// <value>
    /// The index of the axis.
    /// </value>
    public int ScalesYAt { get => _scalesYAt; set => SetProperty(ref _scalesYAt, value); }

    /// <summary>
    /// Gets or sets the index of the z axis.
    /// </summary>
    /// <value>
    /// The index of the z.
    /// </value>
    public int? ZIndex { get => _zIndex; set => SetProperty(ref _zIndex, value); }

    /// <summary>
    /// Gets the paint tasks.
    /// </summary>
    /// <returns></returns>
    internal override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return new[] { _stroke, _fill, _labelPaint };
    }

    /// <summary>
    /// Called when the fill changes.
    /// </summary>
    /// <param name="propertyName"></param>
    protected override void OnPaintChanged(string? propertyName)
    {
        base.OnPaintChanged(propertyName);
        OnPropertyChanged(propertyName);
    }
}

/// <summary>
/// Defines a visual section in a chart.
/// </summary>
/// <typeparam name="TSizedGeometry">The type of the sized geometry.</typeparam>
/// <typeparam name="TLabelGeometry">The type of the label geometry.</typeparam>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="ChartElement{TDrawingContext}" />
public abstract class Section<TSizedGeometry, TLabelGeometry, TDrawingContext> : Section<TDrawingContext>
    where TDrawingContext : DrawingContext
    where TSizedGeometry : ISizedGeometry<TDrawingContext>, new()
    where TLabelGeometry : ILabelGeometry<TDrawingContext>, new()
{
    private string _label = string.Empty;
    private float _labelSize = 12;

    /// <summary>
    /// The fill sized geometry
    /// </summary>
    protected internal TSizedGeometry? _fillSizedGeometry;

    /// <summary>
    /// The stroke sized geometry
    /// </summary>
    protected internal TSizedGeometry? _strokeSizedGeometry;

    /// <summary>
    /// The label geometry.
    /// </summary>
    protected internal TLabelGeometry? _labelGeometry;

    /// <summary>
    /// Gets or sets the label, a string to be displayed within the section.
    /// </summary>
    public string Label { get => _label; set => SetProperty(ref _label, value); }

    /// <summary>
    /// Gets or sets the label size.
    /// </summary>
    public double LabelSize { get => _labelSize; set => SetProperty(ref _labelSize, (float)value); }

    /// <summary>
    /// Measures the specified chart.
    /// </summary>
    /// <param name="chart">The chart.</param>
    public override void Invalidate(Chart<TDrawingContext> chart)
    {
        var drawLocation = chart.DrawMarginLocation;
        var drawMarginSize = chart.DrawMarginSize;

        var cartesianChart = (CartesianChart<TDrawingContext>)chart;
        var primaryAxis = cartesianChart.YAxes[ScalesYAt];
        var secondaryAxis = cartesianChart.XAxes[ScalesXAt];

        var secondaryScale = new Scaler(drawLocation, drawMarginSize, secondaryAxis);
        var primaryScale = new Scaler(drawLocation, drawMarginSize, primaryAxis);

        var xi = Xi is null ? drawLocation.X : secondaryScale.ToPixels(Xi.Value);
        var xj = Xj is null ? drawLocation.X + drawMarginSize.Width : secondaryScale.ToPixels(Xj.Value);

        var yi = Yi is null ? drawLocation.Y : primaryScale.ToPixels(Yi.Value);
        var yj = Yj is null ? drawLocation.Y + drawMarginSize.Height : primaryScale.ToPixels(Yj.Value);

        if (Fill is not null)
        {
            Fill.ZIndex = ZIndex ?? -2.5;

            if (_fillSizedGeometry is null)
            {
                _fillSizedGeometry = new TSizedGeometry
                {
                    X = xi,
                    Y = yi,
                    Width = xj - xi,
                    Height = yj - yi
                };

                _fillSizedGeometry.Animate(cartesianChart.EasingFunction, cartesianChart.AnimationsSpeed);
            }

            _fillSizedGeometry.X = xi;
            _fillSizedGeometry.Y = yi;
            _fillSizedGeometry.Width = xj - xi;
            _fillSizedGeometry.Height = yj - yi;

            Fill.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            Fill.AddGeometryToPaintTask(chart.Canvas, _fillSizedGeometry);
            chart.Canvas.AddDrawableTask(Fill);
        }

        if (Stroke is not null)
        {
            Stroke.ZIndex = ZIndex ?? 0;

            if (_strokeSizedGeometry is null)
            {
                _strokeSizedGeometry = new TSizedGeometry
                {
                    X = xi,
                    Y = yi,
                    Width = xj - xi,
                    Height = yj - yi
                };

                _strokeSizedGeometry.Animate(chart);
            }

            _strokeSizedGeometry.X = xi;
            _strokeSizedGeometry.Y = yi;
            _strokeSizedGeometry.Width = xj - xi;
            _strokeSizedGeometry.Height = yj - yi;

            Stroke.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            Stroke.AddGeometryToPaintTask(chart.Canvas, _strokeSizedGeometry);
            chart.Canvas.AddDrawableTask(Stroke);
        }

        if (LabelPaint is not null)
        {
            LabelPaint.ZIndex = ZIndex ?? 0.01;

            var xil = Xi is null ? drawLocation.X : secondaryScale.ToPixels(Xi.Value);
            var yil = Yi is null ? drawLocation.Y : primaryScale.ToPixels(Yi.Value);

            if (_labelGeometry is null)
            {
                _labelGeometry = new TLabelGeometry
                {
                    X = xi,
                    Y = yi,
                    Padding = new Padding(6)
                };

                _labelGeometry.Animate(
                    cartesianChart.EasingFunction, cartesianChart.AnimationsSpeed,
                    nameof(_labelGeometry.X), nameof(_labelGeometry.Y));

                _labelGeometry.VerticalAlign = Align.Start;
                _labelGeometry.HorizontalAlign = Align.Start;
            }

            _labelGeometry.X = xi;
            _labelGeometry.Y = yi;
            _labelGeometry.Text = _label;
            _labelGeometry.TextSize = _labelSize;

            LabelPaint.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            LabelPaint.AddGeometryToPaintTask(chart.Canvas, _labelGeometry);
            chart.Canvas.AddDrawableTask(LabelPaint);
        }
    }
}
