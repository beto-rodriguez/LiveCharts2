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
using LiveChartsCore.Motion;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <inheritdoc cref="IDoughnutGeometry{TDrawingContext}" />
public class DoughnutGeometry : Geometry, IDoughnutGeometry<SkiaSharpDrawingContext>
{
    private readonly FloatMotionProperty _cxProperty;
    private readonly FloatMotionProperty _cyProperty;
    private readonly FloatMotionProperty _wProperty;
    private readonly FloatMotionProperty _hProperty;
    private readonly FloatMotionProperty _startProperty;
    private readonly FloatMotionProperty _sweepProperty;
    private readonly FloatMotionProperty _pushoutProperty;
    private readonly FloatMotionProperty _innerRadiusProperty;
    private readonly FloatMotionProperty _cornerRadiusProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="DoughnutGeometry"/> class.
    /// </summary>
    public DoughnutGeometry()
    {
        _cxProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(CenterX)));
        _cyProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(CenterY)));
        _wProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Width)));
        _hProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Height)));
        _startProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(StartAngle)));
        _sweepProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(SweepAngle)));
        _pushoutProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(PushOut)));
        _innerRadiusProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(InnerRadius)));
        _cornerRadiusProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(CornerRadius)));
    }

    /// <inheritdoc cref="IDoughnutGeometry{TDrawingContext}.CenterX" />
    public float CenterX { get => _cxProperty.GetMovement(this); set => _cxProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IDoughnutGeometry{TDrawingContext}.CenterY" />
    public float CenterY { get => _cyProperty.GetMovement(this); set => _cyProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IDoughnutGeometry{TDrawingContext}.Width" />
    public float Width { get => _wProperty.GetMovement(this); set => _wProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IDoughnutGeometry{TDrawingContext}.Height" />
    public float Height { get => _hProperty.GetMovement(this); set => _hProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IDoughnutGeometry{TDrawingContext}.StartAngle" />
    public float StartAngle { get => _startProperty.GetMovement(this); set => _startProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IDoughnutGeometry{TDrawingContext}.SweepAngle" />
    public float SweepAngle
    {
        get => _sweepProperty.GetMovement(this);
        set => _sweepProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="IDoughnutGeometry{TDrawingContext}.PushOut" />
    public float PushOut { get => _pushoutProperty.GetMovement(this); set => _pushoutProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IDoughnutGeometry{TDrawingContext}.InnerRadius" />
    public float InnerRadius { get => _innerRadiusProperty.GetMovement(this); set => _innerRadiusProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IDoughnutGeometry{TDrawingContext}.CornerRadius" />
    public float CornerRadius { get => _cornerRadiusProperty.GetMovement(this); set => _cornerRadiusProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IDoughnutGeometry{TDrawingContext}.InvertedCornerRadius" />
    public bool InvertedCornerRadius { get; set; }

    /// <inheritdoc cref="Geometry.OnMeasure(IPaint{SkiaSharpDrawingContext})" />
    protected override LvcSize OnMeasure(IPaint paint)
    {
        return new LvcSize(Width, Height);
    }

    /// <inheritdoc cref="Geometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
    public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        if (CornerRadius == 0)
        {
            ClassicDraw(context, paint);
        }
        else
        {
            // this method should be able to draw the doughnut with rounded corners
            // but this is probably not working as expected, so we will use the classic draw method
            RoundedDraw(context, paint);
        }
    }

    private void ClassicDraw(SkiaSharpDrawingContext context, SKPaint paint)
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

        context.Canvas.DrawPath(path, context.Paint);

        if (pushout > 0) context.Canvas.Restore();
    }

    private void RoundedDraw(SkiaSharpDrawingContext context, SKPaint paint)
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

        if (sweepAngle > 0.01) context.Canvas.DrawPath(path, context.Paint);
        if (pushout > 0) context.Canvas.Restore();
    }
}
