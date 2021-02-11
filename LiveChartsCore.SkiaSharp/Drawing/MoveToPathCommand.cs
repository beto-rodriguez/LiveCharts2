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

namespace LiveChartsCore.Drawing
{
    public class MoveToPathCommand : PathCommand
    {
        private FloatTransition xTransition;
        private FloatTransition yTransition;

        public MoveToPathCommand()
        {
            xTransition = new FloatTransition(0f);
            yTransition = new FloatTransition(0f);
        }

        public MoveToPathCommand(float x, float y)
        {
            xTransition = new FloatTransition(x);
            yTransition = new FloatTransition(y);
        }

        public float X { get => xTransition.GetCurrentMovement(this); set => xTransition.MoveTo(value, this); }
        public float Y { get => yTransition.GetCurrentMovement(this); set => yTransition.MoveTo(value, this); }

        public override void Excecute(SKPath path)
        {
            path.MoveTo(X, Y);
        }
    }
}
