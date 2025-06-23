using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.WPF;
using ViewModelsSamples.StepLines.Custom;

namespace WPFSample.StepLines.Custom;

// WPF xaml parser does not support generic types
// we instead create non-generic classes that inherit from the generic one.
public class StarStepLineSeries : XamlStepLineSeries<double, StarGeometry> { }
public class SvgStepLineSeries : XamlStepLineSeries<double, VariableSVGPathGeometry> { }
public class MyGeometryStepLineSeries : XamlStepLineSeries<double, MyGeometry> { }
