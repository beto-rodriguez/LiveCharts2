using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;

namespace ViewModelsSamples.Box.Basic;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new BoxSeries<BoxValue>
        {
            Name = "Year 2023",
            Values = [
                // max, upper quartile, median, lower quartile, min
                new(100, 80, 60, 20, 70),
                new(90, 70, 50, 30, 60),
                new(80, 60, 40, 10, 50)
            ]
        },
        new BoxSeries<BoxValue>
        {
            Name = "Year 2024",
            Values = [
                new(90, 70, 50, 30, 60),
                new(80, 60, 40, 10, 50),
                new(70, 50, 30, 20, 40)
            ]
        },
        new BoxSeries<BoxValue>
        {
            Name = "Year 2025",
            Values = [
                new(80, 60, 40, 10, 50),
                new(70, 50, 30, 20, 40),
                new(60, 40, 20, 10, 30)
            ]
        }
    ];

    public Axis[] XAxes { get; set; } = [
        new Axis
        {
            Labels = ["Apperitizers", "Mains", "Desserts"],
            LabelsRotation = 0,
            SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
            SeparatorsAtCenter = false,
            TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
            TicksAtCenter = true
        }
    ];
}
