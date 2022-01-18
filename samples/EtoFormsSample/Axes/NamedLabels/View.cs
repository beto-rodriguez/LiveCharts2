using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.Axes.NamedLabels;

namespace WinFormsSample.Axes.NamedLabels;

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
            XAxes = viewModel.XAxes,
            YAxes = viewModel.YAxes,
            TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Left, // mark
            TooltipFont = new System.Drawing.Font("Courier New", 25), // mark
            TooltipTextColor = System.Drawing.Color.FromArgb(255, 242, 244, 195), // mark
            TooltipBackColor = System.Drawing.Color.FromArgb(255, 72, 0, 50), // mark

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
