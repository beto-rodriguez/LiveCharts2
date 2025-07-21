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
using LiveChartsCore.Kernel;

namespace LiveChartsCore;

/// <summary>
/// LiveCharts global settings.
/// </summary>
public static class LiveCharts
{
    private static bool s_useGPU = true;
    private static bool s_gpuSetByUser = false;
    internal static bool s_hasBackend = false;
    internal static bool s_hasDefaultTheme = false;
    internal static bool s_hasDefaultMappers = false;
    internal static bool s_hasDefaultHardwareAcceleration = false;

    /// <summary>
    /// A constant that indicates that the tool tip should not add the current label.
    /// </summary>
    public static string IgnoreToolTipLabel { get; } = $"{{{nameof(IgnoreToolTipLabel)}}}";

    /// <summary>
    /// A constant that indicates that the tool tip should ignore the series name.
    /// </summary>
    public static string IgnoreSeriesName { get; } = "{{Series Name not set}}";

    /// <summary>
    /// Gets or sets a value indicating whether LiveCharts should create a log in the console as
    /// it renders the charts.
    /// </summary>
    public static bool EnableLogging { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether LiveCharts should show the frames per second.
    /// </summary>
    public static bool ShowFPS { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum fps requested.
    /// </summary>
    [Obsolete($"Renamed to {nameof(TargetFps)}")]
    public static double MaxFps { get => TargetFps; set => TargetFps = value; }

    /// <summary>
    /// Gets or sets the target frames per second for the rendering engine,
    /// this property is ignored when <see cref="TryUseVSync"/> is true and
    /// GPU acceleration is enabled, default is 60 fps.
    /// </summary>
    public static double TargetFps { get; set; } = 60;

    /// <summary>
    /// Attempts to align rendering cadence with display refresh rate (VSync) when supported.
    /// Requires GPU acceleration. May be ignored in software-mode or virtual environments.
    /// In WPF and WinUI the rendering cadence is regulated by the CompositionTarget.Rendering event,
    /// which dispatches frame updates synchronized with the display refresh cycle.
    /// In Avalonia, this value is ignored as the chart is rendered based on Avalonia's rendering loop.
    /// </summary>
    public static bool TryUseVSync { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether LiveCharts should use the GPU for rendering.
    /// When set to true, the library will attempt to use GPU acceleration.
    /// This has no effect on Avalonia since Avalonia determines the rendering backend.
    /// When GPU rendering is not available, it will fallback to software rendering.
    /// </summary>
    public static bool UseGPU
    {
        get => s_useGPU;
        set
        {
            s_useGPU = value;
            s_gpuSetByUser = true;
        }
    }

    /// <summary>
    /// Gets the current settings.
    /// </summary>
    /// <value>
    /// The current settings.
    /// </value>
    public static LiveChartsSettings DefaultSettings { get; } = new();

    /// <summary>
    /// Gets the hover key.
    /// </summary>
    /// <value>
    /// The bar series hover key.
    /// </value>
    public static string HoverKey => nameof(HoverKey);

    /// <summary>
    /// Gets a constant that indicates that a rotation angle follows the tangent line, this property is only useful in polar series.
    /// </summary>
    public static double TangentAngle => 1 << 25;

    /// <summary>
    /// Gets a constant that indicates that a rotation angle follows the cotangent line, this property is only useful in polar series.
    /// </summary>
    public static double CotangentAngle => 1 << 26;

    /// <summary>
    /// The disable animations
    /// </summary>
    public static TimeSpan DisableAnimations = TimeSpan.FromMilliseconds(1);

    /// <summary>
    /// Configures LiveCharts.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException">$"{nameof(LiveChartsSettings)} must not be null.</exception>
    public static void Configure(Action<LiveChartsSettings> configuration)
    {
        if (configuration is null) throw new NullReferenceException($"{nameof(LiveChartsSettings)} must not be null.");
        configuration(DefaultSettings);
    }

    /// <summary>
    /// Converts ticks to Date, and prevents overflow exceptions.
    /// </summary>
    /// <param name="ticks">The ticks.</param>
    /// <returns>A DateTime object.</returns>
    public static DateTime AsDate(this double ticks)
    {
        if (ticks < 0) ticks = 0;
        return new DateTime((long)ticks);
    }

    /// <summary>
    /// Converts ticks to TimeSpan, and prevents overflow exceptions.
    /// </summary>
    /// <param name="ticks">The ticks.</param>
    /// <returns>A DateTime object.</returns>
    public static TimeSpan AsTimeSpan(this double ticks)
    {
        if (ticks < 0) ticks = 0;
        return TimeSpan.FromTicks((long)ticks);
    }

    internal static void SetUseGPUIfNotSetByUser(bool value)
    {
        if (s_gpuSetByUser) return;
        s_useGPU = value;
    }
}
