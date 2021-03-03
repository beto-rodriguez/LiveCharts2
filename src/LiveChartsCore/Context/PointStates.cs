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
using System;
using System.Collections.Generic;

namespace LiveChartsCore.Context
{
    public class PointStates<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        private Dictionary<string, StrokeAndFillDrawable<TDrawingContext>> states = new Dictionary<string, StrokeAndFillDrawable<TDrawingContext>>();

        public StrokeAndFillDrawable<TDrawingContext>? this[string stateName]
        {
            get
            {
                if (!states.TryGetValue(stateName, out var state)) return null;
                return state;
            }
            set 
            {
                if (value == null)
                    throw new InvalidOperationException(
                        $"A null intance is not valid at this point, to delete a key please use the {nameof(DeleteState)}() method.");

                states[stateName] = value; 
            }
        }

        public void DeleteState(string stateName)
        {
            states.Remove(stateName);
        }
    }
}
