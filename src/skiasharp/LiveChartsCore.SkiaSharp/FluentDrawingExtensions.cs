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
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView;

/// <summary>
/// Defines the drawing extensions.
/// </summary>
public static class DrawingFluentExtensions
{
    /// <summary>
    /// Adds a paint task to the canvas.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    public static Drawing Draw(this MotionCanvas<SkiaSharpDrawingContext> canvas)
    {
        return new Drawing(canvas);
    }

    /// <summary>
    /// Defines the Drawing class.
    /// </summary>
    public class Drawing
    {
        private IPaint<SkiaSharpDrawingContext>? _selectedPaint;

        /// <summary>
        /// Initializes a new instance of the Drawing class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public Drawing(MotionCanvas<SkiaSharpDrawingContext> canvas)
        {
            Canvas = canvas;
        }

        /// <summary>
        /// Gets the canvas.
        /// </summary>
        public MotionCanvas<SkiaSharpDrawingContext> Canvas { get; }

        /// <summary>
        /// Selects the specified color.
        /// </summary>
        /// <param name="color">The color to draw with.</param>
        /// <param name="strokeWidth">The stroke width.</param>
        /// <param name="isFill">Indicates whether the geometries are filled with the specified color.</param>
        /// <returns>The current drawing instance.</returns>
        public Drawing SelectColor(SKColor color, float? strokeWidth = null, bool? isFill = null)
        {
            strokeWidth ??= 1;
            isFill ??= false;
            _selectedPaint = new SolidColorPaint(color, strokeWidth.Value) { IsFill = isFill.Value };
            Canvas.AddDrawableTask(_selectedPaint);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public Drawing DrawGeometry(IGeometry<SkiaSharpDrawingContext> geometry)
        {
            if (_selectedPaint is null)
                throw new Exception(
                    "There is no paint selected, please select a paint (By calling a Select method) to add the geometry to.");

            _selectedPaint.AddGeometryToPaintTask(Canvas, geometry);

            return this;
        }
    }
}
