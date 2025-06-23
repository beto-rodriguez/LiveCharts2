using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.WPF;
using ViewModelsSamples.Lines.Custom;

namespace WPFSample.Lines.Custom;

// WPF xaml parser does not support generic types
// we instead create a non-generic class that inherits from the generic one.

public class CustomStarLineSeries : XamlLineSeries<double, StarGeometry> { }
public class CustomVariableSVGPathLineSeries : XamlLineSeries<double, VariableSVGPathGeometry> { }
public class CustomMyGeometryLineSeries : XamlLineSeries<double, MyGeometry> { }
