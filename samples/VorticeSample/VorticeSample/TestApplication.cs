// Copyright (c) Amer Koleci and contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// https://github.com/amerkoleci/Vortice.Windows

using LiveChartsCore.Kernel;
using Vortice;
using Vortice.Direct2D1;
using Vortice.DirectWrite;
using static Vortice.Direct2D1.D2D1;
using static Vortice.DirectWrite.DWrite;

namespace VorticeSample;

public class TestApplication : Application
{
    private readonly ID2D1Factory1 _d2dFactory;
    private ID2D1HwndRenderTarget _renderTarget = null!;

    public TestApplication()
        : base(false)
    {
        _d2dFactory = D2D1CreateFactory<ID2D1Factory1>();
        WriteFactory = DWriteCreateFactory<IDWriteFactory1>();

        CreateResources();
        TextRenderer = new AppTextRenderer(_renderTarget!);
    }

    private List<IMyUIFrameworkControl> Controls { get; } = [];

    public void AddControl(IMyUIFrameworkControl control) =>
        Controls.Add(control);

    public override void Dispose()
    {
        TextRenderer.Dispose();

        _renderTarget.Dispose();
        _d2dFactory.Dispose();
        WriteFactory.Dispose();
    }

    private void CreateResources()
    {
        _renderTarget?.Dispose();

        HwndRenderTargetProperties wtp = new()
        {
            Hwnd = MainWindow!.Handle,
            PixelSize = MainWindow!.ClientSize,
            PresentOptions = PresentOptions.Immediately
        };

        _renderTarget = _d2dFactory.CreateHwndRenderTarget(new RenderTargetProperties(), wtp);
    }

    protected override void InitializeBeforeRun()
    { }

    protected override void OnKeyboardEvent(KeyboardKey key, bool pressed)
    { }

    private (int w, int h) _lastSize = (-1, -1);

    protected override void OnDraw(int width, int height)
    {
        if (_lastSize.w != width || _lastSize.h != height)
        {
            MeasureLayout();
            _lastSize = (width, height);
        }

        _renderTarget.BeginDraw();

        foreach (var control in Controls)
            control.DrawFrame(_renderTarget);

        try
        {
            _ = _renderTarget.EndDraw();
        }
        catch
        {
            CreateResources();
        }
    }

    private void MeasureLayout()
    {
        foreach (var control in Controls)
            control.Measure();
    }
}
