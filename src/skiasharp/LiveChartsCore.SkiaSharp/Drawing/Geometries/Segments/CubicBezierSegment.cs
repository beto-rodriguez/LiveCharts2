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
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.Motion;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries.Segments
{
    /// <inheritdoc cref="IBezierSegment{TPathContext}" />
    public class CubicBezierSegment : PathCommand, IBezierSegment<SKPath>
    {
        private readonly FloatMotionProperty _x0Transition;
        private readonly FloatMotionProperty _y0Transition;
        private readonly FloatMotionProperty _x1Transition;
        private readonly FloatMotionProperty _y1Transition;
        private readonly FloatMotionProperty _x2Transition;
        private readonly FloatMotionProperty _y2Transition;

        /// <summary>
        /// Initializes a new instance of the <see cref="CubicBezierSegment"/> class.
        /// </summary>
        public CubicBezierSegment()
        {
            _x0Transition = RegisterMotionProperty(new FloatMotionProperty(nameof(X0), 0f));
            _y0Transition = RegisterMotionProperty(new FloatMotionProperty(nameof(Y0), 0f));
            _x1Transition = RegisterMotionProperty(new FloatMotionProperty(nameof(X1), 0f));
            _y1Transition = RegisterMotionProperty(new FloatMotionProperty(nameof(Y1), 0f));
            _x2Transition = RegisterMotionProperty(new FloatMotionProperty(nameof(X2), 0f));
            _y2Transition = RegisterMotionProperty(new FloatMotionProperty(nameof(Y2), 0f));
        }

        /// <inheritdoc cref="IAnimatableBezierSegment.X0" />
        public float X0 { get => _x0Transition.GetMovement(this); set => _x0Transition.SetMovement(value, this); }

        /// <inheritdoc cref="IAnimatableBezierSegment.Y0" />
        public float Y0 { get => _y0Transition.GetMovement(this); set => _y0Transition.SetMovement(value, this); }

        /// <inheritdoc cref="IAnimatableBezierSegment.X1" />
        public float X1 { get => _x1Transition.GetMovement(this); set => _x1Transition.SetMovement(value, this); }

        /// <inheritdoc cref="IAnimatableBezierSegment.Y1" />
        public float Y1 { get => _y1Transition.GetMovement(this); set => _y1Transition.SetMovement(value, this); }

        /// <inheritdoc cref="IAnimatableBezierSegment.X2" />
        public float X2 { get => _x2Transition.GetMovement(this); set => _x2Transition.SetMovement(value, this); }

        /// <inheritdoc cref="IAnimatableBezierSegment.Y2" />
        public float Y2 { get => _y2Transition.GetMovement(this); set => _y2Transition.SetMovement(value, this); }

        /// <inheritdoc cref="IPathCommand{TPathContext}.Execute(TPathContext, long, Animatable)" />
        public override void Execute(SKPath path, long currentTime, Animatable pathGeometry)
        {
            SetCurrentTime(currentTime);
            path.CubicTo(X0, Y0, X1, Y1, X2, Y2);
        }
    }
}
