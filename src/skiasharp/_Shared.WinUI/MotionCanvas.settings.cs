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
#if __UNO_SKIA__ || DESKTOP
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
            ShowFPS = true
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
            ShowFPS = true
        };
#else
        // ---------------------------------
        // fallback settings
        // ---------------------------------
        = new()
        {
            // ignored, defined by uno
            UseGPU = false,

            // ignored, defined by uno
            TryUseVSync = false,

            // fallback value when VSync is not used.
            LiveChartsRenderLoopFPS = 60,

            // make this true to see the FPS in the top left corner of the chart
            ShowFPS = true
        };
#endif

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    static MotionCanvas()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        LiveCharts.Configure(config => config.UseDefaults(RecommendedUnoRenderingSettings));
    }
}
