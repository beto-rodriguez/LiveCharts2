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

using System;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Generators;

internal class XamlGeneration
{
    public static void OnAxisIntervalChanged(IXamlWrapper<ICartesianAxis> axis, TimeSpan oldValue, TimeSpan newValue)
    {
        axis.WrappedObject.UnitWidth = newValue.Ticks;
        axis.WrappedObject.MinStep = newValue.Ticks;
    }

    public static void OnDateTimeAxisDateFormatterChanged(IXamlWrapper<ICartesianAxis> axis, Func<DateTime, string> oldValue, Func<DateTime, string> newValue) =>
        axis.WrappedObject.Labeler = value => newValue(value.AsDate());


    public static void OnTimeSpanAxisFormatterChanged(IXamlWrapper<ICartesianAxis> axis, Func<TimeSpan, string> oldValue, Func<TimeSpan, string> newValue) =>
        axis.WrappedObject.Labeler = value => newValue(value.AsTimeSpan());


    public static void OnAxisLogBaseChanged(IXamlWrapper<ICartesianAxis> axis, double oldValue, double newValue) =>
        axis.WrappedObject.SetLogBase(newValue);
}
