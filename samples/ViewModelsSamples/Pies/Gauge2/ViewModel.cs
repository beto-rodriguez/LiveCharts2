using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Measure;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Pies.Gauge2;

public partial class ViewModel : ObservableObject
{
    public IEnumerable<ISeries> Series { get; set; }
        = new GaugeBuilder()
        .WithLabelsSize(50)
        .WithInnerRadius(75)
        .WithBackgroundInnerRadius(75)
        .WithBackground(new SolidColorPaint(new SKColor(100, 181, 246, 90)))
        .WithLabelsPosition(PolarLabelsPosition.ChartCenter)
        .AddValue(30, "gauge value", SKColors.YellowGreen, SKColors.Red) // defines the value and the color // mark
        .BuildSeries();
}
