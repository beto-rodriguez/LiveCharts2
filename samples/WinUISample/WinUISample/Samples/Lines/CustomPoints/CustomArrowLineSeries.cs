using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinUI;
using ViewModelsSamples.Lines.CustomPoints;

namespace WinUISample.Lines.CustomPoints;

// WinUI xaml parser does not support generic types
// we instead create a non-generic class that inherits from the generic one.
public class CustomArrowLineSeries : XamlLineSeries<DataPoint, ArrowGeometry> { }
