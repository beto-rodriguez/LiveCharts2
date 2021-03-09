using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using LiveChartsCore.Rx;
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
        protected MotionCanvas motionCanvas;
        private PointF mousePosition = new PointF();
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

            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSK.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetStylesBuilder<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetInitializer();
            if (stylesBuilder.CurrentColors == null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ConstructChart(this);

            InitializeCore();
            mouseMoveThrottler = new ActionThrottler(TimeSpan.FromMilliseconds(10));
            mouseMoveThrottler.Unlocked += MouseMoveThrottlerUnlocked;
        }

        SizeF IChartView.ControlSize => new SizeF { Width = motionCanvas.Width, Height = motionCanvas.Height };

        public Canvas<SkiaSharpDrawingContext> CoreCanvas => motionCanvas.CanvasCore;

        public LegendPosition LegendPosition { get; set; }

        public LegendOrientation LegendOrientation { get; set; }

        public IChartLegend<SkiaSharpDrawingContext> Legend => null;

        public Margin DrawMargin { get; set; }

        public TimeSpan AnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(500);

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<float, float> EasingFunction { get; set; } = EasingFunctions.Lineal;

        public TooltipPosition TooltipPosition { get; set; }

        public TooltipFindingStrategy TooltipFindingStrategy { get; set; }

        public IChartTooltip<SkiaSharpDrawingContext> Tooltip => null;

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
            mouseMoveThrottler.TryRun();
        }

        private void MouseMoveThrottlerUnlocked()
        {
            if (TooltipPosition == TooltipPosition.Hidden) return;
            //tooltip.Show(core.FindPointsNearTo(mousePosition), core);
        }
    }

}
