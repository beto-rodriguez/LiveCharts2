using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.General.Visibility;

namespace EtoFormsSample.General.Visibility;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        InitializeComponent();
        Size = new Eto.Drawing.Size(100, 100);

        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,

            // out of livecharts properties...
            Location = new Eto.Drawing.Point(0, 50),
            Size = new Eto.Drawing.Size(100, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);

        var b1 = new Button { Text = "toggle 1", Location = new Eto.Drawing.Point(0, 0) };
        b1.Click += (object sender, System.EventArgs e) => viewModel.ToogleSeries0();
        Controls.Add(b1);

        var b2 = new Button { Text = "toggle 2", Location = new Eto.Drawing.Point(80, 0) };
        b2.Click += (object sender, System.EventArgs e) => viewModel.ToogleSeries1();
        Controls.Add(b2);

        var b3 = new Button { Text = "toggle 3", Location = new Eto.Drawing.Point(160, 0) };
        b3.Click += (object sender, System.EventArgs e) => viewModel.ToogleSeries2();
        Controls.Add(b3);
    }
}
