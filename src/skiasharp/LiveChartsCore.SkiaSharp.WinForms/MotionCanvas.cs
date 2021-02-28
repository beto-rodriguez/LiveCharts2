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
        private bool isDrawingLoopRunning = false;
        private Canvas<SkiaSharpDrawingContext> canvasCore = new Canvas<SkiaSharpDrawingContext>();
        private double framesPerSecond = 90;

        public MotionCanvas()
        {
            InitializeComponent();
            canvasCore.Invalidated += CanvasCore_Invalidated;
        }

        public double FramesPerSecond { get => framesPerSecond; set => framesPerSecond = value; }

        public Canvas<SkiaSharpDrawingContext> CanvasCore => canvasCore;

        private void SkControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            canvasCore.DrawFrame(new SkiaSharpDrawingContext(e.Info, e.Surface, e.Surface.Canvas));
        }

        private void CanvasCore_Invalidated(Canvas<SkiaSharpDrawingContext> sender)
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
                skControl2.Invalidate();
                await Task.Delay(ts);
            }

            isDrawingLoopRunning = false;
        }
    }
}
