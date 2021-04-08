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
    public abstract class Chart: UserControl, IChartView<SkiaSharpDrawingContext>
    {
        protected Chart<SkiaSharpDrawingContext> core;
        protected IChartLegend<SkiaSharpDrawingContext> legend;
        protected IChartTooltip<SkiaSharpDrawingContext> tooltip = new DefaultTooltip();
        protected MotionCanvas motionCanvas;
        private PointF mousePosition = new();
        private readonly ActionThrottler mouseMoveThrottler;

        public Chart()
        {
            motionCanvas = new MotionCanvas();
            SuspendLayout();
            motionCanvas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
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

        public Chart(IChartTooltip<SkiaSharpDrawingContext> tooltip) : this()
        {
            this.tooltip = tooltip;
        }

        SizeF IChartView.ControlSize => new() { Width = motionCanvas.Width, Height = motionCanvas.Height };

        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => motionCanvas.CanvasCore;

        public LegendPosition LegendPosition { get; set; } = LiveCharts.CurrentSettings.DefaultLegendPosition;

        public LegendOrientation LegendOrientation { get; set; } = LiveCharts.CurrentSettings.DefaultLegendOrientation;

        public IChartLegend<SkiaSharpDrawingContext> Legend => null;

        public Margin DrawMargin { get; set; }

        public TimeSpan AnimationsSpeed { get; set; } = LiveCharts.CurrentSettings.DefaultAnimationsSpeed;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<float, float> EasingFunction { get; set; } = LiveCharts.CurrentSettings.DefaultEasingFunction;

        public TooltipPosition TooltipPosition { get; set; } = LiveCharts.CurrentSettings.DefaultTooltipPosition;

        public TooltipFindingStrategy TooltipFindingStrategy { get; set; } = LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy;

        public IChartTooltip<SkiaSharpDrawingContext> Tooltip => null;

        public Font TooltipFont { get; set; } = new Font(new FontFamily("Trebuchet MS"), 11, FontStyle.Regular);

        public Color TooltipBackColor { get; set; } = Color.FromArgb(255, 250, 250, 250);

        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; }

        protected abstract void InitializeCore();

        private void OnResized(object sender, EventArgs e)
        {
            if (core == null) return;
            core.Update();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var p = e.Location;
            mousePosition = new PointF(p.X, p.Y);
            mouseMoveThrottler.Call();
        }

        private void MouseMoveThrottlerUnlocked()
        {
            if (TooltipPosition == TooltipPosition.Hidden) return;
            tooltip.Show(core.FindPointsNearTo(mousePosition), core);
        }

        private void ChartOnMouseMove(object sender, MouseEventArgs e)
        {
            var p = e.Location;
            mousePosition = new PointF(p.X, p.Y);
            mouseMoveThrottler.Call();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (tooltip is IDisposable disposableTooltip) disposableTooltip.Dispose();
            base.OnHandleDestroyed(e);
        }
    }
}
