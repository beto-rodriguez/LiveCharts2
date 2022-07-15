using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Polar.Basic;

[ObservableObject]
public partial class ViewModel
{
    public ISeries[] Series { get; set; } =
    {
        new PolarLineSeries<double>
        {
            Values = new ObservableCollection<double> { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 },
            DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30)),
            GeometrySize = 30,
            DataLabelsSize = 15,
            DataLabelsPosition = PolarLabelsPosition.Middle,
            DataLabelsRotation = LiveCharts.CotangentAngle,
            IsClosed = true
        }
    };

    public PolarAxis[] RadialAxes { get; set; } =
    {
        new PolarAxis
        {
            LabelsAngle = -60,
            MaxLimit = 30 // null to let the chart autoscale (defualt is null) // mark
        }
    };

    public PolarAxis[] AngleAxes { get; set; } =
    {
        new PolarAxis
        {
            LabelsRotation = LiveCharts.TangentAngle
        }
    };
}
