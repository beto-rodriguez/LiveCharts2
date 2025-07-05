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

// ===================================================
// THIS FILE INCLUDES THE XAML GENERATED OBJECTS.
// For MAUI, the EmptyContentView class is used as a base class
// it is an empty IView that does nothing, but allows the XAML bindings, styles and templates to work.
// ===================================================

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0052 // Remove unread private members

using System;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Generators;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Providers;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using Microsoft.Maui.Controls;

namespace LiveChartsCore.SkiaSharpView.Maui;

[XamlClass(typeof(Axis))]
public partial class XamlAxis : EmptyContentView, ICartesianAxis
{
    string? IPlane.Name { get => _baseType.Name; set => _baseType.Name = value; }
}

[XamlClass(typeof(PolarAxis))]
public partial class XamlPolarAxis : EmptyContentView, IPolarAxis
{
    string? IPlane.Name { get => _baseType.Name; set => _baseType.Name = value; }
}

[XamlClass(typeof(DateTimeAxis), GenerateBaseTypeDeclaration = false)]
public partial class XamlDateTimeAxis : EmptyContentView, ICartesianAxis
{
    private readonly DateTimeAxis _baseType = new(TimeSpan.FromDays(1), date => date.ToString("d"));
    private static readonly DateTimeAxis _defaultDateTimeAxis = new(TimeSpan.FromDays(1), date => date.ToString("d"));

    private static readonly UIProperty<TimeSpan> interval = new(TimeSpan.FromDays(1), XamlGeneration.OnAxisIntervalChanged);
    private static readonly UIProperty<Func<DateTime, string>> dateFormatter = new(null, XamlGeneration.OnDateTimeAxisDateFormatterChanged);

    string? IPlane.Name { get => _baseType.Name; set => _baseType.Name = value; }
}

[XamlClass(typeof(TimeSpanAxis), GenerateBaseTypeDeclaration = false)]
public partial class XamlTimeSpanAxis : EmptyContentView, ICartesianAxis
{
    private readonly TimeSpanAxis _baseType = new(TimeSpan.FromMilliseconds(1), date => $"{date:fff}ms");
    private static readonly TimeSpanAxis _defaultTimeSpanAxis = new(TimeSpan.FromMilliseconds(1), date => $"{date:fff}ms");

    private static readonly UIProperty<TimeSpan> interval = new(TimeSpan.FromSeconds(1), XamlGeneration.OnAxisIntervalChanged);
    private static readonly UIProperty<Func<TimeSpan, string>> timeFormatter = new(null, XamlGeneration.OnTimeSpanAxisFormatterChanged);

    string? IPlane.Name { get => _baseType.Name; set => _baseType.Name = value; }
}

[XamlClass(typeof(LogarithmicAxis), GenerateBaseTypeDeclaration = false)]
public partial class XamlLogarithmicAxis : EmptyContentView, ICartesianAxis
{
    private readonly LogarithmicAxis _baseType = new(10);
    private static readonly LogarithmicAxis _defaultLogarithmicAxis = new(10);

    private static readonly UIProperty<double> logBase = new(10d, XamlGeneration.OnAxisLogBaseChanged);

    string? IPlane.Name { get => _baseType.Name; set => _baseType.Name = value; }
}

[XamlClass(typeof(DrawnLabelVisual), Map = typeof(LabelGeometry), MapPath = "DrawnLabel")]
public partial class XamlDrawnLabelVisual : EmptyContentView, IChartElement, IInternalInteractable
{
    private static readonly LabelGeometry _defaultDrawnLabel = new();
    private LabelGeometry DrawnLabel => (LabelGeometry?)_baseType.DrawnElement
        ?? throw new Exception("Drawn element not found");
}

[XamlClass(typeof(RectangularSection))]
public partial class XamlRectangularSection : EmptyContentView, IChartElement { }

[XamlClass(typeof(ColumnSeries<,,>), TVisual = typeof(RoundedRectangleGeometry))]
public partial class XamlColumnSeries<TModel, TVisual, TLabel> : XamlSeries, IBarSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{ }

[XamlClass(typeof(RowSeries<,,>), TVisual = typeof(RoundedRectangleGeometry))]
public partial class XamlRowSeries<TModel, TVisual, TLabel> : XamlSeries, IBarSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{ }

[XamlClass(typeof(LineSeries<,,>), TVisual = typeof(CircleGeometry))]
public partial class XamlLineSeries<TModel, TVisual, TLabel> : XamlSeries, ILineSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{ }

[XamlClass(typeof(StepLineSeries<,,>), TVisual = typeof(CircleGeometry))]
public partial class XamlStepLineSeries<TModel, TVisual, TLabel> : XamlSeries, IStepLineSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{ }

[XamlClass(typeof(ScatterSeries<,,>), TVisual = typeof(CircleGeometry))]
public partial class XamlScatterSeries<TModel, TVisual, TLabel> : XamlSeries, IScatterSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{ }

