using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Axes.Multiple;

namespace EtoFormsSample.Axes.Multiple;

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
            LegendFont = new System.Drawing.Font("Courier New", 25),
            LegendTextColor = System.Drawing.Color.FromArgb(255, 50, 50, 50),
            LegendBackColor = System.Drawing.Color.FromArgb(255, 250, 250, 250),

            // out of livecharts properties...
            Location = new System.Drawing.Point(0, 0),
            Size = new System.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
