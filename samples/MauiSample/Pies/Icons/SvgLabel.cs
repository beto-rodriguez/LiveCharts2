using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using SkiaSharp;

namespace MauiSample.Pies.Icons;

public class SvgLabel : LabelGeometry
{
    private SKPath _path = new();
    public string? Name { get; set; }
    public float Size { get; set; }
    public string? SvgString
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            _path = SKPath.ParseSvgPathData(value);
        }
    }

    public override void Draw(SkiaSharpDrawingContext context)
    {
        using var iconPaint = new SKPaint
        {
            Color = SKColors.WhiteSmoke,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        Svg.Draw(context, iconPaint, _path, X, Y, Size, Size);

        using var textPaint = new SKPaint
        {
            Color = SKColors.WhiteSmoke,
            Style = SKPaintStyle.Fill,
            TextSize = 16,
            FakeBoldText = true,
            IsAntialias = true
        };

        context.Canvas.DrawText(Name, X, Y - 10, textPaint);
    }
}
