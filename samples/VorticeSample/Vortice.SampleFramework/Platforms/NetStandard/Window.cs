// Copyright (c) Amer Koleci and contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// https://github.com/amerkoleci/Vortice.Windows

using System.Diagnostics;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;
using static Windows.Win32.UI.WindowsAndMessaging.WINDOW_STYLE;
using static Windows.Win32.UI.WindowsAndMessaging.WINDOW_EX_STYLE;
using static Windows.Win32.UI.WindowsAndMessaging.SYSTEM_METRICS_INDEX;
using static Windows.Win32.UI.WindowsAndMessaging.SHOW_WINDOW_CMD;
using Vortice.Mathematics;
using System;

namespace Vortice;

public sealed partial class Window
{
    private const int CW_USEDEFAULT = unchecked((int)0x80000000);
    private HWND hWnd;

    public unsafe nint Handle => (nint)hWnd.Value;

    private unsafe void PlatformConstruct()
    {
        WINDOW_STYLE style = 0;
        const bool resizable = true;
        const bool fullscreen = false;

        // Setup the screen settings depending on whether it is running in full screen or in windowed mode.
        if (fullscreen)
        {
            style = WS_CLIPSIBLINGS | WS_GROUP | WS_TABSTOP;
        }
        else
        {
            style = WS_CAPTION |
                WS_SYSMENU |
                WS_MINIMIZEBOX |
                WS_CLIPSIBLINGS |
                WS_BORDER |
                WS_DLGFRAME |
                WS_THICKFRAME |
                WS_GROUP |
                WS_TABSTOP;

            if (resizable)
            {
                style |= WS_SIZEBOX;
            }
            else
            {
                style |= WS_MAXIMIZEBOX;
            }
        }

        int x = 0;
        int y = 0;
        int windowWidth;
        int windowHeight;

        if (ClientSize.Width > 0 && ClientSize.Height > 0)
        {
            var rect = new RECT
            {
                right = ClientSize.Width,
                bottom = ClientSize.Height
            };

            // Adjust according to window styles
            AdjustWindowRectEx(&rect, style, false, WS_EX_APPWINDOW);

            windowWidth = rect.right - rect.left;
            windowHeight = rect.bottom - rect.top;

            int screenWidth = GetSystemMetrics(SM_CXSCREEN);
            int screenHeight = GetSystemMetrics(SM_CYSCREEN);

            // Place the window in the middle of the screen.WS_EX_APPWINDOW
            x = (screenWidth - windowWidth) / 2;
            y = (screenHeight - windowHeight) / 2;
        }
        else
        {
            x = y = windowWidth = windowHeight = CW_USEDEFAULT;
        }

        hWnd = CreateWindowEx(
            WS_EX_APPWINDOW,
            Application.WindowClassName,
            Title,
            style,
            x,
            y,
            windowWidth,
            windowHeight,
            default,
            default,
            default,
            null
        );

        if (hWnd.Value == null)
        {
            return;
        }

        ShowWindow(hWnd, SW_NORMAL);
        RECT windowRect;
        GetClientRect(hWnd, &windowRect);
        ClientSize = new(windowRect.right - windowRect.left, windowRect.bottom - windowRect.top);
    }

    public void Destroy()
    {
        if (hWnd != IntPtr.Zero)
        {
            HWND destroyHandle = hWnd;
            hWnd = default;

            Debug.WriteLine($"[WIN32] - Destroying window: {destroyHandle}");
            DestroyWindow(destroyHandle);
        }
    }

    public unsafe RectI Bounds
    {
        get
        {
            RECT windowBounds;
            GetWindowRect(hWnd, &windowBounds);

            return RectI.FromLTRB(windowBounds.left, windowBounds.top, windowBounds.right, windowBounds.bottom);
        }
    }
}
