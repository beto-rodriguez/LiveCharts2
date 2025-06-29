using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace EtoFormsSample.General.Sections;

public class View : Panel
{
    public View()
    {
        var values = new ObservablePoint[]
        {
            new(2.2, 5.4),
            new(4.5, 2.5),
            new(4.2, 7.4),
            new(6.4, 9.9),
            new(8.9, 3.9),
            new(9.9, 5.2)
        };

        var series = new ISeries[]
        {
            new ScatterSeries<ObservablePoint>
            {
                Values = values
            }
        };

        var sections = new RectangularSection[]
        {
            new() {
                Xi = 3,
                Xj = 4,
                Fill = new SolidColorPaint(SKColor.Parse("#FFCDD2"))
            },
            new() {
                Xi = 5,
                Xj = 6,
                Yi = 2,
                Yj = 8,
                Fill = new SolidColorPaint(SKColor.Parse("#BBDEFB"))
            },
            new() {
                Xi = 8,
                Label = "A section here!",
                LabelSize = 14,
                LabelPaint = new SolidColorPaint(SKColor.Parse("#FF6F00")),
                Fill = new SolidColorPaint(SKColor.Parse("#F9FBE7"))
            }
        };

        var cartesianChart = new CartesianChart
        {
            Series = series,
            Sections = sections,
        };

        Content = cartesianChart;
    }
}
