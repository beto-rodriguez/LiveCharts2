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

using Avalonia;
using LiveChartsCore.Motion;

namespace LiveChartsCore.AvaloniaView.Motion
{
    public class MatrixMotionProperty : MotionProperty<Matrix>
    {
        public MatrixMotionProperty(string propertyName)
            : base(propertyName)
        {

        }

        protected override Matrix OnGetMovement(float progress)
        {
            return new Matrix(
                fromValue.M11 + progress * (toValue.M11 - fromValue.M11),
                fromValue.M12 + progress * (toValue.M12 - fromValue.M12),
                fromValue.M21 + progress * (toValue.M21 - fromValue.M21),
                fromValue.M22 + progress * (toValue.M22 - fromValue.M22),
                fromValue.M31 + progress * (toValue.M31 - fromValue.M31),
                fromValue.M32 + progress * (toValue.M32 - fromValue.M32));
        }
    }
}
