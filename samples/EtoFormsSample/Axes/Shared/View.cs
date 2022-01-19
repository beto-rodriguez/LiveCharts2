using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Axes.Shared;

namespace EtoFormsSample.Axes.Shared;

public class View : Panel
{
    public View()
    {
        InitializeComponent();
        Size = new Eto.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        var cartesianChart = new CartesianChart
        {
            Series = viewModel.SeriesCollection1,
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,
            XAxes = viewModel.SharedXAxis, // <-- notice we are using the same variable for both charts, this syncs both charts

            // out of livecharts properties...
            Location = new Eto.Drawing.Point(0, 0),
            Size = new Eto.Drawing.Size(50, 50),
            Dock = DockStyle.Fill
        };

        var cartesianChart2 = new CartesianChart
        {
            Series = viewModel.SeriesCollection2,
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,
            XAxes = viewModel.SharedXAxis, // <-- notice we are using the same variable for both charts, this syncs both charts

            // out of livecharts properties...
            Location = new Eto.Drawing.Point(0, 0),
            Size = new Eto.Drawing.Size(50, 50),
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
