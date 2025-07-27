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

using Microsoft.UI.Xaml.Controls;
using LiveChartsCore.Kernel;

#pragma warning disable IDE0028 // Simplify collection initialization

namespace LiveChartsCore.SkiaSharpView.WinUI;

/// <summary>
/// The motion canvas control for WinUI and Uno Platform.
/// </summary>
public partial class MotionCanvas : Canvas
{
    /// <summary>
    /// Gets the recommended rendering settings for Uno and WinUI.
    /// </summary>
    public static RenderingSettings RecommendedUnoRenderingSettings { get; }
#if (__UNO_SKIA__ || DESKTOP) && !BROWSERWASM
        // ---------------------------------
        // if skia renderer
        // ---------------------------------
        = new()
        {
            // ignored, defined by uno
            UseGPU = true,

            // ignored, defined by uno
            TryUseVSync = true,

            // fallback value when VSync is not used.
            LiveChartsRenderLoopFPS = 60,

            // make this true to see the FPS in the top left corner of the chart
            ShowFPS = false
        };
#elif BROWSERWASM
        // note #250727
        // ---------------------------------
        // for a reason the browser wasm using the skia renderer
        // just throws, at least on uno sdk 6.1.23 and skiasharp view 3.119.0
        // it seems that _visual field is missing in wasm builds, which is used by the
        // Uno.WinUI.Graphics2DSK.SKCanvasElement.
        // so for wasm, lets render the charts our way.
        // Unhandled dispatcher exception: Error: Field not found: Microsoft.UI.Composition.ContainerVisual Microsoft.UI.Xaml.UIElement._visual Due to: Could not find field in class (   at LiveChartsCore.SkiaSharpView.WinUI.Rendering.SkiaRenderMode.SkiaRenderMode2.InvalidateRenderer() in /_/src/skiasharp/_Shared.WinUI/Rendering/SkiaRenderMode.cs:line 89
        // at LiveChartsCore.SkiaSharpView.WinUI.Rendering.SkiaRenderMode.InvalidateRenderer() in /_/src/skiasharp/_Shared.WinUI/Rendering/SkiaRenderMode.cs:line 62
        // at LiveChartsCore.Native.NativeFrameTicker.OnCompositonTargetRendering(Object sender, Object e) in /_/src/_Shared.Native/Platforms/WinUI/NativeTicker.cs:line 60
        // at Uno.UI.Dispatching.NativeDispatcher.DispatchItems() in C:\a\1\s\src\Uno.UI.Dispatching\Native\NativeDispatcher.cs:line 106
        // at Uno.UI.Dispatching.NativeDispatcher.DispatcherCallback() in C:\a\1\s\src\Uno.UI.Dispatching\Native\NativeDispatcher.wasm.cs:line 25
        // at Uno.UI.Dispatching.NativeDispatcher.__Wrapper_DispatcherCallback_1192908908(JSMarshalerArgument* __arguments_buffer) in C:\a\1\s\src\Uno.UI.Dispatching\obj\Uno.UI.Dispatching.Wasm\Release\net9.0\Microsoft.Interop.JavaScript.JSImportGenerator\Microsoft.Interop.JavaScript.JSExportGenerator\JSExports.g.cs:line 35
        // Error: Field not found: Microsoft.UI.Composition.ContainerVisual Microsoft.UI.Xaml.UIElement._visual Due to: Could not find field in class
        // ---------------------------------
        = new()
        {
            // try use gl
            UseGPU = true,
            // connect to the browser's requestAnimationFrame via uno?
            TryUseVSync = true,
            // fallback value when VSync is not used.
            LiveChartsRenderLoopFPS = 20,
            // make this true to see the FPS in the top left corner of the chart
            ShowFPS = false
        };
#elif WINDOWS
        // ---------------------------------
        // if winui
        // ---------------------------------
        = new()
        {
            // at least on uno sdk 6.1.23 and skiasharp view 3.119.0
            // SwapChainPanel does not work.
            UseGPU = false,

            // via CompositionTarget.Rendering
            TryUseVSync = true,

            // fallback value when VSync is not used.
            LiveChartsRenderLoopFPS = 60,

            // make this true to see the FPS in the top left corner of the chart
            ShowFPS = false
        };
#elif ANDROID && !__UNO_SKIA__
        // ---------------------------------
        // if android without skia renderer
        // ---------------------------------
        = new()
        {
            // ignored, defined by uno
            UseGPU = true,

            // ignored, defined by uno
            TryUseVSync = true,

            // fallback value when VSync is not used.
            LiveChartsRenderLoopFPS = 60,

            // make this true to see the FPS in the top left corner of the chart
            ShowFPS = false
        };
#endif

    static MotionCanvas()
    {
        LiveCharts.Configure(config => config.UseDefaults(RecommendedUnoRenderingSettings));
    }
}
