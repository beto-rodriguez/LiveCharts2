using System.Drawing;
using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.Axes.Style;

namespace WinFormsSample.Axes.Style;

public partial class View : UserControl
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(100, 100);
        BackColor = Color.FromArgb(255, 50, 50, 50);

        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            XAxes = viewModel.XAxes,
            YAxes = viewModel.YAxes,
            DrawMarginFrame = viewModel.Frame,
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.Both,
            TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Hidden,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(100, 100),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
