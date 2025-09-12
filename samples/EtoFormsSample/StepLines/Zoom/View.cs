using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.StepLines.Zoom;

public class View : Panel
{
    public View()
    {
        var values = Fetch();

        var series = new ISeries[]
        {
            new StepLineSeries<int>
            {
                Values = values
            }
        };

        var cartesianChart = new CartesianChart
        {
            Series = series,
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X
        };

        Content = cartesianChart;
    }

    private static int[] Fetch()
    {
        var values = new int[100];
        var r = new System.Random();
        var t = 0;
        for (var i = 0; i < 100; i++)
        {
            t += r.Next(-90, 100);
            values[i] = t;
        }
        return values;
    }
}
