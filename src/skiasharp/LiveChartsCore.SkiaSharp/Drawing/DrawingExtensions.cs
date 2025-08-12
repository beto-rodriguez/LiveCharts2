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
using System.Linq;
using System.Text;
using LiveChartsCore.Drawing;
using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace LiveChartsCore.SkiaSharpView.Drawing;

internal static class DrawingExtensions
{
    internal static readonly PositionedBlob s_newLine = new(SKTextBlob.Create(string.Empty, new()), -1);
    private static readonly Dictionary<string, SKShaper> s_knownShapers = [];

    internal static void DrawBlobArray(
        this SKCanvas canvas, BlobArray blobArray, BlobArraySettings settings, float x, float y, SKPaint paint)
    {
        var size = blobArray.Size;

        var horizontalAlign = settings.Horizontal switch
        {
            Align.Start => 0f,
            Align.Middle => -size.Width / 2f,
            Align.End => -size.Width,
            _ => throw new ArgumentOutOfRangeException(nameof(settings.Horizontal), settings.Horizontal, null)
        };

        var verticalAlign = settings.Vertical switch
        {
            Align.Start => 0f,
            Align.Middle => -size.Height / 2f,
            Align.End => -size.Height,
            _ => throw new ArgumentOutOfRangeException(nameof(settings.Vertical), settings.Vertical, null)
        };

        var alignedX = x + horizontalAlign;
        var alignedY = y + verticalAlign;

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
            canvas.DrawRect(alignedX, alignedY, size.Width, size.Height, debugPaint);
            canvas.DrawRect(x - 4f, y - 4f, 8, 8, debugPaint2);
        }
#endif

        if (settings.Background != LvcColor.Empty)
        {
            var bg = settings.Background;

            var c = new SKColor(bg.R, bg.G, bg.B, (byte)(bg.A * settings.Opacity));
            using var bgPaint = new SKPaint { Color = c };

            // ToDo draw background rectangle
        }

        foreach (var pb in blobArray.Blobs)
        {
            if (pb == s_newLine)
                continue;

            // ToDo Align text.
            var blobPosition = pb.Position;

            canvas.DrawText(
                pb.Blob,
                alignedX + blobPosition.X,
                alignedY + blobPosition.Y, paint);
        }
    }

    internal static BlobArray AsBlobArray(this string text, SKFont font, SKPaint paint, float maxWidth, Padding padding) =>
        new(
            font.Metrics,
            maxWidth,
            padding,
            [.. Tokenize(text).Select(token => AsPositionedBlob(token, font, paint))]);

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

    private static IEnumerable<string> Tokenize(string text)
    {
        var sb = new StringBuilder();

        foreach (var @char in text)
        {
            if (@char is '\t' or '\r')
                continue;

            _ = sb.Append(@char);

            if (char.IsWhiteSpace(@char) && sb.Length > 0)
            {
                yield return sb.ToString();
                _ = sb.Clear();
            }
        }

        if (sb.Length > 0)
            yield return sb.ToString();
    }

    internal class BlobArray(
        SKFontMetrics metrics, float maxWidth, Padding padding, PositionedBlob[] blobs)
    {
        private bool _hasSize;
        public SKFontMetrics Metrics { get; } = metrics;
        public PositionedBlob[] Blobs { get; set; } = blobs;
        public float MaxWidth { get; } = maxWidth;
        public Padding Padding { get; } = padding;

        public LvcSize Size
        {
            get
            {
                if (_hasSize)
                    return field;

                var x = 0f;
                var y = 0f;
                var knownWidth = 0f;
                var knownHeight = 0f;
                var lineHeight = Metrics.Descent - Metrics.Ascent + Metrics.Leading;

                foreach (var pb in Blobs)
                {
                    var b = pb.Blob;
                    var w = pb.Width;

                    if (x + w > MaxWidth || pb == s_newLine)
                    {
                        x = 0;
                        y += lineHeight;
                        knownHeight = y;
                    }

                    pb.Position = new SKPoint(x + Padding.Left, y + Padding.Top - Metrics.Ascent);
                    x += w;

                    if (x > knownWidth)
                        knownWidth = x;
                }

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
        public SKPoint Position { get; set; }
        public float Width { get; } = width;
        public SKTextBlob Blob { get; } = blob;
#if DEBUG
        public string Text { get; set; } = string.Empty;
#endif
    }

    internal struct BlobArraySettings(
        Align horizontal, Align vertical, LvcColor background, Padding padding, LvcSize size, float opacity)
    {
        public Align Horizontal { get; set; } = horizontal;
        public Align Vertical { get; set; } = vertical;
        public LvcColor Background { get; set; } = background;
        public Padding Padding { get; set; } = padding;
        public LvcSize Size { get; set; } = size;
        public float Opacity { get; set; } = opacity;
    }
}
