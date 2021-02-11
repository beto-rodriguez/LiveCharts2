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

using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class CubicBezierSegment : PathCommand
    {
        private readonly PathGeometry pathGeometry;
        private FloatTransitionProperty x0Transition;
        private FloatTransitionProperty y0Transition;
        private FloatTransitionProperty x1Transition;
        private FloatTransitionProperty y1Transition;
        private FloatTransitionProperty x2Transition;
        private FloatTransitionProperty y2Transition;

        public CubicBezierSegment(PathGeometry pathGeometry)
        {
            x0Transition = new FloatTransitionProperty(nameof(X0), 0f);
            y0Transition = new FloatTransitionProperty(nameof(Y0), 0f);
            x1Transition = new FloatTransitionProperty(nameof(X1), 0f);
            y1Transition = new FloatTransitionProperty(nameof(Y1), 0f);
            x2Transition = new FloatTransitionProperty(nameof(X2), 0f);
            y2Transition = new FloatTransitionProperty(nameof(Y2), 0f);
            this.pathGeometry = pathGeometry;
        }

        public float X0 { get => x0Transition.GetCurrentMovement(pathGeometry); set => x0Transition.MoveTo(value, pathGeometry); }

        public float Y0 { get => y0Transition.GetCurrentMovement(pathGeometry); set => y0Transition.MoveTo(value, pathGeometry); }

        public float X1 { get => x1Transition.GetCurrentMovement(pathGeometry); set => x1Transition.MoveTo(value, pathGeometry); }

        public float Y1 { get => y1Transition.GetCurrentMovement(pathGeometry); set => y1Transition.MoveTo(value, pathGeometry); }

        public float X2 { get => x2Transition.GetCurrentMovement(pathGeometry); set => x2Transition.MoveTo(value, pathGeometry); }

        public float Y2 { get => y2Transition.GetCurrentMovement(pathGeometry); set => y2Transition.MoveTo(value, pathGeometry); }

        public override void Excecute(SKPath path)
        {
            path.CubicTo(X0, Y0, X1, Y1, X2, Y2);
        }
    }
}
