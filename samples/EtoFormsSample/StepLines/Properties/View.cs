using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.StepLines.Properties;

namespace WinFormsSample.StepLines.Properties;

public partial class View : UserControl
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(100, 100);

        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 50),
            Size = new System.Drawing.Size(100, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);

        var b1 = new Button { Text = "new values", Location = new System.Drawing.Point(0, 0) };
        b1.Click += (object sender, System.EventArgs e) => viewModel.ChangeValuesInstance();
        Controls.Add(b1);

        var b2 = new Button { Text = "new fill", Location = new System.Drawing.Point(80, 0) };
        b2.Click += (object sender, System.EventArgs e) => viewModel.NewFill();
        Controls.Add(b2);

        var b3 = new Button { Text = "new stroke", Location = new System.Drawing.Point(160, 0) };
        b3.Click += (object sender, System.EventArgs e) => viewModel.NewStroke();
        Controls.Add(b3);

        var b4 = new Button { Text = "newGfill", Location = new System.Drawing.Point(240, 0) };
        b4.Click += (object sender, System.EventArgs e) => viewModel.NewGeometryFill();
        Controls.Add(b4);

        var b5 = new Button { Text = "newGstroke", Location = new System.Drawing.Point(320, 0) };
        b5.Click += (object sender, System.EventArgs e) => viewModel.NewGeometryStroke();
        Controls.Add(b5);

        var b8 = new Button { Text = "+ size", Location = new System.Drawing.Point(560, 0) };
        b8.Click += (object sender, System.EventArgs e) => viewModel.IncreaseGeometrySize();
        Controls.Add(b8);

        var b9 = new Button { Text = "- size", Location = new System.Drawing.Point(640, 0) };
        b9.Click += (object sender, System.EventArgs e) => viewModel.DecreaseGeometrySize();
        Controls.Add(b9);

        var b10 = new Button { Text = "new series", Location = new System.Drawing.Point(720, 0) };
        b10.Click += (object sender, System.EventArgs e) =>
        {
            viewModel.ChangeSeriesInstance();
            cartesianChart.Series = viewModel.Series;
        };
        Controls.Add(b10);
    }
}
