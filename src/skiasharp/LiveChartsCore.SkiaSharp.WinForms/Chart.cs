// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LiveChartsCore.SkiaSharpView.WinForms
{
    /// <inheritdoc cref="IChartView" />
    public abstract class Chart : UserControl, IChartView<SkiaSharpDrawingContext>
    {
        /// <summary>
        /// The core
        /// </summary>
        protected Chart<SkiaSharpDrawingContext>? core;

        /// <summary>
        /// The legend
        /// </summary>
        protected IChartLegend<SkiaSharpDrawingContext> legend = new DefaultLegend();

        /// <summary>
        /// The tooltip
        /// </summary>
        protected IChartTooltip<SkiaSharpDrawingContext> tooltip = new DefaultTooltip();

        /// <summary>
        /// The motion canvas
        /// </summary>
        protected MotionCanvas motionCanvas;

        private PointF mousePosition = new();
        private LegendPosition legendPosition = LiveCharts.CurrentSettings.DefaultLegendPosition;
        private LegendOrientation legendOrientation = LiveCharts.CurrentSettings.DefaultLegendOrientation;
        private Margin? drawMargin = null;
        private TooltipPosition tooltipPosition = LiveCharts.CurrentSettings.DefaultTooltipPosition;
        private TooltipFindingStrategy tooltipFindingStrategy = LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy;
        private Font tooltipFont = new(new FontFamily("Trebuchet MS"), 11, FontStyle.Regular);
        private Color tooltipBackColor = Color.FromArgb(255, 250, 250, 250);
        private Font legendFont = new(new FontFamily("Trebuchet MS"), 11, FontStyle.Regular);
        private Color legendBackColor = Color.FromArgb(255, 250, 250, 250);
        private readonly ActionThrottler mouseMoveThrottler;

        /// <summary>
        /// Initializes a new instance of the <see cref="Chart"/> class.
        /// </summary>
        /// <param name="tooltip">The default tool tip control.</param>
        /// <param name="legend">The default legend.</param>
        /// <exception cref="MotionCanvas"></exception>
        public Chart(IChartTooltip<SkiaSharpDrawingContext>? tooltip, IChartLegend<SkiaSharpDrawingContext>? legend)
        {
            if (tooltip != null) this.tooltip = tooltip;
            if (legend != null) this.legend = legend;

            motionCanvas = new MotionCanvas();
            SuspendLayout();
            motionCanvas.Dock = DockStyle.Fill;
            motionCanvas.FramesPerSecond = 90D;
            motionCanvas.Location = new Point(0, 0);
            motionCanvas.Name = "motionCanvas";
            motionCanvas.Size = new Size(150, 150);
            motionCanvas.TabIndex = 0;
            motionCanvas.Resize += OnResized;
            motionCanvas.MouseMove += OnMouseMove;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(motionCanvas);
            var l = (Control)this.legend;
            l.Dock = DockStyle.Right;
            Controls.Add(l);
            Name = "CartesianChart";
            ResumeLayout(false);

            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetStylesBuilder<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetInitializer();
            if (stylesBuilder.CurrentColors == null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ConstructChart(this);

            var c = Controls[0].Controls[0];
            c.MouseMove += ChartOnMouseMove;

            InitializeCore();
            mouseMoveThrottler = new ActionThrottler(MouseMoveThrottlerUnlocked, TimeSpan.FromMilliseconds(10));
        }

        SizeF IChartView.ControlSize => new() { Width = motionCanvas.Width, Height = motionCanvas.Height };

        /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => motionCanvas.CanvasCore;

        /// <inheritdoc cref="IChartView.DrawMargin" />
        public Margin? DrawMargin { get => drawMargin; set { drawMargin = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IChartView.AnimationsSpeed" />
        public TimeSpan AnimationsSpeed { get; set; } = LiveCharts.CurrentSettings.DefaultAnimationsSpeed;

        /// <inheritdoc cref="IChartView.AnimationsSpeed" />
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<float, float> EasingFunction { get; set; } = LiveCharts.CurrentSettings.DefaultEasingFunction;

        /// <inheritdoc cref="IChartView.LegendPosition" />
        public LegendPosition LegendPosition { get => legendPosition; set { legendPosition = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IChartView.LegendOrientation" />
        public LegendOrientation LegendOrientation { get => legendOrientation; set { legendOrientation = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the default legend font.
        /// </summary>
        /// <value>
        /// The legend font.
        /// </value>
        public Font LegendFont { get => legendFont; set {legendFont = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the default color of the legend back.
        /// </summary>
        /// <value>
        /// The color of the legend back.
        /// </value>
        public Color LegendBackColor { get => legendBackColor; set { legendBackColor = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Legend" />
        public IChartLegend<SkiaSharpDrawingContext>? Legend => legend;

        /// <inheritdoc cref="IChartView.LegendPosition" />
        public TooltipPosition TooltipPosition { get => tooltipPosition; set { tooltipPosition = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IChartView.TooltipFindingStrategy" />
        public TooltipFindingStrategy TooltipFindingStrategy { get => tooltipFindingStrategy; set { tooltipFindingStrategy = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the default tool tip font.
        /// </summary>
        /// <value>
        /// The tool tip font.
        /// </value>
        public Font TooltipFont { get => tooltipFont; set { tooltipFont = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the color of the default tool tip back.
        /// </summary>
        /// <value>
        /// The color of the tool tip back.
        /// </value>
        public Color TooltipBackColor { get => tooltipBackColor; set { tooltipBackColor = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip" />
        public IChartTooltip<SkiaSharpDrawingContext>? Tooltip => tooltip;

        /// <inheritdoc cref="IChartView{TDrawingContext}.PointStates" />
        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; } = new();

        /// <summary>
        /// Initializes the core.
        /// </summary>
        /// <returns></returns>
        protected abstract void InitializeCore();

        /// <summary>
        /// Called when a property changes.
        /// </summary>
        /// <returns></returns>
        protected void OnPropertyChanged()
        {
            if (core == null) return;
            core.Update();
        }

        /// <summary>
        /// Raises the <see cref="E:HandleDestroyed" /> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (tooltip is IDisposable disposableTooltip) disposableTooltip.Dispose();
            base.OnHandleDestroyed(e);
        }

        private void OnResized(object? sender, EventArgs e)
        {
            if (core == null) return;
            core.Update();
        }

        private void OnMouseMove(object? sender, MouseEventArgs e)
        {
            var p = e.Location;
            mousePosition = new PointF(p.X, p.Y);
            mouseMoveThrottler.Call();
        }

        private void MouseMoveThrottlerUnlocked()
        {
            if (core == null || TooltipPosition == TooltipPosition.Hidden) return;
            tooltip.Show(core.FindPointsNearTo(mousePosition), core);
        }

        private void ChartOnMouseMove(object? sender, MouseEventArgs e)
        {
            var p = e.Location;
            mousePosition = new PointF(p.X, p.Y);
            mouseMoveThrottler.Call();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Chart
            // 
            this.Name = "Chart";
            this.Size = new System.Drawing.Size(643, 418);
            this.ResumeLayout(false);

        }
    }
}
