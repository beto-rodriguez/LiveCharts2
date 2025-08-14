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
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using static LiveChartsCore.SkiaSharpView.Drawing.DrawingTextExtensions;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <inheritdoc cref="BaseLabelGeometry" />
public class LabelGeometry : BaseLabelGeometry, IDrawnElement<SkiaSharpDrawingContext>
{
    private string _previousText = string.Empty;
    private double _previousTextSize = 0f;
    private SKPaint? _previousPaint;
    private BlobArray _activeBlobs = new(new(), 0f, new(), false, []);

    internal BlobArray BlobArray
    {
        get
        {
            PeekPaintInfo(out var skPaint, out _);

            var changed =                           // changed if:
                _previousText != Text ||            //   - the text changed
                _previousTextSize != TextSize ||    //   - the text size changed
                _previousPaint != skPaint;       //   - the paint changed, otherwise we will be using disposed resources

            if (!changed || string.IsNullOrEmpty(Text))
                return _activeBlobs;

            DisposeActiveBlobs();

            _previousText = Text;
            _previousTextSize = TextSize;
            _previousPaint = skPaint;

            return _activeBlobs = this.AsBlobArray();
        }
    }

    /// <inheritdoc cref="IDrawnElement{TDrawingContext}.Draw(TDrawingContext)" />
    public virtual void Draw(SkiaSharpDrawingContext context)
    {
        PeekPaintInfo(out var skPaint, out _);

        var settings = new BlobArraySettings(
            HorizontalAlign, VerticalAlign, Background, Opacity * context.ActiveOpacity);

        context.Canvas.DrawBlobArray(BlobArray, settings, X, Y, skPaint);
    }

    /// <inheritdoc cref="DrawnGeometry.Measure()" />
    public override LvcSize Measure()
    {
        return Paint is null
            ? throw new Exception(
                $"A paint is required to measure a label, please set the {nameof(Paint)} " +
                $"property with the paint that is drawing the label.")
            : BlobArray.Size.GetRotatedSize(RotateTransform);
    }

    internal void PeekPaintInfo(out SKPaint skPaint, out SKFont font)
    {
        var lvcSkiaPaint = (SkiaPaint?)Paint;
        skPaint = lvcSkiaPaint?.UpdateSkiaPaint(null, null)
            ?? throw new Exception("A paint is required to measure a label.");

        font = lvcSkiaPaint._fontBuilder(lvcSkiaPaint.GetSKTypeface(), TextSize);

        skPaint.TextSize = font.Size;
        skPaint.Typeface = font.Typeface;
        skPaint.IsAntialias = true;
        skPaint.LcdRenderText = true;
    }

    private void DisposeActiveBlobs()
    {
        foreach (var positionedBlob in _activeBlobs.Blobs)
        {
            if (positionedBlob == s_newLine)
                continue;

            positionedBlob.Blob.Dispose();
        }
    }

    internal override void OnDisposed()
    {
        DisposeActiveBlobs();

        // its not the job of the label geometry to dispose the paint,
        // but lets clean the reference to the paint.
        _previousPaint = null;

        base.OnDisposed();
    }
}
