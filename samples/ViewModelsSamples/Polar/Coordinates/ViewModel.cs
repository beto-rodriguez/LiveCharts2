using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Polar.Coordinates;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new PolarLineSeries<ObservablePolarPoint>
        {
            Values = [
                new(0, 10),
                new(45, 15),
                new(90, 20),
                new(135, 25),
                new(180, 30),
                new(225, 35),
                new(270, 40),
                new(315, 45),
                new(360, 50),
            ],
            IsClosed = false,
            Fill = null
        }
    ];

    public PolarAxis[] AngleAxes { get; set; } = [
        new PolarAxis
        {
            // force the axis to always show 360 degrees.
            MinLimit = 0,
            MaxLimit = 360,
            Labeler = angle => $"{angle}°",
            ForceStepToMin = true,
            MinStep = 30
        }
    ];
}
