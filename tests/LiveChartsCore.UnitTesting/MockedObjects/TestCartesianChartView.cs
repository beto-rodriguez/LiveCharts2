﻿// The MIT License(MIT)
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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.UnitTesting.MockedObjects;

public class TestCartesianChartView : ICartesianChartView<SkiaSharpDrawingContext>
{
    public TestCartesianChartView()
    {
        if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

        var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
        var initializer = stylesBuilder.GetVisualsInitializer();
        if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
            throw new Exception("Default colors are not valid");
        initializer.ApplyStyleToChart(this);

        Core = new CartesianChart<SkiaSharpDrawingContext>(
            this, LiveChartsSkiaSharp.DefaultPlatformBuilder, CoreCanvas, new RectangleGeometry());
    }

    public bool DesignerMode => false;

    public bool IsInVisualTree => true;

    IChart IChartView.CoreChart => Core;

    public CartesianChart<SkiaSharpDrawingContext> Core { get; }

    public IEnumerable<ICartesianAxis> XAxes { get; set; }

    public IEnumerable<ICartesianAxis> YAxes { get; set; }

    public IEnumerable<ISeries> Series { get; set; }

    public ZoomAndPanMode ZoomMode { get; set; }

    public double ZoomingSpeed { get; set; }

    public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas { get; } = new();

    public IChartLegend<SkiaSharpDrawingContext> Legend => null;

    public IChartTooltip<SkiaSharpDrawingContext> Tooltip => null;

    public LvcSize ControlSize => new(100, 100);

    public Margin DrawMargin { get; set; }

    public TimeSpan AnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(1);

    public Func<float, float> EasingFunction { get; set; } = EasingFunctions.Lineal;

    public LegendPosition LegendPosition { get; set; }

    public LegendOrientation LegendOrientation { get; set; }

    public TooltipPosition TooltipPosition { get; set; }

    public TooltipFindingStrategy TooltipFindingStrategy { get; set; }

    public LvcColor BackColor { get; set; }
    public bool AutoUpdateEnabled { get; set; } = true;
    public TimeSpan UpdaterThrottler { get; set; }
    public DrawMarginFrame<SkiaSharpDrawingContext> DrawMarginFrame { get; set; }
    public IEnumerable<Section<SkiaSharpDrawingContext>> Sections { get; set; }
    public object SyncContext { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements { get; set; }
    public VisualElement<SkiaSharpDrawingContext> Title { get; set; }

    public event ChartEventHandler<SkiaSharpDrawingContext> Measuring;
    public event ChartEventHandler<SkiaSharpDrawingContext> UpdateStarted;
    public event ChartEventHandler<SkiaSharpDrawingContext> UpdateFinished;
    public event ChartPointsHandler DataPointerDown;
    public event ChartPointHandler ChartPointPointerDown;

    public void DummyRaiseEvents()
    {
        Measuring?.Invoke(this);
        UpdateStarted?.Invoke(this);
        UpdateFinished?.Invoke(this);
    }

    public void HideTooltip() { }

    public double[] ScaleUIPoint(LvcPoint point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        return new double[2];
    }

    public void ShowTooltip(IEnumerable<ChartPoint> points) { }

    public void SetTooltipStyle(LvcColor background, LvcColor textColor) { }

    public void InvokeOnUIThread(Action action)
    {
        action();
    }

    public void SyncAction(Action action)
    {
        action();
    }

    public void OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint ponter)
    {
        throw new NotImplementedException();
    }

    public void Invalidate()
    {
        throw new NotImplementedException();
    }
}
