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

namespace LiveChartsCore.Kernel;

/// <summary>
/// LiveCharts rendering settings, this class is used to configure the rendering engine.
/// </summary>
public class RenderingSettings
{
    internal static RenderingSettings Default { get; set; } = new();

    /// <summary>
    /// Indicates whether hardware acceleration is used, this will only work
    /// if the platform and device support it.
    /// This is ignored in Avalonia and Uno-Desktop because in those platforms
    /// the frame rate and rendering cadence is determined by them.
    /// </summary>
    public bool UseGPU { get; set; } = true;

    /// Indicates whether the rendering cadence should be aligned with the display refresh rate,
    /// GPU acceleration is required for this to work.
    /// This is ignored in Avalonia and Uno-Desktop because in those platforms
    /// the frame rate and rendering cadence is determined by them.
    public bool TryUseVSync { get; set; } = true;

    /// <summary>
    /// Defines the desired frames per second when using the LiveCharts render loop, this has no effect
    /// when <see cref="TryUseVSync"/> is true and <see cref="UseGPU"/> is true.
    /// Default is 60.
    /// </summary>
    public double LiveChartsRenderLoopFPS { get; set; } = 60;

    /// <summary>
    /// Gets or sets a value indicating whether the chart should show the frames per second
    /// and more information about the rendering in the top left corner of the chart. Default is false.
    /// </summary>
    public bool ShowFPS { get; set; } = false;

}
