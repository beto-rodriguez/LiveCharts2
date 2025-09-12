using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WPF;
using ViewModelsSamples.General.TemplatedTooltips;

namespace WPFSample.General.TemplatedTooltips;

// WPF xaml parser does not support generic types
// we instead create a non-generic class that inherits from the generic one.
public class CustomGeometryPointColumnSeries : XamlColumnSeries<GeometryPoint> { }
