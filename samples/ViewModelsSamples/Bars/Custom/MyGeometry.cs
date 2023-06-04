using System;
using SkiaSharp;

namespace ViewModelsSamples.Bars.Custom;

public class MyGeometry : LiveChartsCore.SkiaSharpView.Drawing.Geometries.SVGPathGeometry
{
    public MyGeometry()
        : base(() => SelectedPath ?? throw new NotImplementedException("Path not set yet!"))
    {
        // we passed the "path source" to the base class
        // it is a function that returns the path to draw
        // this way we can change the path at runtime

        // Note: LiveCharts geometries do not implement INotifyPropertyChanged
        // so if you change the path at runtime you will need to call a chart update.
    }

    public static SKPath? SelectedPath { get; set; }
}
