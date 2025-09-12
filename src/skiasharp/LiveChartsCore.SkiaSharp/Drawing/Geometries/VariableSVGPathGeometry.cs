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

using LiveChartsCore.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <summary>
/// Defines a geometry that is built from a svg path, the path can change at runtime,
/// this geometry has a performance penalty, use it only when you need to change the path at runtime,
/// if the geometry is static use any other geometry defined in the library, or inherit from
/// <see cref="BaseSVGPathGeometry"/> class instead, and set the path in the constructor.
/// </summary>
public class VariableSVGPathGeometry : BaseSVGPathGeometry, IVariableSvgPath
{
    /// <inheritdoc cref="IVariableSvgPath.SVGPath"/>
    public string? SVGPath
    {
        get;
        set
        {
            if (value == field) return;

            field = value;
            OnPathChanged(value);
        }
    }

    private void OnPathChanged(string? path)
    {
        if (path is null)
        {
            Path = null;
            return;
        }

        Path?.Dispose();
        Path = SKPath.ParseSvgPathData(path);
    }
}
