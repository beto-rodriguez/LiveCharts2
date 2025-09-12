using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView;
using ViewModelsSamples.General.TemplatedTooltips;
using LiveChartsCore;

namespace WinFormsSample.General.TemplatedTooltips;

public partial class View : UserControl
{
    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        var series = new ISeries[]
        {
            new ColumnSeries<GeometryPoint> { Values = viewModel.Values }
        };

        var cartesianChart = new CartesianChart
        {
            Series = series,
            Tooltip = new CustomTooltip(),
            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
