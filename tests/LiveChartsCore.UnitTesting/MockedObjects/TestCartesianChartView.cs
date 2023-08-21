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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.UnitTesting.MockedObjects;

[Obsolete($"We now support in-memory charts, please use {nameof(SKCartesianChart)} instead.")]
public class TestCartesianChartView : ICartesianChartView<SkiaSharpDrawingContext>
{
    public TestCartesianChartView()
    {
        LiveCharts.Configure(config => config.UseDefaults());

        Core = new CartesianChart<SkiaSharpDrawingContext>(
            this, config => config.UseDefaults(), CoreCanvas, new RectangleGeometry());
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
    public object SyncContext { get; set; }
    public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements { get; set; }
    public VisualElement<SkiaSharpDrawingContext> Title { get; set; }
    public IPaint<SkiaSharpDrawingContext> LegendTextPaint { get; set; }
    public IPaint<SkiaSharpDrawingContext> LegendBackgroundPaint { get; set; }
    public double? LegendTextSize { get; set; }
    public IPaint<SkiaSharpDrawingContext> TooltipTextPaint { get; set; }
    public IPaint<SkiaSharpDrawingContext> TooltipBackgroundPaint { get; set; }
    public double? TooltipTextSize { get; set; }
    IChartLegend<SkiaSharpDrawingContext> IChartView<SkiaSharpDrawingContext>.Legend { get; set; }
    IChartTooltip<SkiaSharpDrawingContext> IChartView<SkiaSharpDrawingContext>.Tooltip { get; set; }

    public event ChartEventHandler<SkiaSharpDrawingContext> Measuring;
    public event ChartEventHandler<SkiaSharpDrawingContext> UpdateStarted;
    public event ChartEventHandler<SkiaSharpDrawingContext> UpdateFinished;
    public event ChartPointsHandler DataPointerDown;
    public event ChartPointHandler ChartPointPointerDown;
    public event VisualElementsHandler<SkiaSharpDrawingContext> VisualElementsPointerDown;

    public void DummyRaiseEvents()
    {
        Measuring?.Invoke(this);
        UpdateStarted?.Invoke(this);
        UpdateFinished?.Invoke(this);
    }

    public double[] ScaleUIPoint(LvcPoint point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        return new double[2];
    }

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

    public void OnVisualElementPointerDown(IEnumerable<VisualElement<SkiaSharpDrawingContext>> visualElements, LvcPoint pointer)
    {
        throw new NotImplementedException();
    }

    public LvcPointD ScalePixelsToData(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        throw new NotImplementedException();
    }

    public LvcPointD ScaleDataToPixels(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ChartPoint> GetPointsAt(LvcPoint point, TooltipFindingStrategy strategy = TooltipFindingStrategy.Automatic)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<VisualElement<SkiaSharpDrawingContext>> GetVisualsAt(LvcPoint point)
    {
        throw new NotImplementedException();
    }
}
