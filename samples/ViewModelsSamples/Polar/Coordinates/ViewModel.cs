using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ViewModelsSamples.Polar.Coordinates;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        new PolarLineSeries<ObservablePolarPoint>
        {
            Values = new[]
            {
                new ObservablePolarPoint(0, 10),
                new ObservablePolarPoint(45, 15),
                new ObservablePolarPoint(90, 20),
                new ObservablePolarPoint(135, 25),
                new ObservablePolarPoint(180, 30),
                new ObservablePolarPoint(225, 35),
                new ObservablePolarPoint(270, 40),
                new ObservablePolarPoint(315, 45),
                new ObservablePolarPoint(360, 50),
            },
            IsClosed = false,
            Fill = null
        }
    };

    public PolarAxis[] AngleAxes { get; set; } =
    {
        new PolarAxis
        {
            // force the axis to always show 360 degrees.
            MinLimit = 0,
            MaxLimit = 360,
            Labeler = angle => $"{angle}°",
            ForceStepToMin = true,
            MinStep = 30
        }
    };
}
