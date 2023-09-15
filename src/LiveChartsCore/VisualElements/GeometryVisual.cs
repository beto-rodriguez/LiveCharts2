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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;

namespace LiveChartsCore.VisualElements;

/// <summary>
/// Defines a visual element in a chart that draws a sized geometry in the user interface.
/// </summary>
/// <typeparam name="TGeometry">The type of the geometry.</typeparam>
/// <typeparam name="TLabelGeometry">The type of the label.</typeparam>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
public class GeometryVisual<TGeometry, TLabelGeometry, TDrawingContext> : BaseGeometryVisual<TDrawingContext>
    where TDrawingContext : DrawingContext
    where TGeometry : ISizedGeometry<TDrawingContext>, new()
    where TLabelGeometry : ILabelGeometry<TDrawingContext>, new()
{
    internal TGeometry? _geometry;
    private string _label = string.Empty;
    private float _labelSize = 12;
    internal TLabelGeometry? _labelGeometry;
    private IPaint<TDrawingContext>? _labelPaint = null;

    /// <summary>
    /// Gets or sets the label, a string to be displayed within the section.
    /// </summary>
    public string Label { get => _label; set => SetProperty(ref _label, value); }

    /// <summary>
    /// Gets or sets the label size.
    /// </summary>
    public double LabelSize { get => _labelSize; set => SetProperty(ref _labelSize, (float)value); }

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

    internal override IAnimatable?[] GetDrawnGeometries()
    {
        return new IAnimatable?[] { _geometry, _labelGeometry };
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.OnInvalidated(Chart{TDrawingContext})"/>
    protected internal override void OnInvalidated(Chart<TDrawingContext> chart)
    {
        var l = GetActualCoordinate();
        var size = Measure(chart);
        var clipping = Clipping.GetClipRectangle(ClippingMode, chart);

        if (_geometry is null)
        {
            _geometry = new()
            {
                X = l.X,
                Y = l.Y,
                Width = size.Width,
                Height = size.Height
            };
            _geometry.Animate(chart);
        }

        _geometry.X = l.X;
        _geometry.Y = l.Y;
        _geometry.Width = size.Width;
        _geometry.Height = size.Height;
        _geometry.RotateTransform = (float)Rotation;
        _geometry.TranslateTransform = Translate;

        if (Fill is not null)
        {
            chart.Canvas.AddDrawableTask(Fill);
            Fill.AddGeometryToPaintTask(chart.Canvas, _geometry);
            Fill.SetClipRectangle(chart.Canvas, clipping);
        }

        if (Stroke is not null)
        {
            chart.Canvas.AddDrawableTask(Stroke);
            Stroke.AddGeometryToPaintTask(chart.Canvas, _geometry);
            Stroke.SetClipRectangle(chart.Canvas, clipping);
        }

        if (LabelPaint is not null)
        {
            const int padding = 5;

            if (_labelGeometry is null)
            {
                _labelGeometry = new TLabelGeometry
                {
                    X = l.X + padding,
                    Y = l.Y + padding,
                    Padding = new Padding(6)
                };

                _labelGeometry.Animate(
                    chart,
                    nameof(_labelGeometry.X), nameof(_labelGeometry.Y), nameof(_labelGeometry.Opacity));
                _labelGeometry.VerticalAlign = Align.Start;
                _labelGeometry.HorizontalAlign = Align.Start;
            }

            _labelGeometry.X = l.X + padding;
            _labelGeometry.Y = l.Y + padding;
            _labelGeometry.Text = _label;
            _labelGeometry.TextSize = _labelSize;
            _labelGeometry.RotateTransform = (float)Rotation;
            _labelGeometry.TranslateTransform = Translate;

            chart.Canvas.AddDrawableTask(LabelPaint);
            LabelPaint.AddGeometryToPaintTask(chart.Canvas, _labelGeometry);
            LabelPaint.SetClipRectangle(chart.Canvas, clipping);
        }
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.SetParent(IGeometry{TDrawingContext})"/>
    protected internal override void SetParent(IGeometry<TDrawingContext> parent)
    {
        if (_geometry is null) return;
        _geometry.Parent = parent;
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.Measure(Chart{TDrawingContext})"/>
    public override LvcSize Measure(Chart<TDrawingContext> chart)
    {
        var w = (float)Width;
        var h = (float)Height;

        if (SizeUnit == MeasureUnit.ChartValues)
        {
            if (PrimaryScaler is null || SecondaryScaler is null)
                throw new Exception($"You can not use {MeasureUnit.ChartValues} scale at this element.");

            w = SecondaryScaler.MeasureInPixels(w);
            h = PrimaryScaler.MeasureInPixels(h);
        }

        return new LvcSize(w, h);
    }

    internal override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return new[] { Fill, Stroke, _labelPaint };
    }
}
