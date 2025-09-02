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
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Themes;
using LiveChartsCore.Vortice.Painting;
using Vortice.Mathematics;

namespace LiveChartsCore.Vortice;

public static class LiveChartsVortice
{
    public static LiveChartsSettings AddVortice(this LiveChartsSettings settings) =>
        settings.HasProvider(new VorticeProvider());

    public static LiveChartsSettings AddVorticeDefaultTheme(this LiveChartsSettings settings) =>
        settings
            .HasTheme(theme => theme
                .OnInitialized(() =>
                {
                    theme.AnimationsSpeed = TimeSpan.FromMilliseconds(800);
                    theme.EasingFunction = EasingFunctions.ExponentialOut;
                    theme.Colors = ColorPalletes.MaterialDesign500;
                    theme.VirtualBackroundColor = new(255, 255, 255);
                })
                .HasDefaultTooltip(() => new VorticeDefaultTooltip())
                .HasDefaultLegend(() => new VorticeDefaultLegend())
                .HasRuleForAxes(axis =>
                {
                    axis.TextSize = 16;
                    axis.ShowSeparatorLines = true;
                    axis.LabelsPaint = new SolidColorPaint(new(70 / 255f, 70 / 255f, 70 / 255f));

                    Color4 lineColor = new(180 / 255f, 180 / 255f, 180 / 255f);

                    if (axis is ICartesianAxis cartesian)
                    {
                        axis.SeparatorsPaint = cartesian.Orientation == AxisOrientation.X
                            ? null
                            : new SolidColorPaint(lineColor);

                        cartesian.Padding = new Padding(12);
                    }
                    else
                    {
                        axis.SeparatorsPaint = new SolidColorPaint(lineColor);
                    }
                })
                .HasRuleForBarSeries(barSeries =>
                {
                    var themeColor = theme.GetSeriesColor(barSeries);
                    var color = new Color4(themeColor.R / 255f, themeColor.G / 255f, themeColor.B / 255f);

                    barSeries.Stroke = null;
                    barSeries.Fill = new SolidColorPaint(color);
                    barSeries.Rx = 3;
                    barSeries.Ry = 3;

                    if (barSeries.ShowDataLabels)
                        barSeries.DataLabelsPaint =
                            barSeries.DataLabelsPosition == DataLabelsPosition.Middle
                                ? theme.IsDark
                                    ? new SolidColorPaint(new(45, 45, 45))
                                    : new SolidColorPaint(new(245, 245, 245))
                                : theme.IsDark
                                    ? new SolidColorPaint(new(245, 245, 245))
                                    : new SolidColorPaint(new(45, 45, 45));

                    if (barSeries.ShowError)
                        barSeries.ErrorPaint = theme.IsDark
                            ? new SolidColorPaint(new(245, 245, 245))
                            : new SolidColorPaint(new(45, 45, 45));
                }));
}

// ToDo
public class VorticeDefaultTooltip : IChartTooltip
{
    public void Show(IEnumerable<ChartPoint> foundPoints, Chart chart)
    {
        // ...
    }

    public void Hide(Chart chart)
    {
        // ...
    }
}

// ToDo
public class VorticeDefaultLegend : IChartLegend
{
    public void Draw(Chart chart)
    {
        // ...
    }

    public void Hide(Chart chart)
    {
        // ...
    }

    public LvcSize Measure(Chart chart) => new(0, 0);
}
