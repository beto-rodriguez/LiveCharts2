
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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Uno.Binding;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LiveChartsCore.SkiaSharpView.Uno;

/// <inheritdoc cref="IChartTooltip{TDrawingContext}"/>
public sealed partial class DefaultTooltip : UserControl, IChartTooltip<SkiaSharpDrawingContext>
{
    private readonly DataTemplate _defaultTempalte;
    private IUnoChart? _unoChart;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultTooltip"/> class.
    /// </summary>
    public DefaultTooltip()
    {
        InitializeComponent();
        _defaultTempalte = (DataTemplate)Resources["defaultTemplate"];
    }

    /// <summary>
    /// The actual tempalte property.
    /// </summary>
    public static readonly DependencyProperty ActualTemplateProperty =
        DependencyProperty.Register(nameof(ActualTemplate), typeof(DataTemplate), typeof(DefaultTooltip), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the actual template.
    /// </summary>
    public DataTemplate? ActualTemplate
    {
        get => (DataTemplate?)GetValue(ActualTemplateProperty);
        set => SetValue(ActualTemplateProperty, value);
    }

    void IChartTooltip<SkiaSharpDrawingContext>.Show(IEnumerable<ChartPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
    {
        var unoChart = (IUnoChart)chart.View;
        _unoChart = unoChart;

        var template = unoChart.TooltipTemplate ?? _defaultTempalte;
        if (ActualTemplate != template) ActualTemplate = template;

        LvcPoint? location = null;
        Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

        DataContext = new TooltipBindingContext
        {
            Background = unoChart.TooltipBackground,
            Points = tooltipPoints.Select(x =>
                new BindingPoint
                {
                    ChartPoint = new BindableChartPoint(x),
                    FontFamily = unoChart.TooltipFontFamily,
                    Foreground = unoChart.TooltipTextBrush,
                    FontSize = unoChart.TooltipFontSize,
                    FontWeight = unoChart.TooltipFontWeight,
                    FontStyle = unoChart.TooltipFontStyle,
                    FontStretch = unoChart.TooltipFontStretch
                }).ToArray()
        };

        if (chart is CartesianChart<SkiaSharpDrawingContext> or PolarChart<SkiaSharpDrawingContext>)
        {
            location = tooltipPoints.GetCartesianTooltipLocation(
                chart.TooltipPosition, new LvcSize((float)DesiredSize.Width, (float)DesiredSize.Height), chart.ControlSize);
        }
        if (chart is PieChart<SkiaSharpDrawingContext>)
        {
            location = tooltipPoints.GetPieTooltipLocation(
                chart.TooltipPosition, new LvcSize((float)DesiredSize.Width, (float)DesiredSize.Height));
        }

        if (location is null) throw new Exception("location not supported");

        var canvasPosition = unoChart.GetCanvasPosition();

        var x = location.Value.X + canvasPosition.X;
        var y = location.Value.Y + canvasPosition.Y;
        var s = chart.ControlSize;
        var w = s.Width;
        var h = s.Height;
        if (location.Value.X + DesiredSize.Width > w) x = w - DesiredSize.Width;
        if (location.Value.X < 0) x = 0;
        if (location.Value.Y < 0) y = 0;
        if (location.Value.Y + DesiredSize.Height > h) x = h - DesiredSize.Height;

        Canvas.SetLeft(unoChart.TooltipElement, x);
        Canvas.SetTop(unoChart.TooltipElement, y);
    }

    void IChartTooltip<SkiaSharpDrawingContext>.Hide()
    {
        if (_unoChart is null) return;

        Canvas.SetLeft(_unoChart.TooltipElement, -1000);
        Canvas.SetTop(_unoChart.TooltipElement, -1000);
    }
}
