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

#if IOS || MACCATALYST
using PlatformView = Microsoft.Maui.Platform.ContentView;
#elif ANDROID
using PlatformView = Microsoft.Maui.Platform.ContentViewGroup;
#elif WINDOWS
using PlatformView = Microsoft.Maui.Platform.ContentPanel;
#else
using PlatformView = System.Object;
#endif

using LiveChartsCore.Drawing;
using Microsoft.Maui.Devices;

namespace LiveChartsCore.SkiaSharpView.Maui;

/// <summary>
/// The chart behaviour for MAUI.
/// </summary>
public partial class ChartBehaviour : Behaviours.ChartBehaviour
{
    private static double s_density;
    private static LvcSize s_screenSize;

    static ChartBehaviour()
    {
        var deviceDisplay = DeviceDisplay.Current;
        deviceDisplay.MainDisplayInfoChanged += (_, args) =>
        {
            var displayInfo = args.DisplayInfo;
            UpdateScreenInfo(displayInfo);
        };

        UpdateScreenInfo(deviceDisplay.MainDisplayInfo);
        return;

        static void UpdateScreenInfo(DisplayInfo displayInfo)
        {
            s_density = displayInfo.Density;
            s_screenSize = new LvcSize((float)displayInfo.Width, (float)displayInfo.Height);
        }
    }

    /// <inheritdoc />
    public override LvcSize ScreenSize => s_screenSize;

    /// <inheritdoc />
    public override double Density => s_density;

    /// <summary>
    /// Attaches the native events on the specified platform view.
    /// </summary>
    /// <param name="platformView">The platform view.</param>
    public void On(PlatformView platformView)
    {
#if ANDROID
        platformView.Touch += OnAndroidTouched;
        platformView.Hover += OnAndroidHover;
#endif

#if MACCATALYST || IOS
        platformView.UserInteractionEnabled = true;
#if MACCATALYST
        platformView.AddGestureRecognizer(MacCatalystHoverGestureRecognizer);
#endif
        platformView.AddGestureRecognizer(MacCatalystLongPressGestureRecognizer);
        platformView.AddGestureRecognizer(MacCatalystPinchGestureRecognizer);
        platformView.AddGestureRecognizer(MacCatalystPanGestureRecognizer);
#endif

#if WINDOWS
        platformView.PointerPressed += OnWindowsPointerPressed;
        platformView.PointerMoved += OnWindowsPointerMoved;
        platformView.PointerReleased += OnWindowsPointerReleased;
        platformView.PointerWheelChanged += OnWindowsPointerWheelChanged;
        platformView.PointerExited += OnWindowsPointerExited;
#endif
    }

    /// <summary>
    /// Detaches the native events on the specified platform view.
    /// </summary>
    /// <param name="platformView">The platform view.</param>
    public void Off(PlatformView platformView)
    {
#if ANDROID
        platformView.Touch -= OnAndroidTouched;
        platformView.Hover -= OnAndroidHover;
#endif

#if MACCATALYST || IOS
        platformView.UserInteractionEnabled = false;
#if MACCATALYST
        platformView.RemoveGestureRecognizer(MacCatalystHoverGestureRecognizer);
#endif
        platformView.RemoveGestureRecognizer(MacCatalystLongPressGestureRecognizer);
        platformView.RemoveGestureRecognizer(MacCatalystPinchGestureRecognizer);
        platformView.RemoveGestureRecognizer(MacCatalystPanGestureRecognizer);
#endif

#if WINDOWS
        platformView.PointerPressed -= OnWindowsPointerPressed;
        platformView.PointerMoved -= OnWindowsPointerMoved;
        platformView.PointerReleased -= OnWindowsPointerReleased;
        platformView.PointerWheelChanged -= OnWindowsPointerWheelChanged;
        platformView.PointerExited -= OnWindowsPointerExited;
#endif
    }
}
