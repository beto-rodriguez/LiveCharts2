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

using LiveChartsCore.Generators;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.SkiaSharpView.Blazor;

// ==============================================================================================================
// the static fileds in this file generate bindable/dependency/avalonia or whatever properties...
// the disabled warning make it easier to maintain the code.
//
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0052 // Remove unread private member
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS8618  // Non-nullable field must contain a non-null value when exiting constructor.
#pragma warning disable CS0169  // The field is never used
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0040 // Add accessibility modifiers
#pragma warning disable format
// ==============================================================================================================

public partial class PieChart
{
    static LiveChartsSettings d = LiveCharts.DefaultSettings;

    /// <inheritdoc cref="IPieChartView.IsClockwise"/>
    static UIProperty<bool>                           isClockwise         = new(defaultValue: d.PieIsClockwise);
    /// <inheritdoc cref="IPieChartView.InitialRotation"/>
    static UIProperty<double>                         initialRotation     = new(defaultValue: d.PieInitialRotation);
    /// <inheritdoc cref="IPieChartView.MaxAngle"/>
    static UIProperty<double>                         maxAngle            = new(defaultValue: d.PieMaxAngle);
    /// <inheritdoc cref="IPieChartView.MaxValue"/>
    static UIProperty<double>                         maxValue            = new(defaultValue: d.PieMaxValue);
    /// <inheritdoc cref="IPieChartView.MinValue"/>
    static UIProperty<double>                         minValue            = new(defaultValue: d.PieMinValue);
}
