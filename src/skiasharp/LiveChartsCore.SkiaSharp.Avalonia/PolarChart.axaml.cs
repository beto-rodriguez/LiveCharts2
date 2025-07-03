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
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Markup.Xaml;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Observers;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

/// <inheritdoc cref="IPolarChartView" />
public partial class PolarChart : ChartControl, IPolarChartView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PolarChart"/> class.
    /// </summary>
    /// <exception cref="Exception">Default colors are not valid</exception>
    public PolarChart()
    {
        AvaloniaXamlLoader.Load(this);

        _ = Observe
            .Collection(nameof(Series))
            .Collection(nameof(RadiusAxes))
            .Collection(nameof(AngleAxes))
            .Collection(nameof(VisualElements))
            .Property(nameof(Title));

        Observe.Add(
            nameof(SeriesSource),
            new SeriesSourceObserver(
                InflateSeriesTemplate,
                GetSeriesSource,
                () => SeriesSource is not null && SeriesTemplate is not null));

        AngleAxes = new ObservableCollection<IPolarAxis>();
        RadiusAxes = new ObservableCollection<IPolarAxis>();
        Series = new ObservableCollection<ISeries>();
        VisualElements = new ObservableCollection<IChartElement>();
        SyncContext = new object();
    }

    PolarChartEngine IPolarChartView.Core => (PolarChartEngine)CoreChart;

    /// <inheritdoc cref="IPolarChartView.ScalePixelsToData(LvcPointD, int, int)"/>
    public LvcPointD ScalePixelsToData(LvcPointD point, int angleAxisIndex = 0, int radiusAxisIndex = 0)
    {
        if (CoreChart is not PolarChartEngine cc) throw new Exception("core not found");

        var scaler = new PolarScaler(
            cc.DrawMarginLocation, cc.DrawMarginSize, cc.AngleAxes[angleAxisIndex], cc.RadiusAxes[radiusAxisIndex],
            cc.InnerRadius, cc.InitialRotation, cc.TotalAnge);

        return scaler.ToChartValues(point.X, point.Y);
    }

    /// <inheritdoc cref="IPolarChartView.ScaleDataToPixels(LvcPointD, int, int)"/>
    public LvcPointD ScaleDataToPixels(LvcPointD point, int angleAxisIndex = 0, int radiusAxisIndex = 0)
    {
        if (CoreChart is not PolarChartEngine cc) throw new Exception("core not found");

        var scaler = new PolarScaler(
            cc.DrawMarginLocation, cc.DrawMarginSize, cc.AngleAxes[angleAxisIndex], cc.RadiusAxes[radiusAxisIndex],
            cc.InnerRadius, cc.InitialRotation, cc.TotalAnge);

        var r = scaler.ToPixels(point.X, point.Y);

        return new LvcPointD { X = (float)r.X, Y = (float)r.Y };
    }

    /// <inheritdoc cref="ChartControl.CreateCoreChart"/>
    protected override Chart CreateCoreChart() =>
        new PolarChartEngine(this, config => config.UseDefaults(), CanvasView.CanvasCore);

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        OnXamlPropertyChanged(change);
    }

    private ISeries InflateSeriesTemplate(object item)
    {
        var control = SeriesTemplate.Build(item);

        if (control is not ISeries series)
            throw new InvalidOperationException("The template must be a valid series.");

        control.DataContext = item;

        return series;
    }

    private static object GetSeriesSource(ISeries series) =>
        ((StyledElement)series).DataContext!;
}
