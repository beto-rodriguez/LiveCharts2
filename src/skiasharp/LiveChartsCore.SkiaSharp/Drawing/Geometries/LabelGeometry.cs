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
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <inheritdoc cref="BaseLabelGeometry" />
public class LabelGeometry : BaseLabelGeometry, IDrawnElement<SkiaSharpDrawingContext>
{
    internal float _maxTextHeight = 0f;
    internal SKTextBlob[] _lines = [];
    private string _previousText = string.Empty;
    private double _previousTextSize = 0f;
    private double _previousTextHeight = 0f;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelGeometry"/> class.
    /// </summary>
    public LabelGeometry()
    {
        TransformOrigin = new LvcPoint(0f, 0f);
    }

    /// <inheritdoc cref="IDrawnElement{TDrawingContext}.Draw(TDrawingContext)" />
    public virtual void Draw(SkiaSharpDrawingContext context)
    {
        var p = Padding;
        var bg = Background;

        var paint = (((SkiaPaint?)Paint)?.UpdateSkiaPaint(context, this))
            ?? throw new Exception("A paint is required to draw a label.");
        var size = Measure();
        var lines = GetLinesOrCached();

        var isFirstLine = true;

        var verticalPos =
            lines.Length > 1
                ? VerticalAlign switch // respect alignment on multiline labels
                {
                    Align.Start => 0,
                    Align.Middle => -lines.Length * _maxTextHeight * 0.5f,
                    Align.End => -lines.Length * _maxTextHeight,
                    _ => 0
                }
                : 0;

        foreach (var line in lines)
        {
            var textBounds = line.Bounds;

            var lhd = (textBounds.Height * LineHeight - _maxTextHeight) * 0.5f;
            var ao = GetAlignmentOffset(textBounds);

            if (isFirstLine && bg != LvcColor.Empty)
            {
                var c = new SKColor(bg.R, bg.G, bg.B, (byte)(bg.A * Opacity));
                using var bgPaint = new SKPaint { Color = c };

                context.Canvas.DrawRect(
                    X + ao.X, Y + ao.Y - textBounds.Height, size.Width, size.Height, bgPaint);
            }

            var x = (float)Math.Round(X + ao.X + p.Left);
            var y = (float)Math.Round(Y + ao.Y + p.Top + lhd + verticalPos);

            context.Canvas.DrawText(line, x, y, paint);

#if DEBUG
            if (ShowDebugLines)
            {
                using var r = new SKPaint { Color = new SKColor(255, 0, 0), Style = SKPaintStyle.Stroke };
                using var b = new SKPaint { Color = new SKColor(0, 0, 255), Style = SKPaintStyle.Stroke };

                context.Canvas.DrawRect(
                    X + ao.X,
                    Y + ao.Y - textBounds.Height + verticalPos,
                    textBounds.Width + Padding.Left + Padding.Right,
                    textBounds.Height * LineHeight + Padding.Top + Padding.Bottom,
                    r);

                context.Canvas.DrawRect(
                    X + ao.X + p.Left,
                    Y + ao.Y - textBounds.Height + p.Top + verticalPos,
                    textBounds.Width,
                    textBounds.Height * LineHeight,
                    b);
            }
#endif

            verticalPos += _maxTextHeight * LineHeight;
            isFirstLine = false;
        }
    }

    /// <inheritdoc cref="DrawnGeometry.Measure()" />
    public override LvcSize Measure()
    {
        if (Paint is null)
            throw new Exception(
                $"A paint is required to measure a label, please set the {nameof(Paint)} " +
                $"property with the paint that is drawing the label.");

        var w = 0f;
        _maxTextHeight = 0f;

        foreach (var line in GetLinesOrCached())
        {
            if (line.Bounds.Width > w) w = line.Bounds.Width;
            if (line.Bounds.Height > _maxTextHeight) _maxTextHeight = line.Bounds.Height;
        }

        var h = _maxTextHeight * _lines.Length * LineHeight;

        var size = new LvcSize(
            w + Padding.Left + Padding.Right,
            h + Padding.Top + Padding.Bottom);

        return size.GetRotatedSize(RotateTransform);
    }

