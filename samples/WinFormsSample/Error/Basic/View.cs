using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.Error.Basic;

namespace WinFormsSample.Error.Basic;

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
            Series = viewModel.Series0,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Dock = DockStyle.Fill
        };

        var cartesianChart2 = new CartesianChart
        {
            Series = viewModel.Series1,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Dock = DockStyle.Fill
        };

        var cartesianChart3 = new CartesianChart
        {
            Series = viewModel.Series2,
            XAxes = viewModel.DateTimeAxis,

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
        splitContainer.Panel2.Controls.Add(cartesianChart3);
        Controls.Add(splitContainer);
    }
}
