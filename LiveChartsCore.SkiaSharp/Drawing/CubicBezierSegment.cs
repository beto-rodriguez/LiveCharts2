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

using LiveChartsCore.Context;
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.Drawing
{
    public class CubicBezierSegment : PathCommand
    {
        private FloatTransition x0Transition;
        private FloatTransition y0Transition;
        private FloatTransition x1Transition;
        private FloatTransition y1Transition;
        private FloatTransition x2Transition;
        private FloatTransition y2Transition;

        public CubicBezierSegment()
        {
            x0Transition = new FloatTransition(0f);
            y0Transition = new FloatTransition(0f);
            x1Transition = new FloatTransition(0f);
            y1Transition = new FloatTransition(0f);
            x2Transition = new FloatTransition(0f);
            y2Transition = new FloatTransition(0f);
        }

        public CubicBezierSegment(float x0, float y0, float x1, float y1, float x2, float y2)
        {
            x0Transition = new FloatTransition(x0);
            y0Transition = new FloatTransition(y0);
            x1Transition = new FloatTransition(x1);
            y1Transition = new FloatTransition(y1);
            x2Transition = new FloatTransition(x2);
            y2Transition = new FloatTransition(y2);
        }

        public CubicBezierSegment(BezierData data)
        {
            x0Transition = new FloatTransition(data.X0);
            y0Transition = new FloatTransition(data.Y0);
            x1Transition = new FloatTransition(data.X1);
            y1Transition = new FloatTransition(data.Y1);
            x2Transition = new FloatTransition(data.X2);
            y2Transition = new FloatTransition(data.Y2);
        }

        public float X0 { get => x0Transition.GetCurrentMovement(this); set => x0Transition.MoveTo(value, this); }

        public float Y0 { get => y0Transition.GetCurrentMovement(this); set => y0Transition.MoveTo(value, this); }

        public float X1 { get => x1Transition.GetCurrentMovement(this); set => x1Transition.MoveTo(value, this); }

        public float Y1 { get => y1Transition.GetCurrentMovement(this); set => y1Transition.MoveTo(value, this); }

        public float X2 { get => x2Transition.GetCurrentMovement(this); set => x2Transition.MoveTo(value, this); }

        public float Y2 { get => y2Transition.GetCurrentMovement(this); set => y2Transition.MoveTo(value, this); }

        public override void Excecute(SKPath path)
        {
            path.CubicTo(X0, Y0, X1, Y1, X2, Y2);
        }
    }
}
