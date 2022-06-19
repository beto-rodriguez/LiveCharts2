﻿// The MIT License(MIT)
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
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace LiveChartsCore.SkiaSharpView.Uno.Helpers;

// based on https://youtu.be/RInO5Jqru4s?t=4083

/// <summary>
/// Defines Uno platform helpers.
/// </summary>
public static class UnoPlatformHelpers
{
    /// <summary>
    /// Determines whether the assembly is running in web assembly.
    /// </summary>
    public static bool IsWebAssembly { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"));

    /// <summary>
    /// Invokes a given acction in the UI thread.
    /// </summary>
    /// <param name="action">The action.</param>
    public static void InvokeOnUIThread(Action action)
    {
        if (IsWebAssembly)
        {
            action();
            return;
        }

        // we get about 20 fps when the chart is updated in short periods of time.

        // see general/multithreading2 sample
        // this samples forces the data update to run in the UI thread always, and adds a new data point every 10ms.

        // UWP seems to struggle handling the updates.
        // theorically the chart is redrawn at a rate of 20~ fps
        // but that is not true, the UI thread seems to be locked.

        // NOTES:
        // 1. UWP is the only platform that behaves like this.
        // 2. Notice the WPF.Host sample behaves much better.

        // How can we help UWP to handle this? What am I missing?

        _ = CoreApplication.MainView.CoreWindow.Dispatcher
            .RunAsync(CoreDispatcherPriority.High, () => action());
    }
}

/// <summary>
/// Defines a custom pinch event handler.
/// </summary>
/// <param name="sender"></param>
/// <param name="eventArgs"></param>
public delegate void PinchHandler(object sender, LiveChartsPinchEventArgs eventArgs);
