using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using ViewModelsSamples.Axes.Multiple;

namespace WinFormsSample.Axes.Multiple;

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
            YAxes = viewModel.YAxes,
            LegendPosition = LiveChartsCore.Measure.LegendPosition.Left,
            LegendTextPaint = viewModel.LegendTextPaint,
            LegendBackgroundPaint = viewModel.LedgendBackgroundPaint,
            LegendTextSize = 16,

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
