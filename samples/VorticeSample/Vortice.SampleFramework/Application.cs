// Copyright (c) Amer Koleci and contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// https://github.com/amerkoleci/Vortice.Windows

using System;

namespace Vortice;

public abstract partial class Application : IDisposable
{
    //private bool _paused;
    private bool _exitRequested;

    protected IGraphicsDevice? _graphicsDevice;


    protected Application(bool headless = false)
    {
        Headless = headless;
        Current = this;

        PlatformConstruct();
    }

    public static Application? Current { get; private set; }

    public bool Headless { get; }

    public Window? MainWindow { get; private set; }

    public virtual void Dispose()
    {
        _graphicsDevice?.Dispose();
    }

    protected virtual void InitializeBeforeRun()
    {
    }

    public void Tick()
    {
        if (_graphicsDevice != null)
        {
            _graphicsDevice.DrawFrame(OnDraw);
        }
        else
        {
            OnDraw(MainWindow!.ClientSize.Width, MainWindow.ClientSize!.Height);
        }
    }

    public void Run()
    {
        PlatformRun();
    }

    protected virtual void OnActivated()
    {
    }

    protected virtual void OnDeactivated()
    {
    }

    protected virtual void OnDraw(int width, int height)
    {
    }

    protected virtual void OnKeyboardEvent(KeyboardKey key, bool pressed)
    {

    }
}
