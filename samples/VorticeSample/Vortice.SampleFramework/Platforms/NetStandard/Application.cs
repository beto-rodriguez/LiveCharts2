// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.
// https://github.com/amerkoleci/Vortice.Windows

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using win32 = global::Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;
using static Windows.Win32.UI.WindowsAndMessaging.PEEK_MESSAGE_REMOVE_TYPE;
using static Windows.Win32.UI.WindowsAndMessaging.WNDCLASS_STYLES;
using static Windows.Win32.UI.Input.KeyboardAndMouse.VIRTUAL_KEY;
using System;

namespace Vortice;

public abstract partial class Application : IDisposable
{
    public const string WindowClassName = "VorticeWindow";

    private unsafe void PlatformConstruct()
    {
#nullable disable
        var hInstance = GetModuleHandle((string)null);
#nullable restore

        fixed (char* lpszClassName = WindowClassName)
        {
            PCWSTR szCursorName = new((char*)IDC_ARROW);

            var wndClassEx = new WNDCLASSEXW
            {
                cbSize = (uint)Unsafe.SizeOf<WNDCLASSEXW>(),
                style = CS_HREDRAW | CS_VREDRAW | CS_OWNDC,
                lpfnWndProc = &WndProc,
                hInstance = (HINSTANCE)hInstance.DangerousGetHandle(),
                hCursor = LoadCursor((HINSTANCE)IntPtr.Zero, szCursorName),
                hbrBackground = (win32.Graphics.Gdi.HBRUSH)IntPtr.Zero,
                hIcon = (HICON)IntPtr.Zero,
                lpszClassName = lpszClassName
            };

            ushort atom = RegisterClassEx(&wndClassEx);

            if (atom == 0)
            {
                throw new InvalidOperationException(
                    $"Failed to register window class. Error: {Marshal.GetLastWin32Error()}"
                    );
            }
        }

        if (!Headless)
        {
            // Create main window.
            MainWindow = new Window("Vortice");
        }
    }

