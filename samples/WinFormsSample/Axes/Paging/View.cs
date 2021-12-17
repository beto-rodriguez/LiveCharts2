using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.Axes.Paging;

namespace WinFormsSample.Axes.Paging;

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
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 50),
            Size = new System.Drawing.Size(100, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);

        var b1 = new Button { Text = "Go to page 1", Location = new System.Drawing.Point(0, 0) };
        b1.Click += (object sender, System.EventArgs e) => viewModel.GoToPage1();
        Controls.Add(b1);

        var b2 = new Button { Text = "Go to page 2", Location = new System.Drawing.Point(80, 0) };
        b2.Click += (object sender, System.EventArgs e) => viewModel.GoToPage2();
        Controls.Add(b2);

        var b3 = new Button { Text = "Go to page 3", Location = new System.Drawing.Point(160, 0) };
        b3.Click += (object sender, System.EventArgs e) => viewModel.GoToPage3();
        Controls.Add(b3);

        var b4 = new Button { Text = "Clear", Location = new System.Drawing.Point(24, 0) };
        b4.Click += (object sender, System.EventArgs e) => viewModel.SeeAll();
        Controls.Add(b4);
    }
}
