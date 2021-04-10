// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    public class MotionCanvas : Control
    {
        private readonly MotionCanvas<SkiaSharpDrawingContext> canvasCore = new();
        protected SKElement? skiaElement;
        private bool isDrawingLoopRunning = false;
        private double framesPerSecond = 90;

        static MotionCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MotionCanvas), new FrameworkPropertyMetadata(typeof(MotionCanvas)));
        }

        public MotionCanvas()
        {
            canvasCore.Invalidated += OnCanvasCoreInvalidated;
            Unloaded += OnUnloaded;
        }

        public static readonly DependencyProperty PaintTasksProperty =
            DependencyProperty.Register(
                nameof(PaintTasks), typeof(HashSet<IDrawableTask<SkiaSharpDrawingContext>>), typeof(MotionCanvas),
                new PropertyMetadata(new HashSet<IDrawableTask<SkiaSharpDrawingContext>>(), new PropertyChangedCallback(OnPaintTaskChanged)));

        public HashSet<IDrawableTask<SkiaSharpDrawingContext>> PaintTasks
        {
            get { return (HashSet<IDrawableTask<SkiaSharpDrawingContext>>)GetValue(PaintTasksProperty); }
            set { SetValue(PaintTasksProperty, value); }
        }

        public double FramesPerSecond { get => framesPerSecond; set => framesPerSecond = value; }

        public MotionCanvas<SkiaSharpDrawingContext> CanvasCore => canvasCore;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            skiaElement = Template.FindName("skiaElement", this) as SKElement;
            if (skiaElement == null)
                throw new Exception(
                    $"SkiaElement not found. This was probably caused because the control {nameof(MotionCanvas)} template was overridden, " +
                    $"If you override the template please add an {nameof(SKElement)} to the template and name it 'skiaElement'");

            skiaElement.PaintSurface += OnPaintSurface;
        }

        protected virtual void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs args)
        {
            canvasCore.DrawFrame(new SkiaSharpDrawingContext(args.Info, args.Surface, args.Surface.Canvas));
        }

        private void OnCanvasCoreInvalidated(MotionCanvas<SkiaSharpDrawingContext> sender)
        {
            RunDrawingLoop();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            canvasCore.Invalidated -= OnCanvasCoreInvalidated;
        }

        private async void RunDrawingLoop()
        {
            if (isDrawingLoopRunning || skiaElement == null) return;
            isDrawingLoopRunning = true;

            var ts = TimeSpan.FromSeconds(1 / framesPerSecond);
            while (!canvasCore.IsValid)
            {
                skiaElement.InvalidateVisual();
                await Task.Delay(ts);
            }

            isDrawingLoopRunning = false;
        }

        private static void OnPaintTaskChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var motionCanvas = (MotionCanvas)sender;
            motionCanvas.canvasCore.SetPaintTasks(motionCanvas.PaintTasks);
        }
    }
}
