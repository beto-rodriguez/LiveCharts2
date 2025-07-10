using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.WinUI;

namespace WinUISample.Lines.Basic;

// WinUI xaml parser does not support generic types
// we instead create a non-generic class that inherits from the generic one.
public class CustomStarLineSeries : XamlLineSeries<int, StarGeometry> { }
