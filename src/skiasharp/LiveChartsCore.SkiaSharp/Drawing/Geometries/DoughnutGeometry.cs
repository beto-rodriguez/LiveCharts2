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
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <inheritdoc cref="BaseDoughnutGeometry" />
public class DoughnutGeometry : BaseDoughnutGeometry, IDrawnElement<SkiaSharpDrawingContext>
{
    /// <inheritdoc cref="IDrawnElement{TDrawingContext}.Draw(TDrawingContext)" />
    public virtual void Draw(SkiaSharpDrawingContext context)
    {
        if (CornerRadius == 0)
            ClassicDraw(context);
        else
            // this method should be able to draw the doughnut with rounded corners
            // but this is probably not working as expected, so we will use the classic draw method
            RoundedDraw(context);
    }

    private void ClassicDraw(SkiaSharpDrawingContext context)
    {
        using var path = new SKPath();
        var cx = CenterX;
        var cy = CenterY;
        var wedge = InnerRadius;
        var r = Width * 0.5f;
        var startAngle = StartAngle;
        var sweepAngle = SweepAngle;
        const float toRadians = (float)(Math.PI / 180);
        var pushout = PushOut;

        path.MoveTo(
            (float)(cx + Math.Cos(startAngle * toRadians) * wedge),
            (float)(cy + Math.Sin(startAngle * toRadians) * wedge));
        path.LineTo(
            (float)(cx + Math.Cos(startAngle * toRadians) * r),
            (float)(cy + Math.Sin(startAngle * toRadians) * r));
        path.ArcTo(
            new SKRect { Left = X, Top = Y, Size = new SKSize { Width = Width, Height = Height } },
            startAngle,
            sweepAngle,
            false);
        path.LineTo(
            (float)(cx + Math.Cos((sweepAngle + startAngle) * toRadians) * wedge),
            (float)(cy + Math.Sin((sweepAngle + startAngle) * toRadians) * wedge));
        path.ArcTo(
            new SKPoint { X = wedge, Y = wedge },
            0,
            sweepAngle > 180 ? SKPathArcSize.Large : SKPathArcSize.Small,
            SKPathDirection.CounterClockwise,
            new SKPoint
            {
                X = (float)(cx + Math.Cos(startAngle * toRadians) * wedge),
                Y = (float)(cy + Math.Sin(startAngle * toRadians) * wedge)
            });

        path.Close();

        if (pushout > 0)
        {
            var pushoutAngle = startAngle + 0.5f * sweepAngle;
            var x = pushout * (float)Math.Cos(pushoutAngle * toRadians);
            var y = pushout * (float)Math.Sin(pushoutAngle * toRadians);

            _ = context.Canvas.Save();
            context.Canvas.Translate(x, y);
        }

        context.Canvas.DrawPath(path, context.ActiveSkiaPaint);

        if (pushout > 0) context.Canvas.Restore();
    }

