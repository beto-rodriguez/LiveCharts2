﻿// The MIT License(MIT)
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

namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// Defines a stepline visual point.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <typeparam name="TStepLineSegment">The type of the stepline segment.</typeparam>
    /// <typeparam name="TPathArgs">The type of the path arguments.</typeparam>
    /// <seealso cref="IStepLineVisualChartPoint{TDrawingContext}" />
    public class StepLineVisualPoint<TDrawingContext, TVisual, TStepLineSegment, TPathArgs> : IStepLineVisualChartPoint<TDrawingContext>
        where TVisual : ISizedVisualChartPoint<TDrawingContext>, new()
        where TStepLineSegment : IStepLineSegment<TPathArgs>, new()
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Gets the geometry.
        /// </summary>
        /// <value>
        /// The geometry.
        /// </value>
        public TVisual Geometry { get; set; } = new TVisual();

        /// <summary>
        /// Gets the stepline.
        /// </summary>
        /// <value>
        /// The stepline.
        /// </value>
        public TStepLineSegment StepSegment { get; set; } = new TStepLineSegment();

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public IPathGeometry<TDrawingContext, TPathArgs>? FillPath { get; set; }

        /// <summary>
        /// Gets or sets the stroke path.
        /// </summary>
        /// <value>
        /// The stroke path.
        /// </value>
        public IPathGeometry<TDrawingContext, TPathArgs>? StrokePath { get; set; }

        /// <summary>
        /// Gets the main <see cref="T:LiveChartsCore.Drawing.IDrawable`1" />.
        /// </summary>
        public IDrawable<TDrawingContext>? HighlightableGeometry => Geometry?.HighlightableGeometry;

        ISizedGeometry<TDrawingContext> IStepLineVisualChartPoint<TDrawingContext>.Geometry => Geometry;
        IAnimatableStepLineSegment IStepLineVisualChartPoint<TDrawingContext>.StepLine => StepSegment;
    }
}
