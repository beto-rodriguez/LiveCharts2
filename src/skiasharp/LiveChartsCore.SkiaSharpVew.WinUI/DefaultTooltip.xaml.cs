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
using System.Diagnostics;
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace LiveChartsCore.SkiaSharpView.WinUI;

/// <inheritdoc cref="IChartTooltip{TDrawingContext}"/>
public sealed partial class DefaultTooltip : UserControl, IChartTooltip<SkiaSharpDrawingContext>
{
    private readonly DataTemplate _defaultTempalte;
    private IWinUIChart? _winuiChart;

    /// <summary>
    /// Initialices a new instance of the <see cref="DefaultTooltip"/> class.
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
    public DataTemplate ActualTemplate
    {
        get => (DataTemplate)GetValue(ActualTemplateProperty);
        set => SetValue(ActualTemplateProperty, value);
    }

    void IChartTooltip<SkiaSharpDrawingContext>.Show(IEnumerable<ChartPoint> tooltipPoints, Chart<SkiaSharpDrawingContext> chart)
    {
        var winuiChart = (IWinUIChart)chart.View;
        _winuiChart = winuiChart;

        var template = winuiChart.TooltipTemplate ?? _defaultTempalte;
        if (ActualTemplate != template) ActualTemplate = template;

        LvcPoint? location = null;
        winuiChart.TooltipControl.IsOpen = true;

        DataContext = new TooltipBindingContext
        {
            Background = winuiChart.TooltipBackground,
            Points = tooltipPoints.Select(x =>
                new BindingPoint
                {
                    ChartPoint = x,
                    FontFamily = winuiChart.TooltipFontFamily,
                    Foreground = winuiChart.TooltipTextBrush,
                    FontSize = winuiChart.TooltipFontSize,
                    FontWeight = winuiChart.TooltipFontWeight,
                    FontStyle = winuiChart.TooltipFontStyle,
                    FontStretch = winuiChart.TooltipFontStretch
                }).ToArray()
        };

        Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

        if (chart is CartesianChart<SkiaSharpDrawingContext> or PolarChart<SkiaSharpDrawingContext>)
        {
            Trace.WriteLine($"w => {DesiredSize.Width}, h => {DesiredSize.Height}");
            location = tooltipPoints.GetCartesianTooltipLocation(
                chart.TooltipPosition, new LvcSize((float)DesiredSize.Width, (float)DesiredSize.Height), chart.ControlSize);
        }
        if (chart is PieChart<SkiaSharpDrawingContext>)
        {
            location = tooltipPoints.GetPieTooltipLocation(
                chart.TooltipPosition, new LvcSize((float)DesiredSize.Width, (float)DesiredSize.Height));
        }

        if (location is null) throw new Exception("location not supported");

        // strange.... something is strange here...
        winuiChart.TooltipControl.PlacementRect =
            new Rect(location.Value.X, location.Value.Y + DesiredSize.Height, DesiredSize.Width, DesiredSize.Height);
    }

    void IChartTooltip<SkiaSharpDrawingContext>.Hide()
    {
        if (_winuiChart is null) return;
        _winuiChart.TooltipControl.IsOpen = false;
    }
}
