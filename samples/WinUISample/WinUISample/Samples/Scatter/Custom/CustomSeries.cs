using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.Defaults;
using ViewModelsSamples.Scatter.Custom;
using LiveChartsCore.SkiaSharpView.WinUI;

namespace WinUISample.Scatter.Custom;

// WinUI xaml parser does not support generic types
// we instead create non-generic classes that inherit from the generic one.
public class HeartScatterSeries : XamlScatterSeries<ObservablePoint, HeartGeometry> { }
public class SvgScatterSeries : XamlScatterSeries<ObservablePoint, VariableSVGPathGeometry> { }
public class MyGeometryScatterSeries : XamlScatterSeries<ObservablePoint, MyGeometry> { }
