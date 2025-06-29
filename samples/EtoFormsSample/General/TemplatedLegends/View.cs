using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.General.TemplatedLegends;

namespace EtoFormsSample.General.TemplatedLegends;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        var viewModel = new ViewModel();

        var series = new ISeries[]
        {
            new ColumnSeries<double> { Values = viewModel.RogerValues, Name = "Roger" },
            new ColumnSeries<double> { Values = viewModel.SusanValues, Name = "Susan" }
        };

        cartesianChart = new CartesianChart
        {
            Series = series,
            LegendPosition = LiveChartsCore.Measure.LegendPosition.Right,
            Legend = new CustomLegend()
        };

        Content = cartesianChart;
    }
}
