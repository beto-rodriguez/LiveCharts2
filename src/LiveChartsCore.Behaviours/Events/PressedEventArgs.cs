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

namespace LiveChartsCore.Behaviours.Events;

/// <summary>
/// Defines the pointer event args.
/// </summary>
public class PressedEventArgs : ScreenEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenEventArgs"/> class.
    /// </summary>
    /// <param name="location">The pointer location.</param>
    /// <param name="isSecondaryPress">Indicates whether the action is secondary.</param>
    /// <param name="originalEvent">The original event.</param>
    public PressedEventArgs(LvcPoint location, bool isSecondaryPress, object originalEvent)
        : base(location, originalEvent)
    {
        IsSecondaryPress = isSecondaryPress;
    }

    /// <summary>
    /// Gets a value indicating whether the action is a secondary press.
    /// </summary>
    public bool IsSecondaryPress { get; }
}
