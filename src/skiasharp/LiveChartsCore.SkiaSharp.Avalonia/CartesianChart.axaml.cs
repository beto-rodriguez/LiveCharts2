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
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Observers;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

/// <inheritdoc cref="ICartesianChartView" />
public partial class CartesianChart : ChartControl, ICartesianChartView
{
    private float _previousPinchScale = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartesianChart"/> class.
    /// </summary>
    /// <exception cref="Exception">Default colors are not valid</exception>
    public CartesianChart()
    {
        AvaloniaXamlLoader.Load(this);

        _ = Observe
           .Collection(nameof(Series))
           .Collection(nameof(XAxes))
           .Collection(nameof(YAxes))
           .Collection(nameof(Sections))
           .Collection(nameof(VisualElements))
           .Property(nameof(Title))
           .Property(nameof(DrawMarginFrame));

        Observe.Add(
            nameof(SeriesSource),
            new SeriesSourceObserver(
                InflateSeriesTemplate,
                GetSeriesSource,
                () => SeriesSource is not null && SeriesTemplate is not null));

        XAxes = new ObservableCollection<ICartesianAxis>();
        YAxes = new ObservableCollection<ICartesianAxis>();
        Series = new ObservableCollection<ISeries>();
        Sections = new ObservableCollection<IChartElement>();
        VisualElements = new ObservableCollection<IChartElement>();
        SyncContext = new object();

        var pinchGesture = new PinchGestureRecognizer();
        GestureRecognizers.Add(pinchGesture);
        AddHandler(Gestures.PinchEvent, OnPinched);

        PointerWheelChanged += OnPointerWheelChanged;
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
    {
        if (CoreChart is not CartesianChartEngine cc) throw new Exception("core not found");
        var xScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.XAxes[xAxisIndex]);
        var yScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.YAxes[yAxisIndex]);

        return new LvcPointD { X = xScaler.ToChartValues(point.X), Y = yScaler.ToChartValues(point.Y) };
    }

    /// <inheritdoc cref="ICartesianChartView.ScaleDataToPixels(LvcPointD, int, int)"/>
    public LvcPointD ScaleDataToPixels(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        if (CoreChart is not CartesianChartEngine cc) throw new Exception("core not found");

        var xScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.XAxes[xAxisIndex]);
        var yScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.YAxes[yAxisIndex]);

        return new LvcPointD { X = xScaler.ToPixels(point.X), Y = yScaler.ToPixels(point.Y) };
    }

    /// <inheritdoc cref="ChartControl.CreateCoreChart"/>
    protected override Chart CreateCoreChart() =>
        new CartesianChartEngine(this, config => config.UseDefaults(), CanvasView.CanvasCore);

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        OnXamlPropertyChanged(change);
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        e.Handled = true;

        var c = (CartesianChartEngine)CoreChart;
        var p = e.GetPosition(this);

        c.Zoom(new LvcPoint((float)p.X, (float)p.Y), e.Delta.Y > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
    }

    private void OnPinched(object? sender, PinchEventArgs e)
    {
        var c = (CartesianChartEngine)CoreChart;
        var p = e.ScaleOrigin;
        var s = c.ControlSize;
        var pivot = new LvcPoint((float)(p.X * s.Width), (float)(p.Y * s.Height));

        var scale = (float)e.Scale;
        var delta = _previousPinchScale - scale;
        if (Math.Abs(delta) > 0.05) delta = 0; // ignore the first call.
        _previousPinchScale = scale;

        c.Zoom(pivot, ZoomDirection.DefinedByScaleFactor, 1 - delta, true);
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
