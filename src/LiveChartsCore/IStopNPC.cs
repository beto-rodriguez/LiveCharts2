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

using System.ComponentModel;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines an object that helps the library to stop listening to <see cref="INotifyPropertyChanged"/>, this is used internally
    /// to apply themes and avoid the chart to update as we apply it.
    /// </summary>
    public interface IStopNPC
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is notifying changes, this property is used internally to turn off
        /// notifications while the theme is being applied, this property is not designed to be used by the user.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is notifying changes; otherwise, <c>false</c>.
        /// </value>
        bool IsNotifyingChanges { get; set; }
    }
}
