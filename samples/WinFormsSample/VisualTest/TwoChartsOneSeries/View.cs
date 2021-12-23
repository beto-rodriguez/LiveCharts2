using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.VisualTest.TwoChartsOneSeries;

namespace WinFormsSample.VisualTest.TwoChartsOneSeries;

public partial class View : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        var cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Dock = DockStyle.Fill
        };

        var cartesianChart2 = new CartesianChart
        {
            Series = viewModel.Series,
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Dock = DockStyle.Fill
        };

        var splitContainer = new SplitContainer
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            //IsSplitterFixed = true,
            Orientation = Orientation.Horizontal
        };

        splitContainer.Panel1.Controls.Add(cartesianChart);
        splitContainer.Panel2.Controls.Add(cartesianChart2);
        Controls.Add(splitContainer);
    }
}
