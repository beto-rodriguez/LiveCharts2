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

    internal static Action<DoughnutGeometry, SkiaSharpDrawingContext, SKPaint>? AlternativeDraw { get; set; }

    /// <inheritdoc cref="Geometry.OnMeasure(IPaint{SkiaSharpDrawingContext})" />
    protected override LvcSize OnMeasure(IPaint<SkiaSharpDrawingContext> paint)
    {
        return new LvcSize(Width, Height);
    }

    /// <inheritdoc cref="Geometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
    public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        if (AlternativeDraw is not null)
        {
            AlternativeDraw(this, context, paint);
            return;
        }

        if (CornerRadius > 0) throw new NotImplementedException($"{nameof(CornerRadius)} is not implemented.");

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
}
