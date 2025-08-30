// Copyright (c) Amer Koleci and contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// https://github.com/amerkoleci/Vortice.Windows

using System.Drawing;
using System.Numerics;
using Vortice;
using Vortice.Direct2D1;
using Vortice.DirectWrite;
using Vortice.Mathematics;
using static Vortice.Direct2D1.D2D1;
using static Vortice.DirectWrite.DWrite;

namespace VorticeSample;

public class TestApplication : Application
{
    private static readonly string s_introText =
        @"Hello from Vortice, this is a long text to show some more advanced features like paragraph alignment, custom drawing...";

    private readonly ID2D1Factory1 _d2dFactory;
    private readonly IDWriteFactory1 _dwriteFactory;
    private readonly CustomColorRenderer _textRenderer;

    private ID2D1HwndRenderTarget _renderTarget = null!;

    private readonly IDWriteTextFormat _textFormat = null!;
    private readonly IDWriteTextLayout _textLayout = null!;

    // Various brushes for our example
    private ID2D1SolidColorBrush? _backgroundBrush;
    private ID2D1SolidColorBrush? _defaultBrush;
    private ID2D1SolidColorBrush? _redBrush;

    private ColorDrawingEffect? _greenDrawingEffect;

    private readonly Color4 _bgcolor = new(0.1f, 0.1f, 0.1f, 1.0f);

    //This is the offset where we start our text layout
    private Vector2 _offset = new(202.0f, 250.0f);
    private RectangleF _fullTextBackground;
    private RectangleF _textRegionRect;

    public TestApplication()
        : base(false)
    {
        _d2dFactory = D2D1CreateFactory<ID2D1Factory1>();
        _dwriteFactory = DWriteCreateFactory<IDWriteFactory1>();

        CreateResources();
        _textRenderer = new CustomColorRenderer(_renderTarget!, _defaultBrush!);

        _textFormat = _dwriteFactory.CreateTextFormat("Arial", FontWeight.Regular, FontStyle.Normal, 16.0f);
        _textLayout = _dwriteFactory.CreateTextLayout(s_introText, _textFormat, 300.0f, 200.0f);

        // Apply various modifications to text
        _textLayout.SetUnderline(true, new TextRange(0, 5));
        _textLayout.SetDrawingEffect(_greenDrawingEffect, new TextRange(10, 20));
        _textLayout.SetFontSize(24.0f, new TextRange(6, 4));
        _textLayout.SetFontFamilyName("Comic Sans MS", new TextRange(11, 7));

        //Measure full layout
        var textSize = _textLayout.Metrics;
        _fullTextBackground = new(textSize.Left + _offset.X, textSize.Top + _offset.Y, textSize.Width, textSize.Height);

        var metrics = _textLayout.HitTestTextRange(53, 4, 0.0f, 0.0f)[0];
        _textRegionRect = new(metrics.Left + _offset.X, metrics.Top + _offset.Y, metrics.Width, metrics.Height);
    }

    public override void Dispose()
    {
        _defaultBrush?.Dispose();
        _redBrush?.Dispose();
        _backgroundBrush?.Dispose();

        _textRenderer.Dispose();

        _renderTarget.Dispose();
        _d2dFactory.Dispose();
        _dwriteFactory.Dispose();
    }

    private void CreateResources()
    {
        _renderTarget?.Dispose();
        _defaultBrush?.Dispose();
        _redBrush?.Dispose();
        _backgroundBrush?.Dispose();

        HwndRenderTargetProperties wtp = new()
        {
            Hwnd = MainWindow!.Handle,
            PixelSize = MainWindow!.ClientSize,
            PresentOptions = PresentOptions.Immediately
        };
        _renderTarget = _d2dFactory.CreateHwndRenderTarget(
            new RenderTargetProperties(),
            wtp);

        _defaultBrush = _renderTarget.CreateSolidColorBrush(Colors.White);
        _redBrush = _renderTarget.CreateSolidColorBrush(Colors.Red);
        _backgroundBrush = _renderTarget.CreateSolidColorBrush(new Color4(0.3f, 0.3f, 0.3f, 0.5f));

        _greenDrawingEffect = new ColorDrawingEffect(Colors.Green);

        //_textRenderer.AssignResources(renderTarget, defaultBrush);
    }

    protected override void InitializeBeforeRun()
    {
    }

    protected override void OnKeyboardEvent(KeyboardKey key, bool pressed)
    {
    }

    protected override void OnDraw(int width, int height)
    {
        _renderTarget.BeginDraw();
        _renderTarget.Clear(_bgcolor);

        _renderTarget.FillRectangle(_fullTextBackground, _backgroundBrush);
        _renderTarget.FillRectangle(_textRegionRect, _redBrush);

        _textLayout.Draw(_textRenderer, _offset.X, _offset.Y);

        try
        {
            _ = _renderTarget.EndDraw();
        }
        catch
        {
            CreateResources();
        }
    }
}
