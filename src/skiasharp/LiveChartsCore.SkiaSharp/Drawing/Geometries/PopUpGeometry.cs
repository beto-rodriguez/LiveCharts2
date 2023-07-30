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

using LiveChartsCore.Measure;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <summary>
/// Defines a pop-up geometry.
/// </summary>
public class PopUpGeometry : SizedGeometry
{
    /// <summary>
    /// Gets or sets the wedge size.
    /// </summary>
    public double Wedge { get; set; } = 15;

    /// <summary>
    /// Gets or sets the wedge thickness, it controls the width of the wedge,
    /// the value is normalized, where 1 means the <see cref="Wedge"/> size, default is 2.
    /// </summary>
    public double WedgeThickness { get; set; } = 2;

    /// <summary>
    /// Gets or sets the border radius.
    /// </summary>
    public double BorderRadius { get; set; } = 10;

    /// <summary>
    /// Gets or sets the placement.
    /// </summary>
    public PopUpPlacement Placement { get; set; } = PopUpPlacement.Bottom;

    public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        using var path = new SKPath();

        var wedge = (float)Wedge;
        var br = (float)BorderRadius;
        var wf = (float)WedgeThickness;
        var x = X + (Placement == PopUpPlacement.Right ? 1 : 0) * wedge;
        var y = Y + (Placement == PopUpPlacement.Bottom ? 1 : 0) * wedge;
        var w = Width - (Placement is PopUpPlacement.Right or PopUpPlacement.Left ? 1 : 0) * wedge;
        var h = Height - (Placement is PopUpPlacement.Bottom or PopUpPlacement.Top ? 1 : 0) * wedge;

        path.MoveTo(x + br, y);

        if (Placement == PopUpPlacement.Bottom)
        {
            path.LineTo(x + (w * 0.5f - wedge * wf * 0.5f), y);
            path.LineTo(x + w * 0.5f, y - wedge);
            path.LineTo(x + (w * 0.5f + wedge * wf * 0.5f), y);
        }

        path.LineTo(x + w - br, y);
        path.ArcTo(new SKRect(x + w - br, y, x + w, y + br), 270, 90, false);

        if (Placement == PopUpPlacement.Left)
        {
            path.LineTo(x + w, y + (h * 0.5f - wedge * wf * 0.5f));
            path.LineTo(x + w + wedge, y + h * 0.5f);
            path.LineTo(x + w, y + (h * 0.5f + wedge * wf * 0.5f));
        }

        path.LineTo(x + w, y + h - br);
        path.ArcTo(new SKRect(x + w - br, y + h - br, x + w, y + h), 0, 90, false);

        if (Placement == PopUpPlacement.Top)
        {
            path.LineTo(x + (w * 0.5f + wedge * wf * 0.5f), y + h);
            path.LineTo(x + w * 0.5f, y + h + wedge);
            path.LineTo(x + (w * 0.5f - wedge * wf * 0.5f), y + h);
        }

        path.LineTo(x + br, y + h);
        path.ArcTo(new SKRect(x, y + h - br, x + br, y + h), 90, 90, false);

        if (Placement == PopUpPlacement.Right)
        {
            path.LineTo(x, y + (h * 0.5f + wedge * wf * 0.5f));
            path.LineTo(x - wedge, y + h * 0.5f);
            path.LineTo(x, y + (h * 0.5f - wedge * wf * 0.5f));
        }

        path.LineTo(x, y + br);
        path.ArcTo(new SKRect(x, y, x + br, y + br), 180, 90, false);

        path.Close();

        context.Canvas.DrawPath(path, context.Paint);
    }
}
