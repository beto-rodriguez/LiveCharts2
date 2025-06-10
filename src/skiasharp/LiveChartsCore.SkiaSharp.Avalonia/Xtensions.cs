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
using Avalonia.Markup.Xaml;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.Painting.ImageFilters;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

/// <summary>
/// The base skia paint extension.
/// </summary>
public abstract class BaseSkiaPaintExtention : MarkupExtension
{
    /// <summary>
    /// Gets or sets a value indicating whether the paint should be antialias.
    /// </summary>
    public bool IsAntialias { get; set; } = true;

    /// <summary>
    /// Gets or sets the stroke width.
    /// </summary>
    public float StrokeWidth { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the stroke cap.
    /// </summary>
    public SKStrokeCap StrokeCap { get; set; }

    /// <summary>
    /// Gets or sets the stroke miter.
    /// </summary>
    public float StrokeMiter { get; set; }

    /// <summary>
    /// Gets or sets the stroke join.
    /// </summary>
    public SKStrokeJoin StrokeJoin { get; set; }

    /// <summary>
    /// Gets or sets the image filter.
    /// </summary>
    public ImageFilter? ImageFilter { get; set; } = null;

    /// <summary>
    /// Gets or sets the path effect.
    /// </summary>
    public PathEffect? PathEffect { get; set; } = null;

    /// <summary>
    /// Gets or sets the font family.
    /// </summary>
    public string? FontFamily { get; set; }

    /// <summary>
    /// Gets or sets the font weight.
    /// </summary>
    public SKFontStyleWeight FontWeight { get; set; } = SKFontStyleWeight.Normal;

    /// <summary>
    /// Gets or sets the font width.
    /// </summary>
    public SKFontStyleWidth FontWidth { get; set; } = SKFontStyleWidth.Normal;

    /// <summary>
    /// Gets or sets the font slant.
    /// </summary>
    public SKFontStyleSlant FontSlant { get; set; } = SKFontStyleSlant.Upright;

    /// <summary>
    /// Gets or sets a character to match the typeface to use.
    /// </summary>
    public string? TypefaceMatchesChar { get; set; } = null;

    /// <summary>
    /// Maps the properties to the paint.
    /// </summary>
    /// <param name="paint">The paint.</param>
    protected void MapProperties(SkiaPaint paint)
    {
        paint.IsAntialias = IsAntialias;
        paint.StrokeThickness = StrokeWidth;
        paint.StrokeCap = StrokeCap;
        paint.StrokeMiter = StrokeMiter;
        paint.StrokeJoin = StrokeJoin;
        paint.ImageFilter = ImageFilter;
        paint.PathEffect = PathEffect;

        if (FontWeight != SKFontStyleWeight.Normal || FontWidth != SKFontStyleWidth.Normal || FontSlant != SKFontStyleSlant.Upright)
            paint.SKFontStyle = new SKFontStyle(FontWeight, FontWidth, FontSlant);

        if (FontFamily is not null)
        {
            paint.SKTypeface = SKTypeface.FromFamilyName(FontFamily);
        }
        else if (TypefaceMatchesChar is not null && TypefaceMatchesChar.Length > 0)
        {
            paint.SKTypeface = SKFontManager.Default.MatchCharacter(TypefaceMatchesChar[0]);
        }
    }

    /// <summary>
    /// Parses the point.
    /// </summary>
    protected static SKPoint ParsePoint(string? point, SKPoint @default)
    {
        if (string.IsNullOrWhiteSpace(point)) return @default;

        var split = point.Split(',');

        if (split.Length != 2) return @default;
#pragma warning disable IDE0046 // Convert to conditional expression
        if (!float.TryParse(split[0], out var x) || !float.TryParse(split[1], out var y)) return @default;
#pragma warning restore IDE0046 // Convert to conditional expression

        return new SKPoint(x, y);
    }
}

/// <summary>
/// The solid color paint extension.
/// </summary>
public class SolidColorPaintExtension : BaseSkiaPaintExtention
{
    /// <summary>
    /// Gets or sets the color, default value is #00000000, and means to use the default color, normally
    /// defines by the element being painted, for example the series color.
    /// </summary>
    public string? Color { get; set; } = "#00000000";

    /// <summary>
    /// ...
    /// </summary>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (!SKColor.TryParse(Color, out var color))
            return new SolidColorPaint(SKColors.Transparent);

        var paint = new SolidColorPaint(color);

        MapProperties(paint);

        return paint;
    }
}

/// <summary>
/// The linear gradient paint extension.
/// </summary>
public class LinearGradientPaintExtension : BaseSkiaPaintExtention
{
    /// <summary>
    /// Gets or sets the colors separated by commas, default value is #00000000, and means to use the default color, normally
    /// defines by the element being painted, for example the series color.
    /// </summary>
    public string? Colors { get; set; } = "#00000000";

