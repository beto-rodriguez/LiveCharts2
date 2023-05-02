using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Bars.Custom;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        // use the second type argument to specify the geometry to draw for every point
        // there are already many predefined geometries in the
        // LiveChartsCore.SkiaSharpView.Drawing.Geometries namespace
        new ColumnSeries<double, LiveChartsCore.SkiaSharpView.Drawing.Geometries.OvalGeometry>
        {
            Values = new List<double> { 4, 2, 0, 5, 2, 6 },
            Fill = new SolidColorPaint(SKColors.CornflowerBlue)
        },

        // you can also define your own geometry using SVG
        new ColumnSeries<double, MyGeometry>
        {
            Values = new List<double> { 3, 2, 3, 4, 5, 3 },
            Stroke = new SolidColorPaint(SKColors.Coral, 1),
            Fill = null
        }
    };
}