    internal SKTextBlob[] GetLinesOrCached()
    {
        var changed =                                                           // lines changed if:
            _previousText != Text ||                                            //   - the text changed
            _previousTextSize != TextSize ||                                    //   - the text size changed
            _previousTextHeight != LineHeight;                                  //   - the line height changed
         // !GetPropertyDefinition(nameof(Paint))!.GetMotion(this).IsCompleted; //   - the paint is animating..
                                                                                //     is it necessary to check for the paint animation?
                                                                                //     can the paint change the size of the label?
                                                                                //     not as far as i can see...
        if (!changed) return _lines;

        foreach (var line in _lines)
            line.Dispose();

        var lvcSkiaPaint = (SkiaPaint?)Paint;
        var skiaPaint = lvcSkiaPaint?.UpdateSkiaPaint(null, null)
            ?? throw new Exception("A paint is required to measure a label.");

        var textSize = TextSize;
        var typeFace = lvcSkiaPaint.GetSKTypeface();

        _lines = string.IsNullOrWhiteSpace(Text)
            ? []
            : [..
                GetLines(skiaPaint)
                    .Select(line =>
                    {
                        var font = lvcSkiaPaint._fontBuilder(typeFace, textSize);
                        skiaPaint.TextSize = font.Size;
                        skiaPaint.Typeface = typeFace;

                        return font.AsTextBlob(line);
                    })
            ];

        _previousText = Text;
        _previousTextSize = TextSize;
        _previousTextHeight = LineHeight;

        return _lines;
    }

    private IEnumerable<string> GetLines(SKPaint paint)
    {
        IEnumerable<string> lines = Text.Split([Environment.NewLine], StringSplitOptions.None);

        if (MaxWidth != float.MaxValue)
            lines = lines.SelectMany(x => GetLinesByMaxWidth(x, paint));

        return lines;
    }

    private IEnumerable<string> GetLinesByMaxWidth(string source, SKPaint paint)
    {
        // DISCLAIM ====================================================================
        // WE ARE USING A DOUBLE STRING BUILDER, AND MEASURE THE REAL STRING EVERY TIME
        // BECAUSE IT SEEMS THAT THE SKIA MEASURE TEXT IS INCONSISTENT, FOR EXAMPLE:

        //using var p = new SKPaint() { Color = SKColors.Black, TextSize = 15 };
        //var b = new SKRect();
        //_ = p.MeasureText("nullam. Ut tellus", ref b);

        //var w1 = b.Width;

        //var w2 = 0f;
        //_ = p.MeasureText("nullam.", ref b);
        //w2 += b.Width;
        //_ = p.MeasureText(" Ut", ref b);
        //w2 += b.Width;
        //_ = p.MeasureText(" tellus", ref b);
        //w2 += b.Width;

        //Assert.IsTrue(w1 == w2); THIS IS FALSE!!!!

        var sb = new StringBuilder();
        var sb2 = new StringBuilder();
        var words = source.Split([" ", Environment.NewLine], StringSplitOptions.None);
        var bounds = new SKRect();
        var mw = MaxWidth - Padding.Left - Padding.Right;

        foreach (var word in words)
        {
            _ = sb2.Clear();
            _ = sb2.Append(sb);
            _ = sb2.Append(' ');
            _ = sb2.Append(word);
            _ = paint.MeasureText(sb2.ToString(), ref bounds);

            // if the line has already content and the new word exceeds the max width
            // then we create a new line
            if (sb.Length > 0 && bounds.Width > mw)
            {
                yield return sb.ToString();
                _ = sb.Clear();
            }

            if (sb.Length > 0) _ = sb.Append(' ');
            _ = sb.Append(word);
        }

        if (sb.Length > 0) yield return sb.ToString();
    }

    private LvcPoint GetAlignmentOffset(SKRect bounds)
    {
        var p = Padding;

        var w = bounds.Width + p.Left + p.Right;
        var h = bounds.Height * LineHeight + p.Top + p.Bottom;

        float l = -bounds.Left, t = -bounds.Top;

        switch (VerticalAlign)
        {
            case Align.Start: t += 0; break;
            case Align.Middle: t -= h * 0.5f; break;
            case Align.End: t -= h + 0; break;
            default:
                break;
        }
        switch (HorizontalAlign)
        {
            case Align.Start: l += 0; break;
            case Align.Middle: l -= w * 0.5f; break;
            case Align.End: l -= w + 0; break;
            default:
                break;
        }

        return new(l, t);
    }

    internal override void OnDisposed()
    {
        foreach (var line in _lines)
            line.Dispose();

        base.OnDisposed();
    }
}
