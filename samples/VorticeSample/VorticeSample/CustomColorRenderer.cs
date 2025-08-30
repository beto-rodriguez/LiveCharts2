using System.Numerics;
using System.Runtime.InteropServices;
using SharpGen.Runtime;
using Vortice.DCommon;
using Vortice.Direct2D1;
using Vortice.DirectWrite;
// Copyright (c) Amer Koleci and contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// https://github.com/amerkoleci/Vortice.Windows

namespace VorticeSample;

public class CustomColorRenderer(ID2D1RenderTarget renderTarget, ID2D1SolidColorBrush defaultBrush)
    : TextRendererBase
{
    private readonly ID2D1RenderTarget _renderTarget = renderTarget;
    private readonly ID2D1SolidColorBrush _defaultBrush = defaultBrush;

    /// <inheritdoc/>
    public override void DrawGlyphRun(IntPtr clientDrawingContext, float baselineOriginX, float baselineOriginY, MeasuringMode measuringMode, GlyphRun glyphRun, GlyphRunDescription glyphRunDescription, IUnknown clientDrawingEffect)
    {
        if (clientDrawingEffect is ComObject comObject)
        {
            var drawingEffect = (ColorDrawingEffect)Marshal.GetObjectForIUnknown(comObject.NativePointer);

            using var brush = _renderTarget.CreateSolidColorBrush(drawingEffect.Color);
            _renderTarget.DrawGlyphRun(
                new Vector2(baselineOriginX, baselineOriginY),
                glyphRun,
                brush,
                measuringMode);
        }
        else
        {
            _renderTarget.DrawGlyphRun(
                new Vector2(baselineOriginX, baselineOriginY),
                glyphRun,
                _defaultBrush,
                measuringMode);
        }
    }
}
