using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using a = Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiveChartsCore.SkiaSharp.Avalonia
{
    public class MotionCanvas : UserControl
    {
        private bool isDrawingLoopRunning = false;
        private MotionCanvas<SkiaSharpDrawingContext> canvasCore = new MotionCanvas<SkiaSharpDrawingContext>();
        private double framesPerSecond = 90;

        public MotionCanvas()
        {
            InitializeComponent();
            canvasCore.Invalidated += OnCanvasCoreInvalidated;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static readonly AvaloniaProperty<HashSet<IDrawableTask<SkiaSharpDrawingContext>>> PaintTasksProperty =
            AvaloniaProperty.Register<MotionCanvas, HashSet<IDrawableTask<SkiaSharpDrawingContext>>>(nameof(PaintTasks), inherits: true);

        public HashSet<IDrawableTask<SkiaSharpDrawingContext>> PaintTasks
        {
            get { return (HashSet<IDrawableTask<SkiaSharpDrawingContext>>)GetValue(PaintTasksProperty); }
            set { SetValue(PaintTasksProperty, value); }
        }

        public double FramesPerSecond { get => framesPerSecond; set => framesPerSecond = value; }

        public MotionCanvas<SkiaSharpDrawingContext> CanvasCore => canvasCore;

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

        private void OnCanvasCoreInvalidated(MotionCanvas<SkiaSharpDrawingContext> sender)
        {
            Dispatcher.UIThread.InvokeAsync(RunDrawingLoop, DispatcherPriority.Background);
        }

        public override void Render(a.DrawingContext context)
        {
            context.Custom(new CustomDrawOp(new Rect(0, 0, Bounds.Width, Bounds.Height), canvasCore));
        }

        // based on:
        // https://github.com/AvaloniaUI/Avalonia/blob/554aaec5e5cc96c0b4318b6ed1fbf8159f442889/samples/RenderDemo/Pages/CustomSkiaPage.cs
        class CustomDrawOp : ICustomDrawOperation
        {
            private MotionCanvas<SkiaSharpDrawingContext> motionCanvas;

            public CustomDrawOp(Rect bounds, MotionCanvas<SkiaSharpDrawingContext> motionCanvas)
            {
                this.motionCanvas = motionCanvas;
                Bounds = bounds;
            }

            public void Dispose()
            {
                // No-op
            }

            public Rect Bounds { get; }

            public bool HitTest(Point p) => false;

            public bool Equals(ICustomDrawOperation other) => false;

            public void Render(IDrawingContextImpl context)
            {
                var skiaContext = context as ISkiaDrawingContextImpl;
                if (skiaContext == null) throw new Exception("SkiaSharp is not supported.");

                motionCanvas.DrawFrame(
                    new AvaloniaDrawingContext(
                        new SKImageInfo((int)Bounds.Width, (int)Bounds.Height), skiaContext.SkSurface, skiaContext.SkCanvas));
            }
        }
    }
}
