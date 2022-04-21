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

using System.Linq;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LiveChartsCore.SkiaSharpView.Uno;

/// <inheritdoc cref="IChartLegend{TDrawingContext}"/>
public sealed partial class DefaultLegend : UserControl, IChartLegend<SkiaSharpDrawingContext>
{
    private readonly DataTemplate _defaultTempalte;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultLegend"/> class.
    /// </summary>
    public DefaultLegend()
    {
        InitializeComponent();
        _defaultTempalte = (DataTemplate)Resources["defaultTemplate"];
    }

    /// <summary>
    /// The actual tempalte property.
    /// </summary>
    public static readonly DependencyProperty ActualTemplateProperty =
        DependencyProperty.Register(nameof(ActualTemplate), typeof(DataTemplate), typeof(DefaultLegend), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the actual template.
    /// </summary>
    public DataTemplate ActualTemplate
    {
        get => (DataTemplate)GetValue(ActualTemplateProperty);
        set => SetValue(ActualTemplateProperty, value);
    }

    void IChartLegend<SkiaSharpDrawingContext>.Draw(Chart<SkiaSharpDrawingContext> chart)
    {
        var uwpChart = (IUnoChart)chart.View;

        var series = chart.ChartSeries;
        var legendOrientation = chart.LegendOrientation;
        var legendPosition = chart.LegendPosition;

        var template = uwpChart.LegendTemplate ?? _defaultTempalte;
        if (ActualTemplate != template) ActualTemplate = template;

        var orientation = Orientation.Horizontal;

        switch (legendPosition)
        {
            case LegendPosition.Hidden:
                Visibility = Visibility.Collapsed;
                break;
            case LegendPosition.Top:
                Visibility = Visibility.Visible;
                if (legendOrientation == LegendOrientation.Auto) orientation = Orientation.Horizontal;
                Grid.SetColumn(uwpChart.Legend, 1);
                Grid.SetRow(uwpChart.Legend, 0);
                break;
            case LegendPosition.Left:
                Visibility = Visibility.Visible;
                if (legendOrientation == LegendOrientation.Auto) orientation = Orientation.Vertical;
                Grid.SetColumn(uwpChart.Legend, 0);
                Grid.SetRow(uwpChart.Legend, 1);
                break;
            case LegendPosition.Right:
                Visibility = Visibility.Visible;
                if (legendOrientation == LegendOrientation.Auto) orientation = Orientation.Vertical;
                Grid.SetColumn(uwpChart.Legend, 2);
                Grid.SetRow(uwpChart.Legend, 1);
                break;
            case LegendPosition.Bottom:
                Visibility = Visibility.Visible;
                if (legendOrientation == LegendOrientation.Auto) orientation = Orientation.Horizontal;
                Grid.SetColumn(uwpChart.Legend, 1);
                Grid.SetRow(uwpChart.Legend, 2);
                break;
            default:
                break;
        }

        if (legendOrientation != LegendOrientation.Auto)
            orientation = legendOrientation == LegendOrientation.Horizontal
                ? Orientation.Horizontal
                : Orientation.Vertical;

        DataContext = new LegendBindingContext
        {
            Background = uwpChart.LegendBackground,
            Orientation = orientation,
            SeriesCollection = series.Select(x =>
                new BindingSeries
                {
                    Series = x,
                    FontFamily = uwpChart.LegendFontFamily,
                    Foreground = uwpChart.LegendTextBrush,
                    FontSize = uwpChart.LegendFontSize,
                    FontWeight = uwpChart.LegendFontWeight,
                    FontStyle = uwpChart.LegendFontStyle,
                    FontStretch = uwpChart.LegendFontStretch
                })
        };

        UpdateLayout();
    }
}
