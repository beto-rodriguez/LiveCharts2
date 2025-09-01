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

using LiveChartsCore.Motion;
using LiveChartsCore.Vortice.Drawing;
using Vortice;
using Vortice.Direct2D1;
using Vortice.Mathematics;

namespace LiveChartsCore.Vortice;

// IRenderMode is helpful in cases like maui or uno where we provide different renderers
// based on the os/platform-settings, but in this case we just implement it to connect
// Vortice with LiveCharts, this way LiveCharts will handle animations, frames, etc.
// but this interface is really doing nothing here.
// the key is the IGraphicsDeviceControl, it lets livecharts to draw a frame in our
// direct2d context.

/// <summary>
/// The motion canvas control for Vortce, <see cref="CoreMotionCanvas"/>.
/// </summary>
public partial class MotionCanvas : IRenderMode, IMyUIFrameworkControl
{
    /// <inheritdoc cref="CoreMotionCanvas"/>
    public CoreMotionCanvas CanvasCore { get; } = new();

    public IMyUIFrameworkControl[] Children { get; set; } = [];

    public void DrawFrame(ID2D1HwndRenderTarget renderTarget) =>
        CanvasCore.DrawFrame(new VorticeDrawingContext(CanvasCore, renderTarget, new Color4(1, 1, 1, 1)));

    void IMyUIFrameworkControl.Measure()
    {
        // nothing to do here.
    }

    // ==============================================================================
    // the next lines ares just a hack... maybe we need to improve the design a bit.
    // ==============================================================================

    event CoreMotionCanvas.FrameRequestHandler IRenderMode.FrameRequest
    {
        add => throw new NotImplementedException();
        remove => throw new NotImplementedException();
    }

    void IRenderMode.InitializeRenderMode(CoreMotionCanvas canvas) =>
        throw new NotImplementedException();

    void IRenderMode.InvalidateRenderer()
    { }

    void IRenderMode.DisposeRenderMode() =>
        throw new NotImplementedException();
}
