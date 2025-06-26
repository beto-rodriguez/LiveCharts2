using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.WinUI;
using ViewModelsSamples.Lines.Custom;

namespace WinUISample.Lines.Custom;

// WinUI xaml parser does not support generic types
// we instead create non-generic classes that inherit from the generic one.
public class CustomStarLineSeries : XamlLineSeries<double, StarGeometry> { }
public class CustomVariableSVGPathLineSeries : XamlLineSeries<double, VariableSVGPathGeometry> { }
public class CustomMyGeometryLineSeries : XamlLineSeries<double, MyGeometry> { }
