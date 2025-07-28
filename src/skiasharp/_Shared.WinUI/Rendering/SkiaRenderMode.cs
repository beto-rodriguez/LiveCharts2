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

#if __UNO_SKIA__ || DESKTOP

using System.Diagnostics;
using LiveChartsCore.Drawing;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SkiaSharp;
using Uno.WinUI.Graphics2DSK;
using Windows.Foundation;

namespace LiveChartsCore.SkiaSharpView.WinUI.Rendering;

internal partial class SkiaRenderMode : Grid, IRenderMode
{
    private readonly SkiaRenderMode2 _renderMode = new();

    public SkiaRenderMode()
    {
        IsHitTestVisible = true;
        Background = new SolidColorBrush(Colors.Transparent);
        Children.Add(_renderMode);
    }

    public event CoreMotionCanvas.FrameRequestHandler FrameRequest
    {
        add => _renderMode.FrameRequest += value;
        remove => _renderMode.FrameRequest -= value;
    }

    public void InitializeRenderMode(CoreMotionCanvas canvas) =>
        _renderMode.InitializeRenderMode(canvas);

    public void DisposeRenderMode() =>
        _renderMode.Dispose();

    public void InvalidateRenderer() =>
        _renderMode.InvalidateRenderer();

    // nested because it seems that SKCanvasElement does not support pointer events
    internal partial class SkiaRenderMode2 : SKCanvasElement, IRenderMode
    {
        private CoreMotionCanvas _canvas = null!;

        public event CoreMotionCanvas.FrameRequestHandler? FrameRequest;

        public void InitializeRenderMode(CoreMotionCanvas canvas)
        {
            Background = new SolidColorBrush(Colors.Transparent);
            IsHitTestVisible = true;

            _canvas = canvas;
            CoreMotionCanvas.s_externalRenderer = $"{nameof(SkiaRenderMode)} via {nameof(SKCanvasElement)}";
        }

        private void SkiaRenderMode_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e) =>
            Debug.WriteLine("skia sees pointer moved");

        public void DisposeRenderMode() =>
            _canvas = null!;

        public void InvalidateRenderer() =>
            Invalidate();

        protected override void RenderOverride(SKCanvas canvas, Size area)
        {
            FrameRequest?.Invoke(new SkiaSharpDrawingContext(
                _canvas, new SKImageInfo((int)area.Width, (int)area.Height), canvas, GetBackground().AsSKColor()));
        }

        private LvcColor GetBackground()
        {
            var parentBg = Parent is Control control && control.Background is SolidColorBrush bg
                ? new LvcColor(bg.Color.R, bg.Color.G, bg.Color.B, bg.Color.A)
                : LvcColor.Empty;

            return parentBg != LvcColor.Empty
                ? parentBg
                : _canvas._virtualBackgroundColor;
        }
    }
}

#endif
