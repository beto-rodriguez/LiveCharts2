using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Pies.Doughnut;

namespace EtoFormsSample.Pies.Doughnut;

public class View : Panel
{
    private readonly PieChart pieChart;

    public View()
    {
        InitializeComponent();
        Size = new Eto.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        pieChart = new PieChart
        {
            Series = viewModel.Series,

            // out of livecharts properties...
            Location = new Eto.Drawing.Point(0, 0),
            Size = new Eto.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(pieChart);
    }
}
