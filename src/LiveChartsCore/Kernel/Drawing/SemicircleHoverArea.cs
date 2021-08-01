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
using System;
using System.Drawing;

namespace LiveChartsCore.Kernel.Drawing
{
    /// <summary>
    /// Defines a semicircle hover area.
    /// </summary>
    /// <seealso cref="HoverArea" />
    public class SemicircleHoverArea : HoverArea
    {
        /// <summary>
        /// Gets or sets the center x.
        /// </summary>
        /// <value>
        /// The center x coordinate.
        /// </value>
        public float CenterX { get; set; }

        /// <summary>
        /// Gets or sets the center y.
        /// </summary>
        /// <value>
        /// The center y coordinate.
        /// </value>
        public float CenterY { get; set; }

        /// <summary>
        /// Gets or sets the start angle in degrees.
        /// </summary>
        public float StartAngle { get; set; }

        /// <summary>
        /// Gets or sets the and angle in degrees.
        /// </summary>
        public float EndAngle { get; set; }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>
        /// The radius.
        /// </value>
        public float Radius { get; set; }

        /// <summary>
        /// Sets the area dimensions.
        /// </summary>
        /// <param name="centerX">The center x.</param>
        /// <param name="centerY">The center y.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="endAngle">The end angle.</param>
        /// <param name="radius">The radius.</param>
        /// <returns></returns>
        public SemicircleHoverArea SetDimensions(float centerX, float centerY, float startAngle, float endAngle, float radius)
        {
            CenterX = centerX;
            CenterY = centerY;
            StartAngle = startAngle;
            EndAngle = endAngle;
            Radius = radius;
            return this;
        }

        /// <inheritdoc cref="GetDistanceToPoint(PointF, TooltipFindingStrategy)"/>
        public override float GetDistanceToPoint(PointF point, TooltipFindingStrategy strategy)
        {
            var startAngle = StartAngle % 360;
            // -0.01 is a work around to avoid the case where the last slice (360) would be converted to 0 also
            var endAngle = (EndAngle - 0.01) % 360;

            var dx = CenterX - point.X;
            var dy = CenterY - point.Y;
            var beta = Math.Atan(dy / dx) * (180 / Math.PI);

            if (dx > 0 && dy < 0 || dx > 0 && dy > 0) beta += 180;
            if (dx < 0 && dy > 0) beta += 360;

            var r = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

            return startAngle <= beta && endAngle >= beta && r < Radius ? 0 : float.MaxValue;
        }

        /// <inheritdoc cref="SuggestTooltipPlacement(TooltipPlacementContext)"/>
        public override void SuggestTooltipPlacement(TooltipPlacementContext context)
        {
            var angle = (StartAngle + EndAngle) / 2;
            context.PieX = CenterX + (float)Math.Cos(angle * (Math.PI / 180)) * Radius;
            context.PieY = CenterY + (float)Math.Sin(angle * (Math.PI / 180)) * Radius;
        }
    }
}
