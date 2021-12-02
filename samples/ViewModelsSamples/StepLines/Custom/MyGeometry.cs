using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp;

namespace ViewModelsSamples.StepLines.Custom
{
    public class MyGeometry : LiveChartsCore.SkiaSharpView.Drawing.Geometries.SVGPathGeometry
    {
        // the static field is important to prevent the svg path is parsed multiple times // mark
        // Icon from Google Material Icons font.
        // https://fonts.google.com/icons?selected=Material%20Icons%20Outlined%3Atask_alt%3A
        public static SKPath svgPath = SKPath.ParseSvgPathData(
            "M22,5.18L10.59,16.6l-4.24-4.24l1.41-1.41l2.83,2.83l10-10L22,5.18z M19.79,10.22C19.92,10.79,20,11.39,20,12 " +
            "c0,4.42-3.58,8-8,8s-8-3.58-8-8c0-4.42,3.58-8,8-8c1.58,0,3.04,0.46,4.28,1.25l1.44-1.44C16.1,2.67,14.13,2,12,2C6.48,2,2,6.48,2,12 " +
            "c0,5.52,4.48,10,10,10s10-4.48,10-10c0-1.19-0.22-2.33-0.6-3.39L19.79,10.22z");

        public MyGeometry()
            : base(svgPath)
        { }

        public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
        {
            // lets also draw a white circle as background before the svg path is drawn
            // this will just make things look better

            using (var backgroundPaint = new SKPaint())
            {
                backgroundPaint.Style = SKPaintStyle.Fill;
                backgroundPaint.Color = SKColors.White;

                var r = Width / 2;
                context.Canvas.DrawCircle(X + r, Y + r, r, backgroundPaint);
            }

            base.OnDraw(context, paint);
        }
    }
}
