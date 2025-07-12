using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.WinUI;
using ViewModelsSamples.StepLines.Custom;

namespace WinUISample.StepLines.Custom;

// WinUI xaml parser does not support generic types
// we instead create non-generic classes that inherit from the generic one.
public class StarStepLineSeries : XamlStepLineSeries<double, StarGeometry> { }
public class SvgStepLineSeries : XamlStepLineSeries<double, VariableSVGPathGeometry> { }
public class MyGeometryStepLineSeries : XamlStepLineSeries<double, MyGeometry> { }
