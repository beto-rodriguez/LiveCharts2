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

namespace LiveChartsCore.Measure;

/// <summary>
/// 
/// </summary>
public enum PolarLabelsPosition
{
    /// <summary>
    /// Places the label at the center of the chart.
    /// </summary>
    ChartCenter,

    /// <summary>
    /// Aligns the label to the end of the angle of the shape.
    /// </summary>
    End,

    /// <summary>
    /// Aligns the label to the start of the angle of the shape.
    /// </summary>
    Start,

    /// <summary>
    /// Aligns the label to the middle of the angle of the shape and the at the circumference radius.
    /// </summary>
    Middle,

    /// <summary>
    /// Aligns the label to the middle of the angle of the shape and the at the circumference diameter.
    /// </summary>
    Outer

}
