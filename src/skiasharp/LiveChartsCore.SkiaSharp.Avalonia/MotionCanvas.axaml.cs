// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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

namespace LiveChartsCore.SkiaSharpView.Avalonia
{
    /// <summary>
    /// Thje motion canvas control fro avalonia, <see cref="MotionCanvas{TDrawingContext}"/>.
    /// </summary>
    public class MotionCanvas : UserControl
    {
        private bool isDrawingLoopRunning = false;
        private double framesPerSecond = 90;

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionCanvas"/> class.
        /// </summary>
        public MotionCanvas()
        {
            InitializeComponent();
            CanvasCore.Invalidated += OnCanvasCoreInvalidated;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// The paint tasks property
        /// </summary>
        public static readonly AvaloniaProperty<HashSet<IDrawableTask<SkiaSharpDrawingContext>>> PaintTasksProperty =
            AvaloniaProperty.Register<MotionCanvas, HashSet<IDrawableTask<SkiaSharpDrawingContext>>>(nameof(PaintTasks), inherits: true);

        /// <summary>
        /// Gets or sets the paint tasks.
        /// </summary>
        /// <value>
        /// The paint tasks.
        /// </value>
        public HashSet<IDrawableTask<SkiaSharpDrawingContext>> PaintTasks
        {
            get => (HashSet<IDrawableTask<SkiaSharpDrawingContext>>)GetValue(PaintTasksProperty);
            set => SetValue(PaintTasksProperty, value);
        }

        /// <summary>
        /// Gets or sets the frames per second.
        /// </summary>
        /// <value>
        /// The frames per second.
        /// </value>
        public double FramesPerSecond { get => framesPerSecond; set => framesPerSecond = value; }

        /// <summary>
        /// Gets the canvas core.
        /// </summary>
        /// <value>
        /// The canvas core.
        /// </value>
        public MotionCanvas<SkiaSharpDrawingContext> CanvasCore { get; } = new();

        /// <inheritdoc cref="Render(a.DrawingContext)" />
        public override void Render(a.DrawingContext context)
        {
            context.Custom(new CustomDrawOp(new Rect(0, 0, Bounds.Width, Bounds.Height), CanvasCore));
        }

        /// <inheritdoc cref="OnPropertyChanged{T}(AvaloniaPropertyChangedEventArgs{T})" />
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            if (change.Property.Name == nameof(PaintTasks)) CanvasCore.SetPaintTasks(PaintTasks);

            base.OnPropertyChanged(change);
        }

        private async void RunDrawingLoop()
        {
            if (isDrawingLoopRunning) return;
            isDrawingLoopRunning = true;

            var ts = TimeSpan.FromSeconds(1 / framesPerSecond);
            while (!CanvasCore.IsValid)
            {
                InvalidateVisual();
                await Task.Delay(ts);
            }

            isDrawingLoopRunning = false;
        }

        private void OnCanvasCoreInvalidated(MotionCanvas<SkiaSharpDrawingContext> sender)
        {
            _ = Dispatcher.UIThread.InvokeAsync(RunDrawingLoop, DispatcherPriority.Background);
        }

        // based on:
        // https://github.com/AvaloniaUI/Avalonia/blob/554aaec5e5cc96c0b4318b6ed1fbf8159f442889/samples/RenderDemo/Pages/CustomSkiaPage.cs
        class CustomDrawOp : ICustomDrawOperation
        {
            private readonly MotionCanvas<SkiaSharpDrawingContext> motionCanvas;

            public CustomDrawOp(Rect bounds, MotionCanvas<SkiaSharpDrawingContext> motionCanvas)
            {
                this.motionCanvas = motionCanvas;
                Bounds = bounds;
            }

            public void Dispose() { }

            public Rect Bounds { get; }

            public bool HitTest(Point p)
            {
                return false;
            }

            public bool Equals(ICustomDrawOperation? other)
            {
                return false;
            }

            public void Render(IDrawingContextImpl context)
            {
                if (context is not ISkiaDrawingContextImpl skiaContext)
                    throw new Exception("SkiaSharp is not supported.");

                motionCanvas.DrawFrame(
                   new AvaloniaDrawingContext(
                       new SKImageInfo((int)Bounds.Width, (int)Bounds.Height), skiaContext.SkSurface, skiaContext.SkCanvas));
            }
        }
    }
}
