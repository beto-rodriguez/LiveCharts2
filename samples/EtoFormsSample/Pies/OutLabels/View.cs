using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Pies.OutLabels;

namespace EtoFormsSample.Pies.OutLabels;

public class View : Panel
{
    private readonly PieChart pieChart;

    public View()
    {
        var viewModel = new ViewModel();

        pieChart = new PieChart
        {
            Series = viewModel.Series,
            IsClockwise = false,
            InitialRotation = -90
        };

        Content = pieChart;
    }
}
