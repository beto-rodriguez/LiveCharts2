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

namespace LiveChartsCore.Native;

internal partial class NativeHelpers
{
    private static bool? s_isTouchDevice;

    public static bool IsTouchDevice()
    {
        if (s_isTouchDevice.HasValue)
            return s_isTouchDevice.Value;

        var isWindowsTouchEnabled = false;

#if WINDOWS
        isWindowsTouchEnabled = WindowsTouchSupportHelper.IsWindowsTouchEnabled();
#endif

        var result =
            OperatingSystem.IsAndroid() ||
            (OperatingSystem.IsIOS() && !OperatingSystem.IsMacCatalyst()) ||
            //(OperatingSystem.IsBrowser() && IsTouchSupportedInJs()) || is this needed?
            (OperatingSystem.IsWindows() && isWindowsTouchEnabled);

        s_isTouchDevice = result;

        return result;
    }

#if WINDOWS
    public static class WindowsTouchSupportHelper
    {
        private const int SM_MAXIMUMTOUCHES = 95;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        public static bool IsWindowsTouchEnabled()
        {
            try
            {
                return GetSystemMetrics(SM_MAXIMUMTOUCHES) > 0;
            }
            catch
            {
                return false;
            }
        }
    }
#endif
}
