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
using System.Drawing;
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
    public class SKCartesianChart : ICartesianChartView<SkiaSharpDrawingContext>
    {
        private Color _backColor;

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
            PointStates = view.PointStates;
        }

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

        /// <summary>
        /// Gets the core.
        /// </summary>
        /// <value>
        /// The core.
        /// </value>
        public CartesianChart<SkiaSharpDrawingContext> Core { get; }

        /// <summary>
        /// Gets or sets the x axes.
        /// </summary>
        /// <value>
        /// The x axes.
        /// </value>
        public IEnumerable<IAxis> XAxes { get; set; } = new Axis[] { new Axis() };

        /// <summary>
        /// Gets or sets the y axes.
        /// </summary>
        /// <value>
        /// The y axes.
        /// </value>
        public IEnumerable<IAxis> YAxes { get; set; } = new Axis[] { new Axis() };

        /// <summary>
        /// Gets or sets the sections.
        /// </summary>
        /// <value>
        /// The sections.
        /// </value>
        public IEnumerable<Section<SkiaSharpDrawingContext>> Sections { get; set; } = new RectangularSection[0];

        /// <summary>
        /// Gets or sets the series to plot in the user interface.
        /// </summary>
        /// <value>
        /// The series.
        /// </value>
        public IEnumerable<ISeries> Series { get; set; } = new ISeries[0];

        /// <summary>
        /// Gets or sets the draw margin frame.
        /// </summary>
        /// <value>
        /// The draw margin frame.
        /// </value>
        public DrawMarginFrame<SkiaSharpDrawingContext>? DrawMarginFrame { get; set; }

        /// <summary>
        /// Gets or sets the zoom mode.
        /// </summary>
        /// <value>
        /// The zoom mode.
        /// </value>
        public ZoomAndPanMode ZoomMode { get; set; }

        /// <summary>
        /// Gets or sets the zooming speed.
        /// </summary>
        /// <value>
        /// The zooming speed.
        /// </value>
        public double ZoomingSpeed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [automatic update enaled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic update enaled]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoUpdateEnaled { get; set; }

        /// <summary>
        /// Gets or sets the tooltip finding strategy.
        /// </summary>
        /// <value>
        /// The tooltip finding strategy.
        /// </value>
        public TooltipFindingStrategy TooltipFindingStrategy { get; set; }

        /// <summary>
        /// Gets the core canvas.
        /// </summary>
        /// <value>
        /// The core canvas.
        /// </value>
        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas { get; } = new();

        /// <summary>
        /// Gets the legend.
        /// </summary>
        /// <value>
        /// The legend.
        /// </value>
        public IChartLegend<SkiaSharpDrawingContext>? Legend => null;

        /// <summary>
        /// Gets the tooltip.
        /// </summary>
        /// <value>
        /// The tooltip.
        /// </value>
        public IChartTooltip<SkiaSharpDrawingContext>? Tooltip => null;

        /// <summary>
        /// Gets or sets the point states.
        /// </summary>
        /// <value>
        /// The point states.
        /// </value>
        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; } = new();

        /// <summary>
        /// Gets the core chart.
        /// </summary>
        /// <value>
        /// The core chart.
        /// </value>
        public IChart CoreChart => Core;

        Color IChartView.BackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                Background = new SKColor(_backColor.R, _backColor.G, _backColor.B, _backColor.A);
            }
        }

        /// <summary>
        /// Sets the size of the control.
        /// </summary>
        /// <value>
        /// The size of the control.
        /// </value>
        SizeF IChartView.ControlSize => new(Width, Height);

        /// <summary>
        /// Gets or sets the draw margin.
        /// </summary>
        /// <value>
        /// The draw margin.
        /// </value>
        public Margin? DrawMargin { get; set; }

        /// <summary>
        /// Gets or sets the animations speed.
        /// </summary>
        /// <value>
        /// The animations speed.
        /// </value>
        public TimeSpan AnimationsSpeed { get; set; }

        /// <summary>
        /// Gets or sets the easing function.
        /// </summary>
        /// <value>
        /// The easing function.
        /// </value>
        public Func<float, float>? EasingFunction { get; set; } = null;

        /// <summary>
        /// Gets or sets the updater throttler.
        /// </summary>
        /// <value>
        /// The updater throttler.
        /// </value>
        public TimeSpan UpdaterThrottler { get; set; }

        /// <summary>
        /// Gets or sets the legend position.
        /// </summary>
        /// <value>
        /// The legend position.
        /// </value>
        public LegendPosition LegendPosition { get; set; }

        /// <summary>
        /// Gets or sets the legend orientation.
        /// </summary>
        /// <value>
        /// The legend orientation.
        /// </value>
        public LegendOrientation LegendOrientation { get; set; }

        /// <summary>
        /// Gets or sets the tooltip position.
        /// </summary>
        /// <value>
        /// The tooltip position.
        /// </value>
        public TooltipPosition TooltipPosition { get; set; }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? Measuring;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateStarted;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateFinished;

        /// <summary>
        /// Hides the tooltip.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public void HideTooltip()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="format">The format.</param>
        /// <param name="quality">The quality, an integer from 0 to 100.</param>
        /// <returns></returns>
        public void SaveImage(string path, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 80)
        {
            EasingFunction = null;
            foreach (var series in Series) series.EasingFunction = null;
            foreach (var axis in XAxes) axis.EasingFunction = null;
            foreach (var axis in YAxes) axis.EasingFunction = null;

            using var surface = SKSurface.Create(new SKImageInfo(Width, Height));

            var canvas = surface.Canvas;
            using var clearColor = new SKPaint { Color = Background };
            canvas.DrawRect(0, 0, Width, Height, clearColor);

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

            using var image = surface.Snapshot();
            using var data = image.Encode(format, quality);
            using var stream = File.OpenWrite(path);
            data.SaveTo(stream);
        }

        /// <summary>
        /// Scales the UI point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="xAxisIndex">Index of the x axis.</param>
        /// <param name="yAxisIndex">Index of the y axis.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public PointF ScaleUIPoint(PointF point, int xAxisIndex = 0, int yAxisIndex = 0)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the tooltip style.
        /// </summary>
        /// <param name="background">The background.</param>
        /// <param name="textColor">Color of the text.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public void SetTooltipStyle(Color background, Color textColor) { }

        /// <summary>
        /// Shows the tooltip.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public void ShowTooltip(IEnumerable<TooltipPoint> points)
        {
            throw new NotImplementedException();
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
