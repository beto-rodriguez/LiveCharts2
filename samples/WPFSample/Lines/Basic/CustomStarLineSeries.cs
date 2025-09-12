using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WPF;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace WPFSample.Lines.Basic;

// WPF xaml parser does not support generic types
// we instead create a non-generic class that inherits from the generic one.
public class CustomStarLineSeries : XamlLineSeries<int, StarGeometry> { }
