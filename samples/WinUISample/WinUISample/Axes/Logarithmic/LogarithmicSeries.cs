using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinUI;
using ViewModelsSamples.Axes.Logarithmic;

namespace WinUISample.Axes.Logarithmic;

// WinUI xaml parser does not support generic types
// we instead create a non-generic class that inherits from the generic one.
public class LogarithmicSeries : XamlLineSeries<LogarithmicPoint> { }
