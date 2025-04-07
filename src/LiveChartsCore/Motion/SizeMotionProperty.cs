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

namespace LiveChartsCore.Motion;

/// <summary>
/// Defines the <see cref="LvcSize"/> motion property class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SizeMotionProperty"/> class.
/// </remarks>
/// <param name="defaultValue">The default value.</param>
public class SizeMotionProperty(LvcSize defaultValue = new())
    : MotionProperty<LvcSize>(defaultValue)
{
    /// <inheritdoc cref="MotionProperty{T}.OnGetMovement(float)" />
    protected override LvcSize OnGetMovement(float progress) =>
        new(
            FromValue.Width + progress * (ToValue.Width - FromValue.Width),
            FromValue.Height + progress * (ToValue.Height - FromValue.Height));
}
