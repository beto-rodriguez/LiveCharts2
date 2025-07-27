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

using System;
using LiveChartsCore.Drawing;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace LiveChartsCore.SkiaSharpView.Maui.Rendering;

internal class GPURenderMode : SKGLView, IRenderMode
{
    private CoreMotionCanvas _canvas = null!;
    private float _pixelDensity = 1;

    public event CoreMotionCanvas.FrameRequestHandler? FrameRequest;

    public void InitializeRenderMode(CoreMotionCanvas canvas)
    {
        _canvas = canvas;
        PaintSurface += OnPaintSurface;

        _pixelDensity = (float)DeviceDisplay.MainDisplayInfo.Density;
        DeviceDisplay.MainDisplayInfoChanged += MainDisplayInfoChanged;

#if DEBUG
        System.Diagnostics.Trace.WriteLine($"[LiveCharts Info] LiveCharts is using {nameof(GPURenderMode)}.");
#endif
    }

    public void DisposeRenderMode()
    {
        _canvas = null!;
        PaintSurface -= OnPaintSurface;
        DeviceDisplay.MainDisplayInfoChanged -= MainDisplayInfoChanged;
    }

    public void InvalidateRenderer() =>
        InvalidateSurface();

    private void OnPaintSurface(object? sender, SKPaintGLSurfaceEventArgs e)
    {
        if (_pixelDensity != 1)
            e.Surface.Canvas.Scale(_pixelDensity, _pixelDensity);

        FrameRequest?.Invoke(
            new SkiaSharpDrawingContext(_canvas, e.Info, e.Surface.Canvas, GetBackground().AsSKColor()));
    }

    private void MainDisplayInfoChanged(object? sender, EventArgs e) =>
        _pixelDensity = (float)DeviceDisplay.MainDisplayInfo.Density;

    private LvcColor GetBackground()
    {
        var parentBg = Parent is VisualElement control && control.Background is SolidColorBrush bg && bg.Color is not null
            ? new LvcColor((byte)(bg.Color.Red * 255), (byte)(bg.Color.Green * 255), (byte)(bg.Color.Blue * 255), (byte)(bg.Color.Alpha * 255))
            : LvcColor.Empty;

        return parentBg != LvcColor.Empty
            ? parentBg
            : _canvas._virtualBackgroundColor;
    }
}
