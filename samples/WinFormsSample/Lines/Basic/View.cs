using System.Linq;
using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.Lines.Basic;

namespace WinFormsSample.Lines.Basic;

public partial class View : UserControl
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            //Title = viewModel.Title,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 20),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        cartesianChart.SendToBack();
        Controls.Add(cartesianChart);

        var a = new Button() { Text = "hello world!" };
        a.BringToFront();
        Controls.Add(a);
        a.Click += A_Click;
    }

    private void A_Click(object sender, System.EventArgs e)
    {
        cartesianChart.Series.First().IsVisible = !cartesianChart.Series.First().IsVisible;
    }
}
