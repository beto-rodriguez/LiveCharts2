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

#pragma warning disable IDE0005 // Using directive is unnecessary.

using System;
using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Generators;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Observers;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;

#if AVALONIA_LVC
using Avalonia;
#endif

// ==============================================================================================================
// the static fileds in this file generate bindable/dependency/avalonia or whatever properties...
// the disabled warnings make it easier to maintain the code.
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

namespace LiveChartsGeneratedCode;

#if SKIA_IMAGE_LVC
public partial class SourceGenSKCartesianChart
#else
public partial class SourceGenCartesianChart
#endif
{
    static LiveChartsSettings d = LiveCharts.DefaultSettings;

    /// <inheritdoc cref="ICartesianChartView.ZoomMode"/>
    static UIProperty<ZoomAndPanMode>               zoomMode =              new(defaultValue: d.ZoomMode);
    /// <inheritdoc cref="ICartesianChartView.ZoomingSpeed"/>
    static UIProperty<double>                       zoomingSpeed =          new(defaultValue: d.ZoomSpeed);
    /// <inheritdoc cref="ICartesianChartView.FindingStrategy"/>
    static UIProperty<FindingStrategy>              findingStrategy =       new(defaultValue: d.FindingStrategy);

    /// <inheritdoc cref="ICartesianChartView.XAxes"/>
    static UIProperty<ICollection<ICartesianAxis>>  xAxes =                 new(onChanged: OnObservedPropertyChanged(nameof(XAxes)));
    /// <inheritdoc cref="ICartesianChartView.YAxes"/>
    static UIProperty<ICollection<ICartesianAxis>>  yAxes =                 new(onChanged: OnObservedPropertyChanged(nameof(YAxes)));
    /// <inheritdoc cref="ICartesianChartView.Sections"/>
    static UIProperty<ICollection<IChartElement>>   sections =              new(onChanged: OnObservedPropertyChanged(nameof(Sections)));
    /// <inheritdoc cref="ICartesianChartView.DrawMarginFrame"/>
    static UIProperty<IChartElement?>               drawMarginFrame =       new(onChanged: OnObservedPropertyChanged(nameof(DrawMarginFrame)));

    /// <inheritdoc cref="ICartesianChartView.FindingStrategy" />
    [Obsolete($"Renamed to {nameof(FindingStrategy)}")]
    public TooltipFindingStrategy TooltipFindingStrategy
    {
        get => FindingStrategy.AsOld();
        set => FindingStrategy = value.AsNew();
    }

#if AVALONIA_LVC
    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        OnXamlPropertyChanged(change);
    }
#endif
}
