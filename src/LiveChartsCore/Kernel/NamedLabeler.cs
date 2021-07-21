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

using System.Collections.Generic;

namespace LiveChartsCore.Kernel
{
    /// <summary>
    /// The named labeler helper class.
    /// </summary>
    public class NamedLabeler
    {
        private readonly IList<string> _labels;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedLabeler"/> class.
        /// </summary>
        /// <param name="labels">The labels.</param>
        public NamedLabeler(IList<string> labels)
        {
            _labels = labels;
        }

        /// <summary>
        /// Functions the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public string Function(double value)
        {
            var index = (int)value;

            return index < 0 || index > _labels.Count - 1
                ? string.Empty
                : _labels[index];
        }
    }
}