    private unsafe void PlatformRun()
    {
        InitializeBeforeRun();

        win32.UI.WindowsAndMessaging.MSG msg;

        while (!_exitRequested)
        {
            if (PeekMessage(out msg, default, 0, 0, PM_REMOVE) != false)
            {
                _ = TranslateMessage(&msg);
                _ = DispatchMessage(&msg);

                if (msg.message == WM_QUIT)
                {
                    _exitRequested = true;
                    break;
                }
            }

            Tick();
        }
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static LRESULT WndProc(HWND hWnd, uint message, WPARAM wParam, LPARAM lParam)
    {
        if (message == WM_ACTIVATEAPP)
        {
            if (wParam != 0)
            {
                Current?.OnActivated();
            }
            else
            {
                Current?.OnDeactivated();
            }

            return DefWindowProc(hWnd, message, wParam, lParam);
        }

        switch (message)
        {
            case WM_KEYDOWN:
            case WM_KEYUP:
            case WM_SYSKEYDOWN:
            case WM_SYSKEYUP:
                OnKey(message, wParam, lParam);
                break;

            case WM_DESTROY:
                PostQuitMessage(0);
                break;
        }

        return DefWindowProc(hWnd, message, wParam, lParam);
    }

    private static void OnKey(uint message, nuint wParam, nint lParam)
    {
        if (message == WM_KEYDOWN || message == WM_SYSKEYDOWN)
            Current?.OnKeyboardEvent(ConvertKeyCode(lParam, wParam), true);
        else if (message == WM_KEYUP || message == WM_SYSKEYUP)
            Current?.OnKeyboardEvent(ConvertKeyCode(lParam, wParam), false);
    }

    private static KeyboardKey ConvertKeyCode(nint lParam, nuint wParam)
    {
        uint uWParam = (uint)wParam;
        switch ((VIRTUAL_KEY)uWParam)
        {
            // virtual key codes
            case VK_CLEAR: return KeyboardKey.Clear;
            case VK_MODECHANGE: return KeyboardKey.modeChange;
            case VK_SELECT: return KeyboardKey.Select;
            case VK_EXECUTE: return KeyboardKey.Execute;
            case VK_HELP: return KeyboardKey.Help;
            case VK_PAUSE: return KeyboardKey.Pause;
            case VK_NUMLOCK: return KeyboardKey.numLock;

            case VK_F13: return KeyboardKey.F13;
            case VK_F14: return KeyboardKey.F14;
            case VK_F15: return KeyboardKey.F15;
            case VK_F16: return KeyboardKey.F16;
            case VK_F17: return KeyboardKey.F17;
            case VK_F18: return KeyboardKey.F18;
            case VK_F19: return KeyboardKey.F19;
            case VK_F20: return KeyboardKey.F20;
            case VK_F21: return KeyboardKey.F21;
            case VK_F22: return KeyboardKey.F22;
            case VK_F23: return KeyboardKey.F23;
            case VK_F24: return KeyboardKey.F24;

            case VK_OEM_NEC_EQUAL: return KeyboardKey.NumPadEqual;
            case VK_BROWSER_BACK: return KeyboardKey.Back;
            case VK_BROWSER_FORWARD: return KeyboardKey.forward;
            case VK_BROWSER_REFRESH: return KeyboardKey.refresh;
            case VK_BROWSER_STOP: return KeyboardKey.stop;
            case VK_BROWSER_SEARCH: return KeyboardKey.search;
            case VK_BROWSER_FAVORITES: return KeyboardKey.bookmarks;
            case VK_BROWSER_HOME: return KeyboardKey.Home;
            case VK_VOLUME_MUTE: return KeyboardKey.mute;
            case VK_VOLUME_DOWN: return KeyboardKey.volumeDown;
            case VK_VOLUME_UP: return KeyboardKey.volumeUp;

            case VK_MEDIA_NEXT_TRACK: return KeyboardKey.audioNext;
            case VK_MEDIA_PREV_TRACK: return KeyboardKey.audioPrevious;
            case VK_MEDIA_STOP: return KeyboardKey.audioStop;
            case VK_MEDIA_PLAY_PAUSE: return KeyboardKey.audioPlay;
            case VK_LAUNCH_MAIL: return KeyboardKey.mail;
            case VK_LAUNCH_MEDIA_SELECT: return KeyboardKey.mediaSelect;

            case VK_OEM_102: return KeyboardKey.intlBackslash;

            case VK_ATTN: return KeyboardKey.PrintScreen;
            case VK_CRSEL: return KeyboardKey.crsel;
            case VK_EXSEL: return KeyboardKey.exsel;
            case VK_OEM_CLEAR: return KeyboardKey.Clear;

            case VK_LAUNCH_APP1: return KeyboardKey.app1;
            case VK_LAUNCH_APP2: return KeyboardKey.app2;

            // scan codes
            default:
                {
                    nint scanCode = (lParam >> 16) & 0xFF;
                    if (scanCode <= 127)
                    {
                        bool isExtended = (lParam & (1 << 24)) != 0;

                        switch (scanCode)
                        {
                            case 0x01: return KeyboardKey.Escape;
                            case 0x02: return KeyboardKey.Num1;
                            case 0x03: return KeyboardKey.Num2;
                            case 0x04: return KeyboardKey.Num3;
                            case 0x05: return KeyboardKey.Num4;
                            case 0x06: return KeyboardKey.Num5;
                            case 0x07: return KeyboardKey.Num6;
                            case 0x08: return KeyboardKey.Num7;
                            case 0x09: return KeyboardKey.Num8;
                            case 0x0A: return KeyboardKey.Num9;
                            case 0x0B: return KeyboardKey.Num0;
                            case 0x0C: return KeyboardKey.Minus;
                            case 0x0D: return KeyboardKey.Equal;
                            case 0x0E: return KeyboardKey.Backspace;
                            case 0x0F: return KeyboardKey.Tab;
                            case 0x10: return KeyboardKey.Q;
                            case 0x11: return KeyboardKey.W;
                            case 0x12: return KeyboardKey.e;
                            case 0x13: return KeyboardKey.R;
                            case 0x14: return KeyboardKey.T;
                            case 0x15: return KeyboardKey.Y;
                            case 0x16: return KeyboardKey.U;
                            case 0x17: return KeyboardKey.i;
                            case 0x18: return KeyboardKey.O;
                            case 0x19: return KeyboardKey.P;
                            case 0x1A: return KeyboardKey.LeftBracket;
                            case 0x1B: return KeyboardKey.RightBracket;
                            case 0x1C: return isExtended ? KeyboardKey.NumPadEnter : KeyboardKey.Enter;
                            case 0x1D: return isExtended ? KeyboardKey.rightControl : KeyboardKey.leftControl;
                            case 0x1E: return KeyboardKey.a;
                            case 0x1F: return KeyboardKey.S;
                            case 0x20: return KeyboardKey.d;
                            case 0x21: return KeyboardKey.f;
                            case 0x22: return KeyboardKey.g;
                            case 0x23: return KeyboardKey.h;
                            case 0x24: return KeyboardKey.j;
                            case 0x25: return KeyboardKey.k;
                            case 0x26: return KeyboardKey.l;
                            case 0x27: return KeyboardKey.semicolon;
                            case 0x28: return KeyboardKey.Quote;
                            case 0x29: return KeyboardKey.grave;
                            case 0x2A: return KeyboardKey.leftShift;
                            case 0x2B: return KeyboardKey.Backslash;
                            case 0x2C: return KeyboardKey.Z;
                            case 0x2D: return KeyboardKey.X;
                            case 0x2E: return KeyboardKey.c;
                            case 0x2F: return KeyboardKey.V;
                            case 0x30: return KeyboardKey.b;
                            case 0x31: return KeyboardKey.n;
                            case 0x32: return KeyboardKey.m;
                            case 0x33: return KeyboardKey.comma;
                            case 0x34: return KeyboardKey.period;
                            case 0x35: return isExtended ? KeyboardKey.NumPadDivide : KeyboardKey.slash;
                            case 0x36: return KeyboardKey.rightShift;
                            case 0x37: return isExtended ? KeyboardKey.PrintScreen : KeyboardKey.NumPadMultiply;
                            case 0x38: return isExtended ? KeyboardKey.rightAlt : KeyboardKey.leftAlt;
                            case 0x39: return KeyboardKey.Space;
                            case 0x3A: return isExtended ? KeyboardKey.NumPadPlus : KeyboardKey.CapsLock;
                            case 0x3B: return KeyboardKey.F1;
                            case 0x3C: return KeyboardKey.F2;
                            case 0x3D: return KeyboardKey.F3;
                            case 0x3E: return KeyboardKey.F4;
                            case 0x3F: return KeyboardKey.F5;
                            case 0x40: return KeyboardKey.F6;
                            case 0x41: return KeyboardKey.F7;
                            case 0x42: return KeyboardKey.F8;
                            case 0x43: return KeyboardKey.F9;
                            case 0x44: return KeyboardKey.F10;
                            case 0x45: return KeyboardKey.numLock;
                            case 0x46: return KeyboardKey.scrollLock;
                            case 0x47: return isExtended ? KeyboardKey.Home : KeyboardKey.NumPad7;
                            case 0x48: return isExtended ? KeyboardKey.Up : KeyboardKey.NumPad8;
                            case 0x49: return isExtended ? KeyboardKey.PageUp : KeyboardKey.NumPad9;
                            case 0x4A: return KeyboardKey.NumPadMinus;
                            case 0x4B: return isExtended ? KeyboardKey.Left : KeyboardKey.NumPad4;
                            case 0x4C: return KeyboardKey.NumPad5;
                            case 0x4D: return isExtended ? KeyboardKey.Right : KeyboardKey.NumPad6;
                            case 0x4E: return KeyboardKey.NumPadPlus;
                            case 0x4F: return isExtended ? KeyboardKey.End : KeyboardKey.NumPad1;
                            case 0x50: return isExtended ? KeyboardKey.Down : KeyboardKey.NumPad2;
                            case 0x51: return isExtended ? KeyboardKey.PageDown : KeyboardKey.NumPad3;
                            case 0x52: return isExtended ? KeyboardKey.Insert : KeyboardKey.NumPad0;
                            case 0x53: return isExtended ? KeyboardKey.Del : KeyboardKey.NumPadDecimal;
                            case 0x54: return KeyboardKey.None;
                            case 0x55: return KeyboardKey.None;
                            case 0x56: return KeyboardKey.intlBackslash;
                            case 0x57: return KeyboardKey.F11;
                            case 0x58: return KeyboardKey.F12;
                            case 0x59: return KeyboardKey.Pause;
                            case 0x5A: return KeyboardKey.None;
                            case 0x5B: return KeyboardKey.LeftSuper;
                            case 0x5C: return KeyboardKey.RightSuper;
                            case 0x5D: return KeyboardKey.Menu;
                            case 0x5E: return KeyboardKey.None;
                            case 0x5F: return KeyboardKey.None;
                            case 0x60: return KeyboardKey.None;
                            case 0x61: return KeyboardKey.None;
                            case 0x62: return KeyboardKey.None;
                            case 0x63: return KeyboardKey.None;
                            case 0x64: return KeyboardKey.F13;
                            case 0x65: return KeyboardKey.F14;
                            case 0x66: return KeyboardKey.F15;
                            case 0x67: return KeyboardKey.F16;
                            case 0x68: return KeyboardKey.F17;
                            case 0x69: return KeyboardKey.F18;
                            case 0x6A: return KeyboardKey.F19;
                            case 0x6B: return KeyboardKey.None;
                            case 0x6C: return KeyboardKey.None;
                            case 0x6D: return KeyboardKey.None;
                            case 0x6E: return KeyboardKey.None;
                            case 0x6F: return KeyboardKey.None;
                            case 0x70: return KeyboardKey.katakanaHiragana;
                            case 0x71: return KeyboardKey.None;
                            case 0x72: return KeyboardKey.None;
                            case 0x73: return KeyboardKey.Ro;
                            case 0x74: return KeyboardKey.None;
                            case 0x75: return KeyboardKey.None;
                            case 0x76: return KeyboardKey.None;
                            case 0x77: return KeyboardKey.None;
                            case 0x78: return KeyboardKey.None;
                            case 0x79: return KeyboardKey.henkan;
                            case 0x7A: return KeyboardKey.None;
                            case 0x7B: return KeyboardKey.muhenkan;
                            case 0x7C: return KeyboardKey.None;
                            case 0x7D: return KeyboardKey.Yen;
                            case 0x7E: return KeyboardKey.None;
                            case 0x7F: return KeyboardKey.None;
                            default: return KeyboardKey.None;
                        }
                    }
                    else
                        return KeyboardKey.None;
                }
        }
    }
}
