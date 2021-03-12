using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveChartsCore.AvaloniaView.Drawing;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiveChartsCore.AvaloniaView
{
    public class MotionCanvas : UserControl
    {
        private bool isDrawingLoopRunning = false;
        private MotionCanvas<AvaloniaDrawingContext> canvasCore = new MotionCanvas<AvaloniaDrawingContext>();
        private double framesPerSecond = 1;

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
            //var w = Bounds.Width;
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
            canvasCore.DrawFrame(new AvaloniaDrawingContext(context));
        }
    }
}
