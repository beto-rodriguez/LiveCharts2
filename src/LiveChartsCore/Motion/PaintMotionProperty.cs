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

using LiveChartsCore.Painting;

namespace LiveChartsCore.Motion;

/// <summary>
/// Defines the float motion property class.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PaintMotionProperty"/> class.
/// </remarks>
/// <param name="defaultValue">The default value.</param>
public class PaintMotionProperty(Paint defaultValue = null!)
    : MotionProperty<Paint?>(defaultValue)
{
    internal static Paint? s_activePaint;

    /// <inheritdoc cref="MotionProperty{T}.CanTransitionate"/>
    protected override bool CanTransitionate =>
        (FromValue ?? s_activePaint) is not null &&
        (ToValue ?? s_activePaint) is not null;

    /// <inheritdoc cref="MotionProperty{T}.OnGetMovement(float)" />
    protected override Paint OnGetMovement(float progress)
    {
        // ! canot be null here because of the check in CanTransitionate
        var from = FromValue ?? s_activePaint!.CloneTask();
        var to = ToValue ?? s_activePaint!.CloneTask();

        return from.Transitionate(progress, to);
    }
}
