using Eto.Forms;
using LiveChartsCore.SkiaSharpView.Eto.Forms;
using ViewModelsSamples.Axes.Multiple;

namespace EtoFormsSample.Axes.Multiple;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;

    public View()
    {
        InitializeComponent();
        Size = new Eto.Drawing.Size(50, 50);

        var viewModel = new ViewModel();

        cartesianChart = new CartesianChart
        {
            Series = viewModel.Series,
            YAxes = viewModel.YAxes,
            LegendPosition = LiveChartsCore.Measure.LegendPosition.Left,
            LegendFont = new Eto.Drawing.Font("Courier New", 25),
            LegendTextColor = Eto.Drawing.Color.FromArgb(255, 50, 50, 50),
            LegendBackColor = Eto.Drawing.Color.FromArgb(255, 250, 250, 250),

            // out of livecharts properties...
            Location = new Eto.Drawing.Point(0, 0),
            Size = new Eto.Drawing.Size(50, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);
    }
}
