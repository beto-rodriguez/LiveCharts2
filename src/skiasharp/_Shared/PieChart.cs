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

using System.Collections.ObjectModel;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;

#if AVALONIA_LVC
namespace LiveChartsCore.SkiaSharpView.Avalonia;
#elif BLAZOR_LVC
namespace LiveChartsCore.SkiaSharpView.Blazor;
#elif ETO_LVC
namespace LiveChartsCore.SkiaSharpView.Eto;
#elif MAUI_LVC
namespace LiveChartsCore.SkiaSharpView.Maui;
#elif WINUI_LVC
namespace LiveChartsCore.SkiaSharpView.WinUI;
#elif WINFORMS_LVC
namespace LiveChartsCore.SkiaSharpView.WinForms;
#elif WPF_LVC
namespace LiveChartsCore.SkiaSharpView.WPF;
#endif

/// <inheritdoc cref="IPieChartView" />
public partial class PieChart : ChartControl, IPieChartView
{
    PieChartEngine IPieChartView.Core => (PieChartEngine)CoreChart;

    /// <inheritdoc cref="ChartControl.CreateCoreChart"/>
    protected override Chart CreateCoreChart() =>
         new PieChartEngine(this, config => config.UseDefaults(), CanvasView!.CanvasCore);

    /// <inheritdoc cref="ChartControl.IninializeObservedProperties"/>
    protected override void IninializeObservedProperties()
    {
        Series = new ObservableCollection<ISeries>();
        VisualElements = new ObservableCollection<IChartElement>();
        SyncContext = new object();
    }
}
