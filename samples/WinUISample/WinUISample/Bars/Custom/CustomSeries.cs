using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.WinUI;
using ViewModelsSamples.Bars.Custom;

namespace WinUISample.Bars.Custom;

// WinUI xaml parser does not support generic types
// we instead create non-generic classes that inherit from the generic one.
public class DiamondColumnSeries : XamlColumnSeries<double, DiamondGeometry> { }
public class SvgColumnSeries : XamlColumnSeries<double, VariableSVGPathGeometry> { }
public class MyGeometryColumnSeries : XamlColumnSeries<double, MyGeometry> { }
