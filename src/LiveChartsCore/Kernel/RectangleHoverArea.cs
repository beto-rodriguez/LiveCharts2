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
using System.Drawing;

namespace LiveChartsCore.Kernel
{
    /// <summary>
    /// Defines a rectangle hover area.
    /// </summary>
    /// <seealso cref="HoverArea" />
    public class RectangleHoverArea : HoverArea
    {
        private float height;

        /// <summary>
        /// Gets or sets the x location.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the y location.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public float Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public float Height { get => height; set => height = value; }

        /// <summary>
        /// Sets the area dimensions.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public RectangleHoverArea SetDimensions(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.height = height;
            return this;
        }

        /// <inheritdoc cref="IsTriggerBy(PointF, TooltipFindingStrategy)"/>
        public override bool IsTriggerBy(PointF point, TooltipFindingStrategy strategy)
        {
            return strategy == TooltipFindingStrategy.CompareAll
                ? point.X >= X && point.X <= X + Width && point.Y >= Y && point.Y <= Y + height
                : (strategy == TooltipFindingStrategy.CompareOnlyY || (point.X >= X && point.X <= X + Width)) &&
                  (strategy == TooltipFindingStrategy.CompareOnlyX || (point.Y >= Y && point.Y <= Y + height));
        }

        /// <inheritdoc cref="SuggestTooltipPlacement(TooltipPlacementContext)"/>
        public override void SuggestTooltipPlacement(TooltipPlacementContext cartesianContext)
        {
            if (Y < cartesianContext.MostTop) cartesianContext.MostTop = Y;
            if (Y + height > cartesianContext.MostBottom) cartesianContext.MostBottom = Y + height;
            if (X + Width > cartesianContext.MostRight) cartesianContext.MostRight = X + Width;
            if (X < cartesianContext.MostLeft) cartesianContext.MostLeft = X;
        }
    }
}
