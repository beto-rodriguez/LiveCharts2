using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveChartsCore.SkiaSharpView.WinForms
{
    public partial class MotionCanvas : UserControl
    {
        private bool isDrawingLoopRunning = false;
        private readonly MotionCanvas<SkiaSharpDrawingContext> canvasCore = new();
        private double framesPerSecond = 90;
        private HashSet<IDrawableTask<SkiaSharpDrawingContext>> paintTasks = new();

        public MotionCanvas()
        {
            InitializeComponent();
            canvasCore.Invalidated += CanvasCore_Invalidated;
        }

        public HashSet<IDrawableTask<SkiaSharpDrawingContext>> PaintTasks { get => paintTasks; set { paintTasks = value; OnPaintTasksChanged(); } }

        public double FramesPerSecond { get => framesPerSecond; set => framesPerSecond = value; }

        public MotionCanvas<SkiaSharpDrawingContext> CanvasCore => canvasCore;

        private void SkControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            canvasCore.DrawFrame(new SkiaSharpDrawingContext(e.Info, e.Surface, e.Surface.Canvas));
        }

        private void CanvasCore_Invalidated(MotionCanvas<SkiaSharpDrawingContext> sender)
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

        private void OnPaintTasksChanged()
        {
            canvasCore.SetPaintTasks(paintTasks);
        }
    }
}
