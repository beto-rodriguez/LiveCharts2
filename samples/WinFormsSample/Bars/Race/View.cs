using System.Threading.Tasks;
using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.Bars.Race;

namespace WinFormsSample.Bars.Race;

public partial class View : UserControl
{
    private readonly CartesianChart cartesianChart;
    private readonly ViewModel viewModel;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
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
