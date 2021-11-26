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
using System.IO;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.SKCharts
{

    /// <summary>
    /// In-memory chart that is able to generate a chart images.
    /// </summary>
    public class SKCartesianChart : ICartesianChartView<SkiaSharpDrawingContext>, ISkiaSharpChart
    {
        private LvcColor _backColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SKCartesianChart"/> class.
        /// </summary>
        public SKCartesianChart()
        {
            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetVisualsInitializer();
            if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ApplyStyleToChart(this);

            Core = new CartesianChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, CoreCanvas);
            Core.Measuring += OnCoreMeasuring;
            Core.UpdateStarted += OnCoreUpdateStarted;
            Core.UpdateFinished += OnCoreUpdateFinished;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SKCartesianChart"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        public SKCartesianChart(ICartesianChartView<SkiaSharpDrawingContext> view) : this()
        {
            XAxes = view.XAxes;
            YAxes = view.YAxes;
            Series = view.Series;
            Sections = view.Sections;
            DrawMarginFrame = view.DrawMarginFrame;
        }


        /// <inheritdoc cref="IChartView.DesignerMode" />
        public bool DesignerMode => false;

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public SKColor Background { get; set; } = SKColors.White;

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height { get; set; } = 600;

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width { get; set; } = 900;

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Core"/>
        public CartesianChart<SkiaSharpDrawingContext> Core { get; }

        /// <inheritdoc cref="IChartView.SyncContext"/>
        public object SyncContext { get => CoreCanvas.Sync; set => CoreCanvas.Sync = value; }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.XAxes"/>
        public IEnumerable<ICartesianAxis> XAxes { get; set; } = new Axis[] { new Axis() };

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.YAxes"/>
        public IEnumerable<ICartesianAxis> YAxes { get; set; } = new Axis[] { new Axis() };

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Sections"/>
        public IEnumerable<Section<SkiaSharpDrawingContext>> Sections { get; set; } = new RectangularSection[0];

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Series"/>
        public IEnumerable<ISeries> Series { get; set; } = new ISeries[0];

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.DrawMarginFrame"/>
        public DrawMarginFrame<SkiaSharpDrawingContext>? DrawMarginFrame { get; set; }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ZoomMode"/>
        public ZoomAndPanMode ZoomMode { get; set; }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ZoomingSpeed"/>
        public double ZoomingSpeed { get; set; }

        /// <inheritdoc cref="IChartView{TDrawingContext}.AutoUpdateEnabled"/>
        public bool AutoUpdateEnabled { get; set; }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.TooltipFindingStrategy"/>
        public TooltipFindingStrategy TooltipFindingStrategy { get; set; }

        /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas"/>
        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas { get; } = new();

        /// <inheritdoc cref="IChartView{TDrawingContext}.Legend"/>
        public IChartLegend<SkiaSharpDrawingContext>? Legend => null;

        /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip"/>
        public IChartTooltip<SkiaSharpDrawingContext>? Tooltip => null;

        /// <summary>
        /// Gets or sets the point states.
        /// </summary>
        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; } = new();

        /// <inheritdoc cref="IChartView.CoreChart"/>
        public IChart CoreChart => Core;

        LvcColor IChartView.BackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                Background = new SKColor(_backColor.R, _backColor.G, _backColor.B, _backColor.A);
            }
        }

        LvcSize IChartView.ControlSize => new(Width, Height);

        /// <inheritdoc cref="IChartView.DrawMargin"/>
        public Margin? DrawMargin { get; set; }

        /// <inheritdoc cref="IChartView.AnimationsSpeed"/>
        public TimeSpan AnimationsSpeed { get; set; }

        /// <inheritdoc cref="IChartView.EasingFunction"/>
        public Func<float, float>? EasingFunction { get; set; } = null;

        /// <inheritdoc cref="IChartView.UpdaterThrottler"/>
        public TimeSpan UpdaterThrottler { get; set; }

        /// <inheritdoc cref="IChartView.LegendPosition"/>
        public LegendPosition LegendPosition { get; set; }

        /// <inheritdoc cref="IChartView.LegendOrientation"/>
        public LegendOrientation LegendOrientation { get; set; }

        /// <inheritdoc cref="IChartView.TooltipPosition"/>
        public TooltipPosition TooltipPosition { get; set; }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? Measuring;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateStarted;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateFinished;

        /// <inheritdoc cref="IChartView{TDrawingContext}.HideTooltip"/>
        public void HideTooltip()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ScaleUIPoint(LvcPoint, int, int)"/>
        public double[] ScaleUIPoint(LvcPoint point, int xAxisIndex = 0, int yAxisIndex = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IChartView.SetTooltipStyle(LvcColor, LvcColor)"/>
        public void SetTooltipStyle(LvcColor background, LvcColor textColor) { }

        /// <inheritdoc cref="IChartView{TDrawingContext}.ShowTooltip(IEnumerable{ChartPoint})"/>
        public void ShowTooltip(IEnumerable<ChartPoint> points)
        {
            throw new NotImplementedException();
        }

        void IChartView.InvokeOnUIThread(Action action)
        {
            action();
        }

        /// <inheritdoc cref="IChartView.SyncAction(Action)"/>
        public void SyncAction(Action action)
        {
            lock (CoreCanvas.Sync)
            {
                action();
            }
        }

        /// <inheritdoc cref="ISkiaSharpChart.GetImage"/>
        public SKImage GetImage()
        {
            CoreCanvas.DisableAnimations = true;

            using var surface = SKSurface.Create(new SKImageInfo(Width, Height));

            var canvas = surface.Canvas;
            using var clearColor = new SKPaint { Color = Background };
            canvas.DrawRect(0, 0, Width, Height, clearColor);

            Core.Load();
            Core.Update(new ChartUpdateParams { Throttling = false, IsAutomaticUpdate = false });

            CoreCanvas.DrawFrame(
                new SkiaSharpDrawingContext(
                    CoreCanvas,
                    new SKImageInfo(Height, Width),
                    surface,
                    canvas)
                {
                    ClearColor = Background
                });

            return surface.Snapshot();
        }

        /// <inheritdoc cref="ISkiaSharpChart.SaveImage(string, SKEncodedImageFormat, int)"/>
        public void SaveImage(string path, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 80)
        {
            using var image = GetImage();
            using var data = image.Encode(format, quality);
            using var stream = File.OpenWrite(path);
            data.SaveTo(stream);
        }

        private void OnCoreUpdateFinished(IChartView<SkiaSharpDrawingContext> chart)
        {
            UpdateFinished?.Invoke(this);
        }

        private void OnCoreUpdateStarted(IChartView<SkiaSharpDrawingContext> chart)
        {
            UpdateStarted?.Invoke(this);
        }

        private void OnCoreMeasuring(IChartView<SkiaSharpDrawingContext> chart)
        {
            Measuring?.Invoke(this);
        }
    }
}
