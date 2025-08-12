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
using System.Diagnostics;
using System.Linq;
using System.Text;
using LiveChartsCore.Drawing;
using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace LiveChartsCore.SkiaSharpView.Drawing;

internal static class DrawingTextExtensions
{
    internal static readonly PositionedBlob s_newLine = new(SKTextBlob.Create(string.Empty, new()), -1);
    private static readonly Dictionary<string, SKShaper> s_knownShapers = [];

    internal static void DrawBlobArray(
        this SKCanvas canvas, BlobArray blobArray, BlobArraySettings settings, float x, float y, SKPaint paint)
    {
        var size = blobArray.Size;

        // relative horizontal alignment, the alignment is relative to the x and y coordinates
        var rha = settings.GetHorizontalAlign(size);

        // relative vertical alignment, the alignment is relative to the x and y coordinates
        var rva = settings.GetVerticalAlign(size);

        // relative aligned x and y coordinates
        var rax = x + rha;
        var ray = y + rva;

#if DEBUG
        if (BaseLabelGeometry.ShowDebugLines)
        {
            using var debugPaint = new SKPaint
            {
                Color = SKColors.Gray.WithAlpha(50),
                Style = SKPaintStyle.Fill,
                StrokeWidth = 3
            };
            using var debugPaint2 = new SKPaint
            {
                Color = SKColors.Red.WithAlpha(100),
                Style = SKPaintStyle.Fill,
                StrokeWidth = 3
            };

            // draws the rectangle where the text will be drawn
            canvas.DrawRect(rax, ray, size.Width, size.Height, debugPaint);

            // highlights the relative alignenment, this draws the original coordinates
            canvas.DrawRect(x - 4f, y - 4f, 8, 8, debugPaint2);
        }
#endif

        if (settings.Background != LvcColor.Empty)
        {
            var bg = settings.Background;

            var c = new SKColor(bg.R, bg.G, bg.B, (byte)(bg.A * settings.Opacity));
            using var bgPaint = new SKPaint { Color = c };

            canvas.DrawRect(rax, ray, size.Width, size.Height, bgPaint);
        }

        var horizontalPadding = blobArray.Padding.Left + blobArray.Padding.Right;
        var lao = 0f;

        foreach (var pb in blobArray.Blobs)
        {
            if (pb == s_newLine)
                continue;

            var blobPosition = pb.Position;

            // line alignmen offset.
            if (blobArray.IsRTL)
                lao = size.Width - horizontalPadding - blobArray.LineWidths[pb.Line];

            canvas.DrawText(
            pb.Blob,
            rax + blobPosition.X + lao,
            ray + blobPosition.Y, paint);
        }
    }

    internal static BlobArray AsBlobArray(this string text, SKFont font, SKPaint paint, float maxWidth, Padding padding)
    {
        var tokens = Tokenize(text, font, out var isRLT, out var suggestedTypeface);

        if (suggestedTypeface is not null)
        {
            // a suggested typeface is found when the current typeface does not support
            // the characters in the text, as a default we will use this typeface
            // as the global typeface in the library, the user can override this
            // by defining its own typeface globally or per paint instance.

            LiveChartsSkiaSharp.DefaultSKTypeface ??= suggestedTypeface;
            font.Typeface = suggestedTypeface;
            paint.Typeface = suggestedTypeface;
        }

        return new BlobArray(
            font, maxWidth, padding, isRLT, [.. tokens.Select(token => AsPositionedBlob(token, font, paint))]);
    }

    private static PositionedBlob AsPositionedBlob(string text, SKFont font, SKPaint paint)
    {
        if (text == "\n")
            return s_newLine;

        var typeface = font.Typeface ??
            throw new Exception("A Typeface is required at this point.");

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

        return new(builder.Build(), result.Width)
        {
#if DEBUG
            Text = text
#endif
        };
    }

    private static string[] Tokenize(string text, SKFont currentFont, out bool isRLT, out SKTypeface? suggestedTypeface)
    {
        var tokens = new List<string>(text.Length);
        var sb = new StringBuilder();
        isRLT = false;
        suggestedTypeface = null;

        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            int codepoint;

            // custom "GetRunes", we cant use System.Text.Rune in net462 or netstandard2.0
            if (char.IsHighSurrogate(c) && i + 1 < text.Length && char.IsLowSurrogate(text[i + 1]))
            {
                codepoint = char.ConvertToUtf32(c, text[i + 1]);
                i++; // Skip low surrogate
            }
            else
            {
                codepoint = c;
            }

            // Append full character (could be surrogate pair)
            _ = sb.Append(char.ConvertFromUtf32(codepoint));

            // is RTL?
            if (IsRTL(codepoint))
                isRLT = true;

            // is char supported by the typeface?
            if (suggestedTypeface is null && !currentFont.Typeface.ContainsGlyph(codepoint))
            {
                suggestedTypeface = SKFontManager.Default.MatchCharacter(codepoint);
#if DEBUG
                if (suggestedTypeface is not null)
                {
                    Trace.WriteLine(
                        $"[LiveCharts] Missing glyph for U+{codepoint:X}, " +
                        $"{suggestedTypeface.FamilyName} will be used as a fallback," +
                        $"You could instead configure LiveCharts with your own typeface.");
                }
#endif
            }

            // Token boundary
            if (IsTokenBoundary(codepoint) && sb.Length > 0)
            {
                tokens.Add(sb.ToString());
                _ = sb.Clear();
            }
        }

