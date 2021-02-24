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

using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.Motion;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing
{
    public class ArcToSegment : PathCommand, IArcToSegment<SKPath>
    {
        private readonly FloatMotionProperty xTransition;
        private readonly FloatMotionProperty hTransition;
        private readonly FloatMotionProperty wTransition;
        private readonly FloatMotionProperty yTransition;
        private readonly FloatMotionProperty startAngleTransition;
        private readonly FloatMotionProperty sweepAngleTransition;

        public float StartAngle { get => startAngleTransition.GetMovement(this); set => startAngleTransition.SetMovement(value, this); }
        public float SweepAngle { get => startAngleTransition.GetMovement(this); set => sweepAngleTransition.SetMovement(value, this); }
        public float Width { get => xTransition.GetMovement(this); set => xTransition.SetMovement(value, this); }
        public float Height { get => yTransition.GetMovement(this); set => yTransition.SetMovement(value, this); }
        public float X { get => wTransition.GetMovement(this); set => wTransition.SetMovement(value, this); }
        public float Y { get => hTransition.GetMovement(this); set => hTransition.SetMovement(value, this); }

        public override void Execute(SKPath path, long currentTime, Animatable pathGeometry)
        {
            path.ArcTo(new SKRect { Left = X, Top = Y, Size = new SKSize { Width = Width, Height = Height } }, StartAngle, SweepAngle, false);
        }
    }
}
