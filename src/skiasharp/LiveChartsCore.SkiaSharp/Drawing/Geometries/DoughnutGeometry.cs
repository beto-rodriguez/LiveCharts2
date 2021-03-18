// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Drawing;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Drawing;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries
{
    public class DoughnutGeometry : Geometry, IDoughnutGeometry<SkiaSharpDrawingContext>, IDoughnutVisualChartPoint<SkiaSharpDrawingContext>
    {
        private readonly FloatMotionProperty cxProperty;
        private readonly FloatMotionProperty cyProperty;
        private readonly FloatMotionProperty wProperty;
        private readonly FloatMotionProperty hProperty;
        private readonly FloatMotionProperty startProperty;
        private readonly FloatMotionProperty sweepProperty;
        private readonly FloatMotionProperty pushoutProperty;
        private readonly FloatMotionProperty innerRadiusProperty;

        public DoughnutGeometry()
        {
            cxProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(CenterX)));
            cyProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(CenterY)));
            wProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Width)));
            hProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Height)));
            startProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(StartAngle)));
            sweepProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(SweepAngle)));
            pushoutProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(PushOut)));
            innerRadiusProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(InnerRadius)));
        }

        public float CenterX { get => cxProperty.GetMovement(this); set => cxProperty.SetMovement(value, this); }
        public float CenterY { get => cyProperty.GetMovement(this); set => cyProperty.SetMovement(value, this); }
        public float Width { get => wProperty.GetMovement(this); set => wProperty.SetMovement(value, this); }
        public float Height { get => hProperty.GetMovement(this); set => hProperty.SetMovement(value, this); }
        public float StartAngle { get => startProperty.GetMovement(this); set => startProperty.SetMovement(value, this); }
        public float SweepAngle { get => sweepProperty.GetMovement(this); set => sweepProperty.SetMovement(value, this); }
        public float PushOut { get => pushoutProperty.GetMovement(this); set => pushoutProperty.SetMovement(value, this); }
        public float InnerRadius { get => innerRadiusProperty.GetMovement(this); set => innerRadiusProperty.SetMovement(value, this); }

        protected override SizeF OnMeasure(PaintTask paint)
        {
            return new SizeF(Width, Height);
        }

        public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
        {
            SKPath path = new SKPath();

            var cx = CenterX;
            var cy = CenterY;
            var wedge = InnerRadius;
            var r = Width * 0.5f;
            var startAngle = StartAngle;
            var sweepAngle = SweepAngle;
            var toRadians = (float)(Math.PI / 180);
            float pushout = PushOut;

            path.MoveTo(
                (float)(cx + Math.Cos(startAngle * toRadians) * wedge),
                (float)(cy + Math.Sin(startAngle * toRadians) * wedge));
            path.LineTo(
                (float)(cx + Math.Cos(startAngle * toRadians) * (r + pushout)),
                (float)(cy + Math.Sin(startAngle * toRadians) * (r + pushout)));
            path.ArcTo(
                new SKRect { Left = X, Top = Y, Size = new SKSize { Width = Width, Height = Height } },
                startAngle,
                sweepAngle,
                false);
            path.LineTo(
                (float)(cx + Math.Cos((sweepAngle + startAngle) * toRadians) * wedge),
                (float)(cy + Math.Sin((sweepAngle + startAngle) * toRadians) * wedge));
            path.ArcTo(
                new SKPoint { X = wedge + pushout, Y = wedge + pushout },
                0,
                SKPathArcSize.Small,
                SKPathDirection.CounterClockwise,
                new SKPoint
                {
                    X = (float)(cx + Math.Cos(startAngle * toRadians) * wedge),
                    Y = (float)(cy + Math.Sin(startAngle * toRadians) * wedge)
                });

            path.Close();

            if (pushout > 0)
            {
                float pushoutAngle = startAngle + 0.5f * sweepAngle;
                float x = pushout * (float)Math.Cos(pushoutAngle * toRadians);
                float y = pushout * (float)Math.Sin(pushoutAngle * toRadians);

                context.Canvas.Save();
                context.Canvas.Translate(x, y);
            }

            context.Canvas.DrawPath(path, context.Paint);

            if (pushout > 0) context.Canvas.Restore();
        }
    }
}