    /// <summary>
    /// Gets or sets the start point.
    /// </summary>
    public string? StartPoint { get; set; } = null;

    /// <summary>
    /// Gets or sets the end point.
    /// </summary>
    public string? EndPoint { get; set; } = null;

    /// <summary>
    /// Gets or sets the colors positions separated by commas,
    /// values should be in the range of 0 to 1,
    /// these values indicate the relative positions of the colors, you can set that argument to null to equally
    /// space the colors, default is null.
    /// </summary>
    public string? ColorPositions { get; set; } = null;

    /// <summary>
    /// Gets or sets the tile mode.
    /// </summary>
    public SKShaderTileMode TileMode { get; set; } = SKShaderTileMode.Repeat;

    /// <summary>
    /// ...
    /// </summary>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var colorsHexArray = Colors?.Split(',');
        if (colorsHexArray is null || colorsHexArray.Length == 0)
            return new LinearGradientPaint(SKColors.Transparent, SKColors.Transparent);

        var colors = new SKColor[colorsHexArray.Length];
        for (var i = 0; i < colors.Length; i++)
        {
            colors[i] = SKColor.TryParse(colorsHexArray[i], out var color)
                ? color
                : SKColors.Transparent;
        }

        if (colors.Length == 0)
            return new LinearGradientPaint(SKColors.Transparent, SKColors.Transparent);

        var start = ParsePoint(StartPoint, LinearGradientPaint.DefaultStartPoint);
        var end = ParsePoint(EndPoint, LinearGradientPaint.DefaultEndPoint);

        var colorPositionsSplit = ColorPositions?.Split(',');
        float[]? colorPositions = null;

        if (colorPositionsSplit is not null && colorPositionsSplit.Length > 0)
        {
            colorPositions = new float[colorPositionsSplit.Length];
            for (var i = 0; i < colorPositionsSplit.Length; i++)
            {
                colorPositions[i] = float.TryParse(colorPositionsSplit[i], out var value)
                    ? value
                    : 0f;
            }
        }

        var paint = new LinearGradientPaint([.. colors], start, end, colorPositions, TileMode);

        MapProperties(paint);

        return paint;
    }
}

/// <summary>
/// The radial gradient paint extension.
/// </summary>
public class RadialGradientPaintExtension : BaseSkiaPaintExtention
{
    /// <summary>
    /// Gets or sets the colors separated by commas, default value is #00000000, and means to use the default color, normally
    /// defines by the element being painted, for example the series color.
    /// </summary>
    public string? Colors { get; set; } = "#00000000";

    /// <summary>
    /// Gets or sets the center point.
    /// </summary>
    public string? Center { get; set; } = null;

    /// <summary>
    /// Gets or sets the radius.
    /// </summary>
    public float Radius { get; set; } = 0.5f;

    /// <summary>
    /// Gets or sets the colors positions separated by commas,
    /// values should be in the range of 0 to 1,
    /// these values indicate the relative positions of the colors, you can set that argument to null to equally
    /// space the colors, default is null.
    /// </summary>
    public string? ColorPositions { get; set; } = null;

    /// <summary>
    /// Gets or sets the tile mode.
    /// </summary>
    public SKShaderTileMode TileMode { get; set; } = SKShaderTileMode.Repeat;

    /// <summary>
    /// ...
    /// </summary>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var colorsHexArray = Colors?.Split(',');
        if (colorsHexArray is null || colorsHexArray.Length == 0)
            return new RadialGradientPaint(SKColors.Transparent, SKColors.Transparent);

        var colors = new SKColor[colorsHexArray.Length];
        for (var i = 0; i < colors.Length; i++)
        {
            colors[i] = SKColor.TryParse(colorsHexArray[i], out var color)
                ? color
                : SKColors.Transparent;
        }

        if (colors.Length == 0)
            return new RadialGradientPaint(SKColors.Transparent, SKColors.Transparent);

        var center = ParsePoint(Center, new(0.5f, 0.5f));

        var colorPositionsSplit = ColorPositions?.Split(',');
        float[]? colorPositions = null;

        if (colorPositionsSplit is not null && colorPositionsSplit.Length > 0)
        {
            colorPositions = new float[colorPositionsSplit.Length];
            for (var i = 0; i < colorPositionsSplit.Length; i++)
            {
                colorPositions[i] = float.TryParse(colorPositionsSplit[i], out var value)
                    ? value
                    : 0f;
            }
        }

        var paint = new RadialGradientPaint([.. colors], center, Radius, colorPositions, TileMode);

        MapProperties(paint);

        return paint;
    }
}
