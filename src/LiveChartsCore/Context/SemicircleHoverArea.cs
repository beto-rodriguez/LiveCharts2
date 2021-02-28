// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Drawing;

namespace LiveChartsCore.Context
{
    public class SemicircleHoverArea : HoverArea
    {
        private float startAngle;
        private float endAngle;
        private float radius;
        private float centerX;
        private float centerY;

        public float CenterX { get => centerX; set => centerX = value; }
        public float CenterY { get => centerY; set => centerY = value; }
        /// <summary>
        /// Gets or sets the start angle in degrees.
        /// </summary>
        public float StartAngle { get => startAngle; set => startAngle = value; }
        /// <summary>
        /// Gets or sets the and angle in degrees.
        /// </summary>
        public float EndAngle { get => endAngle; set => endAngle = value; }
        public float Radius { get => radius; set => radius = value; }

        public SemicircleHoverArea SetDimensions(float centerX, float centerY, float startAngle, float endAngle, float radius)
        {
            this.centerX = centerX;
            this.centerY = centerY;
            this.startAngle = startAngle;
            this.endAngle = endAngle;
            this.radius = radius;
            return this;
        }

        public override bool IsTriggerBy(PointF point, TooltipFindingStrategy strategy)
        {
            var dx = centerX - point.X;
            var dy = centerY - point.Y;
            var beta = Math.Atan(dy / dx) * (180 / Math.PI);
            if ((dx > 0 && dy < 0) || (dx > 0 && dy > 0)) beta += 180;
            if (dx < 0 && dy > 0) beta += 360;

            var r = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

            return startAngle <= beta && endAngle >= beta && r < radius;
        }

        public override void SuggestTooltipPlacement(TooltipPlacementContext context)
        {
            var angle = (startAngle + endAngle) / 2;
            context.PieX = centerX + (float) Math.Cos(angle * (Math.PI / 180)) * radius;
            context.PieY = centerY + (float)Math.Sin(angle * (Math.PI / 180)) * radius;
        }
    }
}
