using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp.Views.Desktop;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveChartsCore.SkiaSharpView.WinForms
{
    public partial class MotionCanvas : UserControl
    {
        private readonly SKControl skControl; private bool isDrawingLoopRunning = false;
        private Canvas<SkiaDrawingContext> canvasCore = new Canvas<SkiaDrawingContext>();
        private double framesPerSecond = 90;

        public MotionCanvas()
        {
            InitializeComponent();

            skControl = new SKControl();
            skControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            skControl.PaintSurface += SkControl_PaintSurface;
            Controls.Add(skControl);
        }

        public double FramesPerSecond { get => framesPerSecond; set => framesPerSecond = value; }

        public Canvas<SkiaDrawingContext> CanvasCore => canvasCore;

        private void SkControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            canvasCore.DrawFrame(new SkiaDrawingContext(e.Info, e.Surface, e.Surface.Canvas));
        }

        private void CanvasCore_Invalidated(Canvas<SkiaDrawingContext> sender)
        {
            RunDrawingLoop();
        }

        private async void RunDrawingLoop()
        {
            if (isDrawingLoopRunning) return;
            isDrawingLoopRunning = true;

            var ts = TimeSpan.FromSeconds(1 / framesPerSecond);
            while (!canvasCore.IsValid)
            {
                skControl.Invalidate();
                await Task.Delay(ts);
            }

            isDrawingLoopRunning = false;
        }
    }
}
