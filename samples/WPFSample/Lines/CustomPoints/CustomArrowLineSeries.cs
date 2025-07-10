using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WPF;
using ViewModelsSamples.Lines.CustomPoints;

namespace WPFSample.Lines.CustomPoints;

// WPF xaml parser does not support generic types
// we instead create a non-generic class that inherits from the generic one.
public class CustomArrowLineSeries : XamlLineSeries<DataPoint, ArrowGeometry> { }
