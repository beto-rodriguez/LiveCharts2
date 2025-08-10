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

using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using HarfBuzzSharp;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing;

internal static class DrawingExtensions
{
    private static readonly Dictionary<string, bool> s_knownFontShaping = [];
    private static readonly Dictionary<string, Font> s_knownHBFonts = [];

    internal static SKTextBlob AsTextBlob(this SKFont font, string text)
    {
        var typeface = font.Typeface;

        if (typeface is null)
            return SKTextBlob.Create(text, font);

        if (!s_knownFontShaping.TryGetValue(typeface.FamilyName, out var requiresShaping))
        {
            // Fonts that require shaping (Arabic, Devanagari, Thai, etc.) must include GSUB and/or GPOS
            const uint GSUB = 0x47535542;
            const uint GPOS = 0x47504F53;

            var hasGSUB = typeface.GetTableData(GSUB)?.Length > 0;
            var hasGPOS = typeface.GetTableData(GPOS)?.Length > 0;

            requiresShaping = hasGSUB || hasGPOS;

            if (LiveChartsSkiaSharp.UserFontShaping.TryGetValue(typeface.FamilyName, out var userShaping))
                requiresShaping = userShaping;

            s_knownFontShaping[typeface.FamilyName] = requiresShaping;
        }

        if (!s_knownHBFonts.TryGetValue(typeface.FamilyName, out var hbFont))
        {
            hbFont = typeface.AsHarfBuzzFont();
            s_knownHBFonts[typeface.FamilyName] = hbFont;
        }

        return requiresShaping
            ? GetShapedBlob(text, font, hbFont)
            : SKTextBlob.Create(text, font);
    }

    internal static SKTextBlob GetShapedBlob(string text, SKFont skFont, Font hbFont)
    {
        var buffer = new Buffer();
        buffer.AddUtf8(text);
        buffer.GuessSegmentProperties(); // Detect script, direction, language

        hbFont.Shape(buffer);

        var glyphInfos = buffer.GlyphInfos;
        var glyphPositions = buffer.GlyphPositions;

        var count = glyphInfos.Length;
        var glyphs = new ushort[count];
        var positions = new SKPoint[count];

        float x = 0;
        float y = 0;

        for (var i = 0; i < count; i++)
        {
            glyphs[i] = (ushort)glyphInfos[i].Codepoint;

            var xOffset = glyphPositions[i].XOffset / 64f;
            var yOffset = glyphPositions[i].YOffset / 64f;
            var xAdvance = glyphPositions[i].XAdvance / 64f;
            var yAdvance = glyphPositions[i].YAdvance / 64f;

            positions[i] = new SKPoint(x + xOffset, y + yOffset);

            x += xAdvance;
            y += yAdvance;
        }

        var builder = new SKTextBlobBuilder();
        var runBuffer = builder.AllocatePositionedRun(skFont, count);

        runBuffer.SetGlyphs(glyphs);
        runBuffer.SetPositions(positions);

        return builder.Build();
    }

    internal static Font AsHarfBuzzFont(this SKTypeface typeface)
    {
        // Extract font data from SKTypeface
        using var skStream = typeface.OpenStream();
        using var ms = new MemoryStream();
        var buffer = new byte[4096];
        int bytesRead;

        while ((bytesRead = skStream.Read(buffer, buffer.Length)) > 0)
            ms.Write(buffer, 0, bytesRead);

        var fontData = ms.ToArray();

        // Allocate unmanaged memory
        var unmanagedPtr = Marshal.AllocHGlobal(fontData.Length);
        Marshal.Copy(fontData, 0, unmanagedPtr, fontData.Length);

        // Create HarfBuzz blob with cleanup delegate
        var hbBlob = new Blob(
            unmanagedPtr, fontData.Length, MemoryMode.ReadOnly, () => Marshal.FreeHGlobal(unmanagedPtr));

        // Create face and font
        var hbFace = new Face(hbBlob, 0);
        var hbFont = new Font(hbFace);

        return hbFont;
    }
}
