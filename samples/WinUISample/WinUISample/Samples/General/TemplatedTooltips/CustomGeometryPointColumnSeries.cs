using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinUI;
using ViewModelsSamples.General.TemplatedTooltips;

namespace WinUISample.General.TemplatedTooltips;

// WinUI xaml parser does not support generic types
// we instead create a non-generic class that inherits from the generic one.
public class CustomGeometryPointColumnSeries : XamlColumnSeries<GeometryPoint> { }
