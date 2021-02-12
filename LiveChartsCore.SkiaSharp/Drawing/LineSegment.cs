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

using LiveChartsCore.Motion;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class LineSegment : PathCommand
    {
        private readonly PathGeometry pathGeometry;
        private FloatMotionProperty xTransition;
        private FloatMotionProperty yTransition;

        public LineSegment(PathGeometry pathGeometry)
        {
            xTransition = new FloatMotionProperty(nameof(X), 0f);
            yTransition = new FloatMotionProperty(nameof(Y), 0f);
            this.pathGeometry = pathGeometry;
        }

        public float X { get => xTransition.GetMovement(pathGeometry); set => xTransition.SetMovement(value, pathGeometry); }
        public float Y { get => yTransition.GetMovement(pathGeometry); set => yTransition.SetMovement(value, pathGeometry); }

        public override void Excecute(SKPath path)
        {
            path.LineTo(X, Y);
        }
    }
}
