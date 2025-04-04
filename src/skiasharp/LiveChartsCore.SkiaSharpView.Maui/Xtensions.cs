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
using System.Collections.Generic;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.Painting.ImageFilters;
using Microsoft.Maui.Controls.Xaml;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Maui;

/// <summary>
/// The base skia paint extension.
/// </summary>
public abstract class BaseSkiaPaintExtention
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
    /// Gets or sets the dash array separated by commas.
    /// </summary>
    public string? DashArray { get; set; } = null;

    /// <summary>
    /// Gets or sets the dash phase.
    /// </summary>
    public float DashPhase { get; set; } = 0f;

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

        if (DashArray is not null)
        {
            var dashArray = DashArray.Split(',');
            var dashes = new List<float>(dashArray.Length);
            foreach (var dash in dashArray)
            {
                if (float.TryParse(dash, out var value))
                    dashes.Add(value);
            }
            paint.PathEffect = new DashEffect([.. dashes], DashPhase);
        }

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
public class SolidColorPaintExtension : BaseSkiaPaintExtention, IMarkupExtension<SolidColorPaint>
{
    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    public string? Color { get; set; } = null;

    /// <summary>
    /// ...
    /// </summary>
    public SolidColorPaint ProvideValue(IServiceProvider serviceProvider)
    {
        if (!SKColor.TryParse(Color, out var color))
            return new SolidColorPaint(SKColors.Transparent);

        var paint = new SolidColorPaint(color);

        MapProperties(paint);

        return paint;
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
}

/// <summary>
/// The linear gradient paint extension.
/// </summary>
public class LinearGradientPaintExtension : BaseSkiaPaintExtention, IMarkupExtension<LinearGradientPaint>
{
    /// <summary>
    /// Gets or sets the colors separated by commas.
    /// </summary>
    public string? Colors { get; set; } = null;

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
    public LinearGradientPaint ProvideValue(IServiceProvider serviceProvider)
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

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
}

/// <summary>
/// The radial gradient paint extension.
/// </summary>
public class RadialGradientPaintExtension : BaseSkiaPaintExtention, IMarkupExtension<RadialGradientPaint>
{
    /// <summary>
    /// Gets or sets the colors separated by commas.
    /// </summary>
    public string? Colors { get; set; } = null;

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
    public RadialGradientPaint ProvideValue(IServiceProvider serviceProvider)
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

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
}

/// <summary>
/// The from shared axes extension.
/// </summary>
public class FrameExtension : IMarkupExtension<DrawMarginFrame>
{
    /// <summary>
    /// Gets or sets the fill.
    /// </summary>
    public Paint? Fill { get; set; }

    /// <summary>
    /// Gets or sets the stroke.
    /// </summary>
    public Paint? Stroke { get; set; }

    /// <summary>
    /// ...
    /// </summary>
    public DrawMarginFrame ProvideValue(IServiceProvider serviceProvider)
    {
        return new DrawMarginFrame
        {
            Fill = Fill,
            Stroke = Stroke
        };
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
}

/// <summary>
/// The from shared axes extension.
/// </summary>
public class FromSharedAxesExtension : IMarkupExtension<ICollection<ICartesianAxis>>
{
    /// <summary>
    /// Gets or sets the pair instance.
    /// </summary>
    public SharedAxesPair? AxesPair { get; set; }

    /// <summary>
    /// Gets or sets the element.
    /// </summary>
    public PairElement Element { get; set; }

    /// <summary>
    /// ...
    /// </summary>
    public ICollection<ICartesianAxis> ProvideValue(IServiceProvider serviceProvider)
    {
#pragma warning disable IDE0046 // Convert to conditional expression
        if (AxesPair is null || AxesPair.First is null || AxesPair.Second is null)
            return [];
#pragma warning restore IDE0046 // Convert to conditional expression

        return Element switch
        {
            PairElement.First => [AxesPair.First],
            PairElement.Second => [AxesPair.Second],
            _ => Array.Empty<ICartesianAxis>()
        };
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);

    /// <summary>
    /// The shared axes pair elements.
    /// </summary>
    public enum PairElement
    {
        /// <summary>
        /// The first element.
        /// </summary>
        First,

        /// <summary>
        /// The second element.
        /// </summary>
        Second
    }
}
