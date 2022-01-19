using System.Threading.Tasks;
using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Bars.Race;

namespace EtoFormsSample.Bars.Race;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;
    private readonly ViewModel viewModel;

    public View()
    {
        InitializeComponent();
        Size = new Eto.Drawing.Size(50, 50);

        viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,

            // out of livecharts properties...
            Location = new Eto.Drawing.Point(0, 0),
            Size = new Eto.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);

        UpdateViewModel();
    }

    public async void UpdateViewModel()
    {
        while (true)
        {
            viewModel.RandomIncrement();
            cartesianChart.Series = viewModel.Series;
            await Task.Delay(1500);
        }
    }
}
