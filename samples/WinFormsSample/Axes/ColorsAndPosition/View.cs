using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.Axes.ColorsAndPosition;

namespace WinFormsSample.Axes.ColorsAndPosition;

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
            XAxes = viewModel.XAxes,
            YAxes = viewModel.YAxes,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 50),
            Size = new System.Drawing.Size(100, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);

        var b1 = new Button { Text = "toggle position", Location = new System.Drawing.Point(0, 0) };
        b1.Click += (object sender, System.EventArgs e) => viewModel.TogglePosition();
        Controls.Add(b1);

        var b2 = new Button { Text = "new color", Location = new System.Drawing.Point(80, 0) };
        b2.Click += (object sender, System.EventArgs e) => viewModel.SetNewColor();
        Controls.Add(b2);
    }
}
