using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.Polar.Coordinates;

public class View : Panel
{
    public View()
    {
        var values = new ObservablePolarPoint[]
        {
            new(0, 10),
            new(45, 15),
            new(90, 20),
            new(135, 25),
            new(180, 30),
            new(225, 35),
            new(270, 40),
            new(315, 45),
            new(360, 50)
        };

        var series = new ISeries[]
        {
            new PolarLineSeries<ObservablePolarPoint>
            {
                Values = values,
                IsClosed = true,
                Fill = null
            }
        };

        var angleAxes = new PolarAxis[]
        {
            new() {
                MinLimit = 0,
                MaxLimit = 360,
                Labeler = angle => $"{angle}°",
                ForceStepToMin = true,
                MinStep = 30
            }
        };

        var polarChart = new PolarChart
        {
            Series = series,
            AngleAxes = angleAxes
        };

        Content = polarChart;
    }
}