    private void RoundedDraw(SkiaSharpDrawingContext context)
    {
        using var path = new SKPath();
        var cx = CenterX;
        var cy = CenterY;
        var innerRadius = InnerRadius;
        var r = Width * 0.5f;
        var startAngle = StartAngle;
        var sweepAngle = SweepAngle;
        const float toRadians = (float)(Math.PI / 180);
        var pushout = PushOut;

        var cr = CornerRadius;
        var crDir = InvertedCornerRadius ? -1 : 1;

        var ma = (startAngle + sweepAngle) * 0.6 * toRadians;
        var hiMax = (float)(Math.Sin(ma) * innerRadius);
        var hMax = (r + pushout - innerRadius) * 0.5f;
        if (cr > hMax) cr = hMax;

        var px1 = (float)(cx + Math.Cos(startAngle * toRadians) * innerRadius);
        var py1 = (float)(cy + Math.Sin(startAngle * toRadians) * innerRadius);
        var px2 = (float)(cx + Math.Cos(startAngle * toRadians) * r);
        var py2 = (float)(cy + Math.Sin(startAngle * toRadians) * r);

        var or = Math.Sqrt(Math.Pow(cx - px2, 2) + Math.Pow(cy - py2, 2));
        var coa = crDir * Math.Atan(cr / (or - cr));

        if (sweepAngle * toRadians < Math.Abs(2 * coa))
        {
            cr = 0.01f;
            coa = crDir * Math.Atan(cr / (or - cr));
            crDir = 1;
        }

        var a1 = startAngle * toRadians + coa;
        var xic1 = (float)(cx + Math.Cos(startAngle * toRadians) * (r - cr));
        var yic1 = (float)(cy + Math.Sin(startAngle * toRadians) * (r - cr));
        var xjc1 = (float)(cx + Math.Cos(a1) * r);
        var yjc1 = (float)(cy + Math.Sin(a1) * r);

        var a2 = (startAngle + sweepAngle) * toRadians - coa;
        var xmc2 = (float)(cx + Math.Cos((startAngle + sweepAngle) * toRadians) * r);
        var ymc2 = (float)(cy + Math.Sin((startAngle + sweepAngle) * toRadians) * r);
        var xjc2 = (float)(cx + Math.Cos((startAngle + sweepAngle) * toRadians) * (r - cr));
        var yjc2 = (float)(cy + Math.Sin((startAngle + sweepAngle) * toRadians) * (r - cr));

        var px3 = (float)(cx + Math.Cos((sweepAngle + startAngle) * toRadians) * innerRadius);
        var py3 = (float)(cy + Math.Sin((sweepAngle + startAngle) * toRadians) * innerRadius);
        var ir = Math.Sqrt(Math.Pow(cx - px3, 2) + Math.Pow(cy - py3, 2));
        var cia = crDir * Math.Atan(cr / (ir + cr));

        var a3 = (startAngle + sweepAngle) * toRadians - cia;
        var xic3 = (float)(cx + Math.Cos((startAngle + sweepAngle) * toRadians) * (innerRadius + cr));
        var yic3 = (float)(cy + Math.Sin((startAngle + sweepAngle) * toRadians) * (innerRadius + cr));
        var xjc3 = (float)(cx + Math.Cos(a3) * innerRadius);
        var yjc3 = (float)(cy + Math.Sin(a3) * innerRadius);

        var a4 = startAngle * toRadians + cia;
        var xic4 = (float)(cx + Math.Cos(a4) * innerRadius);
        var yic4 = (float)(cy + Math.Sin(a4) * innerRadius);
        var xjc4 = (float)(cx + Math.Cos(startAngle * toRadians) * (innerRadius + cr));
        var yjc4 = (float)(cy + Math.Sin(startAngle * toRadians) * (innerRadius + cr));

        path.MoveTo(xjc4, yjc4);

        path.LineTo(xic1, yic1);
        path.ArcTo(new SKPoint(px2, py2), new SKPoint(xjc1, yjc1), cr);

        path.ArcTo(
            new SKRect { Left = X, Top = Y, Size = new SKSize { Width = Width, Height = Height } },
            (float)(a1 / toRadians),
            sweepAngle - 2 * (float)(coa / toRadians),
            false);

        path.ArcTo(new SKPoint(xmc2, ymc2), new SKPoint(xjc2, yjc2), cr);

        path.LineTo(xic3, yic3);

        if (cr < innerRadius)
        {
            path.ArcTo(new SKPoint(px3, py3), new SKPoint(xjc3, yjc3), cr);

            path.ArcTo(
                new SKPoint { X = innerRadius, Y = innerRadius },
                0,
                sweepAngle - 2 * cia / toRadians > 180 ? SKPathArcSize.Large : SKPathArcSize.Small,
                SKPathDirection.CounterClockwise,
                new SKPoint { X = xic4, Y = yic4 });

            path.ArcTo(new SKPoint(px1, py1), new SKPoint(xjc4, yjc4), cr);
        }
        else
        {
            path.ArcTo(new SKPoint(cx, cy), new SKPoint(xjc4, yjc4), innerRadius);
        }

        path.Close();

        if (pushout > 0)
        {
            var pushoutAngle = startAngle + 0.5f * sweepAngle;
            var x = pushout * (float)Math.Cos(pushoutAngle * toRadians);
            var y = pushout * (float)Math.Sin(pushoutAngle * toRadians);

            _ = context.Canvas.Save();
            context.Canvas.Translate(x, y);
        }

        if (sweepAngle > 0.01) context.Canvas.DrawPath(path, context.ActiveSkiaPaint);
        if (pushout > 0) context.Canvas.Restore();
    }
}
