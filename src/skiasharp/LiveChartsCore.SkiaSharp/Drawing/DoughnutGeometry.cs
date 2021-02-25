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
using SkiaSharp;
using System;

namespace LiveChartsCore.SkiaSharpView.Drawing
{
    public class DoughnutGeometry : Geometry, IDoughnutGeometry<SkiaDrawingContext>
    {
        private readonly FloatMotionProperty cxProperty;
        private readonly FloatMotionProperty cyProperty;
        private readonly FloatMotionProperty wProperty;
        private readonly FloatMotionProperty hProperty;
        private readonly FloatMotionProperty startProperty;
        private readonly FloatMotionProperty sweepProperty;
        private readonly FloatMotionProperty pushoutProperty;
        private readonly FloatMotionProperty wedgeProperty;

        public DoughnutGeometry()
        {
            cxProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(CenterX)));
            cyProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(CenterY)));
            wProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Width)));
            hProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Height)));
            startProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(StartAngle)));
            sweepProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(SweepAngle)));
            pushoutProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(PushOut)));
            wedgeProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(InnerRadius)));
        }

        public float CenterX { get => cxProperty.GetMovement(this); set => cxProperty.SetMovement(value, this); }
        public float CenterY { get => cyProperty.GetMovement(this); set => cyProperty.SetMovement(value, this); }
        public float Width { get => wProperty.GetMovement(this); set => wProperty.SetMovement(value, this); }
        public float Height { get => hProperty.GetMovement(this); set => hProperty.SetMovement(value, this); }
        public float StartAngle { get => startProperty.GetMovement(this); set => startProperty.SetMovement(value, this); }
        public float SweepAngle { get => sweepProperty.GetMovement(this); set => sweepProperty.SetMovement(value, this); }
        public float PushOut { get => pushoutProperty.GetMovement(this); set => pushoutProperty.SetMovement(value, this); }
        public float InnerRadius { get => wedgeProperty.GetMovement(this); set => wedgeProperty.SetMovement(value, this); }

        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            return new SKSize(Width, Height);
        }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            SKPath path = new SKPath();

            var cx = CenterX;
            var cy = CenterY;
            var wedge = InnerRadius;
            var r = Width * 0.5f;
            var startAngle = StartAngle;
            var sweepAngle = SweepAngle;
            var toRadians = (float)(Math.PI / 180);

            path.MoveTo((float)(cx + Math.Cos(startAngle * toRadians) * wedge), (float)(cy + Math.Sin(startAngle * toRadians) * wedge));
            path.LineTo((float)(cx + Math.Cos(startAngle * toRadians) * r), (float)(cy + Math.Sin(startAngle * toRadians) * r));
            path.ArcTo(
                new SKRect { Left = X, Top = Y, Size = new SKSize { Width = Width, Height = Height } },
                startAngle,
                sweepAngle,
                false);
            path.LineTo(
                (float)(cx + Math.Cos((sweepAngle + startAngle) * toRadians) * wedge),
                (float)(cy + Math.Sin((sweepAngle + startAngle) * toRadians) * wedge));
            path.ArcTo(
                new SKPoint { X = wedge, Y = wedge},
                0,
                SKPathArcSize.Small,
                SKPathDirection.CounterClockwise,
                new SKPoint
                {
                    X = (float)(cx + Math.Cos(startAngle * toRadians) * wedge),
                    Y = (float)(cy + Math.Sin(startAngle * toRadians) * wedge)
                });

            path.Close();

            float explodeOffset = PushOut;

            if (explodeOffset > 0)
            {
                float pushoutAngle = startAngle + 0.5f * sweepAngle;
                float x = explodeOffset * (float)Math.Cos(pushoutAngle * toRadians);
                float y = explodeOffset * (float)Math.Sin(pushoutAngle * toRadians);

                context.Canvas.Save();
                context.Canvas.Translate(x, y);
            }

            context.Canvas.DrawPath(path, context.Paint);

            context.Canvas.Restore();
        }
    }

    //public static class Slice
    //{
    //    public static S‪liceBuilderViewModel Build(
    //        float wedge, float radius, float innerRadius,
    //        float cornerRadius, PointF center, bool forceAngle, float pushOut)
    //    {
    //        return GetPoints(
    //            wedge, radius, innerRadius, cornerRadius, center, forceAngle, pushOut);
    //    }

    //    private static S‪liceBuilderViewModel GetPoints(
    //        float wedge, float outerRadius, float innerRadius,
    //        float cornerRadius, PointF center, bool forceAngle, float pushOut)
    //    {
    //        // See docs/resources/slice.png
    //        // if you require to know more about the formulas in the geometry

    //        PointF[] pts = new PointF[8];

    //        const float toRadians = (float)(Math.PI / 180d);
    //        float a = wedge;

    //        if (a > 359.9f)
    //        {
    //            // workaround..
    //            // for some reason, large arc does not work whether true or false if angle eq 360...
    //            // so lets just make it look like the circle is complete.
    //            a = 359.99f;
    //        }

    //        float angle = a * toRadians;

    //        float cr = (outerRadius - innerRadius) / 2 > cornerRadius ? cornerRadius : (outerRadius - innerRadius) / 2;

    //        float temporal = (float) Math.Atan(cr / Math.Sqrt( Math.Pow(innerRadius + cr, 2) + Math.Pow(cr, 2)));

    //        if (angle < temporal * 2)
    //        {
    //            if (!forceAngle)
    //            {
    //                angle = temporal * 2;
    //            }
    //            else
    //            {
    //                cr = 0;
    //            }
    //        }

    //        float innerRoundingAngle = (float)Math.Atan(cr / Math.Sqrt(Math.Pow(innerRadius + cr, 2) + Math.Pow(cr, 2)));
    //        float outerRoundingAngle = (float)Math.Atan(cr / Math.Sqrt(Math.Pow(outerRadius - cr, 2) + Math.Pow(cr, 2)));

    //        if (float.IsNaN(innerRoundingAngle)) innerRoundingAngle = 0f;
    //        if (float.IsNaN(outerRoundingAngle)) outerRoundingAngle = 0f;

    //        float o1 = (innerRadius + cr) * (float) Math.Cos(innerRoundingAngle);
    //        float o2 = (outerRadius - cr) * (float) Math.Cos(outerRoundingAngle);
    //        float o3 = (float) Math.Sqrt(Math.Pow(outerRadius - cr, 2) - Math.Pow(cr, 2));
    //        float o4 = (float) Math.Sqrt(Math.Pow(innerRadius + cr, 2) - Math.Pow(cr, 2));

    //        float xp = pushOut * (float) Math.Sin(angle / 2);
    //        float yp = pushOut * (float) Math.Cos(angle / 2);

    //        unchecked
    //        {
    //            pts[0] = new PointF(center.X + xp, center.Y + o1 + yp);
    //            pts[1] = new PointF(center.X + xp, center.Y + o2 + yp);
    //            pts[2] = new PointF(
    //                 center.X + outerRadius * (float) Math.Sin(outerRoundingAngle) + xp,
    //                center.Y + outerRadius * (float) Math.Cos(outerRoundingAngle) + yp);
    //            pts[3] = new PointF(
    //                center.X + outerRadius * (float) Math.Sin(angle - outerRoundingAngle) + xp,
    //                center.Y + outerRadius * (float) Math.Cos(angle - outerRoundingAngle) + yp);
    //            pts[4] = new PointF(
    //                center.X + o3 * (float) Math.Sin(angle) + xp,
    //                center.Y + o3 * (float)Math.Cos(angle) + yp);
    //            pts[5] = new PointF(
    //                center.X + o4 * (float)Math.Sin(angle) + xp,
    //                center.Y + o4 * (float)Math.Cos(angle) + yp);
    //            pts[6] = new PointF(
    //                center.X + innerRadius * (float)Math.Sin(angle - innerRoundingAngle) + xp,
    //                center.Y + innerRadius * (float)Math.Cos(angle - innerRoundingAngle) + yp);
    //            pts[7] = new PointF(
    //                center.X + innerRadius * (float)Math.Sin(innerRoundingAngle) + xp,
    //                center.Y + innerRadius * (float)Math.Cos(innerRoundingAngle) + yp);

    //            return new S‪liceBuilderViewModel(
    //                pts,
    //                (float)cr,
    //                angle - outerRoundingAngle * 2 >= Math.PI,
    //                angle - innerRoundingAngle * 2 >= Math.PI);
    //        }
    //    }
    //}

    //public struct S‪liceBuilderViewModel
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="S‪liceBuilderViewModel"/> struct.
    //    /// </summary>
    //    /// <param name="points">The points.</param>
    //    /// <param name="cornerRadius">The corner radius.</param>
    //    /// <param name="isRadiusLargeArc">The is large arc.</param>
    //    /// <param name="isInnerRadiusLargeArc">The is inner radius large arc.</param>
    //    public S‪liceBuilderViewModel(PointF[] points, float cornerRadius, bool isRadiusLargeArc, bool isInnerRadiusLargeArc)
    //    {
    //        Points = points;
    //        CornerRadius = cornerRadius;
    //        IsRadiusLargeArc = isRadiusLargeArc;
    //        IsInnerRadiusLargeArc = isInnerRadiusLargeArc;
    //    }

    //    /// <summary>
    //    /// Gets or sets the points.
    //    /// </summary>
    //    /// <value>
    //    /// The points.
    //    /// </value>
    //    public PointF[] Points { get; set; }

    //    /// <summary>
    //    /// Gets or sets the corner radius.
    //    /// </summary>
    //    /// <value>
    //    /// The corner radius.
    //    /// </value>
    //    public float CornerRadius { get; set; }

    //    /// <summary>
    //    /// Gets or sets a value indicating whether this instance is large arc.
    //    /// </summary>
    //    /// <value>
    //    ///   <c>true</c> if this instance is large arc; otherwise, <c>false</c>.
    //    /// </value>
    //    public bool IsRadiusLargeArc { get; set; }

    //    /// <summary>
    //    /// Gets or sets a value indicating whether this instance is inner radius large arc.
    //    /// </summary>
    //    /// <value>
    //    ///   <c>true</c> if this instance is inner radius large arc; otherwise, <c>false</c>.
    //    /// </value>
    //    public bool IsInnerRadiusLargeArc { get; set; }
    //}
}
