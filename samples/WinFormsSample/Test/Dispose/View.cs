using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.VisualTest.ReattachVisual;

namespace WinFormsSample.Test.Dispose;

public partial class View : UserControl
{
    private readonly ViewModel _viewModel;
    private CartesianChart _cartesianChart;

    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        _viewModel = new ViewModel();

        _cartesianChart = GetNewChart();

        var b = new Button
        {
            Size = new System.Drawing.Size(150, 50),
            Text = "New chart"
        };
        b.Click += B_Click;

        Controls.Add(_cartesianChart);
        Controls.Add(b);
        b.BringToFront();
    }

    private void B_Click(object sender, System.EventArgs e)
    {
        Controls.Remove(_cartesianChart);
        _cartesianChart.Dispose();
        _cartesianChart = GetNewChart();
        Controls.Add(_cartesianChart);
        Update();
    }

    private CartesianChart GetNewChart()
    {
        return new CartesianChart
        {
            Series = _viewModel.Series,

            // out of livecharts properties...
            Location = new System.Drawing.Point(100, 100),
            Size = new System.Drawing.Size(600, 300),
        };
    }
}
