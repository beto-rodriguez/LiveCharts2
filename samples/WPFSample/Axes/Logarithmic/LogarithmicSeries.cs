using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WPF;
using ViewModelsSamples.Axes.Logarithmic;

namespace WPFSample.Axes.Logarithmic;

// WPF xaml parser does not support generic types
// we instead create a non-generic class that inherits from the generic one.
public class LogarithmicSeries : XamlLineSeries<LogarithmicPoint> { }
