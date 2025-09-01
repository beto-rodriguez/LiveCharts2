// Copyright (c) Amer Koleci and contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// https://github.com/amerkoleci/Vortice.Windows

using System.Numerics;
using SharpGen.Runtime;
using Vortice.DCommon;
using Vortice.Direct2D1;
using Vortice.DirectWrite;
using Vortice.Mathematics;

namespace Vortice;

public class AppTextRenderer : TextRendererBase
{
    private readonly ID2D1RenderTarget _renderTarget;
    private readonly ID2D1Brush _defaultBrush;

    public AppTextRenderer(ID2D1RenderTarget renderTarget)
    {
        _renderTarget = renderTarget;
        _defaultBrush = _renderTarget.CreateSolidColorBrush(new Color4(0, 0, 0, 1));
    }

    public ID2D1Brush? ActiveBrush { get; set; }

    /// <inheritdoc/>
    public override void DrawGlyphRun(
        nint clientDrawingContext,
        float baselineOriginX,
        float baselineOriginY,
        MeasuringMode measuringMode,
        GlyphRun glyphRun,
        GlyphRunDescription glyphRunDescription,
        IUnknown clientDrawingEffect)
    {
        _renderTarget.DrawGlyphRun(
            new Vector2(baselineOriginX, baselineOriginY),
            glyphRun,
            ActiveBrush ?? _defaultBrush,
            measuringMode);
    }
}
