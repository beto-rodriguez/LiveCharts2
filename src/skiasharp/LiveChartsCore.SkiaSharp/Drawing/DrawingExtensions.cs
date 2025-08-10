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
using System.Collections.Generic;
using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace LiveChartsCore.SkiaSharpView.Drawing;

internal static class DrawingExtensions
{
    private static readonly Dictionary<string, bool> s_knownFontShaping = [];
    private static readonly Dictionary<string, SKShaper> s_knownShapers = [];

    internal static SKTextBlob AsTextBlob(this SKFont font, SKPaint paint, string text)
    {
        var typeface = font.Typeface;

        if (typeface is null)
            return SKTextBlob.Create(text, font);

        if (!s_knownFontShaping.TryGetValue(typeface.FamilyName, out var requiresShaping))
        {
            // Fonts that require shaping (Arabic, Devanagari, Thai, etc.) must include GSUB and/or GPOS

            // Update:
            // SkiaSharp default in windows uses Segoe UI which have these tables
            // it means it is always shaping on windows, but things are cached a lot, so we should be fine
            // also this will improve the developer experience because non-lating will be rendered correctly
            // without the need to set the `HasGlobalSKTypeface` in the `LiveCharts.Configure`.

            const uint GSUB = 0x47535542;
            const uint GPOS = 0x47504F53;

            var hasGSUB = typeface.GetTableData(GSUB)?.Length > 0;
            var hasGPOS = typeface.GetTableData(GPOS)?.Length > 0;

            requiresShaping = hasGSUB || hasGPOS;

            if (LiveChartsSkiaSharp.UserFontShaping.TryGetValue(typeface.FamilyName, out var userShaping))
                requiresShaping = userShaping;

            s_knownFontShaping[typeface.FamilyName] = requiresShaping;
        }

        if (!requiresShaping)
            return SKTextBlob.Create(text, font);

        if (!s_knownShapers.TryGetValue(typeface.FamilyName, out var shaper))
        {
            shaper = new SKShaper(typeface);
            s_knownShapers[typeface.FamilyName] = shaper;
        }

        var result = shaper.Shape(text, paint);
        var glyphs = Array.ConvertAll(result.Codepoints, cp => (ushort)cp);

        var builder = new SKTextBlobBuilder();
        var runBuffer = builder.AllocatePositionedRun(
            new SKFont(typeface, font.Size),
            glyphs.Length
        );

        runBuffer.SetGlyphs(glyphs);
        runBuffer.SetPositions(result.Points);

        return builder.Build();
    }
}