        if (sb.Length > 0)
            tokens.Add(sb.ToString());

        return [.. tokens];
    }

    private static bool IsTokenBoundary(int codepoint)
    {
        // Line breaks
        if (codepoint is '\n' or '\r')
            return true;

        // Other whitespace
        return char.IsWhiteSpace(char.ConvertFromUtf32(codepoint), 0);
    }

    private static bool IsRTL(int codepoint)
    {
        return codepoint is
            // Core RTL scripts
            (>= 0x0590 and <= 0x05FF) or // Hebrew
            (>= 0x0600 and <= 0x06FF) or // Arabic
            (>= 0x0700 and <= 0x074F) or // Syriac
            (>= 0x0750 and <= 0x077F) or // Arabic Supplement
            (>= 0x0780 and <= 0x07BF) or // Thaana
            (>= 0x07C0 and <= 0x07FF) or // N'Ko
            (>= 0x0800 and <= 0x083F) or // Samaritan
            (>= 0x0840 and <= 0x085F) or // Mandaic
            (>= 0x08A0 and <= 0x08FF) or // Arabic Extended-A

            // RTL presentation forms
            (>= 0xFB1D and <= 0xFDFF) or // Hebrew and Arabic presentation forms
            (>= 0xFE70 and <= 0xFEFC) or // Arabic presentation forms-B

            // RTL formatting characters
            0x200F or // Right-to-Left Mark
            0x202B or // Right-to-Left Embedding
            0x202E or // Right-to-Left Override

            // Optional: Rumi Numeral Symbols (used in Arabic contexts)
            (>= 0x10E60 and <= 0x10E7F);
    }

    internal class BlobArray(
        SKFont font, float maxWidth, Padding padding, bool isRTL, PositionedBlob[] blobs)
    {
        private bool _hasSize;
        public PositionedBlob[] Blobs = blobs;
        public bool IsRTL = isRTL;
        public SKTypeface? FallbackTypeface;
        public List<float> LineWidths = [];
        public SKFont Font { get; } = font;
        public float MaxWidth { get; } = maxWidth;
        public Padding Padding { get; } = padding;

        public LvcSize Size
        {
            get
            {
                if (_hasSize)
                    return field;

                var lineCount = 0;
                var x = 0f;
                var y = 0f;
                var knownWidth = 0f;
                var knownHeight = 0f;
                var metrics = Font.Metrics;
                var lineHeight = metrics.Descent - metrics.Ascent + metrics.Leading;

                foreach (var pb in Blobs)
                {
                    pb.Line = lineCount;
                    var b = pb.Blob;
                    var w = pb.Width;

                    if (x + w > MaxWidth || pb == s_newLine)
                    {
                        lineCount++;
                        LineWidths.Add(x); // Store the width of the line, so we can use it later for alignment.
                        x = 0;
                        y += lineHeight;
                        knownHeight = y;
                    }

                    pb.Position = new SKPoint(x + Padding.Left, y + Padding.Top - metrics.Ascent);
                    x += w;

                    if (x > knownWidth)
                        knownWidth = x;
                }

                LineWidths.Add(x);
                knownHeight += lineHeight;

                _hasSize = true;

                field = new LvcSize(
                    width: knownWidth + Padding.Left + Padding.Right,
                    height: knownHeight + Padding.Top + Padding.Bottom);

                return field;
            }
        } = new();
    }

    internal class PositionedBlob(SKTextBlob blob, float width)
    {
        public int Line;
        public SKPoint Position;
        public float Width { get; } = width;
        public SKTextBlob Blob { get; } = blob;
#if DEBUG
        public string Text = string.Empty;
#endif
    }

    internal readonly struct BlobArraySettings(
        Align horizontal, Align vertical, LvcColor background, Padding padding, LvcSize size, float opacity)
    {
        public Align Horizontal { get; } = horizontal;
        public Align Vertical { get; } = vertical;
        public LvcColor Background { get; } = background;
        public Padding Padding { get; } = padding;
        public LvcSize Size { get; } = size;
        public float Opacity { get; } = opacity;

        public float GetHorizontalAlign(LvcSize size)
        {
            return Horizontal switch
            {
                Align.Start => 0f,
                Align.Middle => -size.Width / 2f,
                Align.End => -size.Width,
                _ => throw new ArgumentOutOfRangeException(nameof(Align), Horizontal, null)
            };
        }

        public float GetVerticalAlign(LvcSize size)
        {
            return Vertical switch
            {
                Align.Start => 0f,
                Align.Middle => -size.Height / 2f,
                Align.End => -size.Height,
                _ => throw new ArgumentOutOfRangeException(nameof(Vertical), Vertical, null)
            };
        }
    }
}
