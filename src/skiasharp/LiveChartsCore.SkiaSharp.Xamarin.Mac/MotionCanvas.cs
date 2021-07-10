using System;
using CoreGraphics;
using Foundation;
using AppKit;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp.Views.Mac;
using System.Threading.Tasks;

namespace LiveChartsCore.SkiaSharpView.Xamarin.Mac
{
	[Register (nameof(MotionCanvas))]
	internal class MotionCanvas : NSView
    {
        private readonly MotionCanvas<SkiaSharpDrawingContext> canvasCore = new MotionCanvas<SkiaSharpDrawingContext>();
        private readonly SKCanvasView canvasView = new SKCanvasView();

        private bool isDrawingLoopRunning;

        // created in code
        public MotionCanvas()
        {
            Initialize();
        }

        // created in code
        public MotionCanvas(CGRect frame)
            : base(frame)
        {
            Initialize();
        }

        // created via designer
        public MotionCanvas(IntPtr p)
            : base(p)
        {
        }

        // created via designer
        public override void AwakeFromNib()
        {
            Initialize();
        }

        private void Initialize()
        {
            AddSubview(canvasView);
            canvasView.PaintSurface += CanvasView_PaintSurface;
            CanvasCore.Invalidated += CanvasCore_Invalidated;
        }

        public override bool MouseDownCanMoveWindow => true;

        public MotionCanvas<SkiaSharpDrawingContext> CanvasCore => canvasCore;

        public void Invalidate()
        {
            BeginInvokeOnMainThread(RunDrawingLoop);
        }

        private void CanvasCore_Invalidated(MotionCanvas<SkiaSharpDrawingContext> obj)
        {
            Invalidate();
        }

        private void CanvasView_PaintSurface(object sender, SkiaSharp.Views.Mac.SKPaintSurfaceEventArgs e)
        {
            canvasCore.DrawFrame(new SkiaSharpDrawingContext(e.Info, e.Surface, e.Surface.Canvas));
        }

        private async void RunDrawingLoop()
        {
            if (isDrawingLoopRunning) return;

            isDrawingLoopRunning = true;
            try
            {
                await DrawingLoop().ConfigureAwait(true);
            }
            finally
            {
                isDrawingLoopRunning = false;
            }

        }

        private async Task DrawingLoop()
        {
            var ts = TimeSpan.FromSeconds(1 / 90d);
            while (!canvasCore.IsValid)
            {
                canvasView.SetNeedsDisplayInRect(canvasView.Bounds);
                canvasView.NeedsToDraw(canvasView.Bounds);

                await Task.Delay(ts).ConfigureAwait(true);
            }
        }
    }
}
