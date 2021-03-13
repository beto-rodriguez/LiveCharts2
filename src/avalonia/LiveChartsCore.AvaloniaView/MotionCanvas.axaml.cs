using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Skia;
using LiveChartsCore.AvaloniaView.Drawing;
using LiveChartsCore.Drawing;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiveChartsCore.AvaloniaView
{
    public class MotionCanvas : UserControl
    {
        private bool isDrawingLoopRunning = false;
        private MotionCanvas<AvaloniaDrawingContext> canvasCore = new MotionCanvas<AvaloniaDrawingContext>();
        private double framesPerSecond = 60;
        private RenderTargetBitmap RenderTarget;

        public MotionCanvas()
        {
            InitializeComponent();

            canvasCore.Invalidated += OnCanvasCoreInvalidated;
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static readonly AvaloniaProperty<HashSet<IDrawableTask<AvaloniaDrawingContext>>> PaintTasksProperty =
            AvaloniaProperty.Register<CartesianChart, HashSet<IDrawableTask<AvaloniaDrawingContext>>>(nameof(PaintTasks), inherits: true);

        public HashSet<IDrawableTask<AvaloniaDrawingContext>> PaintTasks
        {
            get { return (HashSet<IDrawableTask<AvaloniaDrawingContext>>)GetValue(PaintTasksProperty); }
            set { SetValue(PaintTasksProperty, value); }
        }

        public double FramesPerSecond { get => framesPerSecond; set => framesPerSecond = value; }

        public MotionCanvas<AvaloniaDrawingContext> CanvasCore => canvasCore;

        private async void RunDrawingLoop()
        {
            if (isDrawingLoopRunning) return;
            isDrawingLoopRunning = true;

            var ts = TimeSpan.FromSeconds(1 / framesPerSecond);
            while (!canvasCore.IsValid)
            {
                InvalidateVisual();
                await Task.Delay(ts);
            }

            isDrawingLoopRunning = false;
        }

        private void OnCanvasCoreInvalidated(MotionCanvas<AvaloniaDrawingContext> sender)
        {
            RunDrawingLoop();
        }

        public override void Render(Avalonia.Media.DrawingContext context)
        {
            base.Render(context);

            context.DrawImage(
                RenderTarget,
                new Rect(0, 0, RenderTarget.PixelSize.Width, RenderTarget.PixelSize.Height),
                new Rect(0, 0, Width, Height));

            canvasCore.DrawFrame(new AvaloniaDrawingContext(context));
        }

        //public void SkiaDraw()
        //{
        //    using (var lockedBitmap = RenderTarget)
        //    {
        //        SKBitmap bitmap = (SKBitmap)lockedBitmap;
        //        using (SKCanvas canvas = new SKCanvas(bitmap))
        //        {

        //            canvas.DrawCircle(e.GetPosition(this).ToSKPoint(), 5, SKBrush);
        //        }
        //    }
        //}

        public override void EndInit()
        {
            RenderTarget = new RenderTargetBitmap(new PixelSize((int)Width, (int)Height), new Vector(96, 96));

            var context = RenderTarget.CreateDrawingContext(null);
            ((ISkiaDrawingContextImpl)context).SkCanvas.Clear(new SKColor(255, 255, 255));

            base.EndInit();
        }
    }
}