[XamlClass(typeof(CandlesticksSeries<,,>), TVisual = typeof(CandlestickGeometry))]
public partial class XamlCandlesticksSeries<TModel, TVisual, TLabel> : XamlSeries, IFinancialSeries, IInternalSeries
    where TVisual : BaseCandlestickGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{ }

[XamlClass(typeof(BoxSeries<,,>), TVisual = typeof(BoxGeometry))]
public partial class XamlBoxSeries<TModel, TVisual, TLabel> : XamlSeries, IBoxSeries, IInternalSeries
    where TVisual : BaseBoxGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{ }

[XamlClass(typeof(HeatSeries<,,>), TVisual = typeof(ColoredRectangleGeometry))]
public partial class XamlHeatSeries<TModel, TVisual, TLabel> : XamlSeries, IHeatSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, IColoredGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{ }

[XamlClass(typeof(PieSeries<,,>), TVisual = typeof(DoughnutGeometry))]
public partial class XamlPieSeries<TModel, TVisual, TLabel> : XamlSeries, IPieSeries, IInternalSeries
    where TVisual : BaseDoughnutGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{ }

[XamlClass(typeof(PolarLineSeries<,,>), TVisual = typeof(CircleGeometry))]
public partial class XamlPolarLineSeries<TModel, TVisual, TLabel> : XamlSeries, IPolarLineSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{ }

[XamlClass(typeof(StackedAreaSeries<,,>), TVisual = typeof(CircleGeometry))]
public partial class XamlStackedAreaSeries<TModel, TVisual, TLabel> : XamlSeries, ILineSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{ }

[XamlClass(typeof(StackedColumnSeries<,,>), TVisual = typeof(RoundedRectangleGeometry))]
public partial class XamlStackedColumnSeries<TModel, TVisual, TLabel> : XamlSeries, IBarSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{ }

[XamlClass(typeof(StackedRowSeries<,,>), TVisual = typeof(RoundedRectangleGeometry))]
public partial class XamlStackedRowSeries<TModel, TVisual, TLabel> : XamlSeries, IBarSeries, IInternalSeries
    where TVisual : BoundedDrawnGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{ }

[XamlClass(typeof(PieSeries<,,>),
    FileHeader = "using TModel = LiveChartsCore.Defaults.ObservableValue;",
    TVisual = typeof(DoughnutGeometry))]
public partial class XamlGaugeSeries<TVisual, TLabel> : XamlSeries, IPieSeries, IInternalSeries
    where TVisual : BaseDoughnutGeometry, new()
    where TLabel : BaseLabelGeometry, new()
{
    private readonly ObservableValue _value = new(0d);
    private static readonly UIProperty<double> gaugeValue = new(0d, OnGaugeValueChanged);

    /// <inheritdoc cref="PieSeries{TModel, TVisual, TLabel}.PieSeries()"/>
    public XamlGaugeSeries()
    {
        Setup(this, _value, _baseType.DataFactory);
    }

    internal virtual void Setup(
        IXamlWrapper<IPieSeries> series, ObservableValue value, DataFactory<ObservableValue> dataFactory)
            => XamlGeneration.SetupGaugeSeries(this, _value, _baseType.DataFactory);

    private static void OnGaugeValueChanged(
        XamlGaugeSeries<TVisual, TLabel> series, double oldValue, double newValue) =>
            series._value.Value = newValue;
}

/// <inheritdoc cref="PieSeries{TModel, TVisual, TLabel}"/>
public class XamlGaugeBackgroundSeries : XamlGaugeSeries<DoughnutGeometry, LabelGeometry>
{
    internal override void Setup(
        IXamlWrapper<IPieSeries> series, ObservableValue value, DataFactory<ObservableValue> dataFactory)
            => XamlGeneration.SetupBackgroundGaugeSeries(this, value, dataFactory);
}

/// <inheritdoc cref="PieSeries{TModel, TVisual, TLabel}"/>
public class XamlAngularGaugeSeries : XamlGaugeSeries<DoughnutGeometry, LabelGeometry>
{
    internal override void Setup(
        IXamlWrapper<IPieSeries> series, ObservableValue value, DataFactory<ObservableValue> dataFactory)
            => XamlGeneration.SetupAngularGaugeSeries(this, value, dataFactory);
}

[XamlClass(typeof(NeedleVisual))]
public partial class XamlNeedle : EmptyContentView, IChartElement, IInternalInteractable { }

[XamlClass(typeof(AngularTicksVisual))]
public partial class XamlAngularTicks : EmptyContentView, IChartElement, IInternalInteractable { }

/// <inheritdoc cref="BaseSharedAxesPair"/>
public class SharedAxesPair : BaseSharedAxesPair { }

/// <inheritdoc cref="BaseChartPointState"/>
[ContentProperty(nameof(Setters))]
public class ChartPointState : BaseChartPointState { }

/// <inheritdoc cref="BaseSet"/>
public class Set : BaseSet { }
