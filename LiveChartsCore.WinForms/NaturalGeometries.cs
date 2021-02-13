using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;
using SkiaSharp.Views.Desktop;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveChartsCore.WinForms
{
    public partial class NaturalGeometries : UserControl
    {
        private bool isDrawingLoopRunning = false;
        private Canvas<SkiaDrawingContext> canvasCore = new Canvas<SkiaDrawingContext>();
        private double framesPerSecond = 90;

        public NaturalGeometries()
        {
            InitializeComponent();
            canvasCore.Invalidated += CanvasCore_Invalidated;
        }

        public double FramesPerSecond { get => framesPerSecond; set => framesPerSecond = value; }

        public Canvas<SkiaDrawingContext> CanvasCore => canvasCore;

        private void SkiaElement_PaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            canvasCore.DrawFrame(new SkiaDrawingContext(args.Info, args.Surface, args.Surface.Canvas));
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
                skControl1.Invalidate();
                await Task.Delay(ts);
            }

            isDrawingLoopRunning = false;
        }
    }
}
