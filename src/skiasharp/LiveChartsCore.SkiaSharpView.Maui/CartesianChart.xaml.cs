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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using Microsoft.Maui.Controls.Xaml;

namespace LiveChartsCore.SkiaSharpView.Maui;

/// <inheritdoc cref="ICartesianChartView"/>
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CartesianChart : ChartControl, ICartesianChartView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CartesianChart"/> class.
    /// </summary>
    /// <exception cref="Exception">Default colors are not valid</exception>
    public CartesianChart()
    {
        InitializeComponent();

        _ = Observe
            .Collection(nameof(XAxes))
            .Collection(nameof(YAxes))
            .Collection(nameof(Sections))
            .Property(nameof(DrawMarginFrame));

        SetValue(XAxesProperty, new ObservableCollection<ICartesianAxis>());
        SetValue(YAxesProperty, new ObservableCollection<ICartesianAxis>());
        SetValue(SeriesProperty, new ObservableCollection<ISeries>());
        SetValue(SectionsProperty, new ObservableCollection<IChartElement>());
        SetValue(VisualElementsProperty, new ObservableCollection<IChartElement>());
        SetValue(SyncContextProperty, new object());
    }

    CartesianChartEngine ICartesianChartView.Core => (CartesianChartEngine)CoreChart;

    /// <inheritdoc cref="ICartesianChartView.MatchAxesScreenDataRatio" />
    public bool MatchAxesScreenDataRatio
    {
        get;
        set
        {
            field = value;

            if (value) SharedAxes.MatchAxesScreenDataRatio(this);
            else SharedAxes.DisposeMatchAxesScreenDataRatio(this);
        }
    }

    /// <inheritdoc cref="ICartesianChartView.ScalePixelsToData(LvcPointD, int, int)"/>
    public LvcPointD ScalePixelsToData(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
        => ((CartesianChartEngine)CoreChart).ScalePixelsToData(point, xAxisIndex, yAxisIndex);

    /// <inheritdoc cref="ICartesianChartView.ScaleDataToPixels(LvcPointD, int, int)"/>
    public LvcPointD ScaleDataToPixels(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
        => ((CartesianChartEngine)CoreChart).ScaleDataToPixels(point, xAxisIndex, yAxisIndex);

    /// <inheritdoc cref="ChartControl.CreateCoreChart"/>
    protected override Chart CreateCoreChart() =>
        new CartesianChartEngine(this, config => config.UseDefaults(), CanvasView.CanvasCore);

    internal override void OnScrolled(object? sender, Behaviours.Events.ScrollEventArgs args)
    {
        var c = (CartesianChartEngine)CoreChart;
        c.Zoom(args.Location, args.ScrollDelta > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
    }

    internal override void OnPinched(object? sender, Behaviours.Events.PinchEventArgs args)
    {
        var c = (CartesianChartEngine)CoreChart;
        var p = args.PinchStart;
        var s = c.ControlSize;
        var pivot = new LvcPoint((float)(p.X * s.Width), (float)(p.Y * s.Height));
        c.Zoom(pivot, ZoomDirection.DefinedByScaleFactor, args.Scale, true);
    }
}
