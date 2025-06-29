using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using SkiaSharp;

namespace EtoFormsSample.Pies.Icons;

public class SvgLabel : LabelGeometry
{
    private SKPath _path = new();
    public string? Name { get; set; }
    public float Size { get; set; }

    private string? _svgString;
    public string? SvgString
    {
        get => _svgString;
        set
        {
            if (_svgString == value) return;
            _svgString = value;
            _path = value is not null ? SKPath.ParseSvgPathData(value) : new SKPath();
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
