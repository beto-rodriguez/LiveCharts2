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
using System.Collections.Generic;
using LiveChartsCore.Generators;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Observers;
using Microsoft.UI.Xaml;

namespace LiveChartsCore.SkiaSharpView.WinUI;

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

    static XamlProperty<bool>                           isClockwise         = new(defaultValue: d.PieIsClockwise);
    static XamlProperty<double>                         initialRotation     = new(defaultValue: d.PieInitialRotation);
    static XamlProperty<double>                         maxAngle            = new(defaultValue: d.PieMaxAngle);
    static XamlProperty<double>                         maxValue            = new(defaultValue: d.PieMaxValue);
    static XamlProperty<double>                         minValue            = new(defaultValue: d.PieMinValue);

    static XamlProperty<ICollection<ISeries>>           series              = new(onChanged: OnObservedPropertyChanged(nameof(Series)));

    static XamlProperty<IEnumerable<object>>            seriesSource        = new(onChanged: OnSeriesSourceChanged);
    static XamlProperty<DataTemplate>                   seriesTemplate      = new(onChanged: OnSeriesSourceChanged);

    static void OnSeriesSourceChanged(PieChart chart)
    {
        var seriesObserver = (SeriesSourceObserver)chart.Observe[nameof(SeriesSource)];
        seriesObserver.Initialize(chart.SeriesSource);

        if (seriesObserver.Series is not null)
            chart.Series = seriesObserver.Series;
    }

#pragma warning disable IDE0060 // Remove unused parameter, hack for the source generator
    static Action<PieChart, object, object> OnObservedPropertyChanged(
        string propertyName, object? a = null, object? b = null) =>
            (chart, o, n) =>
            {
                chart.Observe[propertyName].Initialize(n);
                chart.CoreChart.Update();
            };
#pragma warning restore IDE0060 // Remove unused parameter
}
